using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CustomForms.Common.Query
{
    public class QueryHelper
    {
       
        public static Expression<Func<T, bool>> BuildCondition<T>(Expression<Func<T, bool>> condtion, ExpressionCriteriaBase[] criteria)
        {
            ParameterExpression p = condtion.Parameters[0];

            Expression body = BuildCondition<T>(p, criteria).Body;

            body = Expression.AndAlso(condtion.Body, body);

            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> BuildOrCondition<T>(Expression<Func<T, bool>> condtion, ExpressionCriteriaBase[] criteria)
        {
            ParameterExpression p = condtion.Parameters[0];

            Expression body = BuildCondition<T>(p, criteria).Body;

            body = Expression.OrElse(condtion.Body, body);

            return Expression.Lambda<Func<T, bool>>(body, p);
        }


        public static Expression<Func<T, bool>> BuildCondition<T>(ExpressionCriteriaBase[] criteria)
        {
            ParameterExpression p = Expression.Parameter(typeof(T), "p");
            return BuildCondition<T>(p, criteria);
        }
        //
        public static Expression<Func<T, bool>> BuildCondition<T>(ParameterExpression p, ExpressionCriteriaBase[] criteria)
        {
            Expression body = null;
            if (criteria != null)
            {
                foreach (var criterion in criteria)
                {
                    if (criterion is CompositeExpressionCriteria)
                    {
                        var composite = (CompositeExpressionCriteria) criterion;
                        Expression left = BuildExpression<T>(p, composite.Left);
                        Expression right = BuildExpression<T>(p, composite.Right);
                        switch (composite.BinaryOperator)
                        {
                            case BinaryOperator.And:
                            case BinaryOperator.No:
                            {
                                Expression temp = null;
                                if (left == null || right == null)
                                {
                                    temp = left ?? right;
                                }
                                else
                                {
                                    temp = Expression.AndAlso(left, right);
                                }
                                body = body == null ? temp : Expression.AndAlso(body, temp);
                            }
                                break;
                            case BinaryOperator.Or:
                            {
                                Expression temp = null;
                                if (left == null || right == null)
                                {
                                    temp = left ?? right;
                                }
                                else
                                {
                                    temp = Expression.OrElse(left, right);
                                }
                                body = body == null ? temp : Expression.AndAlso(body, temp);
                            }
                                break;
                            default:
                            {
                                Expression temp = null;
                                if (left == null || right == null)
                                {
                                    temp = left ?? right;
                                }
                                else
                                {
                                    temp = Expression.AndAlso(left, right);
                                }
                                body = body == null ? temp : Expression.AndAlso(body, temp);
                            }
                                break;
                        }
                    }
                    else
                    {
                        body = body == null
                            ? BuildExpression<T>(p, (ExpressionCriteria) criterion)
                            : Expression.AndAlso(body, BuildExpression<T>(p, (ExpressionCriteria) criterion));
                    }
                }
            }
            if (body == null)
            {
                body = Expression.Constant(true);
            }
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
        //单一查询条件
        private static Expression BuildExpression<T>(ParameterExpression p, ExpressionCriteriaBase criterionbase)
        {
            Expression body = null;
            if (criterionbase == null)
                return null;
            if (criterionbase is ExpressionCriteria)
            {
                var criterion = (ExpressionCriteria) criterionbase;

                var property = Expression.Property(p, criterion.PropertyName);
                var constants = Expression.Constant(criterion.Value);
                if (property.Type.Name == typeof(Nullable<>).Name)
                {
                    property = Expression.Property(property, "Value");
                }

                switch (criterion.Operate)
                {
                    case Operator.Contains:
                        {
                            body = Expression.Call(property,
                                                        typeof(T).GetProperty(criterion.PropertyName).PropertyType.GetMethod(
                                                            "Contains"), constants);
                        }
                        break;
                    case Operator.BeContains:
                        {
                            body = Expression.Call(constants,
                                                        criterion.Value.GetType().GetMethod("Contains"), property);
                        }
                        break;

                    case Operator.NotContains:
                        {
                            body = Expression.Not(Expression.Call(property,
                                                        typeof(T).GetProperty(criterion.PropertyName).PropertyType.GetMethod(
                                                            "Contains"), constants));
                          //  body = Expression.
                        }
                        break;
                    case Operator.NotBeContains:
                        {
                            body = Expression.Not(Expression.Call(constants,
                                                        criterion.Value.GetType().GetMethod("Contains"), property));
                        }
                        break;
                    case Operator.Equal:
                        {
                            body = Expression.Equal(property, constants);
                        }
                        break;
                    case Operator.GreaterThan:
                        {
                            body = Expression.GreaterThan(property, constants);
                        }
                        break;
                    case Operator.GreaterThanOrEqual:
                        {
                            body = Expression.GreaterThanOrEqual(property, constants);
                        }
                        break;
                    case Operator.LessThan:
                        {
                            body = Expression.LessThan(property, constants);
                        }
                        break;
                    case Operator.LessThanOrEqual:
                        {
                            body = Expression.LessThanOrEqual(property, constants);
                        }
                        break;
                    case Operator.NotEqual:
                        {
                            body = Expression.NotEqual(property, constants);
                        }
                        break;
                }
            }
            else if (criterionbase is CompositeExpressionCriteria)
             {
                 var compositeExpressionCriteria = ((CompositeExpressionCriteria)criterionbase);

                 var left = BuildExpression<T>(p, compositeExpressionCriteria.Left);

                 var right = BuildExpression<T>(p, compositeExpressionCriteria.Right);
                 if (left != null && right != null)
                 {
                     body = compositeExpressionCriteria.BinaryOperator == BinaryOperator.And
                                ? Expression.AndAlso(left, right)
                                : Expression.OrElse(left, right);

                 }
                 else
                 {
                     body = left ?? right;
                 }
             }
            return body;
        }

        public static Expression<Func<T, TKey>> BuildSelector<T, TKey>(ExpressionCriteria criteria)
        {
            ParameterExpression p = Expression.Parameter(typeof(T), "p");
            return BuildSelector<T, TKey>(p, criteria);
        }
  
        public static Expression<Func<T, TKey>> BuildSelector<T, TKey>(ParameterExpression p, ExpressionCriteria criteria)
        {
            var property = Expression.Property(p, criteria.PropertyName);
             return Expression.Lambda<Func<T, TKey>>(property, p);
        }
    }

    public abstract class ExpressionCriteriaBase{}

    public class ExpressionCriteria : ExpressionCriteriaBase
    {
        public string PropertyName { get; set; }

        public Operator Operate { get; set; }

        public object Value { get; set; }
    }

    public class CompositeExpressionCriteria : ExpressionCriteriaBase
    {
        public ExpressionCriteriaBase Left { get; set; }

        public BinaryOperator BinaryOperator { get; set; }

        public ExpressionCriteriaBase Right { get; set; }
    }

    public enum Operator
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        BeContains, 
        NotContains,
        NotBeContains,
        NotEqual
    }

    public enum BinaryOperator
    {
        No,
        And,
        Or
    }
}