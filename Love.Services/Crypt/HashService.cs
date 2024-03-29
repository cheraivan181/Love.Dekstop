﻿using System.Security.Cryptography;
using System.Text;

namespace Love.Services.Crypt
{
    public static class HashService
    {
		public static string GetHash(string data)
		{
			using (var sha = SHA512.Create())
			{
				var bytes = Encoding.UTF8.GetBytes(data);
				var hash = sha.ComputeHash(bytes);

				var sb = new StringBuilder();

				for (int i = 0; i < hash.Length; i++)
					sb.Append(hash[i].ToString("X2"));

				return sb.ToString();
			}
		}
	}
}
