using System;
using Godot;

namespace tinySwords.scripts.States.EnemyStates;

public class IdleStateEnemy: IdleState
{
    public IdleStateEnemy(AnimatedSprite2D animatedSprite2D, Func<Vector2> getDirection, Action enterMove) 
        : base(animatedSprite2D, getDirection, enterMove) { }

    public override void Update(double delta)
    {
        if (_getDirection() != Vector2.Zero)
            _enterMove();
    }
}