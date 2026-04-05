using System;
using Godot;

namespace tinySwords.scripts;

public class RunState: IState
{
    private readonly CharacterBody2D _characterBody2D;
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly float _speed;
    private readonly Node2D _hitbox;
    private readonly Func<Vector2> _getDirection;
    private readonly Action _enterIdle;
    private readonly Action _enterAttack;
    private readonly bool _flag;

    // _flag == false -> player, else => enemy
    public RunState(CharacterBody2D character, AnimatedSprite2D sprite, float speed, Node2D hitbox,
        Func<Vector2> getDirection, Action enterIdle, Action enterAttack, bool flag)
    {
        _characterBody2D = character;
        _animatedSprite2D = sprite;
        _speed = speed;
        _hitbox = hitbox;
        _getDirection = getDirection;
        _enterIdle = enterIdle;
        _enterAttack = enterAttack;
        _flag = flag;
    }
    public void Enter()
    {
        _animatedSprite2D.Play("run");
    }

    public void Update(double delta)
    {
        Vector2 direction = _getDirection();

        if (direction == Vector2.Zero)
        {
            _enterIdle();
            return;
        }

        if (Input.IsActionJustPressed("attack") && !_flag)
        {
            _enterAttack();
            return;
        }
        
        _characterBody2D.Velocity = direction.Normalized() * _speed;
        if (direction.X != 0)
        {
            if (_flag == false)
            {
                _hitbox.Scale = new Vector2(direction.X * Mathf.Abs(_hitbox.Scale.X), _hitbox.Scale.Y);
            }
            else
            {
                _hitbox.Scale = new Vector2(Mathf.Sign(direction.X) * Mathf.Abs(_hitbox.Scale.X), _hitbox.Scale.Y);
            }
            _animatedSprite2D.FlipH = direction.X < 0;
        }

        _characterBody2D.MoveAndSlide();
    }

    public void Exit() { }
}