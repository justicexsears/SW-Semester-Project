namespace SemesterProject;

public partial class MainPage : ContentPage
{
	private Controllers.FlashSetController flashsetscontroller;
	public MainPage()
	{
		InitializeComponent();

		flashsetscontroller = new(CollFlashCardSets);
	}

	private async void BtnAddSet(object sender, EventArgs e)
	{
		string name = await DisplayPromptAsync("Add New Set", "Enter Name of Set:");

		flashsetscontroller.AddNewFlashCardSet(name);
	}
	
}

