using Godot;
using System;
using System.Diagnostics;

public partial class PanelShake : Node2D
{  
	private FastNoiseLite _noise = new FastNoiseLite();
	private Vector2 _originalPosition;
	private CpuParticles2D _explosionParticles;
	private void _ready()
	{
		_originalPosition = this.Position;
		_explosionParticles = GetNode<CpuParticles2D>("Explosion"); 
	}
	public void _on_button_mouse_entered()
	{
		_shake_panel(0.2f);
	}
	public void _on_button_pressed()
	{
		_shake_panel(2.0f);
		PlayExplosion();
	}
	private void _shake_panel(float duration)
	{
		var panel_tween = GetTree().CreateTween();
		panel_tween.TweenMethod(
			Callable.From<float>(_start_panel_shake), // Pass method reference
			4.0f, // Initial value
			5.0f, // Final value
			duration  // Duration
		);
		this.Position = _originalPosition;
	}
	private void _start_panel_shake(float intensity)
	{
		var panel_offset = _noise.GetNoise1D(Time.GetTicksMsec()) * intensity;
		this.Position = new Vector2(_originalPosition.X + panel_offset,_originalPosition.Y);
	}
	public void PlayExplosion()
	{
		_explosionParticles.OneShot = true; 
		_explosionParticles.Emitting = true; 
		_explosionParticles.Finished += OnExplosionFinished;
		var panel_tween = GetTree().CreateTween();
		panel_tween.TweenProperty(
			this,
			"scale",
			new Vector2(1.0f,0.0f),
			1.0f  // Duration
		);
	}

	private void OnExplosionFinished()
	{
		var parent = GetParent();
		var grandParent = parent.GetParent();
		grandParent.RemoveChild(parent);
		QueueFree();
	}
}
