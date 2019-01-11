using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Collections;
using Xphter.Framework.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Calculates result of a operation expression.
    /// </summary>
    public class OperationExpressionCalculator {
        public OperationExpressionCalculator(IEnumerable<IOperationRule> rules) {
            if(rules == null || !rules.Any()) {
                throw new ArgumentException("rules is null ro empty", "rules");
            }

            StringBuilder pattern = new StringBuilder(string.Format("(?:{0}[^{0}]*{0})", STRING_DELIMITER));
            foreach(string item in rules.Select((item) => item.OperandPattern).Where((item) => item != null).OrderByDescending((item) => item.Length).Distinct(StringComparer.OrdinalIgnoreCase)) {
                pattern.AppendFormat("|(?:{0})", item);
            }
            foreach(string item in rules.Select((item) => item.OperatorSymbol).OrderByDescending((item) => item.Length).Distinct(StringComparer.OrdinalIgnoreCase)) {
                pattern.AppendFormat("|(?:{0})", RegexUtility.Encode(item));
            }

            this.m_whitespaceRegex = new Regex("\\s+", RegexOptions.Compiled);
            this.m_expressionRegex = new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            this.m_wellFormedRegex = new Regex(pattern.AppendFormat("|\\{0}|\\{1}", LEFT_BRACKET, RIGHT_BRACKET).ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            this.m_bracketRegex = new Regex(string.Format(
                "(?>" +
                    "^{2}*" +
                        "(?:" +
                            "(?:(?'left'{0}){2}+)+" +
                            "(?:(?'inner-left'{1}){2}*)+" +
                        ")*" +
                    "(?(left)(?!))$" +
                ")",
                "\\" + LEFT_BRACKET,
                "\\" + RIGHT_BRACKET,
                string.Format("[^\\{0}\\{1}]", LEFT_BRACKET, RIGHT_BRACKET)), RegexOptions.Compiled);

            this.m_rules = rules.ToIDictionary((item) => item.OperatorSymbol);
        }

        public const char STRING_DELIMITER = '"';
        public const char LEFT_BRACKET = '(';
        public const char RIGHT_BRACKET = ')';

        private Regex m_whitespaceRegex;
        private Regex m_expressionRegex;
        private Regex m_wellFormedRegex;
        private Regex m_bracketRegex;
        private IDictionary<string, IOperationRule> m_rules;

        private string GetInvalidPart(string expressionString) {
            string part = this.m_wellFormedRegex.Replace(expressionString, (match) => string.Empty);
            part = this.m_whitespaceRegex.Replace(part, (match) => string.Empty);
            return part;
        }

        private string ParseBrackets(string expressionString) {
            Match match = this.m_bracketRegex.Match(expressionString);
            if(match == null || !match.Success) {
                throw new InvalidOperationExpressionException("Missing paired left bracket and right bracket.");
            }
            if(match.Groups["inner"].Captures.Count == 0) {
                return expressionString;
            }

            ICollection<Element<int, int>> parts = new List<Element<int, int>>();
            foreach(Capture capture in match.Groups["inner"].Captures) {
                parts.Add(new Element<int, int>(capture.Index, capture.Length));
            }
            parts = new List<Element<int, int>>(parts.OrderByDescending((item) => item.Component1));

            int offset = 0;
            string result = null;
            foreach(Element<int, int> element in parts) {
                result = this.Compute(expressionString.Substring(element.Component1, element.Component2)).ToString();
                expressionString = expressionString.Remove(element.Component1 - 1, element.Component2 + 2);
                expressionString = expressionString.Insert(element.Component1 - 1, result);

                offset = result.Length - element.Component2 - 2;
                foreach(Element<int, int> item in parts.Where((item) => item.Component1 < element.Component1 && item.Component1 + item.Component2 > element.Component1 + element.Component2)) {
                    item.Component2 += offset;
                }
            }

            return expressionString;
        }

        /// <summary>
        /// Gets a value to indicate whether <paramref name="expressionString"/> is valid.
        /// </summary>
        /// <param name="expressionString"></param>
        /// <returns></returns>
        public bool IsWellFormed(string expressionString) {
            if(string.IsNullOrWhiteSpace(expressionString)) {
                return false;
            }

            // check format
            string invalidPart = this.GetInvalidPart(expressionString);
            if(invalidPart.Length > 0) {
                return false;
            }

            // check brackets
            if(!this.m_bracketRegex.IsMatch(expressionString)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates result of <paramref name="expressionString"/>.
        /// </summary>
        /// <param name="expressionString"></param>
        /// <returns></returns>
        public object Compute(string expressionString) {
            if(string.IsNullOrWhiteSpace(expressionString)) {
                throw new ArgumentException("expressionString is null or empty", "expressionString");
            }

            // check format
            string invalidPart = this.GetInvalidPart(expressionString);
            if(invalidPart.Length > 0) {
                throw new InvalidOperationExpressionException(string.Format("Invalid characters in operation expression: {0}", invalidPart));
            }

            // parse brackets
            expressionString = this.ParseBrackets(expressionString);

            // analyse each operand and operator
            IOperationRule rule = null;
            Element<int, IOperationOperator> element = null;
            IOperationOperator currentOperator = null, previousOperator = null;
            OperationExpressionContext context = new OperationExpressionContext();
            List<object> operands = new List<object>();
            Stack<object> operandsStack = new Stack<object>();
            Stack<Element<int, IOperationOperator>> operatorsStack = new Stack<Element<int, IOperationOperator>>();
            foreach(Match match in this.m_expressionRegex.Matches(expressionString)) {
                if(this.m_rules.ContainsKey(match.Value)) {
                    rule = this.m_rules[match.Value];
                    currentOperator = rule.CreateOperator(context);

                    while(operatorsStack.Count > 0) {
                        previousOperator = operatorsStack.Peek().Component2;
                        if(currentOperator.Priority > previousOperator.Priority) {
                            break;
                        }

                        element = operatorsStack.Pop();
                        if((context.AvailableOperandCount += element.Component1) < previousOperator.OperandCount) {
                            throw new InvalidOperationExpressionException(string.Format("{0} operator needs {1} operands.", previousOperator.OwnerRule.OperatorSymbol, previousOperator.OperandCount));
                        }

                        operands.Clear();
                        for(int i = 0; i < previousOperator.OperandCount; i++) {
                            operands.Add(operandsStack.Pop());
                        }
                        operands.Reverse();
                        context.AvailableOperandCount -= previousOperator.OperandCount;

                        operandsStack.Push(previousOperator.Compute(context, operands));
                        ++context.AvailableOperandCount;
                    }

                    operatorsStack.Push(new Element<int, IOperationOperator>(Math.Min(context.AvailableOperandCount, currentOperator.OperandCount), currentOperator));
                    context.AvailableOperandCount -= operatorsStack.Peek().Component1;
                } else {
                    operandsStack.Push(match.Value);
                    ++context.AvailableOperandCount;
                }
            }

            while(operatorsStack.Count > 0) {
                element = operatorsStack.Pop();
                previousOperator = element.Component2;
                if((context.AvailableOperandCount += element.Component1) < previousOperator.OperandCount) {
                    throw new InvalidOperationExpressionException(string.Format("{0} operator needs {1} operands.", previousOperator.OwnerRule.OperatorSymbol, previousOperator.OperandCount));
                }

                operands.Clear();
                for(int i = 0; i < previousOperator.OperandCount; i++) {
                    operands.Add(operandsStack.Pop());
                }
                operands.Reverse();
                context.AvailableOperandCount -= previousOperator.OperandCount;

                operandsStack.Push(previousOperator.Compute(context, operands));
                ++context.AvailableOperandCount;
            }

            if(operandsStack.Count > 1) {
                throw new Exception("Logic error: more than one operand is left finally.");
            }
            return operandsStack.Pop();
        }

        private class OperationExpressionContext : IOperationExpressionContext {
            #region IOperationExpressionContext Members

            public int AvailableOperandCount {
                get;
                set;
            }

            public bool IsString(object operand) {
                if(operand == null) {
                    return false;
                }
                if(!(operand is string)) {
                    return false;
                }

                return ((string) operand).StartsWith(STRING_DELIMITER.ToString()) && ((string) operand).EndsWith(STRING_DELIMITER.ToString());
            }

            #endregion
        }
    }

    /// <summary>
    /// Provides some priority numbers.
    /// </summary>
    public static class OperationOperatorPriorities {
        /*
         *  Unary operators: !, +, -
         */
        public const int HighUnaryPriority = 5300;
        public const int NormalUnaryPriority = 5200;
        public const int LowUnaryPriority = 5100;

        /*
         *  Binary operators: *, /, %
         */
        public const int HighMultiplicationPriority = 4300;
        public const int NormalMultiplicationPriority = 4200;
        public const int LowMultiplicationPriority = 4100;

        /*
         *  Binary operators: +, -
         */
        public const int HighAdditionPriority = 3300;
        public const int NormalAdditionPriority = 3200;
        public const int LowAdditionPriority = 3100;

        /*
         *  Binary operators: >, >=, <, <=, =, !=
         */
        public const int HighRelationalPriority = 2300;
        public const int NormalRelationalPriority = 2200;
        public const int LowRelationalPriority = 2100;

        /*
         *  Binary operators: &&, ||
         */
        public const int HighConditionalPriority = 1200;
        public const int NormalConditionalPriority = 1100;
        public const int LowConditionalPriority = 1000;
    }

    /// <summary>
    /// The exception that is thrown when a operation expression contains invalid data.
    /// </summary>
    [Serializable]
    public class InvalidOperationExpressionException : Exception {
        public InvalidOperationExpressionException()
            : base() {
        }

        public InvalidOperationExpressionException(string message)
            : base(message) {
        }

        public InvalidOperationExpressionException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public InvalidOperationExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }

    /// <summary>
    /// Represents state of a operation expression.
    /// </summary>
    public interface IOperationExpressionContext {
        /// <summary>
        /// Gets the number of usable operands.
        /// </summary>
        int AvailableOperandCount {
            get;
        }

        /// <summary>
        /// Gets a value to indicate whether <paramref name="operands"/> represents a string.
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        bool IsString(object operand);
    }

    /// <summary>
    /// Represents a operation rule.
    /// </summary>
    public interface IOperationRule {
        /// <summary>
        /// Gets the rule name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets the symbol of operator.
        /// </summary>
        string OperatorSymbol {
            get;
        }

        /// <summary>
        /// Gets a regex pattern to find operands.
        /// </summary>
        string OperandPattern {
            get;
        }

        /// <summary>
        /// Creates a new IOperationOperator object from current operation expression.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IOperationOperator CreateOperator(IOperationExpressionContext context);
    }

    /// <summary>
    /// Represents the operator in a operation expression.
    /// </summary>
    public interface IOperationOperator {
        /// <summary>
        /// Gets the ownered operation rule.
        /// </summary>
        IOperationRule OwnerRule {
            get;
        }

        /// <summary>
        /// Gets a descriptive name of this operator.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets operator priority.
        /// </summary>
        int Priority {
            get;
        }

        /// <summary>
        /// Gets the number of operands required.
        /// </summary>
        int OperandCount {
            get;
        }

        /// <summary>
        /// Computes operation result of <paramref name="operands"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        object Compute(IOperationExpressionContext context, IEnumerable<object> operands);
    }

    /// <summary>
    /// Provides a base class of operation rules.
    /// </summary>
    public abstract class GenericOperationRule : IOperationRule {
        #region IOperationRule Members

        public abstract string Name {
            get;
        }

        public abstract string OperatorSymbol {
            get;
        }

        public virtual string OperandPattern {
            get {
                return null;
            }
        }

        public abstract IOperationOperator CreateOperator(IOperationExpressionContext context);

        #endregion

        public override string ToString() {
            return this.Name;
        }
    }

    /// <summary>
    /// Provides a base class of operation rule which needs numeric operands.
    /// </summary>
    public abstract class NumericOperationRule : IOperationRule {
        #region IOperationRule Members

        public abstract string Name {
            get;
        }

        public abstract string OperatorSymbol {
            get;
        }

        public virtual string OperandPattern {
            get {
                return @"\d+(?:\.\d+)?";
            }
        }

        public abstract IOperationOperator CreateOperator(IOperationExpressionContext context);

        #endregion

        public override string ToString() {
            return this.Name;
        }
    }

    /// <summary>
    /// Provides a base class of operation rule which needs boolean operands.
    /// </summary>
    public abstract class BooleanOperationRule : IOperationRule {
        #region IOperationRule Members

        public abstract string Name {
            get;
        }

        public abstract string OperatorSymbol {
            get;
        }

        public virtual string OperandPattern {
            get {
                return @"true|false";
            }
        }

        public abstract IOperationOperator CreateOperator(IOperationExpressionContext context);

        #endregion

        public override string ToString() {
            return this.Name;
        }
    }

    /// <summary>
    /// Provides a base class of operator which only one operands.
    /// </summary>
    public abstract class UnaryOperationOperator : IOperationOperator {
        #region IOperationOperator Members

        public IOperationRule OwnerRule {
            get;
            set;
        }

        public abstract string Name {
            get;
        }

        public abstract int Priority {
            get;
        }

        public int OperandCount {
            get {
                return 1;
            }
        }

        public abstract object Compute(IOperationExpressionContext context, IEnumerable<object> operands);

        #endregion

        public override string ToString() {
            return this.Name;
        }
    }

    /// <summary>
    /// Provides a base class of operator which needs two operands.
    /// </summary>
    public abstract class BinaryOperationOperator : IOperationOperator {
        #region IOperationOperator Members

        public IOperationRule OwnerRule {
            get;
            set;
        }

        public abstract string Name {
            get;
        }

        public abstract int Priority {
            get;
        }

        public int OperandCount {
            get {
                return 2;
            }
        }

        public abstract object Compute(IOperationExpressionContext context, IEnumerable<object> operands);

        #endregion

        public override string ToString() {
            return this.Name;
        }
    }

    /// <summary>
    /// Provides a base class of operator which needs only one numeric operand.
    /// </summary>
    public abstract class UnaryNumericOperationOperator : UnaryOperationOperator {
        protected abstract object Compute(long operand);

        protected abstract object Compute(decimal operand);

        public override object Compute(IOperationExpressionContext context, IEnumerable<object> operands) {
            object result = null;
            object operand = operands.First();

            if(operand is int || operand is long) {
                result = this.Compute((long) operand);
            } else if(operand is float || operand is double || operand is decimal) {
                result = this.Compute(Convert.ToDecimal(operand));
            } else {
                long ivalue = 0L;
                decimal fvalue = 0M;
                bool isInteger = false;
                bool isNumber = false;

                isInteger = long.TryParse(operand.ToString(), out ivalue);
                isNumber = decimal.TryParse(operand.ToString(), out fvalue);
                if(!isNumber) {
                    throw new InvalidOperationExpressionException(string.Format("{0} is not a number", operand));
                }

                if(isInteger) {
                    result = this.Compute(ivalue);
                } else {
                    result = this.Compute(fvalue);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Provides a base class of operator which needs two numeric operands.
    /// </summary>
    public abstract class BinaryNumericOperationOperator : BinaryOperationOperator {
        protected abstract object Compute(long operand1, long operand2);

        protected abstract object Compute(decimal operand1, decimal operand2);

        public override object Compute(IOperationExpressionContext context, IEnumerable<object> operands) {
            object result = null;
            object operand1 = operands.ElementAt(0);
            object operand2 = operands.ElementAt(1);

            if((operand1 is int || operand1 is long) && (operand2 is int || operand2 is long)) {
                result = this.Compute((long) operand1, (long) operand2);
            } else if((operand1 is float || operand1 is double || operand1 is decimal) && (operand2 is float || operand2 is double || operand2 is decimal)) {
                result = this.Compute(Convert.ToDecimal(operand1), Convert.ToDecimal(operand2));
            } else {
                long ivalue1 = 0L, ivalue2 = 0L;
                decimal fvalue1 = 0M, fvalue2 = 0M;
                bool isInteger1 = false, isInteger2 = false;
                bool isNumber1 = false, isNumber2 = false;

                isInteger1 = long.TryParse(operand1.ToString(), out ivalue1);
                isNumber1 = decimal.TryParse(operand1.ToString(), out fvalue1);
                isInteger2 = long.TryParse(operand2.ToString(), out ivalue2);
                isNumber2 = decimal.TryParse(operand2.ToString(), out fvalue2);
                if(!isNumber1) {
                    throw new InvalidOperationExpressionException(string.Format("{0} is not a number", operand1));
                }
                if(!isNumber2) {
                    throw new InvalidOperationExpressionException(string.Format("{0} is not a number", operand2));
                }

                if(isInteger1 && isInteger2) {
                    result = this.Compute(ivalue1, ivalue2);
                } else {
                    result = this.Compute(fvalue1, fvalue2);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Provides a base class of operator which needs only one boolean operand.
    /// </summary>
    public abstract class UnaryBooleanOperationOperator : UnaryOperationOperator {
        protected abstract bool Compute(bool operand);

        public override object Compute(IOperationExpressionContext context, IEnumerable<object> operands) {
            object result = null;
            object operand = operands.First();

            if(operand is bool) {
                result = this.Compute((bool) operand);
            } else {
                bool value = false;
                if(!bool.TryParse(operand.ToString(), out value)) {
                    throw new InvalidOperationExpressionException(string.Format("{0} is not a boolean", operand));
                }

                result = this.Compute(value);
            }

            return result;
        }
    }

    /// <summary>
    /// Provides a base class of operator which needs two boolean operands.
    /// </summary>
    public abstract class BinaryBooleanOperationOperator : BinaryOperationOperator {
        protected abstract bool Compute(bool operand1, bool operand2);

        public override object Compute(IOperationExpressionContext context, IEnumerable<object> operands) {
            object result = null;
            object operand1 = operands.ElementAt(0);
            object operand2 = operands.ElementAt(1);

            if(operand1 is bool && operand2 is bool) {
                result = this.Compute((bool) operand1, (bool) operand2);
            } else {
                bool value1 = false, value2 = false;
                if(!bool.TryParse(operand1.ToString(), out value1)) {
                    throw new InvalidOperationExpressionException(string.Format("{0} is not a boolean", operand1));
                }
                if(!bool.TryParse(operand2.ToString(), out value2)) {
                    throw new InvalidOperationExpressionException(string.Format("{0} is not a boolean", operand2));
                }

                result = this.Compute(value1, value2);
            }

            return result;
        }
    }

    /// <summary>
    /// Operator: +.
    /// </summary>
    public class UnaryAdditionOperationOperator : UnaryNumericOperationOperator {
        protected override object Compute(long operand) {
            return operand;
        }

        protected override object Compute(decimal operand) {
            return operand;
        }

        public override string Name {
            get {
                return "求正";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalUnaryPriority;
            }
        }
    }

    /// <summary>
    /// Operator: -.
    /// </summary>
    public class UnarySubtractionOperationOperator : UnaryNumericOperationOperator {
        protected override object Compute(long operand) {
            return -1 * operand;
        }

        protected override object Compute(decimal operand) {
            return -1 * operand;
        }

        public override string Name {
            get {
                return "求负";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalUnaryPriority;
            }
        }
    }

    /// <summary>
    /// Operator: !.
    /// </summary>
    public class UnaryNegationOperationOperator : UnaryBooleanOperationOperator {
        protected override bool Compute(bool operand) {
            return !operand;
        }

        public override string Name {
            get {
                return "求反";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalUnaryPriority;
            }
        }
    }

    /// <summary>
    /// Operator: *.
    /// </summary>
    public class BinaryMultiplicationOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 * operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 * operand2;
        }

        public override string Name {
            get {
                return "相乘";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalMultiplicationPriority;
            }
        }
    }

    /// <summary>
    /// Operator: /.
    /// </summary>
    public class BinaryDivisionOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 * 1.0M / operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 / operand2;
        }

        public override string Name {
            get {
                return "相除";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalMultiplicationPriority;
            }
        }
    }

    /// <summary>
    /// Operator: %.
    /// </summary>
    public class BinaryRemainderOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 % operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 % operand2;
        }

        public override string Name {
            get {
                return "求余";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalMultiplicationPriority;
            }
        }
    }

    /// <summary>
    /// Operator: +.
    /// </summary>
    public class BinaryAdditionOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 + operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 + operand2;
        }

        public override string Name {
            get {
                return "相加";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalAdditionPriority;
            }
        }
    }

    /// <summary>
    /// Operator: -.
    /// </summary>
    public class BinarySubtractionOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 - operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 - operand2;
        }

        public override string Name {
            get {
                return "相减";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalAdditionPriority;
            }
        }
    }

    /// <summary>
    /// Operator: >.
    /// </summary>
    public class BinaryGreaterThanOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 > operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 > operand2;
        }

        public override string Name {
            get {
                return "大于";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalRelationalPriority;
            }
        }
    }

    /// <summary>
    /// Operator: >=.
    /// </summary>
    public class BinaryGreaterEqualOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 >= operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 >= operand2;
        }

        public override string Name {
            get {
                return "大于等于";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalRelationalPriority;
            }
        }
    }

    /// <summary>
    /// Operator: <.
    /// </summary>
    public class BinaryLessThanOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 < operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 < operand2;
        }

        public override string Name {
            get {
                return "小于";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalRelationalPriority;
            }
        }
    }

    /// <summary>
    /// Operator: <=.
    /// </summary>
    public class BinaryLessEqualOperationOperator : BinaryNumericOperationOperator {
        protected override object Compute(long operand1, long operand2) {
            return operand1 <= operand2;
        }

        protected override object Compute(decimal operand1, decimal operand2) {
            return operand1 <= operand2;
        }

        public override string Name {
            get {
                return "小于等于";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalRelationalPriority;
            }
        }
    }

    /// <summary>
    /// Operator: =.
    /// </summary>
    public class BinaryEquationOperationOperator : BinaryOperationOperator {
        protected virtual bool ComputeResult(IOperationExpressionContext context, IEnumerable<object> operands) {
            object operand1 = operands.ElementAt(0);
            object operand2 = operands.ElementAt(1);

            if(object.ReferenceEquals(operand1, operand2)) {
                return true;
            }
            if(operand1 == operand2) {
                return true;
            }
            if(context.IsString(operand1) ^ context.IsString(operand2)) {
                return false;
            }
            return string.Equals(operand1.ToString(), operand2.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public override string Name {
            get {
                return "等于";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.LowRelationalPriority;
            }
        }

        public override object Compute(IOperationExpressionContext context, IEnumerable<object> operands) {
            return this.ComputeResult(context, operands);
        }
    }

    /// <summary>
    /// Operator: !=.
    /// </summary>
    public class BinaryInequationOperationOperator : BinaryEquationOperationOperator {
        protected override bool ComputeResult(IOperationExpressionContext context, IEnumerable<object> operands) {
            return !base.ComputeResult(context, operands);
        }

        public override string Name {
            get {
                return "不等于";
            }
        }
    }

    /// <summary>
    /// Operator: &&.
    /// </summary>
    public class BinaryAndOperationOperator : BinaryBooleanOperationOperator {
        protected override bool Compute(bool operand1, bool operand2) {
            return operand1 && operand2;
        }

        public override string Name {
            get {
                return "并且";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.NormalConditionalPriority;
            }
        }
    }

    /// <summary>
    /// Operator: ||.
    /// </summary>
    public class BinaryOrOperationOperator : BinaryBooleanOperationOperator {
        protected override bool Compute(bool operand1, bool operand2) {
            return operand1 || operand2;
        }

        public override string Name {
            get {
                return "或者";
            }
        }

        public override int Priority {
            get {
                return OperationOperatorPriorities.LowConditionalPriority;
            }
        }
    }

    /// <summary>
    /// Arithmetic: -.
    /// </summary>
    public class NegationOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "取反";
            }
        }

        public override string OperatorSymbol {
            get {
                return "!";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new UnaryNegationOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Arithmetic: +.
    /// </summary>
    public class AdditionOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "加法";
            }
        }

        public override string OperatorSymbol {
            get {
                return "+";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            if(context.AvailableOperandCount > 0) {
                return new BinaryAdditionOperationOperator {
                    OwnerRule = this,
                };
            } else {
                return new UnaryAdditionOperationOperator {
                    OwnerRule = this,
                };
            }
        }
    }

    /// <summary>
    /// Arithmetic: -.
    /// </summary>
    public class SubtractionOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "减法";
            }
        }

        public override string OperatorSymbol {
            get {
                return "-";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            if(context.AvailableOperandCount > 0) {
                return new BinarySubtractionOperationOperator {
                    OwnerRule = this,
                };
            } else {
                return new UnarySubtractionOperationOperator {
                    OwnerRule = this,
                };
            }
        }
    }

    /// <summary>
    /// Arithmetic: *.
    /// </summary>
    public class MultiplicationOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "乘法";
            }
        }

        public override string OperatorSymbol {
            get {
                return "*";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryMultiplicationOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Arithmetic: /.
    /// </summary>
    public class DivisionOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "除法";
            }
        }

        public override string OperatorSymbol {
            get {
                return "/";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryDivisionOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Arithmetic: %.
    /// </summary>
    public class RemainderOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "取余";
            }
        }

        public override string OperatorSymbol {
            get {
                return "%";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryRemainderOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Relational: >.
    /// </summary>
    public class GreaterThanOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "大于";
            }
        }

        public override string OperatorSymbol {
            get {
                return ">";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryGreaterThanOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Relational: >=.
    /// </summary>
    public class GreaterEqualOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "大于等于";
            }
        }

        public override string OperatorSymbol {
            get {
                return ">=";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryGreaterThanOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Relational: <.
    /// </summary>
    public class LessThanOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "小于";
            }
        }

        public override string OperatorSymbol {
            get {
                return "<";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryLessThanOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Relational: <=.
    /// </summary>
    public class LessEqualOperationRule : NumericOperationRule {
        public override string Name {
            get {
                return "小于等于";
            }
        }

        public override string OperatorSymbol {
            get {
                return "<=";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryLessEqualOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Relational: =.
    /// </summary>
    public class EquationOperationRule : GenericOperationRule {
        public override string Name {
            get {
                return "等于";
            }
        }

        public override string OperatorSymbol {
            get {
                return "=";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryEquationOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Relational: !=.
    /// </summary>
    public class InequationOperationRule : GenericOperationRule {
        public override string Name {
            get {
                return "不等于";
            }
        }

        public override string OperatorSymbol {
            get {
                return "!=";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryInequationOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Conditional: AND.
    /// </summary>
    public class AndOperationRule : BooleanOperationRule {
        public override string Name {
            get {
                return "并且";
            }
        }

        public override string OperatorSymbol {
            get {
                return "&&";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryAndOperationOperator {
                OwnerRule = this,
            };
        }
    }

    /// <summary>
    /// Conditional: OR.
    /// </summary>
    public class OrOperationRule : BooleanOperationRule {
        public override string Name {
            get {
                return "或者";
            }
        }

        public override string OperatorSymbol {
            get {
                return "||";
            }
        }

        public override IOperationOperator CreateOperator(IOperationExpressionContext context) {
            return new BinaryOrOperationOperator {
                OwnerRule = this,
            };
        }
    }
}
