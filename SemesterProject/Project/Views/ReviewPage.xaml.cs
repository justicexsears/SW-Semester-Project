using System.Text.Json.Nodes;

namespace SemesterProject;

public partial class ReviewPage : ContentPage
{

	private Controllers.FlashCardController flashcardscontroller;

	private JsonObject localStack;

	private JsonArray setDataset = new JsonArray();
	public ReviewPage()
	{

		InitializeComponent();
		MauiProgram.updateTheme(MauiProgram.activeProfile);

		
	}
}