using System;
using Godot;

namespace tinySwords.scripts.States.PlayerStates;

public class IdleStatePlayer: IdleState
{
    private readonly Action _enterAttack;

    public IdleStatePlayer(AnimatedSprite2D animatedSprite2D,  Func<Vector2> getDirection, 
        Action enterMove, Action enterAttack): base(animatedSprite2D, getDirection, enterMove)
    {
        _enterAttack = enterAttack;
    }

    public override void Update(double delta)
    {
        if (_getDirection() != Vector2.Zero)
            _enterMove();

        if (Input.IsActionJustPressed("attack"))
            _enterAttack();
    }
}