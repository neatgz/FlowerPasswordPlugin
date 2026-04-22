using System;
using System.Security.Cryptography;
using System.Text;

namespace FlowerPasswordPlugin
{
	/// <summary>
	/// FlowerPassword / 花密 algorithm (kenmick/FlowerPassword pages/index/index.js).
	/// Uses the same MD5 module semantics: two-argument calls are HMAC-MD5 with UTF-8,
	/// key = second argument, message = first argument (blueimp-md5 <c>md5(message, key)</c> mapping).
	/// </summary>
	internal static class FlowerPasswordEngine
	{
		private const string Str1 = "snow";
		private const string Str2 = "kise";
		private const string Str3 = "sunlovesnow1990090127xykab";

		internal static string Generate(string keyword, string code)
		{
			if (keyword == null || code == null)
				throw new ArgumentNullException(keyword == null ? "keyword" : "code");
			if (keyword.Length == 0 || code.Length == 0)
				throw new ArgumentException("记忆密码或区分代号不能为空");

			string md5one = HmacMd5Hex(message: keyword, key: code);
			string md5two = HmacMd5Hex(message: md5one, key: Str1);
			string md5three = HmacMd5Hex(message: md5one, key: Str2);

			char[] rule = md5three.ToCharArray();
			char[] source = md5two.ToCharArray();

			for (int i = 0; i < 32; i++)
			{
				if (IsNaNJsStyle(source[i]))
				{
					if (Str3.IndexOf(rule[i]) >= 0)
						source[i] = char.ToUpperInvariant(source[i]);
				}
			}

			string pwd32 = new string(source);
			char firstChar = pwd32[0];
			if (IsNaNJsStyle(firstChar))
				return pwd32.Substring(0, 16);
			return "K" + pwd32.Substring(1, 16);
		}

		/// <summary>Matches JavaScript <c>isNaN(singleHexChar)</c> on MD5 hex output.</summary>
		private static bool IsNaNJsStyle(char c)
		{
			return c < '0' || c > '9';
		}

		private static string HmacMd5Hex(string message, string key)
		{
			using (var hmac = new HMACMD5(Encoding.UTF8.GetBytes(key)))
			{
				byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
				return ToHexLower(hash);
			}
		}

		private static string ToHexLower(byte[] bytes)
		{
			var sb = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
				sb.Append(bytes[i].ToString("x2"));
			return sb.ToString();
		}
	}
}
