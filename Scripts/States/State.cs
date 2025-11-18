using Godot;
using System;
public abstract partial class State : Node
{
	public string name = "";
	[Signal] public delegate void TransitionedEventHandler(State state, string newStateName);
	public abstract void Enter();
	public abstract void Exit();
	public abstract void Update(double delta);
	public abstract void PhysicsUpdate(double delta);
}
