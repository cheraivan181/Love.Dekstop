using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Utils
{
    public static class StringExtensions
    {
		public static string ToUrlSafeBase64(this byte[] input)
		{
			return Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_");
		}

		public static byte[] FromUrlSafeBase64(this string input)
		{
			return Convert.FromBase64String(input.Replace("-", "+").Replace("_", "/"));
		}

		
	}
}
