using Godot;
using System;
using tinySwords.scripts;

public partial class Player : Character
{
	private Vector2 _direction;
	private float _x;
	private float _y;
	private bool _isAttacking = false;
	private Node2D _hitbox;
	private Area2D _hitboxArea;

	public override void _Ready()
	{
		base._Ready();
		_hitbox = GetNode<Node2D>("div");
		_hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
		
		_character.AnimationFinished += () =>
		{
			if (_character.Animation == "attack")
			{
				_isAttacking = false;
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleAttack();
		HandleMovement();
		HandleFlip();
		UpdateAnimation();
		MoveAndSlide();
	}

	private void HandleMovement()
	{
		_x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		_y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
		
		_direction = new Vector2(_x,_y);

		if (_direction != Vector2.Zero)
		{
			Velocity = _direction.Normalized() * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	}

	private void HandleFlip()
	{
		if (_x != 0)
		{
			_hitbox.Scale = new Vector2(_x * Mathf.Abs(_hitbox.Scale.X), _hitbox.Scale.Y);
			_character.FlipH = _x < 0;
		}
	}

	private void HandleAttack()
	{
		if (Input.IsActionJustPressed("attack") &&  !_isAttacking)
		{
			_isAttacking = true;
			PlayAnimation("attack");
			foreach (var node in _hitboxArea.GetOverlappingBodies())
			{
				if (node != this && node is Character enemyCharacter)
				{
					enemyCharacter.TakeDamage(AttackPower);
				}
			}
		}
	}

	private void UpdateAnimation()
	{
		if (_isAttacking)
			return;
		string anima = _direction != Vector2.Zero ? "run" : "idle";
		PlayAnimation(anima);
	}

	private void PlayAnimation(String animation)
	{
		if(_character.Animation != animation) 
			_character.Play(animation);
	}
}
