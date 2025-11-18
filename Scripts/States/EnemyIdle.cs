using Godot;
using System;

[GlobalClass]
public partial class EnemyIdle : State
{
    [Export] CharacterBody2D enemy;
    [Export] double movementSpeed = 10.0;
    protected Vector2 moveDirection = new();
    protected double wanderTime = 0.0;
    protected Random random = new Random();
    protected CharacterBody2D player;

    protected void RandomizeWander()
	{
        float x = (float)(random.NextDouble() * 2.0 - 1.0);
        float y = (float)(random.NextDouble() * 2.0 - 1.0);
        moveDirection = new Vector2(x, y).Normalized();
        wanderTime = (float) random.NextDouble() * 3.0;
    }

    public override void Enter()
    {
        player = (CharacterBody2D) GetTree().GetFirstNodeInGroup("Player");
        RandomizeWander();
    }

	public override void Exit()
    {
        RandomizeWander();
    }

    public override void Update(double delta)
    {
        if (wanderTime > 0) wanderTime -= delta;
		else RandomizeWander();
    }

    public override void PhysicsUpdate(double delta)
    {
        if (enemy is null) return;

        enemy.Velocity = moveDirection * (float) movementSpeed;

        var direction = (player.GlobalPosition - enemy.GlobalPosition).Normalized();

        if (direction.Length() > 40) return;
        
        EmitSignal(SignalName.Transitioned, "EnemyFollow");
    }
}
