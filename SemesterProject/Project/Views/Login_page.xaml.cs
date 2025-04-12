using Microsoft.UI.Xaml.Media;

namespace SemesterProject;

public partial class Login_page : ContentPage
{
	private Controllers.ProfileController controller;
	public Login_page()
	{
		InitializeComponent();

		controller = new(CollProfiles);
	}

	private async void BtnAddProfile(object sender, EventArgs e)
	{

		string name = await DisplayPromptAsync("Add New Profile", "Enter Name:");

		controller.AddNewProfile(name);
	}

	private async void BtnRemoveProfile(object sender, EventArgs e)
    {
		Button btn = sender as Button;

		string name = Convert.ToString(btn.CommandParameter); 

		bool answer = await DisplayAlert("Remove Profile","Are you sure you want to remove this profile?","Yes","No");

		if (answer == true)
		{
			var result = controller.RemoveProfile(name);
			DisplayAlert("Removed", "Profile Sucessfully Removed!", "OK"); 
		}
    }

	private async void BtnSignIn(object sender, EventArgs e)
	{
		App.Current.Windows[0].Page = new MainPage();
	}
}