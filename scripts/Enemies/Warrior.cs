using Godot;
using System;
using GodotTask;
using tinySwords.scripts;

public partial class Warrior : CharacterBody2D, IDamagable
{
    private readonly Health _health = new Health();
    
    private AnimatedSprite2D _animationSprite;
    private CollisionShape2D _collisionShape2D;
    private Area2D _chaseBox;
    private Node2D _hitbox;
    private Area2D _hitboxArea;
    
    private float _speed = 100f;
    
    private float _attackPower = 50f;
    private float _attackDelay = 0.5f;
    private float _jitterPrevention = 2f;
    private float _attackDistance = 60f;
    
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
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _hitbox = GetNode<Node2D>("div");
        _hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
        _spawnPoint = GlobalPosition;
        
        _animationSprite.AnimationFinished += () =>
        {
            if(_animationSprite.Animation == "attack")
            {
                foreach (var node in _hitboxArea.GetOverlappingBodies())
                {
                    if (node != this && node is IDamagable enemy)
                        enemy.TakeDamage(_attackPower);
                }
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
            _collisionShape2D.Disabled = true;
            PlayAnimation("dead", _animationSprite);
            await ToSignal(_animationSprite, AnimatedSprite2D.SignalName.AnimationFinished);
            QueueFree();
        };
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float distanceToPlayer = (_player.GlobalPosition - GlobalPosition).Length();
        float distanceToSpawn = (GlobalPosition - _spawnPoint).Length();

        if (_isDead)
        {
            _isAttacking = false;
            return;
        }
        
        if (_playerInRange && _player != null)
        {
            _direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            if (distanceToPlayer <= _attackDistance)
            {
                Velocity = Vector2.Zero;
                if (!_isAttacking)
                {
                   HandleAttack().Forget();
                }
            }
            else
            {
                _isAttacking = false;
                Velocity = _direction * _speed;
                PlayAnimation("run", _animationSprite);
            }
        }
        else
        {
            _isAttacking = false;
            if (distanceToSpawn > _jitterPrevention)
            {
                _direction = GlobalPosition.DirectionTo(_spawnPoint);
                Velocity = _direction * _speed;
                PlayAnimation("run", _animationSprite);
            }
            else
            {
                GlobalPosition = _spawnPoint;
                _direction = Vector2.Zero;
                Velocity = Vector2.Zero;
                PlayAnimation("idle", _animationSprite);
                return;
            }
        }

        if (_direction.X != 0)
        {
            _hitbox.Scale = new Vector2(Mathf.Sign(_direction.X) * Mathf.Abs(_hitbox.Scale.X), _hitbox.Scale.Y);
            _animationSprite.FlipH = _direction.X < 0;
        }
        
        MoveAndSlide();
    }
    
    private async GDTask HandleAttack()
    {
        _isAttacking = true;
        PlayAnimation("attack", _animationSprite);
        await GDTask.Delay(TimeSpan.FromSeconds(_attackDelay), DelayType.Realtime);
        await GDTask.WaitForPhysicsProcess();
    }
    
    public void TakeDamage(float damage)
    {
        if (!_isDead) 
            _health.TakeDamage(damage);
    }

    public void Heal(float heal)
    {
        _health.Heal(heal);
    }

    public void PlayAnimation(string animation, AnimatedSprite2D sprite)
    {
        if(sprite.Animation != animation || sprite.Animation == "attack") 
            sprite.Play(animation);
    }
}
