using System.Text.Json.Nodes;
using SemesterProject.Controllers;

namespace SemesterProject;

public partial class ReviewPage : ContentPage
{
	private Controllers.FlashCardController flashcardscontroller;

    private JsonArray cardDataset = new JsonArray();

    private float highlightTint = 0.05f;
    private float tintStrength = 0.7f;

	private string selectedQ = "";
	private int activeCardID = -1;

	string stackFile = "err.json";
	string stackName = "err";

	bool showingFront = true;
	bool awaitingFlip = false;

    public ReviewPage()
    {
        InitializeComponent();
        MauiProgram.updateTheme(MauiProgram.activeProfile);

		stackName = (MauiProgram.activeStack["set-name"]?.GetValue<string>() ?? "err");
        stackFile = stackName + ".json";

		MenuProfileNameLbl.Text = MauiProgram.activeProfile["name"]?.GetValue<string>() ?? "Author N.";

		if (!Directory.Exists((MauiProgram.dirPath + MauiProgram.stackFolder)))
		{
			Directory.CreateDirectory((MauiProgram.dirPath + MauiProgram.stackFolder));
		}

        if(File.Exists((MauiProgram.dirPath + MauiProgram.stackFolder + stackFile)))
        {
            //retrieve set file as array
            cardDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.stackFolder + stackFile);
        }
		else
		{
			App.Current.Windows[0].Page = new MainPage(); //file does not exist, cannot continue
		}

        flashcardscontroller = new(CollFlashCards);

        //set array length > 0?
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
		{
			//nothing to display? no set to review.. returning to main
			App.Current.Windows[0].Page = new MainPage();
		}

		SetHeader();

    }

	private async void BtnBack(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new MainPage(); 
	}

	private async void BtnSignOut(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new LoginPage();
	}

	private async void BtnSettings(object sender, EventArgs e)
	{
		MauiProgram.prevPage = MauiProgram.PageIndex.REVIEW;
		App.Current.Windows[0].Page = new SettingsPage();
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

	private void SetHeader()
	{
		AuthorLabel.Text = MauiProgram.activeStack["author-name"]?.ToString();

		DateLabel.Text = MauiProgram.activeStack["last-edited"]?.ToString();

		NameLabel.Text = MauiProgram.activeStack["set-name"]?.ToString();

	}

	private async void BtnSelectCard(object sender, EventArgs e)
	{
		//dehighlight all profiles
		clearHighlights();
		//highlight selected profile
		highlightCard(sender);

		Button btn = sender as Button;

		
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

		UpdateMainCard();
	}

	private async void BtnFlipCard(object sender, EventArgs e)
	{
		if (awaitingFlip) return;

		if (showingFront)
		{
			FlipCard(cardFront, cardBack);
			showingFront = false;
			awaitingFlip = true;
		}
		else
		{
			FlipCard(cardBack, cardFront);
			showingFront = true;
			awaitingFlip = true;
		}
	}

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

		activeCardID = id;

		UpdateMainCard();
		
	}

	private void DirectHighlightCard(int id)
	{
		flashcardscontroller.FlashCards[id].IsHighlighted = true;
	}

	private void UpdateMainCard()
	{
		selectedQ = flashcardscontroller.FlashCards[activeCardID].CardQ.ToString();
		string aShort = cardDataset[activeCardID]?.AsObject()["a-short"]?.GetValue<string>() ?? "No Short Answer.";
		string aLong = cardDataset[activeCardID]?.AsObject()["a-long"]?.GetValue<string>() ?? "";

		MainCardLabel.Text = selectedQ;
		CardAShort.Text = aShort;
		CardALong.Text = aLong;
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

	//animation function
	private async Task FlipCard(Border startBorder, Border endBorder)
	{
		// Squash horizontally
		await startBorder.ScaleXTo(0.1, 250, Easing.CubicIn);

		//ensure endBorder is in position
		await endBorder.ScaleXTo(0.1, 10);

		//hide startBorder
		startBorder.IsVisible = false;
		endBorder.IsVisible = true;

		// Stretch back out
		await endBorder.ScaleXTo(1.02, 250, Easing.CubicOut);

		// Optional bounce effect
		await endBorder.ScaleXTo(1, 50);

		awaitingFlip = false;
	}
}