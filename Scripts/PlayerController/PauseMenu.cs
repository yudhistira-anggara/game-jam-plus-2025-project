using Godot;
using System;

public partial class PauseMenu : Control
{
	private void _resume()
	{
		GetTree().Paused = false;
	}
	private void _pause()
	{
		GetTree().Paused = true;
	}
	private void _on_resume_pressed()
	{
		resume();
	}
	private void _on_setting_pressed()
	{
		
	}
}
