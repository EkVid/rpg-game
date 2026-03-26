using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float Speed = 300.0f;
	private Vector2 direction;

	public override void _PhysicsProcess(double delta)
	{
		float x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		float y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
		
		direction = new Vector2(x,y);

		if (direction != Vector2.Zero)
		{
			Velocity = direction.Normalized() * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		
		MoveAndSlide();
	}
}
