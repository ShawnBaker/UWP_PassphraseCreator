// Copyright © 2018-2019 Shawn Baker using the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace PassphraseCreator
{
	/// <summary>
	/// Main page.
	/// </summary>
	public sealed partial class MainPage : Page
    {
		// constants
		private const int DefaultNumWords = 5;
		private const int SecondsPerMinute = 60;
		private const int SecondsPerHour = SecondsPerMinute * 60;
		private const int SecondsPerDay = SecondsPerHour * 24;
		private const int SecondsPerWeek = SecondsPerDay * 7;
		private const double SecondsPerMonth = SecondsPerDay * 30.44;
		private const double SecondsPerYear = SecondsPerDay * 365.25;
		private const double SecondsPerCentury = SecondsPerYear * 100;
		private const double SecondsPerMillennium = SecondsPerCentury * 10;
		private const double SecondsPerMillisecond = 0.001;
		private const double SecondsPerMicrosecond = 0.000001;
		private const double SecondsPerNanosecond = 0.000000001;

		// variables
		private List<StringList> wordLists = new List<StringList>();
		private Random random = new Random();
		private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MainPage()
        {
            InitializeComponent();

			if (settings.Values["SetPreferredLaunchViewSize"] == null)
			{
				ApplicationView.PreferredLaunchViewSize = new Size(400, 400);
				ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
				settings.Values["SetPreferredLaunchViewSize"] = true;
			}
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

			InitializeControlsAsync();
		}

		/// <summary>
		/// Handles the list of words selection being changed.
		/// </summary>
		private void HandleListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			StringList wordList = wordLists[listComboBox.SelectedIndex];

			listCountTextBlock.Text = string.Format("({0} {1})", wordList.Count, Res.Str.Words);
			settings.Values["WordList"] = wordList.Name;
		}

		/// <summary>
		/// Handles the number of words changing.
		/// </summary>
		private void HandleNumWordsTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
		{
			string text = "";
			foreach (char ch in numWordsTextBox.Text)
			{
				if (char.IsDigit(ch))
				{
					text += ch;
				}
			}
			numWordsTextBox.Text = text;
			settings.Values["NumWords"] = GetNumWords();
		}

		/// <summary>
		/// Creates a password.
		/// </summary>
		private void HandleCreateButtonClick(object sender, RoutedEventArgs e)
		{
			StringList wordList = wordLists[listComboBox.SelectedIndex];
			int numWords = GetNumWords();
			string passphrase = "";
			for (int i = 0; i < numWords; i++)
			{
				int j = random.Next(wordList.Count);
				passphrase += wordList[j];
			}
			passwordTextBlock.Text = passphrase;
			settings.Values["LastPassphrase"] = passphrase;
			copyButton.IsEnabled = true;
			DisplayEntropy();
		}

		/// <summary>
		/// Copies the password to the clipboard.
		/// </summary>
		private void HandleCopyButtonClick(object sender, RoutedEventArgs e)
		{
			DataPackage dataPackage = new DataPackage();
			dataPackage.SetText(passwordTextBlock.Text);
			Clipboard.SetContent(dataPackage);
		}

		/// <summary>
		/// Displays the about page.
		/// </summary>
		private void HandleAboutButtonClick(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(AboutPage));
		}

		/// <summary>
		/// Reads the word lists and initializes the controls.
		/// </summary>
		public async void InitializeControlsAsync()
		{
			StorageFolder appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
			StorageFolder folder = await appFolder.GetFolderAsync("WordLists");
			var files = await folder.GetFilesAsync();
			int index = (files.Count > 0) ? 0 : -1;
			string defaultListName = settings.Values["WordList"] as string;
			if (string.IsNullOrEmpty(defaultListName))
			{
				defaultListName = "EFF Large";
			}
			for (int i = 0; i < files.Count; i++)
			{
				StorageFile file = files[i];
				IList<string> lines = await FileIO.ReadLinesAsync(file);
				StringList words = new StringList();
				words.Name = Path.GetFileNameWithoutExtension(file.Path);
				if (words.Name == defaultListName)
				{
					index = i;
				}
				foreach (string line in lines)
				{
					string[] parts = line.Split(null);
					words.AddWord((parts.Length > 1) ? parts[1] : line);
				}
				wordLists.Add(words);
				listComboBox.Items.Add(words.Name);
			}
			if (index != -1)
			{
				listComboBox.SelectedIndex = index;
			}

			object obj = settings.Values["NumWords"];
			int numWords = (obj != null) ? ((int)obj) : DefaultNumWords;
			numWordsTextBox.Text = numWords.ToString();

			string passphrase = settings.Values["LastPassphrase"] as string;
			if (!string.IsNullOrEmpty(passphrase))
			{
				passwordTextBlock.Text = passphrase;
				copyButton.IsEnabled = true;
			}

			DisplayEntropy();
		}

		/// <summary>
		/// Calculates and displays the entropy.
		/// </summary>
		private void DisplayEntropy()
		{
			if (passwordTextBlock.Text.Length > 0)
			{
				int index = listComboBox.SelectedIndex;
				int numWords = GetNumWords();
				double wordEntropy = Math.Pow(wordLists[index].Count, numWords);
				double wordLog2 = Math.Log(wordEntropy, 2);
				wordEtropyTextBlock.Text = string.Format("{0:0.#}", wordLog2);
				double entropy = wordEntropy / 1000000;
				SetTimeString(wordMillionTextBlock, entropy);
				entropy /= 1000;
				SetTimeString(wordBillionTextBlock, entropy);
				entropy /= 1000;
				SetTimeString(wordTrillionTextBlock, entropy);

				double charEntropy = Math.Pow(wordLists[index].Symbols.Length, passwordTextBlock.Text.Length);
				double charLog2 = Math.Log(charEntropy, 2);
				charEtropyTextBlock.Text = string.Format("{0:0.#}", charLog2);
				entropy = charEntropy / 1000000;
				SetTimeString(charMillionTextBlock, entropy);
				entropy /= 1000;
				SetTimeString(charBillionTextBlock, entropy);
				entropy /= 1000;
				SetTimeString(charTrillionTextBlock, entropy);
			}
			else
			{
				wordEtropyTextBlock.Text = "0";
				wordMillionTextBlock.Text = "0";
				wordBillionTextBlock.Text = "0";
				wordTrillionTextBlock.Text = "0";
				charEtropyTextBlock.Text = "0";
				charMillionTextBlock.Text = "0";
				charBillionTextBlock.Text = "0";
				charTrillionTextBlock.Text = "0";
			}
		}

		/// <summary>
		/// Gets the number of words from the text box.
		/// </summary>
		/// <returns>Number of words;</returns>
		private int GetNumWords()
		{
			string text = numWordsTextBox.Text.Trim();
			while (text.Length > 0 && text[0] == '0')
			{
				text = text.Substring(1);
			}
			if (text == "")
			{
				text = DefaultNumWords.ToString();
			}
			int numWords = DefaultNumWords;
			if (!int.TryParse(text, out numWords))
			{
				numWords = DefaultNumWords;
			}
			numWordsTextBox.Text = numWords.ToString();
			return numWords;
		}

		/// <summary>
		/// Sets a time string in the entropy table.
		/// </summary>
		/// <param name="textBlock">TextBlock to be set.</param>
		/// <param name="entropy">Entropy value to be displayed.</param>
		private void SetTimeString(TextBlock textBlock, double entropy)
		{
			textBlock.Text = "";
			textBlock.Inlines.Clear();
			if (entropy > SecondsPerMillennium * 1000)
			{
				Run normal = new Run() { Text = "10" };
				textBlock.Inlines.Add(normal);
				Span span = new Span();
				Run superscript = new Run() { Text = string.Format("{0:0}", Math.Log10(entropy)) };
				Typography.SetVariants(superscript, FontVariants.Superscript);
				textBlock.Inlines.Add(superscript);
				Run millennia = new Run() { Text = " " + Res.Str.Millennia };
				textBlock.Inlines.Add(millennia);
			}
			else if (entropy > SecondsPerMillennium)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerMillennium, Res.Str.Millennia);
			}
			else if (entropy > SecondsPerCentury)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerCentury, Res.Str.Centuries);
			}
			else if (entropy > SecondsPerYear)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerYear, Res.Str.Years);
			}
			else if (entropy > SecondsPerMonth)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerMonth, Res.Str.Months);
			}
			else if (entropy > SecondsPerWeek)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerWeek, Res.Str.Weeks);
			}
			else if (entropy > SecondsPerDay)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerDay, Res.Str.Days);
			}
			else if (entropy > SecondsPerHour)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerHour, Res.Str.Hours);
			}
			else if (entropy > SecondsPerMinute)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy / SecondsPerMinute, Res.Str.Minutes);
			}
			else if (entropy > 1)
			{
				textBlock.Text = string.Format("{0:0.#} {1}", entropy, Res.Str.Seconds);
			}
			else if (entropy > SecondsPerMillisecond)
			{
				textBlock.Text = string.Format("{0:0.#} ms", entropy / SecondsPerMillisecond);
			}
			else if (entropy > SecondsPerMicrosecond)
			{
				textBlock.Text = string.Format("{0:0.#} us", entropy / SecondsPerMicrosecond);
			}
			else
			{
				textBlock.Text = string.Format("{0:0.#} ns", entropy / SecondsPerNanosecond);
			}
		}

		/// <summary>
		/// List of strings that has a name and counts the number of unique symbols in the strings.
		/// </summary>
		private class StringList : List<string>
		{
			public string Name { get; set; }
			public string Symbols { get; private set; } = "";

			public void AddWord(string word)
			{
				Add(word);
				foreach (char ch in word)
				{
					if (Symbols.IndexOf(ch) == -1)
					{
						Symbols += ch;
					}
				}
			}
		}
	}
}
