using Godot;
using System;

[GlobalClass]
public partial class EnemyFollow : State
{
	[Export] CharacterBody2D enemy;
	[Export] double followSpeed = 20.0;

	protected CharacterBody2D player;

	public override void Enter()
	{
		player = (CharacterBody2D) GetTree().GetFirstNodeInGroup("Player");
	}

	public override void Exit()
	{
		
	}

	public override void Update(double delta)
	{
		
	}

	public override void PhysicsUpdate(double delta)
	{
		var direction = (player.GlobalPosition - enemy.GlobalPosition).Normalized();

		if (direction.Length() <= 20) return;

		enemy.Velocity = direction * (float) followSpeed;

		if (direction.Length() <= 60) return;
		
		EmitSignal(SignalName.Transitioned, "EnemyIdle");
	}
}
