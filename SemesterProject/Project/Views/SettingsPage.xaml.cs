using System.Text.Json.Nodes;

namespace SemesterProject;

public partial class SettingsPage : ContentPage
{
	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f;

	private JsonObject localProf;
	

	public SettingsPage()
	{
		localProf = MauiProgram.activeProfile;
		InitializeComponent();

		setDefaults();



	}

	private async void BtnApplyChanges(object sender, EventArgs e)
	{
			// Theme Default
		if (ThemePicker.SelectedIndex == 0)
			localProf["theme"] = 0;
		else
		 	localProf["theme"] = 1;

		// Accent Default
		if (AccentPicker.SelectedIndex == 0)
			localProf["accent"] = 0;
		else if (AccentPicker.SelectedIndex == 1)
			localProf["accent"] = 1;
		else if (AccentPicker.SelectedIndex == 2)
			localProf["accent"] = 2;
		else if (AccentPicker.SelectedIndex == 3)
			localProf["accent"] = 3;
		else if (AccentPicker.SelectedIndex == 4)
			localProf["accent"] = 4;
		else if (AccentPicker.SelectedIndex == 5)
			localProf["accent"] = 5;

		// Card Style Default
		if (CStylePicker.SelectedIndex == 1)
			localProf["preferences"].AsObject()["card-style"] = 1;
		else
			localProf["preferences"].AsObject()["card-style"] = 0; 

		// Animations
		if (CAniCheckbox.IsChecked == true)
			localProf["preferences"].AsObject()["card-anims"] = 1;
		else
			localProf["preferences"].AsObject()["card-anims"] = 0;

		//Q - time
		if (QTimePicker.SelectedIndex == 0)
			localProf["preferences"].AsObject()["q-time"] = 30;
		else if (QTimePicker.SelectedIndex == 1)
			localProf["preferences"].AsObject()["q-time"] = 45;
		else if (QTimePicker.SelectedIndex == 2)
			localProf["preferences"].AsObject()["q-time"] = 60;
		else if (QTimePicker.SelectedIndex == 3)
			localProf["preferences"].AsObject()["q-time"] = 90;
		else if (QTimePicker.SelectedIndex == 4)
			localProf["preferences"].AsObject()["q-time"] = 120;

		// Q - attempts
		if (QAttemptPicker.SelectedIndex == 0)
			localProf["preferences"].AsObject()["q-attempts"] = -1;
		else if (QAttemptPicker.SelectedIndex == 1)
			localProf["preferences"].AsObject()["q-attempts"] = 1;
		else if (QAttemptPicker.SelectedIndex == 2)
			localProf["preferences"].AsObject()["q-attempts"] = 2;
		else if (QAttemptPicker.SelectedIndex == 3)
			localProf["preferences"].AsObject()["q-attempts"] = 3;
		else if (QAttemptPicker.SelectedIndex == 5)
			localProf["preferences"].AsObject()["q-attempts"] = 4;

		// Q shuffle
		if (ShuffCheckbox.IsChecked == true)
			localProf["preferences"].AsObject()["q-shuffle"] = 1;
		else 
			localProf["preferences"].AsObject()["q-shuffle"] = 0;
		
		// Q fails/guess
		if (GuessCheckbox.IsChecked == true)
			localProf["preferences"].AsObject()["q-fails"] = 1;
		else
			localProf["preferences"].AsObject()["q-fails"] = 0;
		
		// Q hint
		if (HintCheckbox.IsChecked = true)
			localProf["preferences"].AsObject()["q-hint"] = 1;
		else
			localProf["preferences"].AsObject()["q-hint"] = 0;

	}

	private void setDefaults()
	{
		// Theme Default
		if (localProf["theme"].GetValue<int>() == 0)
			ThemePicker.SelectedIndex = 0;
		else
		 	ThemePicker.SelectedIndex = 1;

		// Accent Default
		if (localProf["accent"].GetValue<int>() == 0)
			AccentPicker.SelectedIndex = 0;
		else if (localProf["accent"].GetValue<int>() == 1)
			AccentPicker.SelectedIndex = 1;
		else if (localProf["accent"].GetValue<int>() == 2)
			AccentPicker.SelectedIndex = 2;
		else if (localProf["accent"].GetValue<int>() == 3)
			AccentPicker.SelectedIndex = 3;
		else if (localProf["accent"].GetValue<int>() == 4)
			AccentPicker.SelectedIndex = 4;
		else if (localProf["accent"]?.GetValue<int>() == 5)
			AccentPicker.SelectedIndex = 5;

		// Card Style Default
		if (localProf["preferences"]?.AsObject()["card-style"]?.GetValue<int>() == 1)
			CStylePicker.SelectedIndex = 1;
		else
			CStylePicker.SelectedIndex = 0; 

		// Animations
		if (localProf["preferences"]?.AsObject()["card-anims"]?.GetValue<int>() == 1)
			CAniCheckbox.IsChecked = true;
		else
			CAniCheckbox.IsChecked = false;

		//Q - time
		if (localProf["preferences"]?.AsObject()["q-time"]?.GetValue<int>() == 30)
			QTimePicker.SelectedIndex = 0;
		else if (localProf["preferences"]?.AsObject()["q-time"]?.GetValue<int>() == 45)
			QTimePicker.SelectedIndex = 1;
		else if (localProf["preferences"]?.AsObject()["q-time"]?.GetValue<int>() == 60)
			QTimePicker.SelectedIndex = 2;
		else if (localProf["preferences"]?.AsObject()["q-time"]?.GetValue<int>() == 90)
			QTimePicker.SelectedIndex = 3;
		else if (localProf["preferences"]?.AsObject()["q-time"]?.GetValue<int>() == 120)
			QTimePicker.SelectedIndex = 4;

		// Q - attempts
		if (localProf["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() == -1)
			QAttemptPicker.SelectedIndex = 0;
		else if (localProf["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() == 1)
			QAttemptPicker.SelectedIndex = 1;
		else if (localProf["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() == 2)
			QAttemptPicker.SelectedIndex = 2;
		else if (localProf["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() == 3)
			QAttemptPicker.SelectedIndex = 3;
		else if (localProf["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() == 5)
			QAttemptPicker.SelectedIndex = 4;

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
}


