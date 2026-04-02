using System;
using Godot;

namespace tinySwords.scripts;

public class IdleState: IState
{
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly Func<Vector2> _getDirection;
    private readonly Action _enterMove;
    private readonly Action _enterAttack;
    
    public IdleState(AnimatedSprite2D animatedSprite2D,  Func<Vector2> getDirection, Action enterMove, Action enterAttack)
    {
        _animatedSprite2D = animatedSprite2D;
        _getDirection = getDirection;
        _enterMove = enterMove;
        _enterAttack = enterAttack;
    }
    
    public void Enter()
    {
        _animatedSprite2D.Play("idle");
    }

    public void Update(double delta)
    {
        if (_getDirection() != Vector2.Zero)
            _enterMove();

        if (Input.IsActionJustPressed("attack"))
            _enterAttack();
    }

    public void Exit() { }
}