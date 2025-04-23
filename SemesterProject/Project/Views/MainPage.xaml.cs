using System.Text.Json.Nodes;

namespace SemesterProject;

public partial class MainPage : ContentPage
{
	private Controllers.FlashSetController flashsetscontroller;

	private JsonArray setDataset = new JsonArray();
	public MainPage()
	{
		InitializeComponent();
		MauiProgram.updateTheme(MauiProgram.activeProfile);



		if(File.Exists((MauiProgram.dirPath + MauiProgram.prefFile)))
		{
			//retrieve profile file as array, has length > 0?
			JsonObject masterSet = MauiProgram.InstantiateProfile();
			setDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.prefFile);
			if (setDataset.Count > 0)
			{
				masterSet = setDataset[0].AsObject();
			
			}
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

					flashsetscontroller.DisplaySet(name,author,date);
				} catch {}
			}
		}
	}

	private async void BtnAddSet(object sender, EventArgs e)
	{
		string name = await DisplayPromptAsync("Add New Set", "Enter Name:");

		if (name != null)
		{
			int tmpID = flashsetscontroller.FlashCardSets.Count;
			flashsetscontroller.AddNewFlashCardSet(name);

			//call JSON profile instantiator, give name & id, rest are defaults
			JsonObject tmpSet = MauiProgram.InstantiateStack(name, tmpID);
			setDataset.Add(tmpSet);

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(setDataset, (MauiProgram.dirPath + MauiProgram.prefFile));
		}
		else
		{
			DisplayAlert("Set not added", "You must provide a name for the set.", "OK");
		}
	}
	
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
		App.Current.Windows[0].Page = new MainPage();
	}

	private async void BtnEditPage(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new EditPage();
	}

}

