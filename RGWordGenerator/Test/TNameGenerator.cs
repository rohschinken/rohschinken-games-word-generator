using RGUtility;
using RGWordGenerator;
using UnityEngine;

namespace RGWordGenerator.Tests
{
	public class TNameGenerator : MonoBehaviour
	{
		public int count = 10;

		private void Start ()
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			for (int i = 0; i < count; i++)
			{
				Console.Log(
					RGUtility.Random.Bool ? NameGenerator.GetFirstNameMale().Colored(StringLoggingExtensions.Colors.blue) : NameGenerator.GetFirstNameFemale().Colored(StringLoggingExtensions.Colors.red),
					NameGenerator.GetLastName()
				);
			}

			sw.Stop();
			Console.Log(sw.ElapsedTicks);
		}
	}
}