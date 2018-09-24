using System;
using System.Linq.Expressions;

namespace QuranX.Persistence.Extensions
{
	public static class ExpressionExtensions
	{
		public static string GetName<TObj, TVal>(this Expression<Func<TObj, TVal>> expression)
		{
			MemberExpression memberExpression = GetMemberInfo(expression);
			return memberExpression.Member.Name;
		}

		public static void GetIndexNameAndPropertyValue<TObj, TVal>(
			this Expression<Func<TObj, TVal>> expression,
			TObj instance,
			out string name,
			out TVal value)
		{
			MemberExpression memberExpression = GetMemberInfo(expression);
			name = typeof(TObj).Name + "_" + memberExpression.Member.Name;
			value = expression.Compile().Invoke(instance);
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
	}
}
