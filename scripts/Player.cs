using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float Speed = 300.0f;
	private Vector2 direction;
	private AnimatedSprite2D character;
	private float x;
	private float y;
	private bool isAttacking = false;

	public override void _Ready()
	{
		base._Ready();
		character = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		character.AnimationFinished += () =>
		{
			if (character.Animation == "attack")
			{
				isAttacking = false;
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
		x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
		
		direction = new Vector2(x,y);

		if (direction != Vector2.Zero)
		{
			Velocity = direction.Normalized() * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	}

	private void HandleFlip()
	{
		if(x != 0) 
			character.FlipH = x < 0;
	}

	private void HandleAttack()
	{
		if (Input.IsActionJustPressed("attack") &&  !isAttacking)
		{
			isAttacking = true;
			PlayAnimation("attack");
		}
	}

	private void UpdateAnimation()
	{
		if (isAttacking)
			return;
		string anima = direction != Vector2.Zero ? "run" : "idle";
		PlayAnimation(anima);
	}

	private void PlayAnimation(String animation)
	{
		if(character.Animation != animation) 
			character.Play(animation);
	}
}
