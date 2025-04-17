namespace SemesterProject;

public partial class MainPage : ContentPage
{
	private Controllers.FlashCardController flashcardcontroller;
	public MainPage()
	{
		InitializeComponent();

		flashcardcontroller = new(CollFlashCardSets);
	}

	private async void BtnAddSet(object sender, EventArgs e)
	{
		string name = await DisplayPromptAsync("Add New Set", "Enter Name of Set:");

		flashcardcontroller.AddNewFlashCardSet(name);
	}
	
}

