using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class StateMachine : Node
{
	[Export] public State initialState;

	protected State currentState = null;
	protected Dictionary<string,State> states = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (State child in GetChildren())
		{
			if (child is not State) continue;

			states[child.name.ToLower()] = child;
			child.Transitioned += OnChildTransition;
		}

		if (initialState is null) return;

		initialState.Enter();
		currentState = initialState;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentState is null) return;

		currentState.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState is null) return;

		currentState.PhysicsUpdate(delta);
	}

	public void OnChildTransition(State state, string newStateName)
	{
		if (state == currentState) return;

		var newState = states[newStateName.ToLower()];
		
		if (newState is null) return;

		if (currentState is null) return;

		currentState.Exit();
		newState.Enter();
		currentState = newState;
	}
}
