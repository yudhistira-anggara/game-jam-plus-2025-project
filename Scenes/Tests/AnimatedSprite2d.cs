using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class AnimatedSprite2d : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Candle = GetNode<AnimatedSprite2D>("../AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public float power = 15f;
	public float x_direction = 0;
	public float y_direction = 0;
	public AnimatedSprite2D Candle;
	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("move_left"))
		{
			x_direction = -1;
		}
		if (@event.IsAction("move_right"))
		{
			x_direction = 1;
		}
		if (@event.IsAction("move_up"))
		{
			y_direction = -1;
		}
		if (@event.IsAction("move_down"))
		{
			y_direction = 1;
		}
		Candle.Position = Candle.Position + new Vector2(x_direction, y_direction) * power;
		x_direction=0;
		y_direction=0;
	}

	public override void _Process(double delta)
	{



	}
}
