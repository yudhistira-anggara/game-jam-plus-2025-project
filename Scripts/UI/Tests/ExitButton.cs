using Godot;
using System;

public partial class ExitButton : Button
{
	private void _on_pressed()
	{
		GetTree().Quit();
	}
}
