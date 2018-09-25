using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace QuranX.Persistence.Extensions
{
	public static class ExpressionExtensions
	{
		private static readonly ConcurrentDictionary<string, object> CompiledExpressions = new ConcurrentDictionary<string, object>();

		public static string GetIndexName<TObj, TVal>(this Expression<Func<TObj, TVal>> expression)
		{
			MemberExpression memberExpression = GetMemberInfo(expression);
			return typeof(TObj).Name + "_" + memberExpression.Member.Name;
		}

		public static void GetIndexNameAndPropertyValue<TObj, TVal>(
			this Expression<Func<TObj, TVal>> expression,
			TObj instance,
			out string name,
			out TVal value)
		{
			MemberExpression memberExpression = GetMemberInfo(expression);
			name = typeof(TObj).Name + "_" + memberExpression.Member.Name;
			value = GetValue(instance, name, expression);
		}

		private static MemberExpression GetMemberInfo(Expression method)
		{
			var lambda = method as LambdaExpression;
			if (lambda == null)
				throw new ArgumentNullException(nameof(method));

			MemberExpression memberExpr = null;

			if (lambda.Body.NodeType == ExpressionType.Convert)
			{
				memberExpr =
					((UnaryExpression)lambda.Body).Operand as MemberExpression;
			}
			else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpr = lambda.Body as MemberExpression;
			}

			if (memberExpr == null)
				throw new ArgumentException(nameof(method));

			return memberExpr;
		}

		private static TVal GetValue<TObj, TVal>(TObj instance, string id, Expression<Func<TObj, TVal>> expression)
		{

			object getValueFunc;
			if (!CompiledExpressions.TryGetValue(id, out getValueFunc))
			{
				getValueFunc = expression.Compile();
				CompiledExpressions[id] = getValueFunc;
			}

			var accessor = (Func<TObj, TVal>)getValueFunc;
			return accessor(instance);
		}
	}
}
