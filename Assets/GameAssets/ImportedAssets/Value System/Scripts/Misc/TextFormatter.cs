using System.Collections.Generic;
using UnityEngine;

namespace ValueSystem.Misc
{
	public static class TextFormatter
	{
		private static List<KeyValuePair<float, string>> suffixes = new List<KeyValuePair<float, string>>()
	   {
			new KeyValuePair<float, string>( 1e+3f , "K" ),
			new KeyValuePair<float, string>( 1e+6f , "M" ),
			new KeyValuePair<float, string>( 1e+9f , "B" ),
			new KeyValuePair<float, string>( 1e+12f , "T" ),
			new KeyValuePair<float, string>( 1e+15f , "q" ),
			new KeyValuePair<float, string>( 1e+18f , "Q" ),
			new KeyValuePair<float, string>( 1e+21f , "s" ),
			new KeyValuePair<float, string>( 1e+24f , "S" ),
			new KeyValuePair<float, string>( 1e+27f , "O" ),
			new KeyValuePair<float, string>( 1e+30f , "N" ),
			new KeyValuePair<float, string>( 1e+33f , "D" ),
			new KeyValuePair<float, string>( 1e+36f , "U" )
		};

		/// <summary>
		/// Makes string which formats 1000 to 1K and etc.
		/// </summary>
		public static string ToFormattedString(this float value) => FormatNumber(value);

		/// <summary>
		/// Makes string which formats 1000 to 1K and etc.
		/// </summary>
		public static string ToFormattedString(this int value) => FormatNumber(value);

		/// <summary>
		/// Makes string which formats 1000 to 1K and etc.
		/// </summary>
		public static string FormatNumber(float number)
		{
			int index = -1;

			while (number >= suffixes[index + 1].Key)
			{
				++index;
				if (index == suffixes.Count - 1)
				{
					break;
				}
			}

			if (index == -1)
			{
				return $"{Mathf.FloorToInt(number)}";
			}

			float partNumber = number / suffixes[index].Key;

			if (partNumber < 10)
			{
				return string.Format("{0:0.00}{1}", partNumber, suffixes[index].Value);
			}
			else if (partNumber < 100)
			{
				return string.Format("{0:00.0}{1}", partNumber, suffixes[index].Value);
			}
			else
			{
				return string.Format("{0:000}{1}", partNumber, suffixes[index].Value);
			}

		}
	}
}