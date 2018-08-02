using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.DocumentModel
{
	public static class StringHelper
	{
		public static string ToHexValues(this string instance)
		{
			byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(instance);
			var result = new StringBuilder();
			foreach (byte @byte in bytes)
				result.AppendFormat("{0:x2}", @byte);
			return result.ToString();
		}



	}
}
