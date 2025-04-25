using System.Text.Json.Nodes;
using System.Diagnostics;

namespace SemesterProject;

public partial class SettingsPage : ContentPage
{
	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f;

	private JsonObject localProf;
	
	Label[] settingLabels;

	string[] incorrectGuessFiller = {"mercury", "Spiers", "spiders", "Solids", "medicine", "jelly", "honey", "lemon", "salts", "forks"};

	public SettingsPage()
	{
		localProf = JsonNode.Parse(MauiProgram.activeProfile.ToJsonString()).AsObject();
		InitializeComponent();

		settingLabels  = new Label[] {
			ThemeLabel, AccentLabel, StyleLabel, CAniLabel,
			QTimeLabel, QAttemptLabel, ShuffleLabel, GuessLabel, HintLabel
		};

		MenuProfileNameLbl.Text = MauiProgram.activeProfile["name"]?.GetValue<string>() ?? "Author N.";

		setDefaults();
		MauiProgram.updateTheme(localProf);
		previewTheme(localProf);
		clearLabelsEmphasis();
	}

	private void BtnPressed(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		Color btnBG = Color.FromRgba(highlightTint, highlightTint, highlightTint, tintStrength);
		btn.Background = btnBG;
	}

	private async void BtnSignOut(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new LoginPage();
	}

	private async void BtnBack(object sender, EventArgs e)
	{
		MauiProgram.returnFromPage();
	}

	private async void BtnMenuPopout(object sender, EventArgs e)
	{
		MenuPopout.IsVisible = true;
	}

	private async void BtnMenuPopoutClose(object sender, EventArgs e)
	{
		MenuPopout.IsVisible = false;
	}

	private async void BtnHome(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new MainPage();
	}

	private void BtnReleased(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		Color btnBG = Color.FromRgba(0f, 0f, 0f, 0f);
		btn.Background = btnBG;
	}

	private async void BtnApplyChanges(object sender, EventArgs e)
	{
		//this will be used as the storage for picker values so they may persist throgh conditionals
		int tmpInt = ThemePicker.SelectedIndex;

		// Theme
		if (tmpInt >= 0)
			localProf["theme"] = tmpInt;
		else
		 	localProf["theme"] = 1;

		//Accent
		tmpInt = AccentPicker.SelectedIndex;
		if (tmpInt >= 0)
			localProf["accent"] = tmpInt;
		else
			localProf["accent"] = 0;

		// Card Style Default
		tmpInt = CStylePicker.SelectedIndex;
		if (CStylePicker.SelectedIndex >= 0)
			localProf["preferences"].AsObject()["card-style"] = tmpInt;
		else
			localProf["preferences"].AsObject()["card-style"] = 0; 

		// Animations
		localProf["preferences"].AsObject()["card-anims"] = CAniCheckbox.IsChecked ? 1 : 0 ;

		//Q - time
		int resultInt = 120;
		tmpInt = QTimePicker.SelectedIndex;
		switch(tmpInt)
		{
			case 0:
				resultInt = 30;
				break;
			case 1:
				resultInt = 45;
				break;
			case 2:
				resultInt = 60;
				break;
			case 3:
				resultInt = 90;
				break;
			case 4:
			default:
				resultInt = 120;
				break;
		}
		localProf["preferences"].AsObject()["q-time"] = resultInt;

		// Q - attempts
		resultInt = -1;
		tmpInt = QAttemptPicker.SelectedIndex;
		switch(tmpInt)
		{
			case 0:
			default:
				resultInt = -1;
				break;
			case 1:
				resultInt = 1;
				break;
			case 2:
				resultInt = 2;
				break;
			case 3:
				resultInt = 3;
				break;
			case 4:
				resultInt = 5;
				break;
		}

		// Q shuffle
		localProf["preferences"].AsObject()["q-shuffle"] = ShuffCheckbox.IsChecked ? 1 : 0;
		
		// Q fails/guess
		localProf["preferences"].AsObject()["q-fails"] = GuessCheckbox.IsChecked ? 1 : 0;
		
		// Q hint
		localProf["preferences"].AsObject()["q-hint"] = HintCheckbox.IsChecked ? 1 : 0;


		//overwrite profile to disk and to local mem as activeProfile in the process
		MauiProgram.applyProfileChanges(localProf);

		MauiProgram.updateTheme(localProf);

		clearLabelsEmphasis();
	}

	public void BtnRevertChanges(object sender, EventArgs e)
	{
		//Debug.WriteLine($"Settings revert: {localProf["theme"]?.GetValue<int>()} to {MauiProgram.activeProfile["theme"]?.GetValue<int>()}");
		localProf = JsonNode.Parse(MauiProgram.activeProfile.ToJsonString()).AsObject();
		setDefaults();
		previewTheme(localProf);
		clearLabelsEmphasis();
	}

	private void setDefaults()
	{
		//this will be used as the storage for any evaluated ints from the json to reduce formatting calls
		int tmpInt = localProf["theme"]?.GetValue<int>() ?? 1;

		//Eval Theme int
		if (tmpInt >= 0 && tmpInt <= 1)
			ThemePicker.SelectedIndex = tmpInt;
		else
			ThemePicker.SelectedIndex = 0;

		// Eval Accent int
		tmpInt = localProf["accent"]?.GetValue<int>() ?? 0;
		if (tmpInt >= 0 && tmpInt <= 5)
			AccentPicker.SelectedIndex = tmpInt;
		else
			AccentPicker.SelectedIndex = 0;

		// Eval Card Int
		tmpInt = localProf["preferences"]?.AsObject()["card-style"]?.GetValue<int>() ?? 0;
		if (tmpInt >= 0 && tmpInt <= 1)
			CStylePicker.SelectedIndex = tmpInt;
		else
			CStylePicker.SelectedIndex = 0; 

		// Animations
		if (localProf["preferences"]?.AsObject()["card-anims"]?.GetValue<int>() == 1)
			CAniCheckbox.IsChecked = true;
		else
			CAniCheckbox.IsChecked = false;

		//Q - time
		tmpInt = localProf["preferences"]?.AsObject()["q-time"]?.GetValue<int>() ?? 120;
		switch (tmpInt)
		{
			case 30:
				QTimePicker.SelectedIndex = 0;
				break;
			case 45:
				QTimePicker.SelectedIndex = 2;
				break;
			case 60:
				QTimePicker.SelectedIndex = 3;
				break;
			case 90:
				QTimePicker.SelectedIndex = 4;
				break;
			case 120:
			default:
				QTimePicker.SelectedIndex = 5;
				break;
		}

		// Q - attempts
		tmpInt = localProf["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() ?? -1;
		switch (tmpInt)
		{
			case -1:
				QAttemptPicker.SelectedIndex = 0;
				break;
			case 1:
				QAttemptPicker.SelectedIndex = 1;
				break;
			case 2:
				QAttemptPicker.SelectedIndex = 2;
				break;
			case 3:
				QAttemptPicker.SelectedIndex = 3;
				break;
			case 5:
			default:
				QAttemptPicker.SelectedIndex = 4;
				break;
		}

		ShowIncorrects(tmpInt);

		// Q shuffle
		if (localProf["preferences"]?.AsObject()["q-shuffle"]?.GetValue<int>() == 1)
			ShuffCheckbox.IsChecked = true;
		else 
			ShuffCheckbox.IsChecked = false;
		
		// Q fails/guess
		if (localProf["preferences"]?.AsObject()["q-fails"]?.GetValue<int>() == 1)
			GuessCheckbox.IsChecked = true;
		else
			GuessCheckbox.IsChecked = false;

		ToggleIncorrects(GuessCheckbox.IsChecked);
		
		// Q hint
		if (localProf["preferences"]?.AsObject()["q-hint"]?.GetValue<int>() == 1)
			HintCheckbox.IsChecked = true;
		else
			HintCheckbox.IsChecked = false;

		ToggleHint(HintCheckbox.IsChecked);
	}

	public void themePickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;

		localProf["theme"] = pick.SelectedIndex;

		//preview changes in small window
		previewTheme(localProf);

		//highlight label if change compared to active
		bool emphasize = localProf["theme"].GetValue<int>() != MauiProgram.activeProfile["theme"].GetValue<int>();
			
		emphasizeTextLabel(0, emphasize);
	}

	public void accentPickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;

		localProf["accent"] = pick.SelectedIndex;

		//preview changes in small window
		previewTheme(localProf);

		//highlight label if change compared to active
		bool emphasize = localProf["accent"].GetValue<int>() != MauiProgram.activeProfile["accent"].GetValue<int>();
		
		emphasizeTextLabel(1, emphasize);
	}

	public void cStylePickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;

		localProf["preferences"].AsObject()["card-style"] = pick.SelectedIndex;

		//preview changes in small window
		previewTheme(localProf);

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["card-style"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["card-style"].GetValue<int>();

		emphasizeTextLabel(2, emphasize);
	}

	public void cAniPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["card-anims"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["card-anims"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["card-anims"].GetValue<int>();

		emphasizeTextLabel(3, emphasize);
	}

	public void qTimePickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;
		int resultInt = 120;

		switch (pick.SelectedIndex)
		{
			case 0:
				resultInt = 30;
				break;
			case 1:
				resultInt = 45;
				break;
			case 2:
				resultInt = 60;
				break;
			case 3:
				resultInt = 90;
				break;
			case 4:
			default:
				resultInt = 120;
				break;
		}

		localProf["preferences"].AsObject()["q-time"] = resultInt;

		//preview changes in small window

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["q-time"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["q-time"].GetValue<int>();

		emphasizeTextLabel(4, emphasize);
	}

	public void qAttemptPickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;
		int resultInt = 120;

		switch (pick.SelectedIndex)
		{
			case 0:
			default:
				resultInt = -1;
				break;
			case 1:
				resultInt = 1;
				break;
			case 2:
				resultInt = 2;
				break;
			case 3:
				resultInt = 3;
				break;
			case 4:
				resultInt = 5;
				break;
		}

		localProf["preferences"].AsObject()["q-attempts"] = resultInt;

		//preview changes in small window
		ShowIncorrects(resultInt);

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["q-attempts"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["q-attempts"].GetValue<int>();

		emphasizeTextLabel(5, emphasize);
	}

	private async void ShowIncorrects(int index)
	{
		//use 10 previews to show infinite
		int limit = (index == -1 ? 10 : index);
		IncorrectAnswersLabel.Text = "";
		for (int i = 0; i < limit - 1; i++) {
			IncorrectAnswersLabel.Text += $"\nIncorrect: {incorrectGuessFiller[i]}";
		}

		await Task.Delay(50);
		await messageFeed.ScrollToAsync(0, IncorrectAnswersLabel.Height, animated: false);
	}

	public void shuffPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["q-shuffle"] = cb.IsChecked ? 1 : 0;

		//no changes to preview in small window;

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["q-shuffle"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["q-shuffle"].GetValue<int>();

		emphasizeTextLabel(6, emphasize);
	}

	//hard feature to describe with one word: show or do not show previous incorrect guesses to the current card?
	//guess? fails? i dont know.
	public void guessPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["q-fails"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window
		ToggleIncorrects(cb.IsChecked);

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["q-fails"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["q-fails"].GetValue<int>();

		emphasizeTextLabel(7, emphasize);
	}

	private void ToggleIncorrects(bool state)
	{
		IncorrectAnswersLabel.IsVisible = state;
	}

	public void hintPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["q-hint"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window
		ToggleHint(cb.IsChecked);

		//highlight label if change compared to active
		bool emphasize = localProf["preferences"].AsObject()["q-hint"].GetValue<int>() != MauiProgram.activeProfile["preferences"].AsObject()["q-hint"].GetValue<int>();

		emphasizeTextLabel(8, emphasize);
	}

	private void ToggleHint(bool state)
	{
		hintLabel.IsVisible = state;
	}

	public static void previewTheme(JsonObject pref)
	{
		string themeName = "DarkGreen";
		string cardstyle = "BW";

		string prefTheme = (pref["theme"]?.GetValue<int>() ?? 0) == 0 ? "Light" : "Dark";
		int prefAccent = pref["accent"]?.GetValue<int>() ?? 0;
		string prefCStyle = (pref["preferences"].AsObject()["card-style"]?.GetValue<int>() ?? 0) == 0 ? "BW" : "Match";
		switch(prefAccent)
		{
			default:
			case 0:
				themeName = prefTheme + "Red";
				break;
			case 1:
				themeName = prefTheme + "Orange";
				break;
			case 2:
				themeName = prefTheme + "Yellow";
				break;
			case 3:
				themeName = prefTheme + "Green";
				break;
			case 4:
				themeName = prefTheme + "Blue";
				break;
			case 5:
				themeName = prefTheme + "Purple";
				break;
		}

		SetPreviewTheme(themeName);

		if (prefCStyle == "Match")
			cardstyle = themeName;
		else
			cardstyle = prefCStyle;

		SetPreviewCardTheme(cardstyle);
	}

	public static void SetPreviewCardTheme(string cardstyle)
	{
		//Preferences.Set("CardStyle", cardstyle);

		ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			foreach (ResourceDictionary dictionaries in mergedDictionaries)
			{
				var backgroundFound = dictionaries.TryGetValue(cardstyle + "CBG", out var cbg);
				if (backgroundFound)
					dictionaries["CardBackgroundPV"] = cbg;

				var textFound = dictionaries.TryGetValue(cardstyle + "CT", out var ctext);
				if (textFound)
					dictionaries["CardTextPV"] = ctext;
			}
		}

	}

	public static void SetPreviewTheme(string themename)
	{
		//Preferences.Set("Theme", themename);

		ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			foreach (ResourceDictionary dictionaries in mergedDictionaries)
			{
				var primaryFound = dictionaries.TryGetValue(themename + "Primary", out var primary);
				if (primaryFound)
					dictionaries["PrimaryPV"] = primary;
				
				var secondaryFound = dictionaries.TryGetValue(themename + "Secondary", out var secondary);
				if (secondaryFound)
					dictionaries["SecondaryPV"] = secondary;

				var tertiaryFound = dictionaries.TryGetValue(themename + "Tertiary", out var tertiary);
				if (tertiaryFound)
					dictionaries["TertiaryPV"] = tertiary;

				var accentFound = dictionaries.TryGetValue(themename + "Accent", out var accent);
				if (accentFound)
					dictionaries["AccentPV"] = accent;
				
				var textFound = dictionaries.TryGetValue(themename + "Text", out var text);
				if (textFound)
					dictionaries["MainTextPV"] = text;

				var headerFound = dictionaries.TryGetValue(themename + "Header", out var header);
				if (textFound)
					dictionaries["HeaderTextPV"] = header;
			}
		}
	}

	private void clearLabelsEmphasis()
	{
		for (int i = 0; i < settingLabels.Length; i++)
		{
			emphasizeTextLabel(i, false);
		}
	}

	private void emphasizeTextLabel(int ind, bool state)
	{
		Label lbl = settingLabels[ind];
		lbl.SetDynamicResource(Label.TextColorProperty, state ? "Accent" : "MainText");
	}
}


