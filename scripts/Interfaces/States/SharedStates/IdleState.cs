namespace tinySwords.scripts;
using System;
using Godot;

public class IdleState: IState
{
    protected readonly AnimatedSprite2D _animatedSprite2D;
    protected readonly Func<Vector2> _getDirection;
    protected readonly Action _enterMove;
    
    public IdleState(AnimatedSprite2D animatedSprite2D,  Func<Vector2> getDirection, Action enterMove)
    {
        _animatedSprite2D = animatedSprite2D;
        _getDirection = getDirection;
        _enterMove = enterMove;
    }
    
    public void Enter()
    {
        _animatedSprite2D.Play("idle");
    }

    public virtual void Update(double delta) { }

    public void Exit() { }
}