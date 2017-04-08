using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RGWordGenerator
{
	/// <summary>
	/// Name generator
	/// </summary>
	public static class NameGenerator
	{
		private static readonly TextAsset RESOURCE_FIRST_NAMES_M;
		private static readonly TextAsset RESOURCE_FIRST_NAMES_F;
		private static readonly TextAsset RESOURCE_LAST_NAMES;
		private static readonly TextAsset RESOURCE_NAME_PRE_STARTERS;
		private static readonly TextAsset RESOURCE_NAME_STARTERS;
		private static readonly TextAsset RESOURCE_NAME_ENDINGS;

		private static readonly string[] FIRST_NAMES_M;
		private static readonly string[] FIRST_NAMES_F;
		private static readonly string[] LAST_NAMES;
		private static readonly string[] NAME_STARTERS;
		private static readonly string[] NAME_ENDINGS;
		private static readonly List<KeyValuePair<string, int>> NAME_PRE_STARTERS;

		private static readonly MarkovWordGenerator _markovLastNameGen;

		private static readonly char[] CONSONANTS = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
		private static readonly char[] VOWELS = { 'a', 'e', 'i', 'o', 'u' };

		private static readonly List<KeyValuePair<string, int>> NAME_PATTERNS = new List<KeyValuePair<string, int>>() {
			new KeyValuePair<string, int>("CvcE", 5),
			new KeyValuePair<string, int>("SvE", 6),
			new KeyValuePair<string, int>("CvE", 7),
			new KeyValuePair<string, int>("SvcE", 2),
			new KeyValuePair<string, int>("ScvE", 2),
			new KeyValuePair<string, int>("SE", 7),
			new KeyValuePair<string, int>("SmE", 1),
			new KeyValuePair<string, int>("ME", 3),
			new KeyValuePair<string, int>("Sm", 3),
			new KeyValuePair<string, int>("MvE", 2),
			new KeyValuePair<string, int>("M", 10),
			new KeyValuePair<string, int>("VE", 2),
			new KeyValuePair<string, int>("PM", 2),
			new KeyValuePair<string, int>("PSvE", 1),
			new KeyValuePair<string, int>("PVE", 1)
		};

		static NameGenerator ()
		{
			RESOURCE_FIRST_NAMES_M = Resources.Load("names/firstnames_m", typeof(TextAsset)) as TextAsset;
			RESOURCE_FIRST_NAMES_F = Resources.Load("names/firstnames_f", typeof(TextAsset)) as TextAsset;
			RESOURCE_LAST_NAMES = Resources.Load("names/lastnames", typeof(TextAsset)) as TextAsset;
			RESOURCE_NAME_PRE_STARTERS = Resources.Load("names/name_pre_starters", typeof(TextAsset)) as TextAsset;
			RESOURCE_NAME_STARTERS = Resources.Load("names/name_starters", typeof(TextAsset)) as TextAsset;
			RESOURCE_NAME_ENDINGS = Resources.Load("names/name_endings", typeof(TextAsset)) as TextAsset;

			FIRST_NAMES_M = RESOURCE_FIRST_NAMES_M.text.Split('\n', '\r');
			FIRST_NAMES_F = RESOURCE_FIRST_NAMES_F.text.Split('\n', '\r');
			LAST_NAMES = RESOURCE_LAST_NAMES.text.Split('\n', '\r');
			NAME_STARTERS = RESOURCE_NAME_STARTERS.text.Split('\n', '\r');
			NAME_ENDINGS = RESOURCE_NAME_ENDINGS.text.Split('\n', '\r');
			NAME_PRE_STARTERS = new List<KeyValuePair<string, int>>();

			string[] preStarterEntries = RESOURCE_NAME_PRE_STARTERS.text.Split('\n', '\r');

			for (int i = 0; i < preStarterEntries.Length; i++)
			{
				string[] entry = preStarterEntries[i].Split(',');
				int weight = 0;
				Int32.TryParse(entry[1], out weight);
				NAME_PRE_STARTERS.Add(new KeyValuePair<string, int>(entry[0], weight));
			}

			_markovLastNameGen = new MarkovWordGenerator(LAST_NAMES, 3, 2);
		}

		#region private static methods
		private static String GetNamePattern ()
		{
			return RGUtility.Random.FromWeighted(NAME_PATTERNS);
		}

		private static Char GetConsonant ()
		{
			return RGUtility.Random.From(CONSONANTS);
		}

		private static Char GetVowel ()
		{
			return RGUtility.Random.From(VOWELS);
		}

		private static String GetNamePreStarter ()
		{
			return RGUtility.Random.FromWeighted(NAME_PRE_STARTERS);
		}

		private static String GetNameStarter ()
		{
			return RGUtility.Random.From(NAME_STARTERS);
		}

		private static String GetNameEnding ()
		{
			return RGUtility.Random.From(NAME_ENDINGS);
		}
		#endregion private static methods

		#region public static methods
		public static string GetLastName ()
		{
			string p = GetNamePattern();
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < p.Length; i++)
			{
				/// C: consonant (uppercase)
				/// c: consonant (lowercase)
				/// V: vowel (uppercase)
				/// v: vowel (lowercase)
				/// P: Name Pre Starter
				/// S: Name Starter
				/// E: Name Ending
				/// M: markov generated name (first letter uppercase)
				/// m: markov generated name (first letter lowercase)
				switch (p[i])
				{
					case 'S':
						string s = GetNameStarter();
						if ((Array.IndexOf(VOWELS, s[s.Length - 1]) > -1) && (p[i + 1] == 'v' || p[i + 1] == 'V'))
						{
							i++;
						}
						sb.Append(s);
						break;

					case 'P':
						sb.Append(GetNamePreStarter());
						break;

					case 'M':
						sb.Append(_markovLastNameGen.NextName);
						break;

					case 'm':
						string name = _markovLastNameGen.NextName;
						sb.Append(Char.ToLowerInvariant(name[0]) + name.Substring(1));
						break;

					case 'c':
						sb.Append(GetConsonant());
						break;

					case 'v':
						sb.Append(GetVowel());
						break;

					case 'C':
						sb.Append(Char.ToUpper(GetConsonant()));
						break;

					case 'V':
						sb.Append(Char.ToUpper(GetVowel()));
						break;

					case 'E':
						string e = GetNameEnding();
						if ((Array.IndexOf(VOWELS, e[0]) > -1) && (p[i - 1] == 'v' || p[i - 1] == 'V'))
						{
							sb.Append(e.Substring(1));
						}
						else
						{
							sb.Append(e);
						}
						break;

					case ' ':
						sb.Append(' ');
						break;
				}
			}

			return sb.ToString() + " (" + p + ")";
		}

		public static String GetFirstNameMale ()
		{
			return RGUtility.Random.From(FIRST_NAMES_M);
		}

		public static String GetFirstNameFemale ()
		{
			return RGUtility.Random.From(FIRST_NAMES_F);
		}
		#endregion public static methods
	}
}