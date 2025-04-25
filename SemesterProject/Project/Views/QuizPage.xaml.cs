using System.Text.Json.Nodes;
using System.Diagnostics;
namespace SemesterProject;

public partial class QuizPage : ContentPage
{
	private float highlightTint = 0.05f;
    private float tintStrength = 0.7f;

	private JsonArray cardDataset = new JsonArray();

	string stackFile = "err.json";
	string stackName = "err";
	int activeCardID = 0;

	bool animCards = true;
	bool showingFront = true;
	bool awaitingFlip = false;

	string activeShortAns = "";
	string userShortAns = "";

	int cardAttempts = -1;
	int activeAttempts = 0;
	bool printIncorrects = true;
	
	int activeTime = 0;
	int cardTime = 0;
	double ProgressWidth;
	CancellationTokenSource timerCancellationSource;

	int userScore = 0;
	int answerableCount = 0;
	bool hintGenerated = false;

	Random rnd = new Random();

	public QuizPage()
	{
		InitializeComponent();
		MauiProgram.updateTheme(MauiProgram.activeProfile);

		stackName = (MauiProgram.activeStack["set-name"]?.GetValue<string>() ?? "err");
        stackFile = stackName + ".json";

		MenuProfileNameLbl.Text = MauiProgram.activeProfile["name"]?.GetValue<string>() ?? "Author N.";


		cardTime = MauiProgram.activeProfile["preferences"]?.AsObject()["q-time"].GetValue<int>() ?? 4;
		switch(cardTime)
		{
			case 30:
				cardTime = 30000;
				break;
			case 45:
				cardTime = 45000;
				break;
			case 60:
				cardTime = 60000;
				break;
			case 90:
				cardTime = 90000;
				break;
			case 120:
			default:
				cardTime = 120000;
				break;
		}

		printIncorrects = (MauiProgram.activeProfile["preferences"]?.AsObject()["q-fails"]?.GetValue<int>() ?? 1) == 1 ? true : false;

		if (!Directory.Exists((MauiProgram.dirPath + MauiProgram.stackFolder)))
		{
			Directory.CreateDirectory((MauiProgram.dirPath + MauiProgram.stackFolder));
		}

        if(File.Exists((MauiProgram.dirPath + MauiProgram.stackFolder + stackFile)))
        {
            //retrieve set file as array
            cardDataset = MauiProgram.LoadJSONArrayFromFile(MauiProgram.dirPath + MauiProgram.stackFolder + stackFile);

			if (cardDataset.Count <= 0)
				App.Current.Windows[0].Page = new MainPage(); //file has no contents, cannot continue
        }
		else
		{
			App.Current.Windows[0].Page = new MainPage(); //file does not exist, cannot continue
		}

		bool shuffle = (MauiProgram.activeProfile["preferences"]?.AsObject()["q-shuffle"]?.GetValue<int>() ?? 1) == 1 ? true : false;

		if (shuffle) ShuffleSet(4); //performs x sequential riffleshuffles with random imperfections

		timerCancellationSource = new CancellationTokenSource();

		SetHeader();
		ShowHalfTimeLabel(((MauiProgram.activeProfile["preferences"]?.AsObject()["q-hint"]?.GetValue<int>() ?? 1) == 0 ? false : true));

		delayedInitLoad();

		animCards = ((MauiProgram.activeProfile["preferences"]?.AsObject()["card-anims"]?.GetValue<int>() ?? 1) == 0 ? false : true);

		activeAttempts = MauiProgram.activeProfile["preferences"]?.AsObject()["q-attempts"]?.GetValue<int>() ?? 0;
	}

	private void ShuffleSet(int repeat)
	{
		JsonArray fullStack = JsonNode.Parse(cardDataset.ToJsonString()).AsArray();

		int halfCount = fullStack.Count / 2;

		JsonArray leftHand = new JsonArray();
		JsonArray rightHand = new JsonArray();

		for (int cycle = 0; cycle < repeat; cycle++)
		{
			leftHand = new JsonArray();
			rightHand = new JsonArray();

			//split pile at halfway
			for (int c = 0; c < halfCount; c++)
			{
				leftHand.Add(JsonNode.Parse(fullStack[c].ToJsonString()).AsObject());
			}

			for (int c = halfCount; c < fullStack.Count; c++)
			{
				rightHand.Add(JsonNode.Parse(fullStack[c].ToJsonString()).AsObject());
			}

			int lIndex = 0;
			int rIndex = 0;

			for (int c = 0; c < fullStack.Count; c++)
			{
				bool left = (rnd.Next(0, 2) == 0) ? true : false;

				if (left && lIndex < leftHand.Count)
				{
					fullStack[c] = JsonNode.Parse(leftHand[lIndex].ToJsonString()).AsObject();
					lIndex++;
				}
				else if (rIndex < rightHand.Count)
				{
					fullStack[c] = JsonNode.Parse(rightHand[rIndex].ToJsonString()).AsObject();
					rIndex++;
				}
				else if (lIndex < leftHand.Count)
				{
					//to get to this point, left must be false, and right must be out of cards, 
					//so pull form left anyway assuming it has the remaining cards
					fullStack[c] = JsonNode.Parse(leftHand[lIndex].ToJsonString()).AsObject();
					lIndex++;
				}
			}
		}

		cardDataset = JsonNode.Parse(fullStack.ToJsonString()).AsArray();
	}

	private async void delayedInitLoad()
	{
		await Task.Delay(250);
		LoadCardComplete(activeCardID);
	}

	private void SetHeader()
	{
		AuthorLabel.Text = MauiProgram.activeStack["author-name"]?.ToString();
		DateLabel.Text = MauiProgram.activeStack["last-edited"]?.ToString();
		NameLabel.Text = MauiProgram.activeStack["set-name"]?.ToString();
	}

	private void SetQuestionNum(int index)
	{
		QNumLabel.Text = $"{(index + 1)} / {cardDataset.Count}";
	}

	private void LoadCardComplete(int index)
	{
		if (index < 0 || index >= cardDataset.Count) return; //card does not exist

		updateSubmitState(false);

		LoadCardFront(index);
		LoadCardBack(index);
		SetQuestionNum(index);

		activeShortAns = cardDataset[index]?.AsObject()["a-short"]?.GetValue<string>() ?? "";
		if (activeShortAns == "")
		{
			IncorrectAnswersLabel.Text += $"Q #{activeCardID+1} has no quiz answer.";
			activeTime = 0;
		}
		else
		{
			cardAttempts = 0;
			answerableCount++; //card has possible answer, add for calculating max final score
			//init timer
			ProgressWidth = this.Width - 150; //-150 for margin dimensions
			if (TimerBar.Width > 0)
			{
				LiveScoreBorder.TranslationX = TimerBar.Width;
			}
			else 
				LiveScoreBorder.TranslationX = ProgressWidth;
				//alternative method to identify end of progress bar for placement

			try {
				ProgressTimer(cardTime, 3000, timerCancellationSource.Token);
			} catch(OperationCanceledException){}
		}
	}

	private void LoadCardFront(int index)
	{
		if (index < 0 || index >= cardDataset.Count) return; //card does not exist

		string q = cardDataset[index]?.AsObject()["question"]?.GetValue<string>() ?? "";
		if (q == null || q == "") q = "";

		MainCardLabel.Text = q;
	}

	private void LoadCardBack(int index)
	{
		if (index < 0 || index >= cardDataset.Count) return; //card does not exist

		string a = cardDataset[index]?.AsObject()["a-short"]?.GetValue<string>() ?? "";
		if (a == null || a == "") a = "";
		
		CardAShort.Text = a;

		string al = cardDataset[index]?.AsObject()["a-long"]?.GetValue<string>() ?? "";
		if (al == null || al == "") al = "";

		CardALong.Text = al;
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
		MauiProgram.prevPage = MauiProgram.PageIndex.QUIZ;
		App.Current.Windows[0].Page = new SettingsPage();
	}

	private void ClearEntry()
	{
		AnswerInput.Text = "";
		userShortAns = "";
	}

	private async void EntryCheckIn(object sender, EventArgs e)
	{
		Entry answerField = sender as Entry;

		userShortAns = answerField.Text;

		if (userShortAns != "")
		{
			updateSubmitState(true);
		}
		else
		{
			//dehighlight submit
			updateSubmitState(false);
		}
	}

	private async void SubmitEntry(object sender, EventArgs e)
	{
		if (userShortAns == "") return;

		if (userShortAns.ToLower() == activeShortAns.ToLower())
		{
			//answer was correct, print message to field
			IncorrectAnswersLabel.Text += $"\nQ #{activeCardID+1} Correct: {activeShortAns}";

			//award score out of total time
			timerCancellationSource?.Cancel();
			userScore += (int) (((float) activeTime / (float) cardTime) * 1000f);
			ScoreLabel.Text = userScore.ToString();

			//clear hint label
			ClearHalfTime();

			//dehighlight submit
			updateSubmitState(false);

			FlipCard(cardFront, cardBack, animCards ? 550 : 0);
			showingFront = false;
			AnswerInput.IsEnabled = false;
			timerCancellationSource?.Cancel(); // cancel any previous run
    		timerCancellationSource = new CancellationTokenSource();
		}
		else
		{
			//answer was incorrect
			if(printIncorrects)
				IncorrectAnswersLabel.Text += $"\nIncorrect: {userShortAns}";

			//inc failed attempts for card
			cardAttempts++;
		}

		AnswerInput.Text = "";

		if (cardAttempts >= activeAttempts)
		{
			//question failed, move to next card, no points
			timerCancellationSource?.Cancel();
			FlipCard(cardFront, cardBack, animCards ? 550 : 0);
			showingFront = false;
			AnswerInput.IsEnabled = false;
		}
		
		await Task.Delay(50);
		await messageFeed.ScrollToAsync(0, IncorrectAnswersLabel.Height, animated: false);
	}

	private async void PassEntry(object sender, EventArgs e)
	{
		if (!showingFront) return;

		IncorrectAnswersLabel.Text += $"\nQ #{activeCardID+1} Passed.";

		cardAttempts = 0;

		//clear hint label
		ClearHalfTime();

		//dehighlight submit
		updateSubmitState(false);

		timerCancellationSource?.Cancel();
		FlipCard(cardFront, cardBack, animCards ? 550 : 0);
		AnswerInput.Text = "";
		showingFront = false;
		AnswerInput.IsEnabled = false;

		await Task.Delay(50);
		await messageFeed.ScrollToAsync(0, IncorrectAnswersLabel.Height, animated: false);
	}

	private async void questionTimeOut()
	{
		IncorrectAnswersLabel.Text += $"\nQ #{activeCardID+1} Timed out.";

		cardAttempts = 0;

		//clear hint label
		ClearHalfTime();

		//dehighlight submit
		updateSubmitState(false);

		FlipCard(cardFront, cardBack, animCards ? 550 : 0);
		AnswerInput.Text = "";
		showingFront = false;
		AnswerInput.IsEnabled = false;

		await Task.Delay(50);
		await messageFeed.ScrollToAsync(0, IncorrectAnswersLabel.Height, animated: false);
	}

	private async void BtnNextCard(object sender, EventArgs e)
	{
		if (activeCardID == cardDataset.Count - 1)
		{
			string blurb = "Another one down!"; //default generic for low scores or miscalculations
			if (userScore > (answerableCount * 1000f * 0.85f)) //user scored 85% or higher
			{
				blurb = "Fantastic Work!";
			}
			else if (userScore > (answerableCount * 1000f * 0.7f)) //user scored 70% - 85%
			{
				blurb = "Keep up the good work!";
			}
			else if (userScore > (answerableCount * 1000f * 0.55f)) //user scored 55% - 70%
			{
				blurb = "Getting somewhere!";
			}
			else if (userScore > (answerableCount * 1000f * 0.30f)) //user scored 30% - 55%
			{
				blurb = "It's progress, but keep trying!";
			}

			//end of game reached, display score and prompt to try again
			bool reattempt = await DisplayAlert($"Final Score: {userScore} / {(answerableCount * 1000)}", $"{blurb}\n\nWould you like to take this quiz again?", "Try Again", "Home Page");

			if (reattempt)
			{
				App.Current.Windows[0].Page = new QuizPage();
			}
			else
			{
				App.Current.Windows[0].Page = new MainPage();
			}
		}


		FlipNext(cardBack, cardFront, (animCards ? 550 : 0), activeCardID);
	}

	private void ClearHalfTime()
	{
		hintLabel.Text = "half-time hint: ";
	}

	//visual helper functions
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

	private void updateSubmitState(bool state)
	{
		if (state)
		{
			SubmitBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Accent");
		}
		else
		{
			SubmitBtn.SetDynamicResource(VisualElement.BackgroundColorProperty, "Primary");
		}
	}

	private void ShowHalfTimeLabel(bool state)
	{
		hintLabel.IsVisible = state;
	}

	//animation functions
	private async Task FlipCard(Border startBorder, Border endBorder, int tMillis)
	{	
		if (tMillis > 50)
		{
			uint phaseMillis = (uint) (tMillis - 50) / 2;
			// Squash horizontally
			await startBorder.ScaleXTo(0.1, phaseMillis, Easing.CubicIn);

			//ensure endBorder is in position
			await endBorder.ScaleXTo(0.1, 10);

			//hide startBorder
			startBorder.IsVisible = false;
			endBorder.IsVisible = true;

			// Stretch back out
			await endBorder.ScaleXTo(1.02, phaseMillis, Easing.CubicOut);

			// Optional bounce effect
			await endBorder.ScaleXTo(1, 50);

			awaitingFlip = false;

			NextBtn.IsVisible = true;
		}
		else
		{
			//hide startBorder
			startBorder.IsVisible = false;
			endBorder.IsVisible = true;

			awaitingFlip = false;

			NextBtn.IsVisible = true;
		}
	}

	private async Task FlipNext(Border startBorder, Border endBorder, int tMillis, int startIndex)
	{	
		if (tMillis > 50)
		{
			uint phaseMillis = (uint) (tMillis - 50) / 2;
			// Squash horizontally
			await startBorder.ScaleXTo(0.1, phaseMillis, Easing.CubicIn);

			//ensure endBorder is in position
			await endBorder.ScaleXTo(0.1, 10);

			//hide startBorder
			startBorder.IsVisible = false;

			//update to next card
			timerCancellationSource = new CancellationTokenSource();
			LoadCardComplete(startIndex + 1);

			//reveal endBorder
			endBorder.IsVisible = true;

			// Stretch back out
			await endBorder.ScaleXTo(1.02, phaseMillis, Easing.CubicOut);

			// Optional bounce effect
			await endBorder.ScaleXTo(1, 50);

			if (activeCardID < cardDataset.Count - 1)
			    activeCardID++;

			awaitingFlip = false;

			NextBtn.IsVisible = false;

			AnswerInput.IsEnabled = true;

			showingFront = true;
		}
		else
		{
			//hide startBorder
			startBorder.IsVisible = false;

			//update to next card
			timerCancellationSource = new CancellationTokenSource();
			LoadCardComplete(startIndex + 1);

			//reveal endBorder
			endBorder.IsVisible = true;

			if (activeCardID < cardDataset.Count - 1)
			    activeCardID++;

			awaitingFlip = false;

			NextBtn.IsVisible = false;

			AnswerInput.IsEnabled = true;

			showingFront = true;
		}
	}

	private async Task ProgressTimer(int t, int millisDelay, CancellationToken ct)
	{
		activeTime = t;

		TimerBar.Progress = 1f;
		LiveScoreLabel.Text = "1000";
		if (TimerBar.Width > 0)
		{
			LiveScoreBorder.TranslationX = TimerBar.Width;
		}
		else 
			LiveScoreBorder.TranslationX = ProgressWidth;
			//alternative method to identify end of progress bar for placement

		hintLabel.Text = "half-time hint: ";
		hintGenerated = false;

		await Task.Delay(millisDelay);

		while(activeTime >= 0)
		{
			ct.ThrowIfCancellationRequested();

			await Task.Delay(50);
			activeTime -= 50;
			float ratio = ((float) activeTime / (float) cardTime);

			if (ratio <= 0) ratio = 0f;
			if (ratio <= 0.5f && ratio >= 0.45f) GenerateHint(activeShortAns);

			int remaining = (int) (ratio * 1000f);
			TimerBar.Progress = ratio;
			LiveScoreLabel.Text = remaining.ToString();

			LiveScoreBorder.TranslationX = ratio * TimerBar.Width;
		}

		//time is up, no more points
		questionTimeOut();
	}

	private async void GenerateHint(string ans)
	{
		if (ans == "" || hintGenerated) return;

		hintGenerated = true;

		hintLabel.Text = "half-time hint: ";
		for (int c = 0; c < ans.Length; c++)
		{
			hintLabel.Text += (ans[c] == ' ') ? ' ' : '*';
		}
	}
}