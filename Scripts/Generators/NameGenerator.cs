using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class NameGenerator : Node
{
	private RandomNumberGenerator rng;
	private Dictionary data;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng = new();
		rng.Randomize();

		Json json = new();
		FileAccess file = FileAccess.Open("res://dictionary/text_data.json", FileAccess.ModeFlags.Read);
		json.Parse(file.GetAsText());
		data = json.Data.AsGodotDictionary();
		GD.Print(GenerateText());
	}

	public string GenerateText()
	{
		var templates = data["templates"].AsGodotArray().ToArray();
		string template = (string)templates[rng.RandiRange(0, templates.Length - 1)];

		foreach (string key in data.Keys.Where(k => k.ToString() != "templates"))
		{
			var words = data[key].AsGodotArray().ToArray();
			string word = (string)words[rng.RandiRange(0, words.Length - 1)];
			template = template.Replace("{" + key + "}", word);
		}

		return template;
	}
}
