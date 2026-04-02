using Godot;
using System;
using System.Threading.Tasks;
using GodotTask;

namespace tinySwords.scripts;

public class DeadState: IState
{
    private readonly CharacterBody2D _characterBody2D;
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly CollisionShape2D _collisionShape2D;
    private readonly Health _health;
    private readonly float _hitpoints;
    private readonly float _respawnDelay;
    private readonly Vector2 _spawnPoint;
    private readonly Action _onRespawn;

    public DeadState(CharacterBody2D characterBody2D, AnimatedSprite2D animatedSprite2D,
        CollisionShape2D collisionShape2D, Health health, float hitpoints,
        float respawnDelay, Vector2 spawnPoint, Action onRespawn)
    {
        _characterBody2D = characterBody2D;
        _animatedSprite2D = animatedSprite2D;
        _collisionShape2D = collisionShape2D;
        _health = health;
        _hitpoints = hitpoints;
        _respawnDelay = respawnDelay;
        _spawnPoint = spawnPoint;
        _onRespawn = onRespawn;
    }
    
    public void Enter()
    {
        HandleDeathAsync().Forget();
    }

    private async GDTask HandleDeathAsync()
    {
        _collisionShape2D.SetDeferred("disabled", true);
        _animatedSprite2D.Play("dead");
        await _animatedSprite2D.ToSignal(_animatedSprite2D, AnimatedSprite2D.SignalName.AnimationFinished);

        _characterBody2D.Visible = false;
        await GDTask.Delay(TimeSpan.FromSeconds(_respawnDelay));
        
        _characterBody2D.GlobalPosition = _spawnPoint;
        _health.Reset(_hitpoints);
        _collisionShape2D.SetDeferred("disabled", false);
        _characterBody2D.Visible = true;
        _animatedSprite2D.Play("idle");
        _onRespawn?.Invoke(); 
    }
    

    public void Update(double delta) { }

    public void Exit() { }
}