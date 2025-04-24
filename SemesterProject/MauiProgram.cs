using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Text.Json.Nodes;

namespace SemesterProject;

public static class MauiProgram
{
	public enum PageIndex
	{
		LOGIN, HOME, SETTINGS, EDIT, QUIZ, REVIEW
	}

	//declare globally accessible field for page loading, profile selections, and file info
	public static string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CardStack\\";
	public static string prefFile = "profiles.json";
	public static string setFile = "stackCollection.json";
	public static string stackFolder = "stacks\\";
	public static PageIndex prevPage = PageIndex.LOGIN;
	

	//declare globally accesible profile fields
	public static int activeID { get; set; } = -1;
	public static JsonObject activeProfile { get; set; } = InstantiateProfile();

	//declare globally accessible stack fields
	public static int stackID { get; set; } = -1;
	public static JsonObject activeStack { get; set; } = InstantiateStack();


	public static void returnFromPage()
	{
		switch(prevPage)
		{
			default:
			case PageIndex.LOGIN:
				App.Current.Windows[0].Page = new LoginPage();
				break;
			case PageIndex.HOME:
				App.Current.Windows[0].Page = new MainPage();
				break;
			case PageIndex.SETTINGS:
				App.Current.Windows[0].Page = new SettingsPage();
				break;
			case PageIndex.EDIT:
				App.Current.Windows[0].Page = new EditPage();
				break;
			case PageIndex.QUIZ:
				App.Current.Windows[0].Page = new QuizPage();
				break;
			case PageIndex.REVIEW:
				App.Current.Windows[0].Page = new ReviewPage();
				break;
		}
	}

	public static void updateTheme(JsonObject pref)
	{
		string themeName = "DarkGreen";
		string cardstyle = "BW";

		string prefTheme = (pref["theme"]?.GetValue<int>() ?? 0) == 0 ? "Light" : "Dark";
		int prefAccent = pref["accent"]?.GetValue<int>() ?? 0;
		string prefCStyle = (pref["preferences"].AsObject()["card-style"]?.GetValue<int>() ?? 0) == 0 ? "BW" : "Match";
		switch(prefAccent)
		{
			default:
			case 0:
				themeName = prefTheme + "Red";
				break;
			case 1:
				themeName = prefTheme + "Orange";
				break;
			case 2:
				themeName = prefTheme + "Yellow";
				break;
			case 3:
				themeName = prefTheme + "Green";
				break;
			case 4:
				themeName = prefTheme + "Blue";
				break;
			case 5:
				themeName = prefTheme + "Purple";
				break;
		}

		SetTheme(themeName);

		if (prefCStyle == "Match")
			cardstyle = themeName;
		else
			cardstyle = prefCStyle;

		SetCardTheme(cardstyle);
	}

	public static void SetCardTheme(string cardstyle)
	{
		Preferences.Set("CardStyle", cardstyle);

		ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			foreach (ResourceDictionary dictionaries in mergedDictionaries)
			{
				var backgroundFound = dictionaries.TryGetValue(cardstyle + "CBG", out var cbg);
				if (backgroundFound)
					dictionaries["CardBackground"] = cbg;

				var textFound = dictionaries.TryGetValue(cardstyle + "CT", out var ctext);
				if (textFound)
					dictionaries["CardText"] = ctext;
			}
		}

	}


	public static void updateTheme()
	{
		updateTheme(activeProfile);
	}

	public static void SetTheme(string themename)
	{
		Preferences.Set("Theme", themename);

		ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			foreach (ResourceDictionary dictionaries in mergedDictionaries)
			{
				var primaryFound = dictionaries.TryGetValue(themename + "Primary", out var primary);
				if (primaryFound)
					dictionaries["Primary"] = primary;
				
				var secondaryFound = dictionaries.TryGetValue(themename + "Secondary", out var secondary);
				if (secondaryFound)
					dictionaries["Secondary"] = secondary;

				var tertiaryFound = dictionaries.TryGetValue(themename + "Tertiary", out var tertiary);
				if (tertiaryFound)
					dictionaries["Tertiary"] = tertiary;

				var accentFound = dictionaries.TryGetValue(themename + "Accent", out var accent);
				if (accentFound)
					dictionaries["Accent"] = accent;
				
				var textFound = dictionaries.TryGetValue(themename + "Text", out var text);
				if (textFound)
					dictionaries["MainText"] = text;
			}
		}


	}

	public static void checkinProfile(JsonObject data)
	{
		activeProfile = data;
	}

	public static void checkinStack(JsonObject data)
	{
		activeStack = data;
	}

	public static JsonArray LoadJSONArrayFromFile(string path)
	{
		JsonArray tmpArray = new JsonArray();

		if (File.Exists(path))
		{
			string content = File.ReadAllText(path);
			try
			{
				tmpArray = JsonNode.Parse(content).AsArray();
			}
			catch { return tmpArray; }
		}

		return tmpArray; 
	}

	public static bool SaveJSONArrayToFile(JsonArray content, string path)
	{
		bool stored = false;

		try 
		{
			string tmpJSONString = content.ToJsonString();
			File.WriteAllText(path, tmpJSONString);

			stored = true;
		}
		catch {}

		return stored;
	}

	public static JsonObject InstantiateProfile(string name, int id)
	{
		JsonObject tmpPref = new JsonObject {
			["card-style"] 	= 0,
			["card-anims"]  = 1,
			["q-time"]		= 120,
			["q-attempts"]  = -1,
			["q-shuffle"]	= 1,
			["q-fails"] 	= 1,
			["q-hint"]		= 0
		};

		JsonObject tmpProfile = new JsonObject {
			["name"] 	= name,
			["id"]		= id,
			["theme"]	= 0,
			["accent"]  = 0,

			["preferences"] = tmpPref
		};

		return tmpProfile;
	}

	public static JsonObject InstantiateStack(string name, int id)
	{
		JsonObject tmpStack = new JsonObject {
			["id"] 			= id,
			["set-name"]	= name,
			["author-name"]	= "No Author",
			["last-edited"] = "01/01/1970" //unix epoch time, because why not i guess
		};

		return tmpStack;
	}

	public static JsonObject InstantiateStack(string name, int id, string authName, string editDate)
	{
		JsonObject tmpStack = new JsonObject {
			["id"] 			= id,
			["set-name"]	= name,
			["author-name"]	= authName,
			["last-edited"] = editDate
		};

		return tmpStack;
	}

	public static JsonObject InstantiateCard(int id)
	{
		JsonObject tmpCard = new JsonObject {
			["id"] 			= id,
			["question"]	= "",
			["a-short"]		= "",
			["a-long"] 		= "" //the long side of the answer card is optional, but the rest must be filled to be saved
		};

		return tmpCard;
	}


	public static JsonObject InstantiateProfile()
	{
		return InstantiateProfile("NONE", -1);
	}
  
  public static JsonObject InstantiateStack()
	{
		return InstantiateStack("NONE", -1);
	}

	public static void applyProfileChanges(JsonObject data)
	{
		activeProfile = data;
		
		JsonArray fromDisk = LoadJSONArrayFromFile(dirPath + prefFile);
		JsonObject fromLocal = JsonNode.Parse(data.ToJsonString()).AsObject();

		fromDisk[activeProfile["id"].GetValue<int>()] = fromLocal;

		SaveJSONArrayToFile(fromDisk, dirPath + prefFile);
	}

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
