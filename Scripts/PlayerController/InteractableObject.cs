using Godot;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public partial class InteractableObject : Node3D, IInteractable
{
	public AnimatedSprite3D ButtonSprite {get; private set;}
	public bool CanInteract { get; private set; } = false;
	private void _ready()
	{
		
		ButtonSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		ButtonSprite.Visible = false;
	}
	/*
	private void _physics_process(double delta)
	{
		if (Area3D.HasOverlappingBodies())
		{
			if (!InArea)
			{
				Entered();
			}
		}
		else
		{
			if (InArea)
			{
				Exited();
			}
		}
		if (InArea)
		{
			if (Input.IsActionPressed("interact",true))
			{
				if (!ButtonPressed)
				{
					ButtonPressed = true;
					Interaction();
				}
			}
			else
			{
				if (ButtonPressed)
				{
					ButtonPressed = false;
					Entered();
				}
			}
		}	
	}*/
	public void Entered()
	{
		ButtonSprite.Visible = true;
		ButtonSprite.Play("Unpressed");
		CanInteract = true;
	}
	public void Exited()
	{
		ButtonSprite.Visible = false;
		CanInteract = false;
	}
	public void Interaction()
	{
		if (CanInteract)
		{
			ButtonSprite.Play("Pressed");
		}
	}
	public void Uninteractable()
	{
		ButtonSprite.Play("Inactive");
		CanInteract = false;
	}
}
