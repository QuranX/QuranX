using System;
using System.Collections.Generic;

namespace QuranX.Web.Helpers
{
	public class ObjectEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> Compare;

		public ObjectEqualityComparer(Func<T, T, bool> compare)
		{
			if (compare == null)
				throw new ArgumentNullException(nameof(compare));

			Compare = compare;
		}

		public bool Equals(T x, T y)
		{
			return Compare(x, y);
		}

		public int GetHashCode(T obj)
		{
			if (ReferenceEquals(obj, null))
				return 0;
			return obj.GetHashCode();
		}
	}
}