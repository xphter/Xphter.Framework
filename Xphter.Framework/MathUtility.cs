using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xphter.Framework.Collections;
using Xphter.Framework.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides a utility of mathematics.
    /// </summary>
    public static class MathUtility {
        #region Exchange

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref bool a, ref bool b) {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref int a, ref int b) {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref uint a, ref uint b) {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref long a, ref long b) {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref ulong a, ref ulong b) {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref float a, ref float b) {
            float c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref double a, ref double b) {
            double c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref decimal a, ref decimal b) {
            decimal c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref DateTime a, ref DateTime b) {
            DateTime c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Exchanges the value of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Exchange(ref object a, ref object b) {
            object c = a;
            a = b;
            b = c;
        }

        #endregion

        #region Operation Expression

        private static OperationExpressionCalculator g_operationExpressionCalculator;

        /// <summary>
        /// Gets all registered operation rules.
        /// </summary>
        public static IEnumerable<IOperationRule> RegisteredOperationRules {
            get {
                return ObjectManager<IOperationRule>.Instance.Objects;
            }
        }

        /// <summary>
        /// Registers operation rules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void RegisterOperationRules(params Assembly[] assemblies) {
            ObjectManager<IOperationRule>.Instance.Register(assemblies);
            g_operationExpressionCalculator = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
        }

        /// <summary>
        /// Registers the specified operation rule.
        /// </summary>
        /// <param name="rule"></param>
        public static void RegisterOperationRule(IOperationRule rule) {
            ObjectManager<IOperationRule>.Instance.Register(rule);
            g_operationExpressionCalculator = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
        }

        /// <summary>
        /// Calculates result of <paramref name="expressionString"/>.
        /// </summary>
        /// <param name="operationExpression"></param>
        /// <returns></returns>
        public static object ComputeExpression(string operationExpression) {
            if(g_operationExpressionCalculator == null) {
                throw new InvalidOperationException(string.Format("Method {0}.{1} has not been called.", typeof(MathUtility).Name, ((MethodCallExpression) (((Expression<Action>) (() => MathUtility.RegisterOperationRules(null))).Body)).Method.Name));
            }

            return g_operationExpressionCalculator.Compute(operationExpression);
        }

        #endregion
    }
}
