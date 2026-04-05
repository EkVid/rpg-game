using System;
using Godot;

namespace tinySwords.scripts.States.PlayerStates;

public class RunStatePlayer: RunState
{
    public RunStatePlayer(CharacterBody2D character, AnimatedSprite2D sprite, float speed, Node2D hitbox, Func<Vector2> getDirection, Action enterIdle, Action enterAttack) 
        : base(character, sprite, speed, hitbox, getDirection, enterIdle, enterAttack) { }

    public override void Update(double delta)
    {
        Vector2 direction = _getDirection();

        if (direction == Vector2.Zero)
        {
            _enterIdle();
            return;
        }

        if (Input.IsActionJustPressed("attack"))
        {
            _enterAttack();
            return;
        }
        _characterBody2D.Velocity = direction.Normalized() * _speed;
        if (direction.X != 0)
        {
            _hitbox.Scale = new Vector2(direction.X * Mathf.Abs(_hitbox.Scale.X), _hitbox.Scale.Y);
            _animatedSprite2D.FlipH = direction.X < 0;
        }

        _characterBody2D.MoveAndSlide();
    }
}