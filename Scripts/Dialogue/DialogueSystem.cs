using Godot;
using Godot.Collections;
using GameJam;
using System;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;

namespace GameJam
{
	public enum DialogueType
	{
		Talk,
		Select
	}

	public partial class DialogueSystem : Control
	{
		[Export]
		public PackedScene TextDisplayer { get; set; }
		public IDialogueBox DialogueBox { get; set; }

		[Export]
		public PackedScene SelectBox { get; set; }
		public DialogueSelectDefault DialogueSelection { get; set; }

		[ExportGroup("Dialogue")]
		[Export(PropertyHint.File, $"*.json")]
		public string Path { get; set; }

		public List<DialogueSerializeable> DialogueLines { get; set; }

		public string TargetID { get; set; }

		public int CurrentDialogue { get; set; } = 0;

		public int CurrentPage { get; set; } = 0;
		public int TotalPages { get; set; } = 1;

		public void LoadAndRunDialogueScript()
		{
			var conditions = DialogueLines[CurrentDialogue].Contents[CurrentPage].Script != "" ?
			DialogueLines[CurrentDialogue].Contents[CurrentPage].Script : "";

			if (conditions == "")
				return;

			var scriptResource = ResourceLoader.Load<Script>(conditions);
			var scriptNodeInstance = new Node
			{
				Name = "DialogueScriptNode"
			};
			AddChild(scriptNodeInstance);
			scriptNodeInstance.SetScript(scriptResource);
			scriptNodeInstance = GetNode("DialogueScriptNode");
			scriptNodeInstance.Call("DialogueScriptCall");
			scriptNodeInstance.QueueFree();
		}

		public void ParseJson(string path)
		{
			var content = Utils.LoadFromFile(path);
			DialogueLines = JsonSerializer.Deserialize<List<DialogueSerializeable>>(content);
			ParseDialogue(0);

			/*
			Trying to fix assembly unloading errors, code from:
				https://github.com/godotengine/godot/issues/78513
				https://github.com/dotnet/runtime/issues/65323
			*/
			var assembly = typeof(JsonSerializerOptions).Assembly;
			var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
			var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
			clearCacheMethod?.Invoke(null, [null]);
		}

		public void ParseDialogue(int dial)
		{
			var dia = DialogueLines[dial];
			CurrentPage = 0;
			TotalPages = dia.Contents.Count - 1;
			CurrentDialogue = dial;
			TargetID = dia.Next;
		}

		public void ChangeDialogue(string target = "", bool init = false)
		{
			var to = 0;

			if (!init)
				to = target != "" ? DialogueLines.FindIndex(x => x.ID == target) : DialogueLines.FindIndex(x => x.ID == TargetID);

			if (to < 0)
			{
				GD.PrintErr(new IndexOutOfRangeException("Target not found?"));
				return;
			}

			ParseDialogue(to);
			var dialogue = DialogueLines[to];

			if (dialogue.Type == DialogueType.Talk.ToString())
			{
				HandleTalk(dialogue);
			}
			else if (dialogue.Type == DialogueType.Select.ToString())
			{
				HandleSelect(dialogue);
			}
		}

		public void InstantiateUIElements()
		{
			if (DialogueBox == null || !IsInstanceValid((Node)DialogueBox))
			{
				var instance = TextDisplayer.Instantiate();
				AddChild(instance);
				DialogueBox = instance as IDialogueBox;
			}

			if (DialogueSelection == null || !IsInstanceValid((Node)DialogueSelection))
			{
				var instance = SelectBox.Instantiate();
				AddChild(instance);
				DialogueSelection = (DialogueSelectDefault)instance;
			}

			ChangeDialogue(init: true);
		}

		public void Instantiation()
		{
			ParseJson(Path);
			InstantiateUIElements();
		}

		public override void _Ready()
		{
			Instantiation();
		}

		public override void _Input(InputEvent @event)
		{
			if (DialogueLines[CurrentDialogue].Type == DialogueType.Select.ToString())
				return;

			if (@event.IsActionPressed("DialogueTestPrevious"))
			{
				HandlePage(false);
			}
			else if (@event.IsActionPressed("DialogueTestNext"))
			{
				HandlePage(true);
			}
		}

		public void HandlePage(bool next)
		{
			if (next == true && CurrentPage < TotalPages)
				CurrentPage++;
			else if (next == false && CurrentPage > 0)
				CurrentPage--;
			else if (CurrentPage == TotalPages)
				ChangeDialogue();

			DialogueBox.ReadValues(DialogueLines[CurrentDialogue].Contents[CurrentPage], CurrentPage, TotalPages);
			LoadAndRunDialogueScript();
		}

		public void HandleTalk(DialogueSerializeable dialogue)
		{
			DialogueBox.SetVisibility(true);
			DialogueSelection.SetVisibility(false);
			DialogueBox.ReadValues(dialogue.Contents[CurrentPage], CurrentPage, TotalPages);
		}

		public void HandleSelect(DialogueSerializeable dialogue)
		{
			DialogueSelection.OptionSelected += OnButtonSignalReceived;
			DialogueBox.SetVisibility(false);
			DialogueSelection.SetVisibility(true);
			DialogueSelection.ReadValues(dialogue.Contents);
		}

		public void OnButtonSignalReceived(string id)
		{
			ChangeDialogue(id);
			DialogueSelection.OptionSelected -= OnButtonSignalReceived;
		}
	}
}
