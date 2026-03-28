using Godot;
using System;
using System.Threading.Tasks;
using GodotTask;
using tinySwords.scripts;

public partial class Player : CharacterBody2D, IDamagable
{
	private float _speed = 300f;
	private AnimatedSprite2D _animationSprite;
	private float _attackPower = 50f;
	private float _attackDelay = 0.5f;
	private Vector2 _direction;
	private float _x;
	private float _y;
	private bool _isAttacking = false;
	private Node2D _hitbox;
	private Area2D _hitboxArea;
	private readonly Health _health = new Health();
	
	public override void _Ready()
	{
		base._Ready();
		_hitbox = GetNode<Node2D>("div");
		_hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
		_animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		_animationSprite.AnimationFinished += () =>
		{
			if (_animationSprite.Animation == "attack")
			{
				_isAttacking = false;
			}
		};

		_health.OnDeath += QueueFree;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleAttack().Forget();
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
			Velocity = _direction.Normalized() * _speed;
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
			_animationSprite.FlipH = _x < 0;
		}
	}

	private async GDTask HandleAttack()
	{
		if (Input.IsActionJustPressed("attack") &&  !_isAttacking)
		{
			_isAttacking = true;
			PlayAnimation("attack");
			await GDTask.Delay(TimeSpan.FromSeconds(_attackDelay), DelayType.Realtime);
			await GDTask.WaitForPhysicsProcess();
			foreach (var node in _hitboxArea.GetOverlappingBodies())
			{
				if (node != this && node is IDamagable enemyCharacter)
					enemyCharacter.TakeDamage(_attackPower);
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
		if(_animationSprite.Animation != animation) 
			_animationSprite.Play(animation);
	}

	public void TakeDamage(float damage)
	{
		_health.TakeDamage(damage);
	}
}
