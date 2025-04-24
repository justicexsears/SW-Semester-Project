using System.Text.Json.Nodes;
using System.Diagnostics;

namespace SemesterProject;

public partial class EditPage : ContentPage
{
	private Controllers.FlashCardController flashcardscontroller;

	private JsonArray cardDataset = new JsonArray();

	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f; 

	string stackFile = "err.json";
	string stackName = "err";
	int activeCardID = -1;

	public EditPage()
	{
		InitializeComponent();
		MauiProgram.updateTheme(MauiProgram.activeProfile);

		stackName = (MauiProgram.activeStack["set-name"]?.GetValue<string>() ?? "err");
		stackFile = stackName + ".json";

		stackNameLbl.Text = stackName;
		lastEditedLbl.Text = MauiProgram.activeStack["last-edited"]?.GetValue<string>() ?? "MM/dd/yyyy";
		authorLbl.Text = MauiProgram.activeStack["author-name"]?.GetValue<string>() ?? "John Doe";

		MenuProfileNameLbl.Text = MauiProgram.activeProfile["name"]?.GetValue<string>() ?? "Author N.";

		if (!Directory.Exists((MauiProgram.dirPath + MauiProgram.stackFolder)))
		{
			Directory.CreateDirectory((MauiProgram.dirPath + MauiProgram.stackFolder));
		}

		if(File.Exists((MauiProgram.dirPath + MauiProgram.stackFolder + stackFile)))
		{
			//retrieve profile file as array
			cardDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.stackFolder + stackFile);
		}

		flashcardscontroller = new(CollCards);

		//profiles array length > 0?
		if (cardDataset.Count > 0)
		{
			for (int p = 0; p < cardDataset.Count; p++)
			{
				string question = "";

				JsonObject tmpCard = new JsonObject();
				try {
					tmpCard = cardDataset[p].AsObject();

					question = tmpCard["question"]?.ToString() ?? "";

					flashcardscontroller.DisplayCard(question);
				} catch {}
			}

			DirectSelectCard(0);
		}
		else 
			RevealPreview(false);

		MauiProgram.prevPage = MauiProgram.PageIndex.EDIT;
	}

	//stack management functions
	private void ReindexJSONArray()
	{
		for (int p = 0; p < cardDataset.Count; p++)
		{
			cardDataset[p].AsObject()["id"] = p;
		}
	}

	private async void BtnAddCard(object sender, EventArgs e)
	{
		int tmpID = flashcardscontroller.FlashCards.Count;

		flashcardscontroller.AddNewFlashCard();

		//call JSON profile instantiator, give id, rest are blank to be filled by user
		JsonObject tmpCard = MauiProgram.InstantiateCard(tmpID);
		cardDataset.Add(tmpCard);

		//save modified JSON array to file
		MauiProgram.SaveJSONArrayToFile(cardDataset, (MauiProgram.dirPath + MauiProgram.stackFolder + stackFile));

		DirectSelectCard(tmpID);
	}

	private async void BtnSelectCard(object sender, EventArgs e)
	{
		//dehighlight all profiles
		clearHighlights();
		//highlight selected profile
		highlightCard(sender);

		Button btn = sender as Button;

		//set active id to id attached to button
		activeCardID = (int) btn.CommandParameter;

		if (activeCardID >= 0 && activeCardID < flashcardscontroller.FlashCards.Count)
		{
			RevealPreview(true);
		}
	}

	private void DirectSelectCard(int id)
	{
		if (id < 0 || id >= flashcardscontroller.FlashCards.Count)
		{
			return; //id outside of range
		}

		//dehighlight all profiles
		clearHighlights();
		//highlight selected profile
		DirectHighlightCard(id);

		activeCardID = id;

		RevealPreview(true);

		indexSelector.Text = activeCardID.ToString();
	}

	private async void BtnDeleteCard(object sender, EventArgs e)
	{
		int id = activeCardID;

		bool answer = await DisplayAlert("Remove Card","Are you sure you want to remove this card?","Yes","No");

		if (answer == true)
		{
			//dehighlight all cards
			clearHighlights();
			//reduce activeID
			activeCardID -= 1;


			var result = flashcardscontroller.RemoveCard(id);
			cardDataset.RemoveAt(id);

			DisplayAlert("Removed", "Card Sucessfully Removed!", "OK"); 

			ReindexJSONArray();

			//save modified JSON array to file
			MauiProgram.SaveJSONArrayToFile(cardDataset, (MauiProgram.dirPath + MauiProgram.stackFolder + stackFile));

			if (activeCardID < 0)
			{
				RevealPreview(false);
			}
			else
			{
				DirectSelectCard(activeCardID);
			}
		}
	}

	private async void BtnMoveCard(object sender, EventArgs e)
	{
		int id = activeCardID;

		int index = -1;
		
		int.TryParse((await DisplayPromptAsync("Move Card","Enter new index:")), out index);

		if (index == -1)
			return;

		//clamp target index to reasonable bounds
		if (index >= flashcardscontroller.FlashCards.Count)
			index = flashcardscontroller.FlashCards.Count - 1;
		
		if (index <= 0)
			index = 0;


		flashcardscontroller.FlashCards.Move(id, index);
		flashcardscontroller.ReindexCards();

		JsonNode tmpCard = cardDataset[id];

		cardDataset.RemoveAt(id);

		cardDataset.Insert(index, tmpCard);

		ReindexJSONArray();

		//save modified JSON array to file
		MauiProgram.SaveJSONArrayToFile(cardDataset, (MauiProgram.dirPath + MauiProgram.stackFolder + stackFile));

		activeCardID = index;
	}

	private async void BtnApplyCard(object sender, EventArgs e)
	{
		checkInCard();
	}

	private async void BtnRevertCard(object sender, EventArgs e)
	{
		//this actually pulls from the stored data to load the preview anyway, so effectively reverts.
		RevealPreview(true);
	}

	private async void BtnPrevCard(object sender, EventArgs e)
	{
		int tmpID = activeCardID - 1;

		if (tmpID < 0 || tmpID >= flashcardscontroller.FlashCards.Count)
			return;

		DirectSelectCard(tmpID);
	}

	private async void BtnNextCard(object sender, EventArgs e)
	{
		int tmpID = activeCardID + 1;

		if (tmpID < 0 || tmpID >= flashcardscontroller.FlashCards.Count)
			return;

		DirectSelectCard(tmpID);
	}

	private async void EntryCardSelection(object sender, EventArgs e)
	{
		Entry selector = sender as Entry;

		int index = -1;

		int.TryParse(selector.Text, out index);

		if (index == -1)
		{
			indexSelector.Text = activeCardID.ToString();
			return;
		}

		//clamp target index to reasonable bounds
		if (index >= flashcardscontroller.FlashCards.Count)
			index = flashcardscontroller.FlashCards.Count - 1;
		
		if (index <= 0)
			index = 0;

		DirectSelectCard(index);

		indexSelector.Text = index.ToString();
	}

	private void checkInCard()
	{
		//commit entry field datas to local object
		cardDataset[activeCardID].AsObject()["question"] = qEditor.Text;
		cardDataset[activeCardID].AsObject()["a-short"] = aShortEditor.Text;
		cardDataset[activeCardID].AsObject()["a-long"] = aLongEditor.Text;

		flashcardscontroller.FlashCards[activeCardID].CardQ = qEditor.Text;

		//commit local object to file
		MauiProgram.SaveJSONArrayToFile(cardDataset, (MauiProgram.dirPath + MauiProgram.stackFolder + stackFile));
	}

	//navigation button events
	private async void BtnHome(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new MainPage();
	}

	private async void BtnBack(object sender, EventArgs e)
	{
		BtnHome(sender, e);
	}

	private async void BtnMenuPopout(object sender, EventArgs e)
	{
		MenuPopout.IsVisible = true;
	}

	private async void BtnMenuPopoutClose(object sender, EventArgs e)
	{
		MenuPopout.IsVisible = false;
	}

	private async void BtnSignOut(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new LoginPage();
	}

	private async void BtnSettings(object sender, EventArgs e)
	{
		MauiProgram.prevPage = MauiProgram.PageIndex.EDIT;
		App.Current.Windows[0].Page = new SettingsPage();
	}

	//visual helper functions
	private void clearHighlights()
	{
		for (int b = 0; b < flashcardscontroller.FlashCards.Count; b++)
		{
			flashcardscontroller.FlashCards[b].IsHighlighted = false;
		}
	}

	private void highlightCard (object sender)
	{
		Button btn = sender as Button;

		int id = (int) btn.CommandParameter; 

		flashcardscontroller.FlashCards[id].IsHighlighted = true;
	}

	private void DirectHighlightCard(int id)
	{
		flashcardscontroller.FlashCards[id].IsHighlighted = true;
	}

	private void RevealPreview(bool state)
	{
		if (state)
		{
			//fill out text fields from cardDataset[activeCardID] data
			qEditor.Text = cardDataset[activeCardID]?.AsObject()["question"]?.GetValue<string>() ?? "";
			aShortEditor.Text = cardDataset[activeCardID]?.AsObject()["a-short"]?.GetValue<string>() ?? "";
			aLongEditor.Text = cardDataset[activeCardID]?.AsObject()["a-long"]?.GetValue<string>() ?? "";
		}

		//shift preview shield forward or backward to show or hide other preview elements
		previewShield.ZIndex = state ? -1 : 2;
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