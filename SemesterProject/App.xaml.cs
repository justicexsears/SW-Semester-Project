namespace SemesterProject;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		const int newHeight = 720;
    	const int newWidth = 1280;

    	var newWindow = new Window(new Login_page())
    	{
        	Height = newHeight,
       	 	Width = newWidth
    	};

    	return newWindow;
	}
}