// Copyright © 2018-2019 Shawn Baker using the MIT License.
using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PassphraseCreator
{
	/// <summary>
	/// About page.
	/// </summary>
	public sealed partial class AboutPage : Page
	{
		public AboutPage()
		{
			InitializeComponent();

			nameTextBlock.Text = Res.Str.AppName;
			PackageVersion version = Package.Current.Id.Version;
			string ver = string.Format("{0} {1}.{2}", Res.Str.Version, version.Major, version.Minor);
			if (version.Build != 0)
			{
				ver += "." + version.Build.ToString();
			}
			if (version.Revision != 0)
			{
				ver += "." + version.Revision.ToString();
			}
			versionTextBlock.Text = ver;
			copyrightTextBlock.Text = "Copyright \u00A9 2018-2019 Shawn Baker";
		}

		/// <summary>
		/// Displays the previous page.
		/// </summary>
		private void HandleBackButtonClick(object sender, RoutedEventArgs e)
		{
			Frame.GoBack();
		}

		/// <summary>
		/// Displays the help page.
		/// </summary>
		private void HandleHelpButtonClick(object sender, RoutedEventArgs e)
		{
			Uri uri = new Uri(@"http://xkcd.com/936/");
			Windows.System.Launcher.LaunchUriAsync(uri);
		}
	}
}
