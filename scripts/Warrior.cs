using Godot;
using System;
using tinySwords.scripts;

public partial class Warrior : CharacterBody2D, IDamagable, IPlayAnimation
{
    private readonly Health _health = new Health();
    private readonly Animations _animations = new Animations();
    
    private AnimatedSprite2D _animationSprite;
    private Area2D _chaseBox;
    
    private float _speed = 100f;
    
    private float _attackPower = 50f;
    private float _attackDelay = 0.5f;
    private float _jitterPrevention = 0.5f;
    
    private bool _isAttacking = false;
    private bool _playerInRange = false;
    private bool _isDead = false;
    
    private Vector2 _direction = Vector2.Zero;
    private Vector2 _spawnPoint;
    private Player _player;
    public override void _Ready()
    {
        base._Ready();
        _player = GetNode<Player>("%Player");
        _animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _chaseBox = GetNode<Area2D>("ChaseBox");
        _spawnPoint = GlobalPosition;
        
        _animationSprite.AnimationFinished += () =>
        {
            if (_animationSprite.Animation == "attack")
            {
                _isAttacking = false;
            }
        };

        _chaseBox.BodyEntered += (Node2D body) =>
        {
            if (body is Player)
                _playerInRange = true;
        };

        _chaseBox.BodyExited += (Node2D body) =>
        {
            if (body is Player)
                _playerInRange = false;
        };

        _health.OnDeath += async () =>
        {
            _isDead = true;
            PlayAnimation("dead", _animationSprite);
            await ToSignal(_animationSprite, AnimatedSprite2D.SignalName.AnimationFinished);
            GD.Print("DEAD");
            QueueFree();
        };
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_isDead) return;
        
        if (_playerInRange && _player != null)
        {
            _direction = (_player.GlobalPosition - GlobalPosition).Normalized();
            Velocity = _direction * _speed;
        }
        else
        {
            if ((GlobalPosition - _spawnPoint).Length() > _jitterPrevention)
            {
                _direction = (_spawnPoint - GlobalPosition).Normalized();
                Velocity = _direction * _speed;
            }
            else
            {
                _direction = Vector2.Zero;
                Velocity = Vector2.Zero;   
            }
        }
        
        UpdateAnimation(_direction, _animationSprite, false);
        MoveAndSlide();
    }

    public void TakeDamage(float damage)
    {
        if (!_isDead) 
            _health.TakeDamage(damage);
    }

    public void UpdateAnimation(Vector2 direction, AnimatedSprite2D sprite, bool isAttacking)
    {
        _animations.UpdateAnimation(_direction, _animationSprite, false);
    }

    public void PlayAnimation(string animation, AnimatedSprite2D sprite)
    {
        _animations.PlayAnimation(animation, sprite);
    }
}
