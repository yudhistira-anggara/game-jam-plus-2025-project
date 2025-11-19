using Godot;
using System;
using System.Diagnostics;

public partial class PlayerController : CharacterBody3D
{
	private AnimatedSprite3D _animatedSprite;
	public float _walkSpeed = 1f;
	private Vector3 _velocity;
	private Vector3 _characterDirection;
	private int _facingDirection = 6;


	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		_characterDirection.X = Input.GetAxis("move_left","move_right");
		_characterDirection.Z = Input.GetAxis("move_up","move_down");
		_characterDirection = _characterDirection.Normalized();

		//flip
		if (_characterDirection.X > 0) _animatedSprite.FlipH = true;
		if (_characterDirection.X < 0) _animatedSprite.FlipH = false;

		if (_characterDirection != new Vector3(0f,0f,0f)){
			Velocity =_characterDirection * _walkSpeed ;
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


		MoveAndSlide();
	}
	public void PlayWalkAnimation()
	{
		if (_characterDirection == new Vector3(0f, 0f, 0f))
		{
			_animatedSprite.Play("WalkForward");
		}
		
	}

}
