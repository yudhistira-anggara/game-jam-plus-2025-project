using Godot;
using System.Text.Json;

/*
Example dialogue .json file format
[
	{
		"id": "test_dialogue_01",
		"contents": [
			{
				"name": "Person A",
				"options": {
					"voice": "person_a_talk.wav",
					"script": "", // for dialogue requiring special scripting, if required (can be empty)
				},
				"text": "Hello, world!"
			},
			{
				"name": "Person B",
				"script": "",
				"text": "Hi, Person A!"
			}
		]
	},
	{
		"id": "test_dialogue_02",
		"contents": [
			{
			}
		]
	}
]
*/
public partial class DialogueSystem : Node
{
	[Signal] public delegate void LineChangedEventHandler(string text);
	[Signal] public delegate void DialogueFinishedEventHandler();

	private JsonElement _data;
	private int _index;

	public override void _Ready() 
	{
		_index = 0;
	}

	public void LoadDialogue(string path)
	{
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var json = file.GetAsText();
		file.Close();

		var doc = JsonDocument.Parse(json);
		_data = doc.RootElement;
	}

	public void StartDialogue()
	{
		_index = 0;
		ShowLine();
	}

	public void StartDialogueFile(string path)
	{
		LoadDialogue(path);
		StartDialogue();
	}

	public void Next()
	{
		_index++;

		if (_index >= _data.GetProperty("lines").GetArrayLength())
		{
			EmitSignal(SignalName.DialogueFinished);
			return;
		}

		ShowLine();
	}

	private void ShowLine()
	{
		var line = _data.GetProperty("lines")[_index].GetString();
		EmitSignal(SignalName.LineChanged, line);
	}
}
