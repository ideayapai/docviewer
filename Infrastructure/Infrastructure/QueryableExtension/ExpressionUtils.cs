using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrasturcture.QueryObjects;

namespace Infrasturcture.QueryableExtension
{
    public class ExpressionUtils
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
            if (criteria == null || criteria.Length == 0)
            {
                return p => true;
            }
            else
            {
                ParameterExpression p = Expression.Parameter(typeof (T), "p");
                return BuildCondition<T>(p, criteria);
            }
        }

        //
        public static Expression<Func<T, bool>> BuildCondition<T>(ParameterExpression p, ExpressionCriteriaBase[] criteria)
        {
            Expression body = null;
            foreach (var criterion in criteria)
            {
                if (criterion is CompositeExpressionCriteria)
                {
                    var composite = (CompositeExpressionCriteria) criterion;
                    Expression left = BuildExpression<T>(p, composite.Left);
                    Expression right = BuildExpression<T>(p, composite.Right);
                    switch(composite.BinaryOperator)
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
                    body = body == null ? BuildExpression<T>(p, (ExpressionCriteria)criterion) : Expression.AndAlso(body, BuildExpression<T>(p, (ExpressionCriteria)criterion));
                }
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

        public static ExpressionCriteriaBase[] BuildExpressionCriteria(List<QueryObjectItem> queryObjs)
        {
            if (queryObjs.Count == 0)
                return null;
            var criteria = new List<ExpressionCriteriaBase>(queryObjs.Count);
            var composite = new CompositeExpressionCriteria();
            var temp = composite;

            var count = (queryObjs.Count + 1) / 2;
            for (int i = 0; i < count; i++)
            {
                temp.Left = new CompositeExpressionCriteria();
                temp = (CompositeExpressionCriteria)temp.Left;
                for (int j = i * 2; j < i * 2 + 2 && j < queryObjs.Count; j++)
                {
                    if (j % 2 == 0)
                    {
                        var left = new ExpressionCriteria
                        {
                            PropertyName = queryObjs[j].PropertyName,
                            Operate = (Operator)Enum.Parse(typeof(Operator), queryObjs[j].Operate),
                        };
                        switch (queryObjs[j].ValueType.ToLower())
                        {
                            case "datetime":
                                left.Value = DateTime.Parse(queryObjs[j].Value.ToString());
                                break;
                            default:
                                left.Value = queryObjs[j].Value;
                                break;
                        }
                        temp.Left = left;
                        temp.BinaryOperator = (BinaryOperator)Enum.Parse(typeof(BinaryOperator), queryObjs[j].BinaryOperator, true);
                    }
                    else
                    {
                        var right = new ExpressionCriteria
                        {
                            PropertyName = queryObjs[j].PropertyName,
                            Operate = (Operator)Enum.Parse(typeof(Operator), queryObjs[j].Operate),
                        };
                        switch (queryObjs[j].ValueType.ToLower())
                        {
                            case "datetime":
                                right.Value = DateTime.Parse(queryObjs[j].Value.ToString());
                                break;
                            default:
                                right.Value = queryObjs[j].Value;
                                break;
                        }
                        temp.Right = right;
                    }
                }
            }
            criteria.Add(composite);

            return criteria.ToArray();

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
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 包含
        /// </summary>
        Contains,
        /// <summary>
        /// 被包含
        /// </summary>
        BeContains, 
        /// <summary>
        /// 不包含
        /// </summary>
        NotContains,
        /// <summary>
        /// 不被包含
        /// </summary>
        NotBeContains,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual
    }

    public enum BinaryOperator
    {
        No,
        And,
        Or
    }
}