using Microsoft.UI.Xaml.Media;

namespace SemesterProject;

public partial class Login_page : ContentPage
{
	private string selectedProfile = "";
	private Controllers.ProfileController profileController;

	private float highlightTint = 0.05f;
	private float tintStrength = 0.7f;

	public Login_page()
	{
		InitializeComponent();

		profileController = new(CollProfiles);
	}

	private async void BtnPressed(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		Color btnBG = Color.FromRgba(highlightTint, highlightTint, highlightTint, tintStrength);
		btn.Background = btnBG;
	}

	private async void BtnReleased(object sender, EventArgs e)
	{
		Button btn = sender as Button;

		Color btnBG = Color.FromRgba(0f, 0f, 0f, 0f);
		btn.Background = btnBG;
	}

	private async void BtnAddProfile(object sender, EventArgs e)
	{

		string name = await DisplayPromptAsync("Add New Profile", "Enter Name:");

		if (name != null)
			profileController.AddNewProfile(name);
	}

	private async void BtnRemoveProfile(object sender, EventArgs e)
    {
		Button btn = sender as Button;

		string name = Convert.ToString(btn.CommandParameter); 

		bool answer = await DisplayAlert("Remove Profile","Are you sure you want to remove this profile?","Yes","No");

		if (answer == true)
		{
			var result = profileController.RemoveProfile(name);
			DisplayAlert("Removed", "Profile Sucessfully Removed!", "OK"); 
		}
    }

	private async void BtnSelectProfile(object sender, EventArgs e) 
	{
		
	}

	private async void BtnSignIn(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new MainPage();
	}
}