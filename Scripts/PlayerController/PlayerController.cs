using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualBasic;

public partial class PlayerController : CharacterBody3D
{

	private AnimatedSprite3D _animatedSprite;
	public float _walkSpeed = 1f;
	private Vector3 _velocity;
	private Vector3 _characterDirection;
	private int _facingDirection = 6;

	private Area3D _interactionArea;
	private bool _canInteract = false;
	private bool _isInteracting = false;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_interactionArea = GetNode<Area3D>("Area3D");
		_interactionArea.Monitoring = true;
		_interactionArea.BodyEntered += _OnBodyEntered;
		_interactionArea.BodyExited += _OnBodyExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		_characterDirection.X = Input.GetAxis("move_left", "move_right");
		_characterDirection.Z = Input.GetAxis("move_up", "move_down");
		_characterDirection = _characterDirection.Normalized();

		//flip
		if (_characterDirection.X > 0) _animatedSprite.FlipH = true;
		if (_characterDirection.X < 0) _animatedSprite.FlipH = false;

		if (_characterDirection != new Vector3(0f, 0f, 0f))
		{
			Velocity = _characterDirection * _walkSpeed;
			if (_characterDirection.Z == 0f)
			{
				if (_animatedSprite.Animation != "Walk_Left") _animatedSprite.Play("Walk_Left");
				_facingDirection = 9;
			}
			else
			{
				if (_characterDirection.X == 0f)
				{
					if (_characterDirection.Z > 0f)
					{
						if (_animatedSprite.Animation != "Walk_Down") _animatedSprite.Play("Walk_Down");
						_facingDirection = 6;
					}
					else
					{
						if (_animatedSprite.Animation != "Walk_Up") _animatedSprite.Play("Walk_Up");
						_facingDirection = 12;
					}
				}
				else
				{
					if (_characterDirection.Z > 0f)
					{
						if (_animatedSprite.Animation != "Walk_DiagDown") _animatedSprite.Play("Walk_DiagDown");
						_facingDirection = 7;
					}
					else
					{
						if (_animatedSprite.Animation != "Walk_DiagUp") _animatedSprite.Play("Walk_DiagUp");
						_facingDirection = 10;
					}
				}
			}
		}
		else
		{
			Velocity = Velocity.MoveToward(_characterDirection, _walkSpeed);
			switch (_facingDirection)
			{
				case 6:
					if (_animatedSprite.Animation != "Idle_Down") _animatedSprite.Play("Idle_Down");
					break;
				case 9:
					if (_animatedSprite.Animation != "Idle_Left") _animatedSprite.Play("Idle_Left");
					break;
				case 12:
					if (_animatedSprite.Animation != "Idle_Up") _animatedSprite.Play("Idle_Up");
					break;
				case 7:
					if (_animatedSprite.Animation != "Idle_DiagDown") _animatedSprite.Play("Idle_DiagDown");
					break;
				case 10:
					if (_animatedSprite.Animation != "Idle_DiagUp") _animatedSprite.Play("Idle_DiagUp");
					break;
			}
		}
		if (_interactionArea.HasOverlappingBodies())
		{
			if (!_isInteracting)
			{
				var bodies = _interactionArea.GetOverlappingBodies();
				var shortestDistance = Mathf.Inf;
				var bodyDistance = 0f;
				var farthestBody = new Node3D();
				var closestBody = new Node3D();
				foreach (var body in bodies)
				{
					var collisionBody = body.GetNode<CollisionShape3D>("CollisionShape3D");
					bodyDistance = collisionBody.GlobalPosition.DistanceTo(this.GlobalPosition);
					farthestBody = body;
					if (bodyDistance < shortestDistance)
					{
						farthestBody = closestBody;
						closestBody = body;
						shortestDistance = bodyDistance;
					}
				}
				var farthestParent = farthestBody.GetParent() as IInteractable;
				if (farthestParent != null)
				{
					farthestParent?.Uninteractable();
				}
				var closestParent = closestBody.GetParent() as IInteractable;
				//Debug.Print("Farthest body: " + farthestBody.Name);
				if (Input.IsActionPressed("interact"))
				{
					{
						_isInteracting = true;
						closestParent?.Interaction();
					}
				}
				else
				{
					_isInteracting = false;
					closestParent?.Entered();
					//Debug.Print("else");
				}
			}
			else
			{
				if (_isInteracting && !Input.IsActionPressed("interact"))
				{
					_isInteracting = false;
					var bodies = _interactionArea.GetOverlappingBodies();
					foreach (var body in bodies)
					{
						var interactable = body.GetParent() as IInteractable;
						if (interactable.CanInteract == true)
						{
							interactable?.Entered();
						}
					}
				}
			}
			/*else
			{
				if (_isInteracting)
				{
					_isInteracting = false;
					var bodies = _interactionArea.GetOverlappingBodies();
					foreach (var body in bodies)
					{
						var interactable = body.GetParent() as IInteractable;
						interactable?.Entered();
					}
				}
			}*/
		}/*
		
		if (Input.IsActionPressed("interact"))
		{
			if (!_isInteracting)
			{
				_isInteracting = true;
				var bodies = _interactionArea.GetOverlappingBodies();
				var shortestDistance = Mathf.Inf;
				var bodyDistance = 0f;
				var farthestBody = new Node3D();
				var closestBody = new Node3D();
				foreach (var body in bodies)
				{
					var collisionBody = body.GetNode<CollisionShape3D>("CollisionShape3D");
					bodyDistance = collisionBody.GlobalPosition.DistanceTo(this.GlobalPosition);
					farthestBody = body;
					if (bodyDistance < shortestDistance)
					{
						farthestBody = closestBody;
						closestBody = body;
						shortestDistance = bodyDistance;
						Debug.Print("Found closer body: " + body.Name + " at distance " + bodyDistance);
					}
				}
				var farthestParent = farthestBody.GetParent() as IInteractable;
				farthestParent?.Uninteractable();
				var closestParent = closestBody.GetParent() as IInteractable;
				closestParent?.Interaction();
			}
		
		}
		else
		{
			if (_isInteracting)
			{
				_isInteracting = false;
				var bodies = _interactionArea.GetOverlappingBodies();
				foreach (var body in bodies)
				{
					var interactable = body.GetParent() as IInteractable;
					interactable?.Entered();
				}
			}
		}*/

		MoveAndSlide();
	}
	#region Alert System
	private void _OnBodyEntered(Node3D body)
	{
		//Debug.Print("Entered interaction area with " + body.Name);
		var interactable = body.GetParent() as IInteractable;
		interactable?.Entered();
	}

	private void _OnBodyExited(Node3D body)
	{
		//Debug.Print("Exited interaction area with " + body.Name);
		var interactable = body.GetParent() as IInteractable;
		interactable?.Exited();
	}
	#endregion
	public void PlayWalkAnimation()
	{
		if (_characterDirection == new Vector3(0f, 0f, 0f))
		{
			_animatedSprite.Play("WalkForward");
		}

	}

	public void NearObject()
	{
		if (GetNode<Area3D>("Area3D").HasOverlappingBodies())
		{

		}
	}
}
