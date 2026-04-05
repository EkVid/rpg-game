using Godot;
using System;
using System.Threading.Tasks;
using GodotTask;

namespace tinySwords.scripts;

public abstract class DeadState: IState
{
    protected readonly CharacterBody2D _characterBody2D;
    protected readonly AnimatedSprite2D _animatedSprite2D;
    protected readonly CollisionShape2D _collisionShape2D;

    
    public DeadState(CharacterBody2D characterBody2D, AnimatedSprite2D animatedSprite2D,
        CollisionShape2D collisionShape2D)
    {
        _characterBody2D = characterBody2D;
        _animatedSprite2D = animatedSprite2D;
        _collisionShape2D = collisionShape2D; 
    }
    
    public void Enter()
    {
        HandleDeathAsync().Forget();
    }

    public abstract GDTask HandleDeathAsync();
    

    public void Update(double delta) { }

    public void Exit() { }
}