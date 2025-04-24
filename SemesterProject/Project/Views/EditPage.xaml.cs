using System.Text.Json.Nodes;

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

		if(File.Exists((MauiProgram.dirPath + MauiProgram.stackFolder + stackFile)))
		{
			//retrieve profile file as array
			cardDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.stackFolder + stackFile);
		}

		flashcardscontroller = new(CollCards);

		//profiles array length > 0?
		if (cardDataset.Count > 0)
		{
			activeCardID = 0;
			RevealPreview(true);

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
		}

		RevealPreview(false);
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

	private async void BtnDeleteCard(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		int id = (int) btn.CommandParameter; 

		bool answer = await DisplayAlert("Remove Card","Are you sure you want to remove this card?","Yes","No");

		if (answer == true)
		{
			//dehighlight all cards
			clearHighlights();
			//clear activeID
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
		}
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
}