using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrasturcture.QueryableExtension
{
    public static class QueryableBuilderExtensions
    {
        private readonly static Dictionary<NameTypePair, Expression> ExpressionCache = new Dictionary<NameTypePair, Expression>();

        private readonly static Dictionary<NameTypePair, string> MappingCache = new Dictionary<NameTypePair, string>();

        public static Expression<Func<TIn, TOut>> BuildSelectorClause<TIn, TOut>(string[] properties) where TOut : class, new()
        {
            Array.Sort(properties);

            var key = new NameTypePair(string.Join("_", properties), typeof(TIn), typeof(TOut));
            if (!ExpressionCache.ContainsKey(key))
            {
                var param = Expression.Parameter(typeof(TIn), "p");
                var ctor = typeof(TOut).GetConstructor(Type.EmptyTypes);
                Debug.Assert(ctor != null, "ctor is null");

                var memberBindings = new MemberBinding[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    var p = typeof(TOut).GetProperty(properties[i]);
                    Debug.Assert(p.CanWrite, string.Format("{0} not support set", properties[i]));

                    PropertyInfo p2;
                    var attrs = (MappingAttribute[])p.GetCustomAttributes(typeof(MappingAttribute), false);
                    if (attrs.Length > 0)
                    {
                        var attr = attrs.FirstOrDefault(a => a.ToType == typeof(TIn)) ?? attrs[0];
                        p2 = typeof(TIn).GetProperty(attr.To ?? p.Name);
                        Debug.Assert(p2 != null, string.Format("{0} not found property: {1}", typeof(TIn), attr.To));
                    }
                    else
                    {
                        p2 = typeof(TIn).GetProperty(properties[i]);
                        Debug.Assert(p2 != null, string.Format("{0} not found property: {1}", typeof(TIn), properties[i]));
                    }

                    Debug.Assert(p2.CanRead, string.Format("{0} not support get", p2.Name));
                    if (p2.PropertyType.Name == typeof(Nullable<>).Name)
                    {
                        var member = Expression.Property(param, p2);
                        memberBindings[i] = Expression.Bind(typeof(TOut).GetProperty(properties[i]).SetMethod,
                            Expression.Call(member, p2.PropertyType.GetMethod("GetValueOrDefault", Type.EmptyTypes)));
                    }
                    else
                    {
                        memberBindings[i] = Expression.Bind(typeof(TOut).GetProperty(properties[i]).SetMethod,
                            Expression.Property(param, p2.GetMethod));
                    }
                }

                var newExp = Expression.New(ctor);
                var memberInit = Expression.MemberInit(newExp, memberBindings);

                var rs = Expression.Lambda<Func<TIn, TOut>>(memberInit, param);
                ExpressionCache.Add(key, rs);
                return rs;
            }
            return ExpressionCache[key] as Expression<Func<TIn, TOut>>;
        }

        public static IQueryable<TOut> BuildSelectorClause<TIn, TOut>(this IQueryable<TIn> source, string[] properties) where TOut : class, new()
        {
            return source.Select(BuildSelectorClause<TIn, TOut>(properties));
        }


        public static string GetMappingPropertyName<TSource, T>(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                var key = new NameTypePair(propertyName, typeof(TSource), typeof(T));
                if (!MappingCache.ContainsKey(key))
                {
                    var p = typeof(TSource).GetProperty(propertyName);
                    var attrs = (MappingAttribute[])p.GetCustomAttributes(typeof(MappingAttribute), false);
                    if (attrs.Length == 0)
                    {
                        throw new NotSupportedException(string.Format("property {0} not supported mapping", propertyName));
                    }

                    var attr = attrs.FirstOrDefault(a => a.ToType == typeof(T)) ?? attrs[0];
                    PropertyInfo p2 = typeof(T).GetProperty(attr.To ?? p.Name);
                    Debug.Assert(p2 != null, string.Format("{0} not found property: {1}", typeof(T), attr.To));

                    MappingCache.Add(key, p2.Name);
                }
                return MappingCache[key];
            }
            return null;
        }

        public static Expression<Func<T, TKey>> BuildOrderClause<T, TKey, TSource>(string propertyName) where T : class
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                var property = GetMappingPropertyName<TSource, T>(propertyName);

                return BuildOrderClause<T, TKey>(property);
            }

            throw new ArgumentNullException("propertyName");
        }


        public static Expression<Func<T, TKey>> BuildOrderClause<T, TKey>(string propertyName) where T : class
        {
            var key = new NameTypePair(propertyName, typeof(T), typeof(TKey));
            if (!ExpressionCache.ContainsKey(key))
            {
                var param = Expression.Parameter(typeof(T), "p");
                var pi = typeof(T).GetProperty(propertyName);
                Expression member = null;
                if (pi.PropertyType.Name == typeof(Nullable<>).Name)
                {
                    var property = Expression.Property(param, pi.GetMethod);
                    member = Expression.Call(property, pi.PropertyType.GetMethod("GetValueOrDefault", Type.EmptyTypes));
                }
                else
                {
                    member = Expression.Property(param, pi.GetMethod);
                }
                var rs = Expression.Lambda<Func<T, TKey>>(member, param);
                ExpressionCache.Add(key, rs);
                return rs;
            }
            return ExpressionCache[key] as Expression<Func<T, TKey>>;
        }

        public static IQueryable<T> BuildOrderClause<T>(this IQueryable<T> query, string propertyName, string orderType) where T : class
        {
            var type = typeof(T).GetProperty(propertyName).PropertyType;

        lable1:
            switch (type.Name)
            {
                case "UInt16":
                    {
                        var selector = BuildOrderClause<T, ushort>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }
                case "Int16":
                    {
                        var selector = BuildOrderClause<T, short>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "SByte":
                    {
                        var selector = BuildOrderClause<T, sbyte>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Byte":
                    {
                        var selector = BuildOrderClause<T, byte>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                             ? query.OrderBy(selector)
                             : query.OrderByDescending(selector);
                    }

                case "Int32":
                    {
                        var selector = BuildOrderClause<T, int>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "UInt32":
                    {
                        var selector = BuildOrderClause<T, uint>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Int64":
                    {
                        var selector = BuildOrderClause<T, long>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "UInt64":
                    {
                        var selector = BuildOrderClause<T, ulong>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Single":
                    {
                        var selector = BuildOrderClause<T, float>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Double":
                    {
                        var selector = BuildOrderClause<T, double>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Decimal":
                    {
                        var selector = BuildOrderClause<T, decimal>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                    break;
                case "String":
                    {
                        var selector = BuildOrderClause<T, string>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Char":
                    {
                        var selector = BuildOrderClause<T, char>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                    break;
                case "DateTime":
                    {
                        var selector = BuildOrderClause<T, DateTime>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Boolean":
                    {
                        var selector = BuildOrderClause<T, bool>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }

                case "Guid":
                    {
                        var selector = BuildOrderClause<T, Guid>(propertyName);
                        return string.Compare("asc", orderType, StringComparison.OrdinalIgnoreCase) == 0
                            ? query.OrderBy(selector)
                            : query.OrderByDescending(selector);
                    }
                case "Nullable`1":
                    {
                        type = type.GetGenericArguments()[0];
                        goto lable1;
                    }

                default:
                    throw new InvalidOperationException("Not Supported Type: " + type.Name);
            }

        }

        class NameTypePair
        {
            private readonly Type _source;

            private readonly Type _target;

            private readonly string _name;
            public NameTypePair(string name, Type source, Type target)
            {
                _name = name;
                _source = source;
                _target = target;
            }

            public override bool Equals(object obj)
            {
                var pair = obj as NameTypePair;
                if (pair != null)
                {
                    return pair._source == _source &&
                        pair._target == _target &&
                        pair._name == _name; ;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _name.GetHashCode() & _source.GetHashCode() & _target.GetHashCode();
            }
        }
    }
}