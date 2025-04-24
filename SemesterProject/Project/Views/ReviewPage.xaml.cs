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
	private int selectedID = 0;

    public ReviewPage()
    {
        InitializeComponent();
        MauiProgram.updateTheme(MauiProgram.activeProfile);
        string stackFile = (MauiProgram.activeStack["set-name"]?.GetValue<string>() ?? "No Name") + ".json";

        if(File.Exists((MauiProgram.dirPath + MauiProgram.stackFolder + stackFile)))
        {
            //retrieve profile file as array
            cardDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.stackFolder + stackFile);
        }

        flashcardscontroller = new(CollFlashCards);

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
		App.Current.Windows[0].Page = new SettingsPage();
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
		highlightSet(sender);

		Button btn = sender as Button;

		
	}

	private async void BtnFlipCard(object sender, EventArgs e)
	{
		
	}

	private void clearHighlights()
	{
		for (int b = 0; b < flashcardscontroller.FlashCards.Count; b++)
		{
			flashcardscontroller.FlashCards[b].IsHighlighted = false;
		}
	}

	private void highlightSet (object sender)
	{
		Button btn = sender as Button;

		int id = (int) btn.CommandParameter; 

		flashcardscontroller.FlashCards[id].IsHighlighted = true;

		selectedID = id;

		UpdateMainCard();
		
	}

	private void UpdateMainCard()
	{
		selectedQ = flashcardscontroller.FlashCards[selectedID].CardQ.ToString();

		MainCardLabel.Text = selectedQ;
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