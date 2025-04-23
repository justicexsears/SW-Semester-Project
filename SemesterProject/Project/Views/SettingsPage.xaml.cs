using System.Text.Json.Nodes;
using System.Diagnostics;

namespace SemesterProject;

public partial class SettingsPage : ContentPage
{
	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f;

	private JsonObject localProf;
	

	public SettingsPage()
	{
		localProf = JsonNode.Parse(MauiProgram.activeProfile.ToJsonString()).AsObject();
		InitializeComponent();

		setDefaults();
		MauiProgram.updateTheme(localProf);
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
	}

	public void BtnRevertChanges(object sender, EventArgs e)
	{
		//Debug.WriteLine($"Settings revert: {localProf["theme"]?.GetValue<int>()} to {MauiProgram.activeProfile["theme"]?.GetValue<int>()}");
		localProf = JsonNode.Parse(MauiProgram.activeProfile.ToJsonString()).AsObject();
		setDefaults();
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
		
		// Q hint
		if (localProf["preferences"]?.AsObject()["q-hint"]?.GetValue<int>() == 1)
			HintCheckbox.IsChecked = true;
		else
			HintCheckbox.IsChecked = false;
	}

	public void themePickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;

		localProf["theme"] = pick.SelectedIndex;

		//preview changes in small window
	}

	public void accentPickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;

		localProf["accent"] = pick.SelectedIndex;

		//preview changes in small window
	}

	public void cStylePickerPreview(object sender, EventArgs e)
	{
		Picker pick = sender as Picker;

		localProf["preferences"].AsObject()["card-style"] = pick.SelectedIndex;

		//preview changes in small window
	}

	public void cAniPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["card-anims"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window
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
	}

	public void shuffPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["q-shuffle"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window
	}

	//hard feature to describe with one word: show or do not show previous incorrect guesses to the current card?
	//guess? fails? i dont know.
	public void guessPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["q-fails"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window
	}

	public void hintPreview(object sender, EventArgs e)
	{
		CheckBox cb = sender as CheckBox;

		localProf["preferences"].AsObject()["q-hint"] = cb.IsChecked ? 1 : 0;

		//preview changes in small window
	}
}


