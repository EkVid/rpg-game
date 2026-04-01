using Godot;

namespace tinySwords.scripts;

public partial class Enemy: CharacterBody2D
{
    private readonly Health _health = new Health();
    
    protected AnimatedSprite2D _animationSprite;
    protected CollisionShape2D _collisionShape2D;
    protected Area2D _chaseBox;
    protected Node2D _hitbox;
    protected Area2D _hitboxArea;
    
    protected float _speed = 100f;
    
    protected float _attackPower = 50f;
    protected float _attackDelay = 0.5f;
    protected float _jitterPrevention = 2f;
    protected float _attackDistance = 60f;
    
    protected bool _isAttacking = false;
    protected bool _playerInRange = false;
    protected bool _isDead = false;
    
    protected Vector2 _direction = Vector2.Zero;
    protected Vector2 _spawnPoint;
    protected Player _player;

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
    }

    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }
}