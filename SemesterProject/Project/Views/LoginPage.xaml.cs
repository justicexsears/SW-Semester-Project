//using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Diagnostics;

namespace SemesterProject;

public partial class LoginPage : ContentPage
{
	private string selectedProfile = "";
	private Controllers.ProfileController profileController;

	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f;

	//declare empty profiles array
	private JsonArray profileDataset = new JsonArray();

	public LoginPage()
	{
		InitializeComponent();

		updateSignInState(false);

		//does the profiles file exist on the system?
		if(File.Exists((MauiProgram.dirPath + MauiProgram.prefFile)))
		{
			//retrieve profile file as array, has length > 0?
			profileDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.prefFile);
			if (profileDataset.Count > 0)
			{
				//update global theme fields, and update local UI controls
				MauiProgram.updateTheme(profileDataset[0].AsObject());
				updateUI(profileDataset[0].AsObject());
			}
		}

		profileController = new(CollProfiles);

		//profiles array length > 0?
		if (profileDataset.Count > 0)
		{
			for (int p = 0; p < profileDataset.Count; p++)
			{
				string name = "";
				int theme = 0;
				int accent = 0;

				JsonObject tmpProf = new JsonObject();
				try {
					tmpProf = profileDataset[p].AsObject();

					name = tmpProf["name"]?.ToString() ?? "";
					theme = tmpProf["theme"]?.GetValue<int>() ?? 0;
					accent = tmpProf["accent"]?.GetValue<int>() ?? 0;

					profileController.DisplayProfile(name, theme, accent);
				} catch {}
			}
		}
	}

	private void ReindexJSONArray()
	{
		for (int p = 0; p < profileDataset.Count; p++)
		{
			profileDataset[p].AsObject()["id"] = p;
		}
	}

	private void updateUI(JsonObject pref)
	{

	}

	private void BtnPressed(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		Color btnBG = Color.FromRgba(highlightTint, highlightTint, highlightTint, tintStrength);
		btn.Background = btnBG;
	}

	private void BtnReleased(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		Color btnBG = Color.FromRgba(0f, 0f, 0f, 0f);
		btn.Background = btnBG;
	}

	private async void BtnAddProfile(object sender, EventArgs e)
	{

		string name = await DisplayPromptAsync("Add New Profile", "Enter Name:");

		if (name != null)
		{
			int tmpID = profileController.Profiles.Count;
			profileController.AddNewProfile(name);

			//call JSON profile instantiator, give name & id, rest are defaults
			JsonObject tmpProfile = MauiProgram.InstantiateProfile(name, tmpID);
			profileDataset.Add(tmpProfile);

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(profileDataset, (MauiProgram.dirPath + MauiProgram.prefFile));
		}
	}

	private async void BtnRemoveProfile(object sender, EventArgs e)
    {
		ImageButton btn = sender as ImageButton;

		int id = (int) btn.CommandParameter; 

		bool answer = await DisplayAlert("Remove Profile","Are you sure you want to remove this profile?","Yes","No");

		if (answer == true)
		{
			//does active id match deleting id
			if (MauiProgram.activeID == id)
			{
				//dehighlight all profiles
				clearHighlights();
				//clear activeID
				MauiProgram.activeID = -1;
			}
			else
			{
				//the active id may be disrupted by the index change on profile deletion
				if (id < MauiProgram.activeID)
				{
					//subtract 1 from activeID
					//this offsets the lack of a preceeding button that has been removed somewhere
					MauiProgram.activeID -= 1;
				}
			}


			var result = profileController.RemoveProfile(id);
			profileDataset.RemoveAt(id);
			DisplayAlert("Removed", "Profile Sucessfully Removed!", "OK"); 

			ReindexJSONArray();

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(profileDataset, (MauiProgram.dirPath + MauiProgram.prefFile));

			Debug.WriteLine($"Profiles saved to: {MauiProgram.dirPath + MauiProgram.prefFile}");

			if (MauiProgram.activeID == -1)
			{
				updateSignInState(false);
			}
		}
    }

	private async void BtnSelectProfile(object sender, EventArgs e) 
	{
		//dehighlight all profiles
		clearHighlights();
		//highlight selected profile
		highlightProfile(sender);

		Button btn = sender as Button;

		//set active id to id attached to button
		MauiProgram.activeID = (int) btn.CommandParameter;

		if (MauiProgram.activeID >= 0 && MauiProgram.activeID < profileController.Profiles.Count)
		{
			updateSignInState(true);
		}
	}

	private async void BtnSignIn(object sender, EventArgs e)
	{
		if (MauiProgram.activeID >= 0 && MauiProgram.activeID < profileController.Profiles.Count)
		{
			updateSignInState(false);
			return;
		}

		//save modified JSON array to file
		MauiProgram.SaveJSONArrayToFile(profileDataset, (MauiProgram.dirPath + MauiProgram.prefFile));

		//set active profile fields to match profile at active id
		MauiProgram.checkinProfile(profileDataset[MauiProgram.activeID].AsObject());

		App.Current.Windows[0].Page = new MainPage();
	}

	private void clearHighlights()
	{
		for (int b = 0; b < profileController.Profiles.Count; b++)
		{
			profileController.Profiles[b].IsHighlighted = false;
		}
	}

	private void highlightProfile (object sender)
	{
		Button btn = sender as Button;

		int id = (int) btn.CommandParameter; 

		profileController.Profiles[id].IsHighlighted = true;
	}

	private void updateSignInState(bool state)
	{
		if (state)
		{
			SignInBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
		}
		else
		{
			SignInBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
		}
	}
}