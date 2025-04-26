using System.Text.Json.Nodes;
using System.Linq;
using System.Diagnostics;

namespace SemesterProject;

public partial class MainPage : ContentPage
{
	private Controllers.FlashSetController flashsetscontroller;

	private JsonArray setDataset = new JsonArray();
	private List<setData> sortableDataset;

	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f; 

	public MainPage()
	{
		InitializeComponent();
        MauiProgram.updateTheme(MauiProgram.activeProfile);

		MenuProfileNameLbl.Text = MauiProgram.activeProfile["name"]?.GetValue<string>() ?? "Author N.";

		if(File.Exists((MauiProgram.dirPath + MauiProgram.setFile)))
		{
			//retrieve profile file as array
			setDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.setFile);
		}

		flashsetscontroller = new(CollFlashCardSets);

		MauiProgram.prevPage = MauiProgram.PageIndex.HOME;

		MauiProgram.stackID = -1;

		sortableDataset = setDataset
			.Select(item => new setData(
				item?["id"]?.GetValue<int>() ?? -1,
				item?["author-name"]?.GetValue<string>() ?? "",
				item?["set-name"]?.GetValue<string>() ?? "",
				item?["last-edited"]?.GetValue<string>() ?? ""
			))
			.ToList();

		PopulateDisplay(sortableDataset);
		SearchTypePicker.SelectedIndex = 0;
		SortTypePicker.SelectedIndex = 0;
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

			//flashsetscontroller.AddNewFlashCardSet(name, editDate, authName);

			//call JSON profile instantiator, give name & id, rest are defaults
			JsonObject tmpSet = MauiProgram.InstantiateStack(name, tmpID, authName, editDate);
			setDataset.Add(tmpSet);

			//add to sortable dataset as well
			sortableDataset.Add(new setData(
				tmpSet?["id"]?.GetValue<int>() ?? -1,
				tmpSet?["author-name"]?.GetValue<string>() ?? "",
				tmpSet?["set-name"]?.GetValue<string>() ?? "",
				tmpSet?["last-edited"]?.GetValue<string>() ?? ""
			));

			//call sort method to regenerate set
			SortDataset();

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(setDataset, (MauiProgram.dirPath + MauiProgram.setFile));
		}
		else if (name != null)
		{
			DisplayAlert("Set not added", "You must provide a name for the set.", "OK");
		}
	}

	private async void BtnRemoveSet(object sender, EventArgs e)
    {
		ImageButton btn = sender as ImageButton;

		bool answer = await DisplayAlert("Remove Profile","Are you sure you want to remove this profile?","Yes","No");

		if (answer == true)
		{
			//remove from sortable dataset
			setData toRemove = sortableDataset[MauiProgram.stackID];
			sortableDataset.Remove(toRemove);

			//remove from collection view
			var result = flashsetscontroller.RemoveSetID(MauiProgram.stackID);

			//remove from stored dataset
			setDataset.RemoveAt(MauiProgram.stackID);

			DisplayAlert("Removed", "Profile Sucessfully Removed!", "OK"); 

			ReindexJSONArray();

			ReindexSortable();

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(setDataset, (MauiProgram.dirPath + MauiProgram.setFile));

			MauiProgram.stackID = -1;

			if (MauiProgram.stackID == -1)
			{
				updatePageBtnStates(false);
			}
		}
    }

	private void ReindexJSONArray()
	{
		for (int s = 0; s < setDataset.Count; s++)
		{
			setDataset[s].AsObject()["id"] = s;
		}
	}

	private void ReindexSortable()
	{
		for (int s = 0; s < setDataset.Count; s++)
		{
			sortableDataset[s].SetID = s;
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

		flashsetscontroller.GetByID(id).IsHighlighted = true;
	}

	private void highlightSetID (int id)
	{
		if (id >= 0 && id < flashsetscontroller.FlashCardSets.Count)
		{
			flashsetscontroller.GetByID(id).IsHighlighted = true;
		}
	}

	private void updatePageBtnStates(bool state)
	{
		if (state)
		{
			EditBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
			QuizBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
			ReviewBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
			RemoveSetBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
		}
		else
		{
			EditBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
			QuizBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
			ReviewBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
			RemoveSetBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
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

	private void SortTrigger(object sender, EventArgs e)
	{
		SortDataset();
	}

	private void SortDataset()
	{
		string searchTerm = searchEntry.Text?.ToLower() ?? "";

		int searchType = SearchTypePicker.SelectedIndex;
		int sortType = SortTypePicker.SelectedIndex;

		bool dateFiltering = DateFiltering.IsChecked;
		string start = StartDate.Date.ToString("yyyy-MM-dd");
		string end = EndDate.Date.ToString("yyyy-MM-dd");

		List<setData> sortedList;

		//filter list to reduce sort cost
		if (searchTerm != "")
		{
			switch (searchType)
			{
				default:
				case 0: //searching by title
					sortedList = sortableDataset
						.Where(s => s.SetName.ToLower().Contains(searchTerm))
						.ToList();
					break;
				case 1: //searching by author
					sortedList = sortableDataset
						.Where(s => s.SetAuth.ToLower().Contains(searchTerm))
						.ToList();
					break;
			}
		}
		else //no search term provided, cannot filter by search, copy whole list
			sortedList = new List<setData>(sortableDataset);

		//filter by date if applicable
		if (dateFiltering)
		{
			sortedList = sortedList
				.Where(s => 
					(string.Compare(s.SetFormattedDate, start) >= 0) &&
					(string.Compare(s.SetFormattedDate, end) <= 0)
				)
				.ToList();
		}

		//determine sort parameters and sort filtered list
		switch(sortType)
		{
			//default and sort by set name
			default:
			case 0:
				sortedList.Sort((a, b) => a.SetName.CompareTo(b.SetName));
				break;
			case 1:
				sortedList.Sort((a, b) => b.SetName.CompareTo(a.SetName));
				break;

			//sort by set author
			case 2:
				sortedList.Sort((a, b) => a.SetAuth.CompareTo(b.SetAuth));
				break;
			case 3:
				sortedList.Sort((a, b) => b.SetAuth.CompareTo(a.SetAuth));
				break;

			//sort by set date
			case 4:
				sortedList.Sort((a, b) => a.SetFormattedDate.CompareTo(b.SetFormattedDate));
				break;
			case 5:
				sortedList.Sort((a, b) => b.SetFormattedDate.CompareTo(a.SetFormattedDate));
				break;
		}

		//check to see if highlighted is still available
		if (MauiProgram.stackID >= 0)
		{
			bool present = false;
			for (int s = 0; s < sortedList.Count; s++)
			{
				if (sortedList[s].SetID == MauiProgram.stackID)
				{
					present = true;
					break;
				}
			}

			if (!present)
			{
				MauiProgram.stackID = -1;
				updatePageBtnStates(false);
			}
		}

		RepopulateDisplay(sortedList);
	}

	private void PopulateDisplay(List<setData> displaySets)
	{
		if (displaySets.Count > 0)
		{
			for (int p = 0; p < displaySets.Count; p++)
			{
				int id = 0;
				string name = "";
				string author = "";
				string date = "";

				setData tmpSet;
				try {
					tmpSet = displaySets[p];

					name = tmpSet.SetName;
					author = tmpSet.SetAuth;
					date = tmpSet.SetDate;
					id = tmpSet.SetID;

					flashsetscontroller.DisplaySet(name, author, date, id);
				} catch {}
			}
		}
	}

	private void RepopulateDisplay(List<setData> sortedSets)
	{
		//clear current population
		flashsetscontroller.Clear();

		//populate with new data
		PopulateDisplay(sortedSets);

		//rehighlight active set
		highlightSetID(MauiProgram.stackID);
	}
}

public class setData
{
	public int SetID {get;set;} = 0;
	public string SetAuth {get;set;} = "";
	public string SetName {get;set;} = "";
	public string SetDate {get;set;} = "";
	public string SetFormattedDate {get;set;} = "";

	public setData(int id, string a, string n, string d)
	{
		SetID = id;
		SetAuth = a;
		SetName = n;
		SetDate = d;
		SetFormattedDate = reformatJsonDate(d);
	}

	public string reformatJsonDate(string old)
	{
		string tmpDate = "";

		//rearranges from MM-dd-yyyy to yyyy-MM-dd for alphabetic sorting properties
		tmpDate += old.Substring(6) + "-";
		tmpDate += old.Substring(0, 2) + "-";
		tmpDate += old.Substring(3, 2);

		return tmpDate;
	}
}