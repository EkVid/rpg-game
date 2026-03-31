using Godot;
using System;
using System.Threading.Tasks;
using GodotTask;
using tinySwords.scripts;
using Animation = Godot.Animation;

public partial class Player : CharacterBody2D, IDamagable
{
	private float _speed = 300f;
	private float _hitpoints = 300f;
	private AnimatedSprite2D _animationSprite;
	private float _attackPower = 50f;
	private float _attackDelay = 0.5f;
	private bool _isAttacking = false;
	private Node2D _hitbox;
	private Area2D _hitboxArea;
	private Vector2 _direction;
	private float _x;
	private float _y;
	private readonly Health _health = new Health();
	
	private Camera2D _camera;
	private Vector2 _spawnPoint;
	private float _respawnDelay = 2f;
	private bool _isDead = false;
	private CollisionShape2D _collisionShape2D;

	public override void _Ready()
	{
		base._Ready();
		_health.Hitpoints = _hitpoints;
		_hitbox = GetNode<Node2D>("div");
		_hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
		_animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		_camera = GetNode<Camera2D>("Camera2D");
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		_spawnPoint = GlobalPosition;
		
		_animationSprite.AnimationFinished += () =>
		{
			if (_animationSprite.Animation == "attack")
			{
				_isAttacking = false;
			}
		};

		_health.OnDeath += async() =>
		{
			_isDead = true;
			_isAttacking = false;
			_collisionShape2D.SetDeferred("disabled", true);
			PlayAnimation("dead", _animationSprite);
			await ToSignal(_animationSprite, AnimatedSprite2D.SignalName.AnimationFinished);

			Visible = false;
			await ToSignal(GetTree().CreateTimer(_respawnDelay), SceneTreeTimer.SignalName.Timeout);
			GlobalPosition = _spawnPoint;
			_health.Reset(_hitpoints);
			_isDead = false;
			_isAttacking = false;
			_direction = Vector2.Zero;
			Velocity = Vector2.Zero;
			
			
			_collisionShape2D.SetDeferred("disabled", false);
			Visible = true;
			PlayAnimation("idle", _animationSprite);
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead) return;
		TryAttack();
		HandleMovement();
		HandleFlip();
		UpdateAnimation(_direction, _animationSprite, _isAttacking);
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

	private void TryAttack()
	{
		if (Input.IsActionJustPressed("attack") &&  !_isAttacking)
		{ 
			Attack().Forget();
		}
	}

	private async GDTask Attack()
	{
		_isAttacking = true;
		PlayAnimation("attack", _animationSprite);
		await GDTask.Delay(TimeSpan.FromSeconds(_attackDelay), DelayType.Realtime);
		await GDTask.WaitForPhysicsProcess();
		foreach (var node in _hitboxArea.GetOverlappingBodies())
		{
			if (node != this && node is IDamagable enemyCharacter)
				enemyCharacter.TakeDamage(_attackPower);
		}
	}

	public void TakeDamage(float damage)
	{
		_health.TakeDamage(damage);
	}

	public void UpdateAnimation(Vector2 direction, AnimatedSprite2D sprite, bool isAttacking)
	{
		if (isAttacking || _isDead)
			return;
		string anima = direction != Vector2.Zero ? "run" : "idle";
		PlayAnimation(anima, sprite);
	}

	public void PlayAnimation(string animation, AnimatedSprite2D sprite)
	{
		if (sprite.Animation != animation)
		{			
			sprite.Play(animation);
		}
	}
}
