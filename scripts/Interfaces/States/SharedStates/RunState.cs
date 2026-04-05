using System;
using Godot;

namespace tinySwords.scripts;

public class RunState: IState
{
    protected readonly CharacterBody2D _characterBody2D;
    protected readonly AnimatedSprite2D _animatedSprite2D;
    protected readonly float _speed;
    protected readonly Node2D _hitbox;
    protected readonly Func<Vector2> _getDirection;
    protected readonly Action _enterIdle;
    protected readonly Action _enterAttack;

    public RunState(CharacterBody2D character, AnimatedSprite2D sprite, float speed, Node2D hitbox,
        Func<Vector2> getDirection, Action enterIdle, Action enterAttack)
    {
        _characterBody2D = character;
        _animatedSprite2D = sprite;
        _speed = speed;
        _hitbox = hitbox;
        _getDirection = getDirection;
        _enterIdle = enterIdle;
        _enterAttack = enterAttack;
    }
    public void Enter() => _animatedSprite2D.Play("run");

    public virtual void Update(double delta) { }

    public void Exit() { }
}