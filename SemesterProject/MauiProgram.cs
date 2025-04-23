using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Text.Json.Nodes;

namespace SemesterProject;

public static class MauiProgram
{

	//declare globally accessible field for page loading, profile selections, and file info
	public static string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CardStack\\";
	public static string prefFile = "profiles.json";
	public static string setFile = "stackCollection.json";

	//declare globally accesible profile fields
	public static int activeID { get; set; } = -1;
	public static JsonObject activeProfile { get; set; } = InstantiateProfile();


	public static void updateTheme(JsonObject pref)
	{
		string themename = "DarkGreen";

		if (activeProfile["theme"].GetValue<int>() == 0 && activeProfile["accent"].GetValue<int>() == 0)
			themename = "LightRed";
		else if (activeProfile["theme"].GetValue<int>() == 0 && activeProfile["accent"].GetValue<int>() == 1)
			themename = "LightOrange";
		else if (activeProfile["theme"].GetValue<int>() == 0 && activeProfile["accent"].GetValue<int>() == 2)
			themename = "LightYellow";
		else if (activeProfile["theme"].GetValue<int>() == 0 && activeProfile["accent"].GetValue<int>() == 3)
			themename = "LightGreen";
		else if (activeProfile["theme"].GetValue<int>() == 0 && activeProfile["accent"].GetValue<int>() == 4)
			themename = "LightBlue";
		else if (activeProfile["theme"].GetValue<int>() == 0 && activeProfile["accent"].GetValue<int>() == 5)
			themename = "LightPurple";
		else if (activeProfile["theme"].GetValue<int>() == 1 && activeProfile["accent"].GetValue<int>() == 0)
			themename = "DarkRed";
		else if (activeProfile["theme"].GetValue<int>() == 1 && activeProfile["accent"].GetValue<int>() == 1)
			themename = "DarkOrange";
		else if (activeProfile["theme"].GetValue<int>() == 1 && activeProfile["accent"].GetValue<int>() == 2)
			themename = "DarkYellow";
		else if (activeProfile["theme"].GetValue<int>() == 1 && activeProfile["accent"].GetValue<int>() == 3)
			themename = "DarkGreen";
		else if (activeProfile["theme"].GetValue<int>() == 1 && activeProfile["accent"].GetValue<int>() == 4)
			themename = "DarkBlue";
		else if (activeProfile["theme"].GetValue<int>() == 1 && activeProfile["accent"].GetValue<int>() == 5)
			themename = "DarkPurple";

		SetTheme(themename);
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
			}
		}
	}

	public static void checkinProfile(JsonObject data)
	{
		activeProfile = data;
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

	public static JsonObject InstantiateProfile()
	{
		return InstantiateProfile("NONE", -1);
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
