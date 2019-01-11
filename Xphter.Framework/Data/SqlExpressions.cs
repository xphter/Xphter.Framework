using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data {
    /// <summary>
    /// Represents a common SQL expression.
    /// </summary>
    public interface ISqlExpression {
        /// <summary>
        /// Gets string representation of this expression.
        /// </summary>
        string SqlString {
            get;
        }

        /// <summary>
        /// Gets parameters used by this expression.
        /// </summary>
        ReadOnlyCollection<IDataParameter> Parameters {
            get;
        }
    }

    /// <summary>
    /// Represents a SQL SELECT clause.
    /// </summary>
    public interface ISqlSelectClause : ISqlClause {
        /// <summary>
        /// Gets all expressions in this clause.
        /// </summary>
        ReadOnlyCollection<ISqlExpression> Expressions {
            get;
        }

        /// <summary>
        /// Gets or sets whether this result records set should be distincted.
        /// </summary>
        bool IsDistinct {
            get;
            set;
        }

        /// <summary>
        /// Set the value of IsDistinct property.
        /// </summary>
        /// <param name="value">A boolean value.</param>
        /// <returns>This clause.</returns>
        ISqlSelectClause SetIsDistinct(bool value);

        /// <summary>
        /// Add these specified expressions to this clause.
        /// </summary>
        /// <param name="fields">SQL expressions.</param>
        /// <returns>This clause.</returns>
        ISqlSelectClause AddExpressions(params ISqlExpression[] expressions);

        /// <summary>
        /// Remove the specified expression from this clause.
        /// </summary>
        /// <param name="field">A SQL expression.</param>
        /// <returns>This clause.</returns>
        ISqlSelectClause RemoveExpression(ISqlExpression expression);

        /// <summary>
        /// Remove all expressions from this clause.
        /// </summary>
        /// <returns>This clause.</returns>
        ISqlSelectClause ClearExpressions();
    }

    /// <summary>
    /// Represents a SQL FROM clause.
    /// </summary>
    public interface ISqlFromClause : ISqlClause {
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        ISqlExpression Source {
            get;
            set;
        }

        /// <summary>
        /// Set SQL source of this clause.
        /// </summary>
        /// <param name="source">A SQL object.</param>
        /// <returns>This clasue.</returns>
        ISqlFromClause SetSource(ISqlObject source);

        /// <summary>
        /// Current source cross join the specified source.
        /// </summary>
        /// <param name="source">A SQL object.</param>
        /// <returns>This clasue.</returns>
        /// <exception cref="System.InvalidOperationException">There is not a data source currently.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        ISqlFromClause CrossJoin(ISqlObject source);

        /// <summary>
        /// Current source inner join the specified source.
        /// </summary>
        /// <param name="source">A SQL object.</param>
        /// <param name="condition">Join condition expression.</param>
        /// <returns>This clasue</returns>
        /// <exception cref="System.InvalidOperationException">There is not a data source currently.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="condition"/> is null.</exception>
        ISqlFromClause InnerJoin(ISqlObject source, ISqlExpression condition);

        /// <summary>
        /// Current source full outer join the specified source.
        /// </summary>
        /// <param name="source">A SQL object.</param>
        /// <param name="condition">Join condition expression.</param>
        /// <returns>This clasue.</returns>
        /// <exception cref="System.InvalidOperationException">There is not a data source currently.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="condition"/> is null.</exception>
        ISqlFromClause FullOuterJoin(ISqlObject source, ISqlExpression condition);

        /// <summary>
        /// Current source left outer join the specified source.
        /// </summary>
        /// <param name="source">A SQL object.</param>
        /// <param name="condition">Join condition expression.</param>
        /// <returns>This clasue.</returns>
        /// <exception cref="System.InvalidOperationException">There is not a data source currently.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="condition"/> is null.</exception>
        ISqlFromClause LeftOuterJoin(ISqlObject source, ISqlExpression condition);

        /// <summary>
        /// Current source right outer join the specified source.
        /// </summary>
        /// <param name="source">A SQL object.</param>
        /// <param name="condition">Join condition expression.</param>
        /// <returns>This clasue.</returns>
        /// <exception cref="System.InvalidOperationException">There is not a data source currently.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="condition"/> is null.</exception>
        ISqlFromClause RightOuterJoin(ISqlObject source, ISqlExpression condition);
    }

    /// <summary>
    /// Represents a SQL WHERE clause.
    /// </summary>
    public interface ISqlWhereClause : ISqlClause {
        /// <summary>
        /// Gets or sets the search condition.
        /// </summary>
        ISqlExpression Condition {
            get;
            set;
        }

        /// <summary>
        /// Set the search condition.
        /// </summary>
        /// <param name="expression">A SQL expression</param>
        /// <returns>This clause</returns>
        ISqlWhereClause SetCondition(ISqlExpression expression);
    }

    /// <summary>
    /// Represents a SQL ORDER BY clause.
    /// </summary>
    public interface ISqlOrderClause : ISqlClause {
        /// <summary>
        /// Gets fields in this clause.
        /// </summary>
        ReadOnlyCollection<ISqlExpression> Expressions {
            get;
        }

        /// <summary>
        /// Adds a expression to this clause.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="order">SQL order type.</param>
        /// <returns>This clause.</returns>
        ISqlOrderClause AddExpression(ISqlExpression expression, SqlOrder order);

        /// <summary>
        /// Adds a expression order to this clause.
        /// </summary>
        /// <param name="order">A SQL expression and it's order.</param>
        /// <returns>This clause.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="order"/> is null.</exception>
        ISqlOrderClause AddExpression(SqlExpressionOrder order);

        /// <summary>
        /// Removes a expression order from this clause.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <returns>This clause.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null.</exception>
        ISqlOrderClause RemoveExpression(ISqlExpression expression);

        /// <summary>
        /// Remove all fields from this clause.
        /// </summary>
        /// <returns>This clause.</returns>
        ISqlOrderClause ClearExpressions();

        /// <summary>
        /// Gets the expression order in this clause.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <returns>Expression order</returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> is not existing in this clause.</exception>
        SqlOrder GetExpressionOrder(ISqlExpression expression);
    }

    /// <summary>
    /// Represents a SQL GROUP BY clause.
    /// </summary>
    public interface ISqlGroupClause : ISqlClause {
        /// <summary>
        /// Gets fields in this clause.
        /// </summary>
        IEnumerable<ISqlExpression> Expressions {
            get;
        }

        /// <summary>
        /// Gets the SQL HAVING clause of this GROUP BY clause.
        /// </summary>
        ISqlHavingClause HavingClause {
            get;
        }

        /// <summary>
        /// Adds a expression to this clause.
        /// </summary>
        /// <param name="expressions">SQL expressions</param>
        /// <returns>This clause.</returns>
        ISqlGroupClause AddExpressions(params ISqlExpression[] expressions);

        /// <summary>
        /// Removes a expression from this clause.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <returns>This clause.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null.</exception>
        ISqlGroupClause RemoveExpression(ISqlExpression expression);

        /// <summary>
        /// Remove all expressions from this clause.
        /// </summary>
        /// <returns>This clause.</returns>
        ISqlGroupClause ClearExpressions();
    }

    /// <summary>
    /// Represents a SQL HAVING clause.
    /// </summary>
    public interface ISqlHavingClause : ISqlClause {
        /// <summary>
        /// Gets or sets the filter condition.
        /// </summary>
        ISqlExpression Condition {
            get;
            set;
        }

        /// <summary>
        /// Set the filter condition.
        /// </summary>
        /// <param name="expression">A SQL expression</param>
        /// <returns>This clause</returns>
        ISqlHavingClause SetCondition(ISqlExpression expression);
    }

    /// <summary>
    /// Represents a SQL SELECT statement.
    /// </summary>
    public interface ISqlSelectStatement : ISqlStatement {
        /// <summary>
        /// Gets or sets the count of records will be fetched.
        /// If records count is undefined, return -1.
        /// </summary>
        int Count {
            get;
            set;
        }

        /// <summary>
        /// Sets the value of Count property.
        /// </summary>
        /// <param name="value">A int value.</param>
        /// <returns>This statement.</returns>
        ISqlSelectStatement SetCount(int value);

        /// <summary>
        /// Gets SELECT clause of this statement.
        /// </summary>
        ISqlSelectClause SelectClause {
            get;
        }

        /// <summary>
        /// Gets FROM clause of this statement.
        /// </summary>
        ISqlFromClause FromClause {
            get;
        }

        /// <summary>
        /// Gets WHERE clause of this statement.
        /// </summary>
        ISqlWhereClause WhereClause {
            get;
        }

        /// <summary>
        /// Gets clause of this statement.
        /// </summary>
        ISqlGroupClause GroupClause {
            get;
        }

        /// <summary>
        /// Gets ORDER clause of this statement.
        /// </summary>
        ISqlOrderClause OrderClause {
            get;
        }
    }

    /// <summary>
    /// Represents a SQL INSERT statement.
    /// </summary>
    public interface ISqlInsertStatement : ISqlStatement {
        /// <summary>
        /// Gets field and value clause of this statement.
        /// </summary>
        ISqlFieldValueClause FieldValueClause {
            get;
        }
    }

    /// <summary>
    /// Represents a SQL UPDATE statement.
    /// </summary>
    public interface ISqlUpdateStatement : ISqlStatement {
        /// <summary>
        /// Gets field and value clause of this statement.
        /// </summary>
        ISqlFieldValueClause FieldValueClause {
            get;
        }

        /// <summary>
        /// Gets FROM clause of this statement.
        /// </summary>
        ISqlFromClause FromClause {
            get;
        }

        /// <summary>
        /// Gets WHERE clause of this statement.
        /// </summary>
        ISqlWhereClause WhereClause {
            get;
        }
    }

    /// <summary>
    /// Represents a SQL DELETE statement.
    /// </summary>
    public interface ISqlDeleteStatement : ISqlStatement {
        /// <summary>
        /// Gets FROM clause of this statement.
        /// </summary>
        ISqlFromClause FromClause {
            get;
        }

        /// <summary>
        /// Gets WHERE clause of this statement.
        /// </summary>
        ISqlWhereClause WhereClause {
            get;
        }
    }

    /// <summary>
    /// Represents a SQL function.
    /// </summary>
    public interface ISqlFunction : ISqlExpression {
        /// <summary>
        /// Gets function name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets function arguments.
        /// </summary>
        ReadOnlyCollection<ISqlExpression> Arguments {
            get;
        }

        /// <summary>
        /// Add arguments.
        /// </summary>
        /// <param name="expressions">Arguments expressions.</param>
        /// <returns>This expression.</returns>
        ISqlFunction AddArgument(params ISqlExpression[] expressions);

        /// <summary>
        /// Add a SQL field to arguments.
        /// </summary>
        /// <param name="field">A SQL object.</param>
        /// <param name="distinct">Indicate whether this function compute distinct value of this field only.</param>
        /// <returns>This expression.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="field"/> is null.</exception>
        ISqlFunction AddArgument(ISqlObject field, bool distinct);

        /// <summary>
        /// Remove a arguments.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        /// <returns>This expression.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null.</exception>
        ISqlFunction RemoveArgument(ISqlExpression expression);

        /// <summary>
        /// Remove all arguments from this function.
        /// </summary>
        /// <returns>This expression.</returns>
        ISqlFunction ClearArguments();
    }

    /// <summary>
    /// Represents a SQL constant expressions factory.
    /// </summary>
    public interface IConstantSqlExpressionsFactory {
        /// <summary>
        /// Create a SQL expression from a boolean.
        /// </summary>
        /// <param name="value">A boolean.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(bool value);

        /// <summary>
        /// Create a SQL expression from a byte.
        /// </summary>
        /// <param name="value">A byte.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(byte value);

        /// <summary>
        /// Create a SQL expression from a char.
        /// </summary>
        /// <param name="value">A char.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(char value);

        /// <summary>
        /// Create SQL expression from a short int.
        /// </summary>
        /// <param name="value">A short int.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(short value);

        /// <summary>
        /// Create a SQL expression from a unsigned short int.
        /// </summary>
        /// <param name="value">A unsigned short int.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(ushort value);

        /// <summary>
        /// Create a SQL expression from a int.
        /// </summary>
        /// <param name="value">A int.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(int value);

        /// <summary>
        /// Create a SQL expression from a unsigned int.
        /// </summary>
        /// <param name="value">A unsigned int.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(uint value);

        /// <summary>
        /// Create SQL expression from a long int.
        /// </summary>
        /// <param name="value">A long int.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(long value);

        /// <summary>
        /// Create a SQL expression from a unsigned long int.
        /// </summary>
        /// <param name="value">A unsigned long int.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(ulong value);

        /// <summary>
        /// Create a SQL expression from a float.
        /// </summary>
        /// <param name="value">A float.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(float value);

        /// <summary>
        /// Create a SQL expression from a double float.
        /// </summary>
        /// <param name="value">A double float.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(double value);

        /// <summary>
        /// Create a SQL expression from a decimal.
        /// </summary>
        /// <param name="value">A decimal.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(decimal value);

        /// <summary>
        /// Create a SQL expression from a DateTime.
        /// </summary>
        /// <param name="value">A DateTime.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(DateTime value);

        /// <summary>
        /// Create a SQL expression from a string.
        /// </summary>
        /// <param name="value">A string.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(string value);

        /// <summary>
        /// Creates a SQL expression which represents start with the specified string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlExpression CreateStartWith(string value);

        /// <summary>
        /// Creates a SQL expression which represents end with the specified string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlExpression CreateEndWith(string value);

        /// <summary>
        /// Creates a SQL expression which represents contain the specified string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlExpression CreateContain(string value);

        /// <summary>
        /// Create a SQL expression from a byte array.
        /// </summary>
        /// <param name="value">A byte array.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(byte[] value);

        /// <summary>
        /// Create a SQL expression from a GUID.
        /// </summary>
        /// <param name="value">A byte array.</param>
        /// <returns>The created SQL expression.</returns>
        ISqlExpression Create(Guid value);
    }

    /// <summary>
    /// Indicates the order type in SQL ORDER clause.
    /// </summary>
    public enum SqlOrder {
        /// <summary>
        /// Order from small to large.
        /// </summary>
        Asc = 0x00,

        /// <summary>
        /// Order from large to small.
        /// </summary>
        Desc = 0x01,
    }

    /// <summary>
    /// Represents a SQL expression and it's order.
    /// </summary>
    public class SqlExpressionOrder {
        /// <summary>
        /// Initialize a new instance of SqlExpressionOrder.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="order">The order of this SQL expression.</param>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null.</exception>
        public SqlExpressionOrder(ISqlExpression expression, SqlOrder order) {
            if(expression == null) {
                throw new ArgumentException("SQL expression is null.", "expression");
            }

            this.Expression = expression;
            this.Order = order;
        }

        /// <summary>
        /// Gets the SQL expression.
        /// </summary>
        public ISqlExpression Expression {
            get;
            private set;
        }

        /// <summary>
        /// Gets the order of this SQL expression.
        /// </summary>
        public SqlOrder Order {
            get;
            private set;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is SqlExpressionOrder)) {
                return false;
            }

            SqlExpressionOrder order = (SqlExpressionOrder) obj;
            return this.Expression.Equals(order.Expression) && this.Order.Equals(order.Order);
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0} {1}", this.Expression.SqlString, this.Order);
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL operator.
    /// </summary>
    public interface ISqlOperator {
        /// <summary>
        /// Gets the operator name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Operate this operator with the specified expression.
        /// </summary>
        /// <param name="operands">SQL expressions as operands</param>
        /// <returns>The new expression after operation</returns>
        /// <exception cref="System.ArgumentException"><paramref name="operands"/> is null or empty or has null expresions or it's count is not matched with this operator.</exception>
        ISqlExpression Compute(params ISqlExpression[] operands);
    }

    /// <summary>
    /// Provides extension method of ISqlExpression objects.
    /// </summary>
    public static class SqlExpressionExtension {
        /// <summary>
        /// SQL AND operator.
        /// </summary>
        public static ISqlOperator AndOperator = new SqlOperator("And", new SqlKeywordsOperatorResultSqlStringProvider("AND"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// AND operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression And(this ISqlExpression left, ISqlExpression right) {
            return AndOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL OR operator.
        /// </summary>
        public static ISqlOperator OrOperator = new SqlOperator("Or", new SqlKeywordsOperatorResultSqlStringProvider("OR"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// OR operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Or(this ISqlExpression left, ISqlExpression right) {
            return OrOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL NOT operator.
        /// </summary>
        public static ISqlOperator NotOperator = new SqlOperator("Not", new SqlKeywordsOperatorResultSqlStringProvider("NOT"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// NOT operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Not(this ISqlExpression left) {
            return NotOperator.Compute(left);
        }

        /// <summary>
        /// SQL IN operator.
        /// </summary>
        public static ISqlOperator InOperator = new SqlOperator("In", new SqlInOperatorResultSqlStringProvider(), new SqlOperatorResultAllParametersProvider());

        /// <summary>
        /// IN operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expressions.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression In(this ISqlExpression left, params ISqlExpression[] right) {
            ISqlExpression[] expressions = new ISqlExpression[right.Length + 1];
            expressions[0] = left;
            Array.Copy(right, 0, expressions, 1, right.Length);

            return InOperator.Compute(expressions);
        }

        /// <summary>
        /// SQL LIKE operator.
        /// </summary>
        public static ISqlOperator LikeOperator = new SqlOperator("Like", new SqlKeywordsOperatorResultSqlStringProvider("LIKE"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// LIKE operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Like(this ISqlExpression left, ISqlExpression right) {
            return LikeOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL BETWEEN operator.
        /// </summary>
        public static ISqlOperator BetweenOperator = new SqlOperator("Between", new SqlKeywordsOperatorResultSqlStringProvider("BETWEEN", "AND"), new SqlKeywordsOperatorResultParametersProvider(2));

        /// <summary>
        /// BETWEEN operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="middle">Middle expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Between(this ISqlExpression left, ISqlExpression middle, ISqlExpression right) {
            return BetweenOperator.Compute(left, middle, right);
        }

        /// <summary>
        /// SQL EQUAL operator.
        /// </summary>
        public static ISqlOperator EqualOperator = new SqlOperator("Equal", new SqlKeywordsOperatorResultSqlStringProvider("="), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// EQUAL operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Equal(this ISqlExpression left, ISqlExpression right) {
            return EqualOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL GREATER THAN operator.
        /// </summary>
        public static ISqlOperator GreaterThanOperator = new SqlOperator("GreaterThan", new SqlKeywordsOperatorResultSqlStringProvider(">"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// GREATER THAN operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression GreaterThan(this ISqlExpression left, ISqlExpression right) {
            return GreaterThanOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL LESS THAN operator.
        /// </summary>
        public static ISqlOperator LessThanOperator = new SqlOperator("LessThan", new SqlKeywordsOperatorResultSqlStringProvider("<"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// LESS THAN operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression LessThan(this ISqlExpression left, ISqlExpression right) {
            return LessThanOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL GREATER EQUAL operator.
        /// </summary>
        public static ISqlOperator GreaterEqualOperator = new SqlOperator("GreatherEqual", new SqlKeywordsOperatorResultSqlStringProvider(">="), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// GREATER EQUAL operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression GreaterEqual(this ISqlExpression left, ISqlExpression right) {
            return GreaterEqualOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL LESS EQUAL operator.
        /// </summary>
        public static ISqlOperator LessEqualOperator = new SqlOperator("LessEqual", new SqlKeywordsOperatorResultSqlStringProvider("<="), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// LESS EQUAL operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression LessEqual(this ISqlExpression left, ISqlExpression right) {
            return LessEqualOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL BITWISE AND operator.
        /// </summary>
        public static ISqlOperator BitwiseAndOperator = new SqlOperator("BitwiseAnd", new SqlKeywordsOperatorResultSqlStringProvider("&"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// BITWISE AND operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression BitwiseAnd(this ISqlExpression left, ISqlExpression right) {
            return BitwiseAndOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL BITWISE OR operator.
        /// </summary>
        public static ISqlOperator BitwiseOrOperator = new SqlOperator("BitwiseOr", new SqlKeywordsOperatorResultSqlStringProvider("|"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// BITWISE OR operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression BitwiseOr(this ISqlExpression left, ISqlExpression right) {
            return BitwiseOrOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL EXCLUSIVE OR operator.
        /// </summary>
        public static ISqlOperator ExclusiveOrOperator = new SqlOperator("ExclusiveOr", new SqlKeywordsOperatorResultSqlStringProvider("^"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// EXCLUSIVE OR operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression ExclusiveOr(this ISqlExpression left) {
            return ExclusiveOrOperator.Compute(left);
        }

        /// <summary>
        /// SQL ADD operator.
        /// </summary>
        public static ISqlOperator AddOperator = new SqlOperator("Add", new SqlKeywordsOperatorResultSqlStringProvider("+"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// ADD operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Add(this ISqlExpression left, ISqlExpression right) {
            return AddOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL SUBTRACT operator.
        /// </summary>
        public static ISqlOperator SubtractOperator = new SqlOperator("Subtract", new SqlKeywordsOperatorResultSqlStringProvider("-"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// SUBTRACT operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Subtract(this ISqlExpression left, ISqlExpression right) {
            return SubtractOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL MULTIPLY operator.
        /// </summary>
        public static ISqlOperator MultiplyOperator = new SqlOperator("Multiply", new SqlKeywordsOperatorResultSqlStringProvider("*"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// MULTIPLY operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Multiply(this ISqlExpression left, ISqlExpression right) {
            return MultiplyOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL DIVIDE operator.
        /// </summary>
        public static ISqlOperator DivideOperator = new SqlOperator("Divide", new SqlKeywordsOperatorResultSqlStringProvider("/"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// DIVIDE operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Divide(this ISqlExpression left, ISqlExpression right) {
            return DivideOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL MODE operator.
        /// </summary>
        public static ISqlOperator ModeOperator = new SqlOperator("Mode", new SqlKeywordsOperatorResultSqlStringProvider("%"), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// MODE operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Mode(this ISqlExpression left, ISqlExpression right) {
            return ModeOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL Assignment operator.
        /// </summary>
        public static ISqlOperator AssignOperator = new SqlOperator("Assignment", new SqlAssignOperatorResultSqlStringProvider(), new SqlKeywordsOperatorResultParametersProvider(1));

        /// <summary>
        /// MODE operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression Assign(this ISqlExpression left, ISqlExpression right) {
            return AssignOperator.Compute(left, right);
        }

        /// <summary>
        /// SQL IS NULL operator.
        /// </summary>
        public static ISqlOperator IsNullOperator = new SqlOperator("Is null", new SqlIsNullOperatorResultSqlStringProvider(), new SqlOperatorResultAllParametersProvider());

        /// <summary>
        /// IS NULL operation.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <returns>The expression after computing.</returns>
        public static ISqlExpression IsNull(this ISqlExpression left) {
            return IsNullOperator.Compute(left);
        }
    }

    /// <summary>
    /// Represents a common SQL operator.
    /// </summary>
    internal class SqlOperator : ISqlOperator {
        /// <summary>
        /// Initialize a new instance of SqlOperator.
        /// </summary>
        /// <param name="name">Operator name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlOperator(string name, ISqlOperatorResultSqlStringProvider stringProvider, ISqlOperatorResultParametersProvider parametersProvider) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Operator name is null or empty.", "name");
            }
            if(stringProvider == null) {
                throw new ArgumentException("Operator result SQL string provider is null.", "stringProvider");
            }
            if(parametersProvider == null) {
                throw new ArgumentException("Operator result parameters provider is null.", "parametersProvider");
            }

            this.m_name = name;
            this.m_stringProvider = stringProvider;
            this.m_parametersProvider = parametersProvider;
        }

        /// <summary>
        /// operator name.
        /// </summary>
        protected string m_name;

        protected ISqlOperatorResultSqlStringProvider m_stringProvider;
        /// <summary>
        /// Gets the SQL string provider of result.
        /// </summary>
        public virtual ISqlOperatorResultSqlStringProvider ResultSqlStringProvider {
            get {
                return this.m_stringProvider;
            }
        }

        protected ISqlOperatorResultParametersProvider m_parametersProvider;
        /// <summary>
        /// Gets the parameters provider of result.
        /// </summary>
        public virtual ISqlOperatorResultParametersProvider ResultParametersProvider {
            get {
                return this.m_parametersProvider;
            }
        }

        #region ISqlOperator Members

        /// <inheritdoc />
        public virtual string Name {
            get {
                return this.m_name;
            }
        }

        /// <inheritdoc />
        public virtual ISqlExpression Compute(params ISqlExpression[] operands) {
            //bounds checking
            if(operands == null || operands.Length == 0) {
                throw new ArgumentException("operands", "Operands is null or empty or has null expresions.");
            }
            foreach(ISqlExpression operand in operands) {
                if(operand == null) {
                    throw new ArgumentException("operands", "Operands is null or empty or has null expresions.");
                }
            }

            return new SqlStringExpression(this.m_stringProvider.GetSqlString(operands), this.m_parametersProvider.GetParameters(operands));
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.m_name.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is SqlOperator)) {
                return false;
            }
            return this.m_name.Equals(((SqlOperator) obj).m_name);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.m_name;
        }

        #endregion
    }

    /// <summary>
    /// Create a SQL expression from a string and some parameters.
    /// </summary>
    public class SqlStringExpression : SqlExpression {
        /// <summary>
        /// Initialize a new instance of SqlExpression class.
        /// </summary>
        /// <param name="sqlString">SQL string.</param>
        /// <param name="parameters">Parameters in this expression</param>
        /// <exception cref="System.ArgumentException"><paramref name="sqlString"/> is null or empty.</exception>
        public SqlStringExpression(string sqlString, params IDataParameter[] parameters) {
            if(string.IsNullOrWhiteSpace(sqlString)) {
                throw new ArgumentException("sqlString is null or empty", "sqlString");
            }

            this.m_sqlString = sqlString;
            if(parameters != null && parameters.Length > 0) {
                foreach(IDataParameter parameter in parameters) {
                    if(parameter != null && !this.m_parameters.Contains(parameter)) {
                        this.m_parameters.Add(parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Create a SQL expression from a IDataParameter.
        /// </summary>
        /// <param name="parameter">A IDataParameter object.</param>
        /// <returns>A SQL expression.</returns>
        public static ISqlExpression FromParameter(IDataParameter parameter) {
            return new SqlStringExpression(parameter.ParameterName, parameter);
        }
    }

    /// <summary>
    /// A SQL expression serve as a named SQL object.
    /// </summary>
    public interface ISqlObject : ISqlExpression {
        /// <summary>
        /// Gets the object name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets the restrictive object name.
        /// </summary>
        string Fullname {
            get;
        }

        /// <summary>
        /// Gets the owner of this object.
        /// </summary>
        ISqlObject Owner {
            get;
        }

        /// <summary>
        /// Gets the based SQL expression.
        /// </summary>
        ISqlExpression Expression {
            get;
        }
    }

    /// <summary>
    /// Represents a common SQL object.
    /// </summary>
    public class SqlObject : SqlExpression, ISqlObject {
        /// <summary>
        /// Initialize a new instance of SqlObject.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="name">Object name.</param>
        /// <param name="fullnameProvider">A fullname provider.</param>
        /// <param name="stringProvider">A SQL string provider.</param>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="fullnameProvider"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stringProvider"/> is null.</exception>
        public SqlObject(ISqlExpression expression, string name, ISqlObjectFullnameProvider fullnameProvider, ISqlObjectSqlStringProvider stringProvider)
            : this(null, expression, name, fullnameProvider, stringProvider) {
        }

        /// <summary>
        /// Initialize a new instance of SqlObject.
        /// </summary>
        /// <param name="owner">Object owner.</param>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="name">Object name.</param>
        /// <param name="fullnameProvider">A fullname provider.</param>
        /// <param name="stringProvider">A SQL string provider.</param>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="fullnameProvider"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="stringProvider"/> is null.</exception>
        public SqlObject(ISqlObject owner, ISqlExpression expression, string name, ISqlObjectFullnameProvider fullnameProvider, ISqlObjectSqlStringProvider stringProvider) {
            if(expression == null) {
                throw new ArgumentException("Based SQL expression is null.", "expression");
            }
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Object name is null.", "name");
            }

            this.m_owner = owner;
            this.m_expression = expression;
            this.m_name = name;
            this.m_fullnameProvider = fullnameProvider;
            this.m_stringProvider = stringProvider;
        }

        /// <summary>
        /// Object owner.
        /// </summary>
        protected ISqlObject m_owner;

        /// <summary>
        /// Based SQL expression.
        /// </summary>
        protected ISqlExpression m_expression;

        /// <summary>
        /// Object name.
        /// </summary>
        protected string m_name;

        protected ISqlObjectFullnameProvider m_fullnameProvider;
        /// <summary>
        /// Gets <typeparamref name="ISqlObjectFullnameProvider"/> of this object.
        /// </summary>
        public ISqlObjectFullnameProvider FullnameProvider {
            get {
                return this.m_fullnameProvider;
            }
        }

        protected ISqlObjectSqlStringProvider m_stringProvider;
        /// <summary>
        /// Gets <typeparamref name="ISqlObjectStringProvider"/> of this object.
        /// </summary>
        public ISqlObjectSqlStringProvider SqlStringProvider {
            get {
                return this.m_stringProvider;
            }
        }

        #region ISqlObject Members

        /// <inheritdoc />
        public string Name {
            get {
                return this.m_name;
            }
        }

        /// <inheritdoc />
        public string Fullname {
            get {
                return this.m_fullnameProvider.GetFullname(this);
            }
        }

        /// <inheritdoc />
        public ISqlObject Owner {
            get {
                return this.m_owner;
            }
        }

        /// <inheritdoc />
        public ISqlExpression Expression {
            get {
                return this.m_expression;
            }
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                return (this.m_sqlString = this.m_stringProvider.GetSqlString(this));
            }
        }

        /// <inheritdoc />
        public override ReadOnlyCollection<IDataParameter> Parameters {
            get {
                return this.m_expression.Parameters;
            }
        }

        /// <summary>
        /// Create a SQL object from a server variable.
        /// </summary>
        /// <param name="variable">Server variable name.</param>
        /// <returns>A SQL object</returns>
        /// <exception cref="System.ArgumentException"><paramref name="variable"/> is null or empty.</exception>
        public static ISqlObject FromVariable(string variable) {
            return FromVariable(new SqlStringExpression(variable));
        }

        /// <summary>
        /// Create a SQL object from a server variable.
        /// </summary>
        /// <param name="variable">A Server variable expression.</param>
        /// <returns>A SQL object</returns>
        /// <exception cref="System.ArgumentException"><paramref name="variable"/> is null.</exception>
        public static ISqlObject FromVariable(ISqlExpression variable) {
            return new SqlObject(variable, variable.SqlString, new SqlObjectExpressionSqlStringFullnameProvider(), new SqlObjectExpressionSqlStringSqlStringProvider());
        }

        /// <summary>
        /// Create a SQL object from a function.
        /// </summary>
        /// <param name="function">A SQL function.</param>
        /// <returns>A SQL object</returns>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        public static ISqlObject FromFunction(ISqlFunction function) {
            return new SqlObject(function, function.Name, new SqlObjectExpressionSqlStringFullnameProvider(), new SqlObjectExpressionSqlStringSqlStringProvider());
        }
    }

    /// <summary>
    /// Provide the fullname of a ISqlObject.
    /// </summary>
    public interface ISqlObjectFullnameProvider {
        /// <summary>
        /// Gets fullname of this specified object.
        /// </summary>
        /// <param name="obj">A SQL object.</param>
        /// <returns>The fullname of <paramref name="obj"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is null.</exception>
        string GetFullname(ISqlObject obj);
    }

    /// <summary>
    /// Provide the SQL string of a ISqlObject.
    /// </summary>
    public interface ISqlObjectSqlStringProvider {
        /// <summary>
        /// Gets SQL string of this specified object.
        /// </summary>
        /// <param name="obj">A SQL object.</param>
        /// <returns>The SQL string of <paramref name="obj"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is null.</exception>
        string GetSqlString(ISqlObject obj);
    }

    /// <summary>
    /// Provide SQL string of a SQL object with it's fullname.
    /// </summary>
    internal class SqlObjectFullnameSqlStringProvider : ISqlObjectSqlStringProvider {
        #region ISqlObjectSqlStringProvider members

        /// <inheritdoc />
        public virtual string GetSqlString(ISqlObject obj) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            return obj.Fullname;
        }

        #endregion
    }

    /// <summary>
    /// Provide the SQL string of the result after computing.
    /// </summary>
    public interface ISqlOperatorResultSqlStringProvider {
        /// <summary>
        /// Gets the SQL string of the results obtained by calculating with the specified operands.
        /// </summary>
        /// <param name="operands">Operands.</param>
        /// <returns>The SQL string of the results obtained by calculating with <paramref name="operands"/>.</returns>
        string GetSqlString(params ISqlExpression[] operands);
    }

    /// <summary>
    /// Provide the parameters of the result after computing.
    /// </summary>
    public interface ISqlOperatorResultParametersProvider {
        /// <summary>
        /// Gets the parameters of the results obtained by calculating with the specified operands.
        /// </summary>
        /// <param name="operands">Operands.</param>
        /// <returns>The parameters of the results obtained by calculating with <paramref name="operands"/>.</returns>
        IDataParameter[] GetParameters(params ISqlExpression[] operands);
    }

    /// <summary>
    /// Represents a SQL operator which use some keywords to join operands.
    /// </summary>
    internal class SqlKeywordsOperatorResultSqlStringProvider : ISqlOperatorResultSqlStringProvider {
        /// <summary>
        /// Initialize a new instance of SqlKeywordsOperatorResultSqlStringProvider.
        /// </summary>
        /// <param name="keywords">Operator keywords.</param>
        /// <exception cref="System.ArgumentException"><paramref name="keywords"/> is null or empty.</exception>
        public SqlKeywordsOperatorResultSqlStringProvider(params string[] keywords) {
            if(keywords == null || keywords.Length == 0) {
                throw new ArgumentException("Operator keywords is null or empty.", "keywords");
            }
            foreach(string keyword in keywords) {
                if(string.IsNullOrWhiteSpace(keyword)) {
                    throw new ArgumentException("Operator keywords have null or empty keywords.", "keywords");
                }
            }

            this.m_keywords = keywords;
        }

        /// <summary>
        /// keywords used in this operator.
        /// </summary>
        private string[] m_keywords;

        #region ISqlOperatorResultSqlStringProvider Members

        /// <inheritdoc />
        public virtual string GetSqlString(params ISqlExpression[] operands) {
            if(this.m_keywords.Length > 1 && operands.Length <= this.m_keywords.Length) {
                throw new ArgumentException("Operands's count is not matched with this operator.", "operands");
            }

            StringBuilder sqlString = new StringBuilder();

            switch(operands.Length) {
                case 1:
                    sqlString.AppendFormat("{0} ({1})", this.m_keywords[0], operands[0].SqlString);
                    break;
                default:
                    sqlString.AppendFormat("({0})", operands[0].SqlString);
                    for(int i = 0; i < m_keywords.Length; i++) {
                        sqlString.AppendFormat(" {0}", m_keywords[i]);
                        sqlString.AppendFormat(" ({0})", operands[i + 1].SqlString);
                    }
                    break;
            }

            return sqlString.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL operator which use some keywords to join operands.
    /// </summary>
    internal class SqlKeywordsOperatorResultParametersProvider : ISqlOperatorResultParametersProvider {
        /// <summary>
        /// Initialize a new instance of SqlKeywordsOperatorResultParametersProvider.
        /// </summary>
        /// <param name="count">Operator keywords count.</param>
        /// <exception cref="System.ArgumentException"><paramref name="count"/> is less than or equals zero.</exception>
        public SqlKeywordsOperatorResultParametersProvider(int count) {
            if(count <= 0) {
                throw new ArgumentException("Operator keywords count is less than or equals zero.");
            }

            this.m_keywordsCount = count;
        }

        /// <summary>
        /// keywords count.
        /// </summary>
        private int m_keywordsCount;

        #region ISqlOperatorResultParametersProvider Members

        /// <inheritdoc />
        public virtual IDataParameter[] GetParameters(params ISqlExpression[] operands) {
            List<IDataParameter> all = new List<IDataParameter>();
            for(int i = 0; i < m_keywordsCount + 1 && i < operands.Length; i++) {
                all.AddRange(operands[i].Parameters);
            }

            ICollection<IDataParameter> parameters = new List<IDataParameter>(all.Count);
            if(all.Count > 0) {
                foreach(IDataParameter parameter in all) {
                    if(!parameters.Contains(parameter)) {
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters.ToArray<IDataParameter>();
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL IN operator.
    /// </summary>
    internal class SqlInOperatorResultSqlStringProvider : ISqlOperatorResultSqlStringProvider {
        #region ISqlOperatorResultSqlStringProvider Members

        /// <inheritdoc />
        public virtual string GetSqlString(params ISqlExpression[] operands) {
            StringBuilder sqlString = new StringBuilder(string.Format("({0}) IN (", operands[0].SqlString));

            sqlString.Append(operands[1].SqlString);
            for(int i = 2; i < operands.Length; i++) {
                sqlString.AppendFormat(", {0}", operands[i].SqlString);
            }
            sqlString.Append(")");

            return sqlString.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL 
    /// </summary>
    internal class SqlOperatorResultAllParametersProvider : ISqlOperatorResultParametersProvider {
        #region ISqlOperatorResultParametersProvider Members

        /// <inheritdoc />
        public virtual IDataParameter[] GetParameters(params ISqlExpression[] operands) {
            List<IDataParameter> all = new List<IDataParameter>();
            for(int i = 0; i < operands.Length; i++) {
                all.AddRange(operands[i].Parameters);
            }

            ICollection<IDataParameter> parameters = new List<IDataParameter>(all.Count);
            if(all.Count > 0) {
                foreach(IDataParameter parameter in all) {
                    if(!parameters.Contains(parameter)) {
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters.ToArray<IDataParameter>();
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL function.
    /// </summary>
    public class SqlFunction : SqlCombinedExpression<ISqlExpression>, ISqlFunction {
        /// <summary>
        /// Initialize a new instance of SqlFunction class.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlFunction(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Function name is null or empty.", "name");
            }

            this.m_name = name;
        }

        /// <summary>
        /// Function name.
        /// </summary>
        protected string m_name;

        #region SqlCombinedExpression Members

        /// <inheritdoc />
        protected override string GetSqlString() {
            StringBuilder sqlString = new StringBuilder(string.Format("{0}(", this.m_name));

            if(this.m_components.Count > 0) {
                sqlString.Append(this.m_components[0].SqlString);
                for(int i = 1; i < this.m_components.Count; i++) {
                    sqlString.AppendFormat(", {0}", this.m_components[i].SqlString);
                }
            }

            sqlString.Append(")");
            return sqlString.ToString();
        }

        /// <inheritdoc />
        public override bool IsAllowDuplicateComponent {
            get {
                return true;
            }
        }

        #endregion

        #region ISqlFunction Members

        /// <inheritdoc />
        public virtual string Name {
            get {
                return this.m_name;
            }
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<ISqlExpression> Arguments {
            get {
                return this.m_components.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public virtual ISqlFunction AddArgument(params ISqlExpression[] expressions) {
            this.AddComponent(expressions);

            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFunction AddArgument(ISqlObject field, bool distinct) {
            if(field == null) {
                throw new ArgumentException("filed is null.", "field");
            }

            this.AddArgument(new SqlStringExpression(string.Format(distinct ? "DISTINCT {0}" : "{0}", field.Fullname), field.Parameters.ToArray<IDataParameter>()));
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFunction RemoveArgument(ISqlExpression expression) {
            if(expression == null) {
                throw new ArgumentException("expression is null.", "expression");
            }

            this.RemoveComponent(expression);
            return this;
        }

        public virtual ISqlFunction ClearArguments() {
            this.ClearComponents();
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL databse, it is a SQL expressions factory.
    /// </summary>
    public interface ISqlExpressionFactory {
        /// <summary>
        /// Gets the constanct expression factory.
        /// </summary>
        IConstantSqlExpressionsFactory ConstantFactory {
            get;
        }

        /// <summary>
        /// Create a SQL data source.
        /// </summary>
        /// <param name="name">Table name or view name.</param>
        /// <returns>A ISqlObject.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        ISqlObject CreateSource(string name);

        /// <summary>
        /// Create a SQL data source from a subquery.
        /// </summary>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <param name="name">Subquery alias.</param>
        /// <returns>A ISqlObject.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="statement"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        ISqlObject CreateSource(ISqlSelectStatement statement, string name);

        /// <summary>
        /// Create a SQL field.
        /// </summary>
        /// <param name="source">A SQL source.</param>
        /// <param name="name">Field name.</param>
        /// <returns>A ISqlObject.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        ISqlObject CreateField(ISqlObject source, string name);

        /// <summary>
        /// Create a SQL field.
        /// </summary>
        /// <param name="source">A SQL source.</param>
        /// <param name="name">Field name.</param>
        /// <param name="alias">Field alias</param>
        /// <returns>A ISqlObject.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        ISqlObject CreateField(ISqlObject source, string name, string alias);

        /// <summary>
        /// Create a SQL field from a server variable.
        /// </summary>
        /// <param name="variable">Server variable name.</param>
        /// <param name="name">Field name.</param>
        /// <returns>A ISqlObject.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="variable"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        ISqlObject CreateField(string variable, string name);

        /// <summary>
        /// Create a SQL field from a function.
        /// </summary>
        /// <param name="function">A SQL function.</param>
        /// <param name="name">Field name.</param>
        /// <returns>A ISqlObject.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        ISqlObject CreateField(ISqlFunction function, string name);

        /// <summary>
        /// Create a SQL SELECT statement.
        /// </summary>
        /// <returns>A SQL SELECT statement</returns>
        ISqlSelectStatement CreateSelectStatement();

        /// <summary>
        /// Create a SQL INSERT statement.
        /// </summary>
        /// <returns>A SQL INSERT statement</returns>
        ISqlInsertStatement CreateInsertStatement();

        /// <summary>
        /// Create a SQL UPDATE statement.
        /// </summary>
        /// <returns>A SQL UPDATE statement</returns>
        ISqlUpdateStatement CreateUpdateStatement();

        /// <summary>
        /// Create a SQL DELETE statement.
        /// </summary>
        /// <returns>A SQL DELETE statement</returns>
        ISqlDeleteStatement CreateDeleteStatement();
    }

    /// <summary>
    /// A combined SQL expression, which was consit by some other expresions.
    /// </summary>
    public abstract class SqlCombinedExpression<T> : SqlExpression where T : ISqlExpression {
        /// <summary>
        /// Components of this expression.
        /// </summary>
        protected List<T> m_components = new List<T>();

        /// <summary>
        /// Add components to this expression.
        /// </summary>
        /// <param name="expressions">SQL expressions.</param>
        protected virtual void AddComponent(params T[] expressions) {
            if(expressions == null || expressions.Length == 0) {
                return;
            }

            this.m_sqlString = null;
            List<IDataParameter> all = new List<IDataParameter>(expressions.Length);

            foreach(T expression in expressions) {
                if(expression == null) {
                    continue;
                }

                if(expression.Parameters.Count > 0) {
                    all.AddRange(expression.Parameters);
                }

                if(this.IsAllowDuplicateComponent || !this.m_components.Contains(expression)) {
                    this.m_components.Add(expression);
                }
            }

            foreach(IDataParameter parameter in all) {
                if(parameter != null && !this.m_parameters.Contains(parameter)) {
                    this.m_parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// Remove a component from this expression.
        /// </summary>
        /// <param name="expressions">A SQL expression.</param>
        protected virtual void RemoveComponent(T expression) {
            if(expression != null && this.m_components.Contains(expression)) {
                this.m_sqlString = null;
                this.m_components.Remove(expression);
                /***************************************
                 * Don't remove parameters here, because other expression may be use these parameters.
                 * *************************************/
            }
        }

        /// <summary>
        /// Remove all components from this expression.
        /// </summary>
        protected virtual void ClearComponents() {
            this.m_sqlString = null;
            this.m_components.Clear();
            this.m_parameters.Clear();
        }

        /// <summary>
        /// Gets SQL string of this expression.
        /// </summary>
        /// <returns>SQL string of this expression.</returns>
        protected abstract string GetSqlString();

        /// <summary>
        /// Gets whether this expression allow duplicate component.
        /// </summary>
        public abstract bool IsAllowDuplicateComponent {
            get;
        }

        /// <inheritdoc />
        public override string SqlString {
            get {
                return this.m_sqlString ?? (this.m_sqlString = this.GetSqlString());
            }
        }
    }

    /// <summary>
    /// Represents a common SQL expression implementation.
    /// </summary>
    public abstract class SqlExpression : ISqlExpression {
        static SqlExpression() {
            True = new SqlStringExpression("1=1");
            False = new SqlStringExpression("1=2");
        }

        /// <summary>
        /// SQL string.
        /// </summary>
        protected string m_sqlString;

        /// <summary>
        /// SQL parameters.
        /// </summary>
        protected List<IDataParameter> m_parameters = new List<IDataParameter>();

        /// <summary>
        /// Gets a const ISqlExpression object which represents SQL true condition.
        /// </summary>
        public static readonly ISqlExpression True;

        /// <summary>
        /// Gets a const ISqlExpression object which represents SQL false condition.
        /// </summary>
        public static readonly ISqlExpression False;

        #region ISqlExpression Members

        /// <inheritdoc />
        public virtual string SqlString {
            get {
                return this.m_sqlString;
            }
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<IDataParameter> Parameters {
            get {
                return this.m_parameters.AsReadOnly();
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.SqlString != null ? this.SqlString.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is SqlExpression)) {
                return false;
            }
            return string.Equals(this.SqlString, ((SqlExpression) obj).SqlString);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.SqlString ?? string.Empty;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL GROUP clause.
    /// </summary>
    internal class SqlGroupClause : SqlCombinedExpression<ISqlExpression>, ISqlGroupClause {
        /// <summary>
        /// The having clause.
        /// </summary>
        private ISqlHavingClause m_havingClause = new SqlHavingClause();

        protected virtual string GetSqlString(ISqlExpression expression) {
            return expression is ISqlObject ? ((ISqlObject) expression).Fullname : expression.SqlString;
        }

        #region SqlCombinedExpression Members

        /// <inheritdoc />
        protected override string GetSqlString() {
            if(this.m_components.Count == 0) {
                return string.Empty;
            }

            StringBuilder sqlString = new StringBuilder(this.Keyword);
            sqlString.AppendFormat(" {0}", this.GetSqlString(this.m_components[0]));
            for(int i = 1; i < this.m_components.Count; i++) {
                sqlString.AppendFormat(", {0}", this.GetSqlString(this.m_components[i]));
            }
            if(this.m_havingClause.IsAvailable) {
                sqlString.AppendFormat(" {0}", this.m_havingClause.SqlString);
            }

            return sqlString.ToString();
        }

        /// <inheritdoc />
        public override bool IsAllowDuplicateComponent {
            get {
                return false;
            }
        }

        #endregion

        #region ISqlGroupClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "GROUP BY";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_components.Count > 0;
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<ISqlExpression> Expressions {
            get {
                return this.m_components;
            }
        }

        /// <inheritdoc />
        public ISqlHavingClause HavingClause {
            get {
                return this.m_havingClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlGroupClause AddExpressions(params ISqlExpression[] expressions) {
            this.AddComponent(expressions);
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlGroupClause RemoveExpression(ISqlExpression expression) {
            if(expression == null) {
                throw new ArgumentException("expression is null.", "expression");
            }

            this.RemoveComponent(expression);
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlGroupClause ClearExpressions() {
            this.ClearComponents();
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL HAVING clause.
    /// </summary>
    internal class SqlHavingClause : SqlExpression, ISqlHavingClause {
        /// <summary>
        /// Query condition.
        /// </summary>
        protected ISqlExpression m_condition;

        #region ISqlHavingClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "HAVING";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_condition != null;
            }
        }

        /// <inheritdoc />
        public virtual ISqlExpression Condition {
            get {
                return this.m_condition;
            }
            set {
                this.m_parameters.Clear();
                if((this.m_condition = value) != null) {
                    this.m_parameters.AddRange(value.Parameters);
                }
            }
        }

        /// <inheritdoc />
        public virtual ISqlHavingClause SetCondition(ISqlExpression expression) {
            this.Condition = expression;
            return this;
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                return this.m_sqlString = (this.m_condition != null ? string.Format("{0} {1}", this.Keyword, this.m_condition.SqlString) : string.Empty);
            }
        }
    }

    /// <summary>
    /// Represents a SQL ORDER clause.
    /// </summary>
    internal class SqlOrderClause : SqlCombinedExpression<ISqlExpression>, ISqlOrderClause {
        /// <summary>
        /// Field order map.
        /// </summary>
        protected IDictionary<ISqlExpression, SqlOrder> m_orders = new Dictionary<ISqlExpression, SqlOrder>();

        /// <summary>
        /// Get SQL string of the specified SQL order.
        /// </summary>
        /// <param name="order">SQL order.</param>
        /// <returns>SQL string of the specified SQL order.</returns>
        protected virtual string GetOrderString(SqlOrder order) {
            switch(order) {
                case SqlOrder.Desc:
                    return "DESC";
                default:
                    return "ASC";
            }
        }

        protected virtual string GetSqlString(ISqlExpression expression) {
            return expression is ISqlObject ? ((ISqlObject) expression).Fullname : expression.SqlString;
        }

        #region SqlCombinedExpression Members

        /// <inheritdoc />
        protected override string GetSqlString() {
            if(this.m_components.Count == 0) {
                return string.Empty;
            }

            StringBuilder sqlString = new StringBuilder(this.Keyword);
            sqlString.AppendFormat(" ({0}) {1}", this.GetSqlString(this.m_components[0]), this.GetOrderString(this.m_orders[this.m_components[0]]));
            for(int i = 1; i < this.m_components.Count; i++) {
                sqlString.AppendFormat(", ({0}) {1}", this.GetSqlString(this.m_components[i]), this.GetOrderString(this.m_orders[this.m_components[1]]));
            }

            return sqlString.ToString();
        }

        /// <inheritdoc />
        public override bool IsAllowDuplicateComponent {
            get {
                return false;
            }
        }

        #endregion

        #region ISqlOrderClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "ORDER BY";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_components.Count > 0;
            }
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<ISqlExpression> Expressions {
            get {
                return this.m_components.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public virtual ISqlOrderClause AddExpression(ISqlExpression expression, SqlOrder order) {
            if(expression == null) {
                throw new ArgumentNullException("expression");
            }

            this.AddComponent(expression);
            this.m_orders[expression] = order;
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlOrderClause AddExpression(SqlExpressionOrder order) {
            if(order == null) {
                throw new ArgumentNullException("order");
            }

            return this.AddExpression(order.Expression, order.Order);
        }

        /// <inheritdoc />
        public virtual ISqlOrderClause RemoveExpression(ISqlExpression expression) {
            if(expression == null) {
                throw new ArgumentException("expression is null.", "expression");
            }

            this.RemoveComponent(expression);
            if(this.m_orders.ContainsKey(expression)) {
                this.m_orders.Remove(expression);
            }
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlOrderClause ClearExpressions() {
            this.ClearComponents();
            this.m_orders.Clear();
            return this;
        }

        /// <inheritdoc />
        public virtual SqlOrder GetExpressionOrder(ISqlExpression expression) {
            if(expression == null) {
                throw new ArgumentException("expression is null.", "expression");
            }
            if(!this.m_components.Contains(expression)) {
                throw new ArgumentOutOfRangeException("expression", "expression is not existing in this clause.");
            }

            return this.m_orders[expression];
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL WHERE clause.
    /// </summary>
    internal class SqlWhereClause : SqlExpression, ISqlWhereClause {
        /// <summary>
        /// Query condition.
        /// </summary>
        protected ISqlExpression m_condition;

        #region ISqlWhereClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "WHERE";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_condition != null;
            }
        }

        /// <inheritdoc />
        public virtual ISqlExpression Condition {
            get {
                return this.m_condition;
            }
            set {
                this.m_parameters.Clear();
                if((this.m_condition = value) != null) {
                    this.m_parameters.AddRange(value.Parameters);
                }
            }
        }

        /// <inheritdoc />
        public virtual ISqlWhereClause SetCondition(ISqlExpression expression) {
            this.Condition = expression;
            return this;
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                return this.m_sqlString = (this.m_condition != null ? string.Format("{0} {1}", this.Keyword, this.m_condition.SqlString) : string.Empty);
            }
        }
    }

    /// <summary>
    /// Represents a SQL FROM clause.
    /// </summary>
    internal class SqlFromClause : SqlExpression, ISqlFromClause {
        /// <summary>
        /// Data source.
        /// </summary>
        protected ISqlExpression m_source;

        /// <summary>
        /// Get parameters of these specified expresions.
        /// </summary>
        /// <param name="expressions">SQL expressions.</param>
        /// <returns>Parameters of <paramref name="expressions"/>.</returns>
        protected virtual IDataParameter[] GetParameters(params ISqlExpression[] expressions) {
            if(expressions == null || expressions.Length == 0) {
                return new IDataParameter[0];
            }

            List<IDataParameter> all = new List<IDataParameter>(expressions.Length);
            foreach(ISqlExpression expression in expressions) {
                if(expression != null && expression.Parameters.Count > 0) {
                    all.AddRange(expression.Parameters);
                }
            }

            List<IDataParameter> parameters = new List<IDataParameter>(all.Count);
            if(all.Count > 0) {
                foreach(IDataParameter parameter in all) {
                    if(parameter != null && !parameters.Contains(parameter)) {
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters.ToArray();
        }

        #region ISqlFromClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "FROM";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_source != null;
            }
        }

        /// <inheritdoc />
        public virtual ISqlExpression Source {
            get {
                return this.m_source;
            }
            set {
                this.m_parameters.Clear();
                if((this.m_source = value) != null) {
                    this.m_parameters.AddRange(value.Parameters);
                }
            }
        }

        /// <inheritdoc />
        public virtual ISqlFromClause SetSource(ISqlObject source) {
            this.Source = source;
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFromClause CrossJoin(ISqlObject source) {
            if(this.m_source == null) {
                throw new InvalidOperationException("There is not a data source currently.");
            }
            if(source == null) {
                throw new ArgumentException("Source is null.", "source");
            }

            this.Source = new SqlStringExpression(string.Format("{0} CROSS JOIN {1}", this.m_source.SqlString, source.SqlString), this.GetParameters(this.m_source, source));
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFromClause InnerJoin(ISqlObject source, ISqlExpression condition) {
            if(this.m_source == null) {
                throw new InvalidOperationException("There is not a data source currently.");
            }
            if(source == null) {
                throw new ArgumentException("Source is null.", "source");
            }
            if(condition == null) {
                throw new ArgumentException("Inner join condition is null.", "condition");
            }

            this.Source = new SqlStringExpression(string.Format("{0} INNER JOIN {1} ON {2}", this.m_source.SqlString, source.SqlString, condition.SqlString), this.GetParameters(this.m_source, source, condition));
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFromClause FullOuterJoin(ISqlObject source, ISqlExpression condition) {
            if(this.m_source == null) {
                throw new InvalidOperationException("There is not a data source currently.");
            }
            if(source == null) {
                throw new ArgumentException("Source is null.", "source");
            }
            if(condition == null) {
                throw new ArgumentException("Inner join condition is null.", "condition");
            }

            this.Source = new SqlStringExpression(string.Format("{0} FULL OUTER JOIN {1} ON {2}", this.m_source.SqlString, source.SqlString, condition.SqlString), this.GetParameters(this.m_source, source, condition));
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFromClause LeftOuterJoin(ISqlObject source, ISqlExpression condition) {
            if(this.m_source == null) {
                throw new InvalidOperationException("There is not a data source currently.");
            }
            if(source == null) {
                throw new ArgumentException("Source is null.", "source");
            }
            if(condition == null) {
                throw new ArgumentException("Inner join condition is null.", "condition");
            }

            this.Source = new SqlStringExpression(string.Format("{0} LEFT OUTER JOIN {1} ON {2}", this.m_source.SqlString, source.SqlString, condition.SqlString), this.GetParameters(this.m_source, source, condition));
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFromClause RightOuterJoin(ISqlObject source, ISqlExpression condition) {
            if(this.m_source == null) {
                throw new InvalidOperationException("There is not a data source currently.");
            }
            if(source == null) {
                throw new ArgumentException("Source is null.", "source");
            }
            if(condition == null) {
                throw new ArgumentException("Inner join condition is null.", "condition");
            }

            this.Source = new SqlStringExpression(string.Format("{0} RIGHT OUTER JOIN {1} ON {2}", this.m_source.SqlString, source.SqlString, condition.SqlString), this.GetParameters(this.m_source, source, condition));
            return this;
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                return this.m_sqlString = (this.m_source != null ? string.Format("{0} {1}", this.Keyword, this.m_source.SqlString) : string.Empty);
            }
        }
    }

    /// <summary>
    /// Represents a SQL clause interface.
    /// </summary>
    public interface ISqlClause : ISqlExpression {
        /// <summary>
        /// Gets the keyword of this clause.
        /// </summary>
        string Keyword {
            get;
        }

        /// <summary>
        /// Gets whether this clause can used in a SQL statement.
        /// </summary>
        bool IsAvailable {
            get;
        }
    }

    /// <summary>
    /// Represents a SQL SELECT clause.
    /// </summary>
    internal class SqlSelectClause : SqlCombinedExpression<ISqlExpression>, ISqlSelectClause {
        /// <summary>
        /// Whether this clause only fetch the distinct records.
        /// </summary>
        protected bool m_isDistinct;

        #region SqlCombinedExpression Members

        /// <inheritdoc />
        protected override string GetSqlString() {
            if(this.m_components.Count == 0) {
                return string.Empty;
            }

            StringBuilder sqlString = new StringBuilder(this.Keyword);
            if(this.m_isDistinct) {
                sqlString.Append(" DISTINCT");
            }
            sqlString.AppendFormat(" {0}", this.m_components[0].SqlString);
            for(int i = 1; i < this.m_components.Count; i++) {
                sqlString.AppendFormat(", {0}", this.m_components[i].SqlString);
            }

            return sqlString.ToString();
        }

        /// <inheritdoc />
        public override bool IsAllowDuplicateComponent {
            get {
                return false;
            }
        }

        #endregion

        #region ISqlSelectClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "SELECT";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_components.Count > 0;
            }
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<ISqlExpression> Expressions {
            get {
                return this.m_components.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public virtual bool IsDistinct {
            get {
                return this.m_isDistinct;
            }
            set {
                if(this.m_isDistinct != value) {
                    this.m_sqlString = null;
                    this.m_isDistinct = value;
                }
            }
        }

        /// <inheritdoc />
        public virtual ISqlSelectClause SetIsDistinct(bool value) {
            this.IsDistinct = value;
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlSelectClause AddExpressions(params ISqlExpression[] expressions) {
            this.AddComponent(expressions);
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlSelectClause RemoveExpression(ISqlExpression expression) {
            this.RemoveComponent(expression);
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlSelectClause ClearExpressions() {
            this.ClearComponents();
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL DELETE statement.
    /// </summary>
    public class SqlDeleteStatement : SqlStatement, ISqlDeleteStatement {
        /// <summary>
        /// FROM clause.
        /// </summary>
        protected ISqlFromClause m_fromClause = new SqlFromClause();

        /// <summary>
        /// WHERE clause.
        /// </summary>
        private ISqlWhereClause m_whereClause = new SqlWhereClause();

        #region SqlStatement Members

        /// <inheritdoc />
        protected override void Initialize() {
            this.m_clauses.Clear();
            this.m_clauses.Add(this.m_fromClause);
            this.m_clauses.Add(this.m_whereClause);
        }

        /// <inheritdoc />
        public override string Keyword {
            get {
                return "DELETE";
            }
        }

        /// <inheritdoc />
        public override bool IsValid {
            get {
                return this.m_fromClause.IsAvailable;
            }
        }

        #endregion

        #region ISqlDeleteStatement Members

        /// <inheritdoc />
        public virtual ISqlFromClause FromClause {
            get {
                return this.m_fromClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlWhereClause WhereClause {
            get {
                return this.m_whereClause;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL statement interface.
    /// </summary>
    public interface ISqlStatement : ISqlExpression {
        /// <summary>
        /// Gets the keyword of this statement.
        /// </summary>
        string Keyword {
            get;
        }

        /// <summary>
        /// Gets whether this is a valid SQL statement.
        /// </summary>
        bool IsValid {
            get;
        }

        /// <summary>
        /// Gets SQL clauses used in this statement.
        /// </summary>
        ReadOnlyCollection<ISqlClause> Clauses {
            get;
        }
    }

    /// <summary>
    /// Represents a common SQL statement.
    /// </summary>
    public abstract class SqlStatement : SqlExpression, ISqlStatement {
        /// <summary>
        /// Initialize a new instance of SqlStatement class.
        /// </summary>
        public SqlStatement() {
            this.Initialize();
        }

        /// <summary>
        /// SQL clauses in this statement.
        /// </summary>
        protected List<ISqlClause> m_clauses = new List<ISqlClause>();

        /// <summary>
        /// Gets SQL string of this statement.
        /// </summary>
        /// <returns>SQL string of this statement.</returns>
        protected virtual string GetSqlString() {
            if(this.m_clauses.Count == 0) {
                return string.Empty;
            }

            StringBuilder sqlString = new StringBuilder();

            if(string.IsNullOrWhiteSpace(this.Keyword)) {
                int i = 0;
                while(i < this.m_clauses.Count) {
                    if(this.m_clauses[i].IsAvailable) {
                        sqlString.Append(this.m_clauses[i].SqlString);
                        ++i;
                        break;
                    } else {
                        ++i;
                    }
                }
                for(; i < this.m_clauses.Count; i++) {
                    if(this.m_clauses[i].IsAvailable) {
                        sqlString.AppendFormat(" {0}", this.m_clauses[i].SqlString);
                    }
                }
            } else {
                sqlString.Append(this.Keyword);
                foreach(ISqlClause clause in this.m_clauses) {
                    if(clause.IsAvailable) {
                        sqlString.AppendFormat(" {0}", clause.SqlString);
                    }
                }
            }

            return sqlString.ToString();
        }

        /// <summary>
        /// Gets parameters of this statement.
        /// </summary>
        /// <returns>Parameters of this statement.</returns>
        protected virtual List<IDataParameter> GetParameters() {
            List<IDataParameter> all = new List<IDataParameter>(this.m_clauses.Count);
            foreach(ISqlClause clause in this.m_clauses) {
                if(clause != null && clause.Parameters.Count > 0) {
                    all.AddRange(clause.Parameters);
                }
            }

            List<IDataParameter> parameters = new List<IDataParameter>(all.Count);
            if(all.Count > 0) {
                foreach(IDataParameter parameter in all) {
                    if(parameter != null && !parameters.Contains(parameter)) {
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// Initialize this statement.
        /// </summary>
        protected abstract void Initialize();

        #region ISqlStatement Members

        /// <inheritdoc />
        public abstract string Keyword {
            get;
        }

        /// <inheritdoc />
        public abstract bool IsValid {
            get;
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<ISqlClause> Clauses {
            get {
                return this.m_clauses.AsReadOnly();
            }
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                if(!this.IsValid) {
                    throw new InvalidOperationException("This SQL statement is not a valid.");
                }
                return this.m_sqlString = this.GetSqlString();
            }
        }

        /// <inheritdoc />
        public override ReadOnlyCollection<IDataParameter> Parameters {
            get {
                return (this.m_parameters = this.GetParameters()).AsReadOnly();
            }
        }
    }

    /// <summary>
    /// Create a SQL statement from a string and some parameters.
    /// </summary>
    public class SqlStringStatement : SqlStatement {
        /// <summary>
        /// Initialize a new instance of SqlStringStatement class.
        /// </summary>
        /// <param name="sqlString">SQL string.</param>
        /// <param name="parameters">Parameters in this expression</param>
        /// <exception cref="System.ArgumentException"><paramref name="sqlString"/> is null or empty.</exception>
        public SqlStringStatement(string sqlString, params IDataParameter[] parameters) {
            if(string.IsNullOrWhiteSpace(sqlString)) {
                throw new ArgumentException("sqlString is null or empty", "sqlString");
            }

            this.m_sqlString = sqlString;
            if(parameters != null && parameters.Length > 0) {
                foreach(IDataParameter parameter in parameters) {
                    if(parameter != null && !this.m_parameters.Contains(parameter)) {
                        this.m_parameters.Add(parameter);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Initialize() {
        }

        /// <inheritdoc />
        protected override string GetSqlString() {
            return this.m_sqlString;
        }

        /// <inheritdoc />
        protected override List<IDataParameter> GetParameters() {
            return this.m_parameters;
        }

        /// <inheritdoc />
        public override string Keyword {
            get {
                return null;
            }
        }

        /// <inheritdoc />
        public override bool IsValid {
            get {
                return true;
            }
        }
    }

    /// <summary>
    /// Represents a SQL UPDATE statement.
    /// </summary>
    public class SqlUpdateStatement : SqlStatement, ISqlUpdateStatement {
        /// <summary>
        /// Field and value clause.
        /// </summary>
        protected ISqlFieldValueClause m_fieldValueClause = new SqlFieldValueClause(new SqlUpdateFieldValueClauseSqlStringProvider());

        /// <summary>
        /// From clause.
        /// </summary>
        protected ISqlFromClause m_fromClause = new SqlFromClause();

        /// <summary>
        /// Where clause.
        /// </summary>
        protected ISqlWhereClause m_whereClause = new SqlWhereClause();

        #region SqlStatement Members

        /// <inheritdoc />
        protected override void Initialize() {
            this.m_clauses.Clear();
            this.m_clauses.Add(this.m_fieldValueClause);
            this.m_clauses.Add(this.m_fromClause);
            this.m_clauses.Add(this.m_whereClause);
        }

        /// <inheritdoc />
        public override string Keyword {
            get {
                return "UPDATE";
            }
        }

        /// <inheritdoc />
        public override bool IsValid {
            get {
                return this.m_fieldValueClause.IsAvailable;
            }
        }

        #endregion

        #region ISqlUpdateStatement Members

        /// <inheritdoc />
        public virtual ISqlFieldValueClause FieldValueClause {
            get {
                return this.m_fieldValueClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlFromClause FromClause {
            get {
                return this.m_fromClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlWhereClause WhereClause {
            get {
                return this.m_whereClause;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL fields and values clause.
    /// </summary>
    public interface ISqlFieldValueClause : ISqlClause {
        /// <summary>
        /// Gets or sets SQL source of this clause.
        /// </summary>
        ISqlObject Source {
            get;
            set;
        }

        /// <summary>
        /// Sets SQL source of this clause.
        /// </summary>
        /// <param name="source">A SQL source.</param>
        /// <returns>This clause.</returns>
        ISqlFieldValueClause SetSource(ISqlObject source);

        /// <summary>
        /// Gets fields in this clause.
        /// </summary>
        ReadOnlyCollection<ISqlObject> Fields {
            get;
        }

        /// <summary>
        /// Gets SQL string provider.
        /// </summary>
        ISqlFieldValueClauseSqlStringProvider SqlStringProvider {
            get;
        }

        /// <summary>
        /// Add a field to this clause.
        /// </summary>
        /// <param name="field">A SQL field.</param>
        /// <param name="value">The value of the field.</param>
        /// <returns>This statement.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="field"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="value"/> is null.</exception>
        ISqlFieldValueClause AddField(ISqlObject field, ISqlExpression value);

        /// <summary>
        /// Remove a field from this clause.
        /// </summary>
        /// <param name="field">A SQL field.</param>
        /// <returns>This statement.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="field"/> is null.</exception>
        ISqlFieldValueClause RemoveField(ISqlObject field);

        /// <summary>
        /// Remove all fields and values from this statement.
        /// </summary>
        /// <returns>This statement.</returns>
        ISqlFieldValueClause ClearField();

        /// <summary>
        /// Get value of the specified field.
        /// </summary>
        /// <param name="field">A SQL field.</param>
        /// <returns>Value of <paramref name="field"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="field"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="field"/> is not existing in this clause.</exception>
        ISqlExpression GetValue(ISqlObject field);
    }

    /// <summary>
    /// Provide SQL string for <typeparamref name="ISqlFieldValueClause"/>.
    /// </summary>
    public interface ISqlFieldValueClauseSqlStringProvider {
        /// <summary>
        /// Gets SQL string of a <typeparamref name="ISqlFieldValueClause"/>.
        /// </summary>
        /// <param name="clause">A SQL field and value clause.</param>
        /// <returns>SQL string.</returns>
        string GetSqlString(ISqlFieldValueClause clause);
    }

    /// <summary>
    /// Represents a SQL field and value clause.
    /// </summary>
    internal class SqlFieldValueClause : SqlCombinedExpression<ISqlObject>, ISqlFieldValueClause {
        /// <summary>
        /// Initialize a new instance of SqlFieldValueClause class.
        /// </summary>
        /// <param name="stringProvider">A SQL string provider.</param>
        /// <exception cref="System.ArgumentException"><paramref name="stringProvider"/> is null.</exception>
        public SqlFieldValueClause(ISqlFieldValueClauseSqlStringProvider stringProvider) {
            if(stringProvider == null) {
                throw new ArgumentException("SQL string provider of this clause is null.", "stringProvider");
            }

            this.m_stringProvider = stringProvider;
        }

        /// <summary>
        /// SQL source.
        /// </summary>
        protected ISqlObject m_source;

        /// <summary>
        /// Field values.
        /// </summary>
        protected IDictionary<ISqlObject, ISqlExpression> m_values = new Dictionary<ISqlObject, ISqlExpression>();

        /// <summary>
        /// SQL string provider.
        /// </summary>
        protected ISqlFieldValueClauseSqlStringProvider m_stringProvider;

        #region SqlCombinedExpression Members

        /// <inheritdoc />
        protected override string GetSqlString() {
            return this.m_stringProvider.GetSqlString(this);
        }

        /// <inheritdoc />
        public override bool IsAllowDuplicateComponent {
            get {
                return false;
            }
        }

        #endregion

        #region ISqlFieldValueClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_source != null && this.m_components.Count > 0;
            }
        }

        /// <inheritdoc />
        public virtual ISqlObject Source {
            get {
                return this.m_source;
            }
            set {
                this.m_sqlString = null;
                this.m_source = value;
            }
        }

        /// <inheritdoc />
        public virtual ISqlFieldValueClause SetSource(ISqlObject source) {
            this.Source = source;
            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<ISqlObject> Fields {
            get {
                return this.m_components.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public virtual ISqlFieldValueClauseSqlStringProvider SqlStringProvider {
            get {
                return this.m_stringProvider;
            }
        }

        /// <inheritdoc />
        public virtual ISqlFieldValueClause AddField(ISqlObject field, ISqlExpression value) {
            if(field == null) {
                throw new ArgumentException("SQL field is null.", "field");
            }
            if(value == null) {
                throw new ArgumentException("SQL field value is null.", "field");
            }

            this.AddComponent(field);
            this.m_values[field] = value;
            if(value.Parameters.Count > 0) {
                foreach(IDataParameter parameter in value.Parameters) {
                    if(parameter != null && !this.m_parameters.Contains(parameter)) {
                        this.m_parameters.Add(parameter);
                    }
                }
            }

            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFieldValueClause RemoveField(ISqlObject field) {
            if(field == null) {
                throw new ArgumentException("SQL field is null.", "field");
            }

            this.RemoveComponent(field);
            if(this.m_values.ContainsKey(field)) {
                this.m_values.Remove(field);
            }

            return this;
        }

        /// <inheritdoc />
        public virtual ISqlFieldValueClause ClearField() {
            this.ClearComponents();
            this.m_values.Clear();
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlExpression GetValue(ISqlObject field) {
            if(field == null) {
                throw new ArgumentException("SQL field is null.", "field");
            }
            if(!this.m_components.Contains(field)) {
                throw new ArgumentOutOfRangeException("field", "This field is not existing in this clause.");
            }

            return this.m_values[field];
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL string provider for a field and value clause used in SQL UPDATE statement.
    /// </summary>
    internal class SqlUpdateFieldValueClauseSqlStringProvider : ISqlFieldValueClauseSqlStringProvider {
        #region ISqlFieldValueClauseSqlStringProvider Members

        /// <inheritdoc />
        public virtual string GetSqlString(ISqlFieldValueClause clause) {
            StringBuilder sqlString = new StringBuilder();

            ReadOnlyCollection<ISqlObject> fields = clause.Fields;
            sqlString.AppendFormat("{0} SET", clause.Source.Fullname);
            sqlString.AppendFormat(" {0}", fields[0].Assign(clause.GetValue(fields[0])));
            for(int i = 1; i < fields.Count; i++) {
                sqlString.AppendFormat(", {0}", fields[i].Assign(clause.GetValue(fields[i])));
            }

            return sqlString.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL string provider for a field and value clause used in SQL INSERT statement.
    /// </summary>
    internal class SqlInsertFieldValueClauseSqlStringProvider : ISqlFieldValueClauseSqlStringProvider {
        #region ISqlFieldValueClauseSqlStringProvider Members

        /// <inheritdoc />
        public virtual string GetSqlString(ISqlFieldValueClause clause) {
            StringBuilder fieldsString = new StringBuilder();
            StringBuilder valuesString = new StringBuilder();

            ReadOnlyCollection<ISqlObject> fields = clause.Fields;
            fieldsString.AppendFormat("{0} (", clause.Source.Fullname);
            valuesString.Append("VALUES (");
            fieldsString.Append(fields[0].Fullname);
            valuesString.Append(clause.GetValue(fields[0]).SqlString);
            for(int i = 1; i < fields.Count; i++) {
                fieldsString.AppendFormat(", {0}", fields[i].Fullname);
                valuesString.AppendFormat(", {0}", clause.GetValue(fields[i]).SqlString);
            }
            fieldsString.Append(")");
            valuesString.Append(")");

            return string.Format("{0} {1}", fieldsString, valuesString);
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL assignment operator.
    /// </summary>
    internal class SqlAssignOperatorResultSqlStringProvider : ISqlOperatorResultSqlStringProvider {
        #region ISqlOperatorResultSqlStringProvider Members

        /// <inheritdoc />
        public virtual string GetSqlString(params ISqlExpression[] operands) {
            if(operands.Length < 2) {
                throw new ArgumentException("Assignment operation need two operands at least.", "operands");
            }

            return string.Format("{0} = ({1})", operands[0].SqlString, operands[1].SqlString);
        }

        #endregion
    }

    public class SqlInsertStatement : SqlStatement, ISqlInsertStatement {
        /// <summary>
        /// Field and value clause.
        /// </summary>
        protected ISqlFieldValueClause m_fieldValueClause = new SqlFieldValueClause(new SqlInsertFieldValueClauseSqlStringProvider());

        #region SqlStatement Members

        /// <inheritdoc />
        protected override void Initialize() {
            this.m_clauses.Clear();
            this.m_clauses.Add(this.m_fieldValueClause);
        }

        /// <inheritdoc />
        public override string Keyword {
            get {
                return "INSERT INTO";
            }
        }

        /// <inheritdoc />
        public override bool IsValid {
            get {
                return this.m_fieldValueClause.IsAvailable;
            }
        }

        #endregion

        #region ISqlOperatorResultSqlStringProvider Members

        /// <inheritdoc />
        public virtual ISqlFieldValueClause FieldValueClause {
            get {
                return this.m_fieldValueClause;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL SELECT statement.
    /// </summary>
    public abstract class SqlSelectStatement : SqlStatement, ISqlSelectStatement {
        /// <summary>
        /// Select clause.
        /// </summary>
        protected ISqlSelectClause m_selectClause = new SqlSelectClause();

        /// <summary>
        /// From clause.
        /// </summary>
        protected ISqlFromClause m_fromClause = new SqlFromClause();

        /// <summary>
        /// Where clause.
        /// </summary>
        protected ISqlWhereClause m_whereClause = new SqlWhereClause();

        /// <summary>
        /// Group clause.
        /// </summary>
        protected ISqlGroupClause m_groupClause = new SqlGroupClause();

        /// <summary>
        /// Order clause.
        /// </summary>
        protected ISqlOrderClause m_orderClause = new SqlOrderClause();

        #region SqlStatement Members

        /// <inheritdoc />
        protected override void Initialize() {
            this.m_clauses.Clear();
            this.m_clauses.Add(this.m_selectClause);
            this.m_clauses.Add(this.m_fromClause);
            this.m_clauses.Add(this.m_whereClause);
            this.m_clauses.Add(this.m_groupClause);
            this.m_clauses.Add(this.m_orderClause);
        }

        /// <inheritdoc />
        public override string Keyword {
            get {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public override bool IsValid {
            get {
                return this.m_selectClause.IsAvailable;
            }
        }

        #endregion

        #region ISqlSelectStatement Members

        /// <inheritdoc />
        public abstract int Count {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual ISqlSelectStatement SetCount(int value) {
            this.Count = value;
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlSelectClause SelectClause {
            get {
                return this.m_selectClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlFromClause FromClause {
            get {
                return this.m_fromClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlWhereClause WhereClause {
            get {
                return this.m_whereClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlGroupClause GroupClause {
            get {
                return this.m_groupClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlOrderClause OrderClause {
            get {
                return this.m_orderClause;
            }
        }

        #endregion
    }

    /// <summary>
    /// SQL object fullname provider for SQL all field.
    /// </summary>
    internal class SqlAllFieldFullnameProvider : ISqlObjectFullnameProvider {
        #region ISqlObjectFullnameProvider Members

        /// <inheritdoc />
        public virtual string GetFullname(ISqlObject obj) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            return (obj.Owner != null ? string.Format("{0}.", obj.Owner.Fullname) : string.Empty) + "*";
        }

        #endregion
    }

    /// <summary>
    /// Represents a all fields wildcard.
    /// </summary>
    public class SqlAllField : SqlObject {
        /// <summary>
        /// Initialize a new instance of SqlAllField class.
        /// </summary>
        /// <param name="source">A SQL source.</param>
        public SqlAllField(ISqlObject source)
            : base(source, new SqlStringExpression("*"), "AllFields", new SqlAllFieldFullnameProvider(), new SqlObjectFullnameSqlStringProvider()) {
        }
    }

    /// <summary>
    /// Provide fullname of a  SQL object with the SQL string of it's based expression.
    /// </summary>
    internal class SqlObjectExpressionSqlStringFullnameProvider : ISqlObjectFullnameProvider {
        #region ISqlObjectFullnameProvider Members

        /// <inheritdoc />
        public virtual string GetFullname(ISqlObject obj) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            return obj.Expression.SqlString;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL object which can has a alias.
    /// </summary>
    public class SqlAliasObject : SqlExpression, ISqlAliasObject {
        /// <summary>
        /// Initialize a new instance of SqlField class.
        /// </summary>
        /// <param name="obj">Based SQL object.</param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is null.</exception>
        public SqlAliasObject(ISqlObject obj)
            : this(obj, null, null, null) {
        }

        /// <summary>
        /// Initialize a new instance of SqlField class.
        /// </summary>
        /// <param name="obj">Based SQL object.</param>
        /// <param name="alias">Object alias.</param>
        /// <param name="stringProvider">SQL string provider.</param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is null.</exception>
        public SqlAliasObject(ISqlObject obj, string alias, ISqlAliasObjectFullnameProvider fullnameProvider, ISqlAliasObjectSqlStringProvider stringProvider) {
            if(obj == null) {
                throw new ArgumentException("Based SQL object is null.", "obj");
            }
            if(!string.IsNullOrWhiteSpace(m_alias) && fullnameProvider == null) {
                throw new ArgumentException("Fullname provider of this object is null.", "stringProvider");
            }
            if(!string.IsNullOrWhiteSpace(m_alias) && stringProvider == null) {
                throw new ArgumentException("SQL string provider of this object is null.", "stringProvider");
            }

            this.m_object = obj;
            this.m_alias = alias;
            this.m_fullnameProvider = fullnameProvider;
            this.m_sqlStringProvider = stringProvider;
        }

        /// <summary>
        /// Based SQL object.
        /// </summary>
        protected ISqlObject m_object;

        /// <summary>
        /// Object alias.
        /// </summary>
        protected string m_alias;

        /// <summary>
        /// Fullname provider.
        /// </summary>
        protected ISqlAliasObjectFullnameProvider m_fullnameProvider;

        /// <summary>
        /// SQL string provider.
        /// </summary>
        protected ISqlAliasObjectSqlStringProvider m_sqlStringProvider;

        /// <summary>
        /// Gets SQL string provider of this object.
        /// </summary>
        public virtual ISqlAliasObjectSqlStringProvider SqlStringProvider {
            get {
                return this.m_sqlStringProvider;
            }
        }

        /// <summary>
        /// Gets fullname provider of this object.
        /// </summary>
        public virtual ISqlAliasObjectFullnameProvider FullnameProvider {
            get {
                return this.m_fullnameProvider;
            }
        }

        #region ISqlAliasObject Members

        /// <inheritdoc />
        public virtual string Alias {
            get {
                return this.m_alias;
            }
        }

        /// <inheritdoc />
        public virtual ISqlObject Object {
            get {
                return this.m_object;
            }
        }

        #endregion

        #region ISqlObject Members

        /// <inheritdoc />
        public virtual string Name {
            get {
                return string.IsNullOrWhiteSpace(this.m_alias) ? this.m_object.Name : this.m_alias;
            }
        }

        /// <inheritdoc />
        public virtual string Fullname {
            get {
                return string.IsNullOrWhiteSpace(this.m_alias) ? this.m_object.Fullname : this.m_fullnameProvider.GetFullname(this);
            }
        }

        /// <inheritdoc />
        public virtual ISqlObject Owner {
            get {
                return this.m_object.Owner;
            }
        }

        /// <inheritdoc />
        public virtual ISqlExpression Expression {
            get {
                return this.m_object.Expression;
            }
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                return this.m_sqlString = (string.IsNullOrWhiteSpace(this.m_alias) ? this.m_object.SqlString : this.m_sqlStringProvider.GetSqlString(this));
            }
        }

        /// <inheritdoc />
        public override ReadOnlyCollection<IDataParameter> Parameters {
            get {
                return this.m_object.Parameters;
            }
        }
    }

    /// <summary>
    /// Provide SQL string of a SqlAliasObject.
    /// </summary>
    public interface ISqlAliasObjectSqlStringProvider {
        /// <summary>
        /// Gets SQL string of the specified object.
        /// </summary>
        /// <param name="field">A SQL alias object.</param>
        /// <returns>SQL string of <paramref name="field"/>.</returns>
        string GetSqlString(ISqlAliasObject field);
    }

    /// <summary>
    /// Provide SQL string a SQL object with the SQL string of it's based expression.
    /// </summary>
    internal class SqlObjectExpressionSqlStringSqlStringProvider : ISqlObjectSqlStringProvider {
        #region ISqlObjectSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(ISqlObject obj) {
            return obj.Expression.SqlString;
        }

        #endregion
    }

    /// <summary>
    /// Provide fullname of a SqlAliasObject.
    /// </summary>
    public interface ISqlAliasObjectFullnameProvider {
        /// <summary>
        /// Get fullname of the specified object.
        /// </summary>
        /// <param name="obj">A SqlAliasObject.</param>
        /// <returns></returns>
        string GetFullname(ISqlAliasObject obj);
    }

    /// <summary>
    /// Represents a SQL object which can has a alias.
    /// </summary>
    public interface ISqlAliasObject : ISqlObject {
        /// <summary>
        /// Gets alias of this object.
        /// </summary>
        string Alias {
            get;
        }

        /// <summary>
        /// Gets the base SQL object.
        /// </summary>
        ISqlObject Object {
            get;
        }
    }

    /// <summary>
    /// Provide fullname of a SQL alias object with the fullname of it's base SQL object.
    /// </summary>
    internal class SqlAliasObjectFullnameProvider : ISqlAliasObjectFullnameProvider {
        #region ISqlAliasObjectFullnameProvider Members

        /// <inheritdoc />
        public string GetFullname(ISqlAliasObject obj) {
            return obj.Object.Fullname;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL object and it's value.
    /// </summary>
    public class SqlObjectValue {
        /// <summary>
        /// Initialize a new instance of SqlObjectValue.
        /// </summary>
        /// <param name="obj">A SQL object.</param>
        /// <param name="type">The DbType of <paramref name="obj"/>.</param>
        /// <param name="value">The value of <paramref name="obj"/>.</param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is null.</exception>
        public SqlObjectValue(ISqlObject obj, DbType type, object value) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            this.Object = obj;
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Gets the SQL object.
        /// </summary>
        public ISqlObject Object {
            get;
            private set;
        }

        /// <summary>
        /// Gets the DbType of this SQL object.
        /// </summary>
        public DbType Type {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value of this SQL object.
        /// </summary>
        public object Value {
            get;
            private set;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Object.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is SqlObjectValue)) {
                return false;
            }
            return this.Object.Equals(((SqlObjectValue) obj).Object);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Object.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents a SQL IS NULL operator.
    /// </summary>
    internal class SqlIsNullOperatorResultSqlStringProvider : ISqlOperatorResultSqlStringProvider {
        #region ISqlOperatorResultSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(params ISqlExpression[] operands) {
            if(operands == null || operands.Length < 1) {
                throw new ArgumentException("Is null operation need one operands at least.", "operands");
            }

            return string.Format("({0}) IS NULL", operands[0].SqlString);
        }

        #endregion
    }
}
