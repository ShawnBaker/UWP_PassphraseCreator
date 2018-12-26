// Copyright © 2018-2019 Shawn Baker using the MIT License.
using Windows.ApplicationModel.Resources;

namespace PassphraseCreator
{
	public static class Res
	{
		// resource loader
		private static ResourceLoader res = ResourceLoader.GetForViewIndependentUse();

		// localized strings
		public static class Str
		{
			public static string AppName { get { return res.GetString("AppName"); } }
			public static string Centuries { get { return res.GetString("Centuries"); } }
			public static string Days { get { return res.GetString("Days"); } }
			public static string Hours { get { return res.GetString("Hours"); } }
			public static string Millennia { get { return res.GetString("Millennia"); } }
			public static string Minutes { get { return res.GetString("Minutes"); } }
			public static string Months { get { return res.GetString("Months"); } }
			public static string Seconds { get { return res.GetString("Seconds"); } }
			public static string Version { get { return res.GetString("Version"); } }
			public static string Weeks { get { return res.GetString("Weeks"); } }
			public static string Words { get { return res.GetString("Words"); } }
			public static string Years { get { return res.GetString("Years"); } }
		}
	}
}
