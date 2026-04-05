using System;
using Godot;
using GodotTask;

namespace tinySwords.scripts.States.EnemyStates;

public class DeadStateEnemy: DeadState
{
    private readonly Area2D _hitboxArea;

    public DeadStateEnemy(CharacterBody2D characterBody2D, AnimatedSprite2D animatedSprite2D,
        CollisionShape2D collisionShape2D, Area2D hitboxArea): base(characterBody2D, animatedSprite2D, collisionShape2D)
    {
        _hitboxArea = hitboxArea;
    }

    public override async GDTask HandleDeathAsync()
    {
        _collisionShape2D.SetDeferred("disabled", true);
        _hitboxArea.SetDeferred("monitoring", false); 
        _hitboxArea.SetDeferred("monitorable", false); 
        _animatedSprite2D.Play("dead");
        await _animatedSprite2D.ToSignal(_animatedSprite2D, AnimatedSprite2D.SignalName.AnimationFinished);
        _characterBody2D.Visible = false;
    }
}