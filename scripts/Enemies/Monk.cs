using System;
using System.Linq;
using Godot;
using GodotTask;

namespace tinySwords.scripts;

public partial class Monk: CharacterBody2D, IDamagable
{
    private readonly Health _health = new Health();
    private AnimatedSprite2D _animationSprite;
    private AnimatedSprite2D _healAnimation;
    private CollisionShape2D _collisionShape2D;
    
    private Area2D _chaseBox;
    private Area2D _healbox;
    
    
    private float _speed = 100f;
    
    private float _attackPower = 50f;
    private float _attackDelay = 1f;
    private float _jitterPrevention = 2f;
    private float _attackDistance = 60f;
    
    private bool _isAttacking = false;
    private bool _isDead = false;
    
    private Vector2 _direction = Vector2.Zero;
    private Vector2 _spawnPoint;
    private Player _player;
    
    private bool _playerInRange = false;
    private bool _teamInRange = false;
    private bool _teamInHealingRange = false;


    public override void _Ready()
    {
        base._Ready();
        _player = GetNode<Player>("%Player");
        _animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _healAnimation = GetNode<AnimatedSprite2D>("HealAnimation");
        _chaseBox = GetNode<Area2D>("ChaseBox");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _healbox = GetNode<Area2D>("HealBox");
        _spawnPoint = GlobalPosition;
        
        _animationSprite.AnimationFinished += () =>
        {
            if(_animationSprite.Animation == "attack")
            {
                _isAttacking = false;
                PlayAnimation("idle",  _animationSprite);
                _healAnimation.Visible = false;
            }
        };

        _chaseBox.BodyEntered += (Node2D body) =>
        {
            if (body is Player)
                _playerInRange = true;
            else if(body != this && body is IDamagable)
                _teamInRange = true;
        };

        _chaseBox.BodyExited += (Node2D body) =>
        {
            if (body is Player)
                _playerInRange = false;
            else if(body!= this && body is IDamagable)
                _teamInRange = false;
        };

        _healbox.BodyEntered += (Node2D body) =>
        {
            if (body != this && body is IDamagable && body is not Player)
                _teamInHealingRange = true;
        };

        _healbox.BodyExited += (Node2D body) =>
        {
            if (body != this && body is IDamagable && body is not Player)
                _teamInHealingRange = false;
        }; 
        
        _health.OnDeath += async () =>
        {
            _isDead = true;
            _collisionShape2D.Disabled = true;
            PlayAnimation("dead", _animationSprite);
            await ToSignal(_animationSprite, AnimatedSprite2D.SignalName.AnimationFinished);
            QueueFree();
        };
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (_isDead)
        {
            _isAttacking = false;
            return;
        }
        
        if ((_health.Hitpoints < 100 && !_isAttacking) || _teamInHealingRange)
        {
            HandleHeal(_attackPower).Forget();
        }
        
        if (_teamInRange && !_isAttacking)
        {
            if (_chaseBox.GetOverlappingBodies().Where(node => node != this && node is IDamagable teammate).MinBy((body) =>
                    GlobalPosition.DistanceTo(body.GlobalPosition)) is Node2D allyNode2D)
            {
                Vector2 direction = GlobalPosition.DirectionTo(allyNode2D.GlobalPosition);
                Velocity = direction * _speed;
                MoveAndSlide();
                PlayAnimation("run", _animationSprite);
                _healAnimation.Visible = false;
            }
        }
        

    }
    
    private async GDTask HandleHeal(float heal)
    {
        _isAttacking = true;
        PlayAnimation("attack", _animationSprite);
        _healAnimation.Visible = true;
        PlayAnimation("heal", _healAnimation);
        await GDTask.Delay(TimeSpan.FromSeconds(_attackDelay), DelayType.Realtime);
        await GDTask.WaitForPhysicsProcess();
        Heal(heal);
        foreach (var node in _chaseBox.GetOverlappingBodies())
        {
            if (node != this && node is not Player && node is IDamagable teammate and not Area2D)
            {
                teammate.Heal(heal);
            }
        }
        
    }

    
    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }

    public void Heal(float heal)
    {
        _health.Heal(heal);
    }

    private void PlayAnimation(string animation, AnimatedSprite2D sprite)
    {
        if(sprite.Animation != animation || sprite.Animation == "attack") 
            sprite.Play(animation);
    }
}