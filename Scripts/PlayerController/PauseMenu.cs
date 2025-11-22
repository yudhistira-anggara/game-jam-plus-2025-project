using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class PauseMenu : Control
{
	private async void _resume()
	{
		await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
		GetTree().Paused = false;
		Hide();
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Resume/AnimatedSprite2D").Play("Default");
	}
	private void _pause()
	{
		GetTree().Paused = true;
	}

	//---------------Resume Button---------------

	private void _on_resume_pressed()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Resume/AnimatedSprite2D").Play("On_Click");
		_resume();
	}
	private void _on_resume_mouse_entered()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Resume/AnimatedSprite2D").Play("On_Hover");
	}
	private void _on_resume_mouse_exited()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Resume/AnimatedSprite2D").Play("Default");
	}

	//---------------Settings Button---------------

	private void _on_settings_pressed()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Settings/AnimatedSprite2D").Play("On_Click");
	}
	private void _on_settings_mouse_entered()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Settings/AnimatedSprite2D").Play("On_Hover");
	}
	private void _on_settings_mouse_exited()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Settings/AnimatedSprite2D").Play("Default");
	}

	//---------------Exit Button---------------

	private void _on_exit_pressed()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Exit/AnimatedSprite2D").Play("On_Click");
		GetTree().Quit();
	}
	private void _on_exit_mouse_entered()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Exit/AnimatedSprite2D").Play("On_Hover");
	}
	private void _on_exit_mouse_exited()
	{
		GetNode<AnimatedSprite2D>("PanelContainer/VBoxContainer/Exit/AnimatedSprite2D").Play("Default");
	}
}
