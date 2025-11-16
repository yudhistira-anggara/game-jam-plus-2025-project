using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	private AnimatedSprite3D _animatedSprite;
	public float _walkSpeed = 1f;
	private Vector3 _velocity;
	private Vector3 _characterDirection;


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
			}
			else
			{
				if (_characterDirection.X == 0f)
				{
					if (_characterDirection.Z > 0f)
					{
						if (_animatedSprite.Animation != "Walk_Down") _animatedSprite.Play("Walk_Down");
					}
					else
					{
						if (_animatedSprite.Animation != "Walk_Up") _animatedSprite.Play("Walk_Up");
					}
				}
				else
				{
					if (_characterDirection.Z > 0f)
					{
						if (_animatedSprite.Animation != "Walk_DiagDown") _animatedSprite.Play("Walk_DiagDown");
					}
					else
					{
						if (_animatedSprite.Animation != "Walk_DiagUp") _animatedSprite.Play("Walk_DiagUp");
					}
				}
			}
		}
		else
		{
			Velocity = Velocity.MoveToward(_characterDirection, _walkSpeed);
			if (_animatedSprite.Animation != "Idle") _animatedSprite.Play("Idle");
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
