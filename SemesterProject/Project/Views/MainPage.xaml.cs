using System.Text.Json.Nodes;

namespace SemesterProject;

public partial class MainPage : ContentPage
{
	private Controllers.FlashSetController flashsetscontroller;

	private JsonArray setDataset = new JsonArray();

	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f; 

	public MainPage()
	{
		InitializeComponent();
		MauiProgram.updateTheme(MauiProgram.activeProfile);

		if(File.Exists((MauiProgram.dirPath + MauiProgram.setFile)))
		{
			//retrieve profile file as array
			setDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.setFile);
		}

		flashsetscontroller = new(CollFlashCardSets);

		//profiles array length > 0?
		if (setDataset.Count > 0)
		{
			for (int p = 0; p < setDataset.Count; p++)
			{
				string name = "";
				string author = "";
				string date = "";

				JsonObject tmpSet = new JsonObject();
				try {
					tmpSet = setDataset[p].AsObject();

					name = tmpSet["set-name"]?.ToString() ?? "";
					author = tmpSet["author-name"]?.ToString() ?? "";
					date = tmpSet["last-edited"]?.ToString() ?? "";

					flashsetscontroller.DisplaySet(name, author, date);
				} catch {}
			}
		}
	}

	//stack management functions
	private async void BtnAddSet(object sender, EventArgs e)
	{
		string name = await DisplayPromptAsync("Add New Set", "Enter Name:");

		if (name != null && name.Length > 0)
		{
			int tmpID = flashsetscontroller.FlashCardSets.Count;
			string authName = MauiProgram.activeProfile["name"]?.ToString() ?? "No Author";
			string editDate = DateTime.Today.ToString("MM/dd/yyyy");	

			flashsetscontroller.AddNewFlashCardSet(name, editDate, authName);

			//call JSON profile instantiator, give name & id, rest are defaults
			JsonObject tmpSet = MauiProgram.InstantiateStack(name, tmpID, authName, editDate);
			setDataset.Add(tmpSet);

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(setDataset, (MauiProgram.dirPath + MauiProgram.setFile));
		}
		else if (name != null)
		{
			DisplayAlert("Set not added", "You must provide a name for the set.", "OK");
		}
	}

	private async void BtnSelectSet(object sender, EventArgs e)
	{
		//dehighlight all profiles
		clearHighlights();
		//highlight selected profile
		highlightSet(sender);

		Button btn = sender as Button;

		//set active id to id attached to button
		MauiProgram.stackID = (int) btn.CommandParameter;

		if (MauiProgram.activeID >= 0 && MauiProgram.activeID < flashsetscontroller.FlashCardSets.Count)
		{
			updatePageBtnStates(true);
		}
	}
	
	//Top bar button events
	private async void BtnSignOut(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new LoginPage();
	}

	private async void BtnSettings(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new SettingsPage();
	}

	private async void BtnBack(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new LoginPage(); //back from home = sign out
	}


	//stack centric button events
	private async void BtnEditPage(object sender, EventArgs e)
	{
		if(!FinalizeSelection()) return; //selection was invalid, bail

		App.Current.Windows[0].Page = new EditPage();
	}

	private async void BtnQuizPage(object sender, EventArgs e)
	{
		if(!FinalizeSelection()) return; //selection was invalid, bail

		App.Current.Windows[0].Page = new QuizPage();
	}

	private async void BtnReviewPage(object sender, EventArgs e)
	{
		if(!FinalizeSelection()) return; //selection was invalid, bail

		App.Current.Windows[0].Page = new ReviewPage();
	}

	private bool FinalizeSelection()
	{
		if (MauiProgram.stackID < 0 || MauiProgram.stackID > flashsetscontroller.FlashCardSets.Count)
		{
			updatePageBtnStates(false);
			return false; //if invalid stackID stored / stackID does not point to item in set, abort btn press
		}

		//save modified JSON array to file
		MauiProgram.SaveJSONArrayToFile(setDataset, (MauiProgram.dirPath + MauiProgram.setFile));

		//set active stack fields to match stack at stack id
		MauiProgram.checkinStack(setDataset[MauiProgram.stackID].AsObject());
		
		//selection was successful
		return true;
	}


	//visual helper functions
	private void clearHighlights()
	{
		for (int b = 0; b < flashsetscontroller.FlashCardSets.Count; b++)
		{
			flashsetscontroller.FlashCardSets[b].IsHighlighted = false;
		}
	}

	private void highlightSet (object sender)
	{
		Button btn = sender as Button;

		int id = (int) btn.CommandParameter; 

		flashsetscontroller.FlashCardSets[id].IsHighlighted = true;
	}

	private void updatePageBtnStates(bool state)
	{
		if (state)
		{
			EditBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
			QuizBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
			ReviewBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
		}
		else
		{
			EditBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
			QuizBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
			ReviewBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
		}
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
}

