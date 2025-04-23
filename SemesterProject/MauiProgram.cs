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
	public static string stackFolder = "stacks\\";

	//declare globally accesible profile fields
	public static int activeID { get; set; } = -1;
	public static JsonObject activeProfile { get; set; } = InstantiateProfile();

	public static int stackID { get; set; } = -1;
	public static JsonObject activeStack { get; set; } = InstantiateStack();


	public static void updateTheme(JsonObject pref)
	{

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

	public static JsonObject InstantiateStack(string name, int id)
	{
		JsonObject tmpStack = new JsonObject {
			["id"] 			= id,
			["set-name"]	= name,
			["author-name"]	= "",
			["last-edited"] = "01/01/1970" //unix epoch time, because why not i guess
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
