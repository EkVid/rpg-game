using Godot;
using System;
using tinySwords.scripts;

public partial class Warrior : CharacterBody2D, IDamagable
{
    private readonly Health _health = new Health();
    private float _speed = 300f;
    private AnimatedSprite2D _animationSprite;
    private float _attackPower = 50f;
    private float _attackDelay = 0.5f;
    private bool _isAttacking = false;
    private Node2D _hitbox;
    private Area2D _hitboxArea;
    public override void _Ready()
    {
        base._Ready();
        // _hitbox = GetNode<Node2D>("div");
        // _hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
        _animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        _animationSprite.AnimationFinished += () =>
        {
            if (_animationSprite.Animation == "attack")
            {
                _isAttacking = false;
            }
        };
        
        _health.OnDeath += QueueFree;
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }
}
