using Godot;
using System;

public interface IInteractable
{
	AnimatedSprite3D ButtonSprite { get; }
	bool CanInteract { get;}
	void Entered();
	void Exited();
	void Interaction();
	void Uninteractable();
}
