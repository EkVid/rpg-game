namespace tinySwords.scripts;
using System;
using Godot;

// _flag == false -> player, else => enemy
public class IdleState: IState
{
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly Func<Vector2> _getDirection;
    private readonly bool _flag;
    private readonly Action _enterMove;
    private readonly Action _enterAttack;
    
    public IdleState(AnimatedSprite2D animatedSprite2D,  Func<Vector2> getDirection, 
        bool flag, Action enterMove, Action enterAttack)
    {
        _animatedSprite2D = animatedSprite2D;
        _getDirection = getDirection;
        _flag = flag;
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

        if (Input.IsActionJustPressed("attack") && !_flag)
            _enterAttack();
    }

    public void Exit() { }
}