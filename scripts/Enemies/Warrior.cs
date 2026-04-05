using Godot;
using System;
using GodotTask;
using tinySwords.scripts;
using tinySwords.scripts.States.EnemyStates;

public partial class Warrior : CharacterBody2D, IDamagable, IHealable
{
    private readonly Health _health = new();
    
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

    private bool _isDead = false;
    private bool _playerInRange = false;
    private bool isEnemy = true;
    
    private Vector2 _direction = Vector2.Zero;
    private Vector2 _spawnPoint;
    private Player _player;


    public StateMachine StateMachine { get; set; } = new();
        
    public IState IdleState { get; private set; }
    
    public IState RunState { get; private set; }
        
    public IState AttackState  { get; private set; }
        
    public IState DeadStateEnemy { get; private set; }
    
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
        
        IdleState = new IdleState(_animationSprite, GetDirection, isEnemy,
            () => StateMachine.ChangeState(RunState), () => StateMachine.ChangeState(AttackState));
        
        RunState = new RunState(this, _animationSprite, _speed, _hitbox, GetDirection, 
            () => StateMachine.ChangeState(IdleState), 
            () => StateMachine.ChangeState(AttackState), isEnemy);

        AttackState = new AttackState(this, _animationSprite, _attackDelay, _hitboxArea, _attackPower,
            () => StateMachine.ChangeState(IdleState));

        DeadStateEnemy = new DeadStateEnemy(this, _animationSprite, _collisionShape2D, _hitboxArea);
        
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

        _health.OnDeath += () =>
        {
            _isDead = true;
            StateMachine.ChangeState(DeadStateEnemy);
        };
        
        StateMachine.ChangeState(IdleState);
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float distanceToPlayer = (_player.GlobalPosition - GlobalPosition).Length();
        float distanceToSpawn = (GlobalPosition - _spawnPoint).Length();
        
        if (_isDead) return;
        
        if (_playerInRange && _player != null)
        {
            _direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            if (distanceToPlayer <= _attackDistance)
            {
                // enter attack state
                if (StateMachine._currentState != AttackState)
                    StateMachine.ChangeState(AttackState);
            }
            else
            {
                // enter run state
                if (StateMachine._currentState != RunState) 
                    StateMachine.ChangeState(RunState);
            }
        }
        else
        {
            if (distanceToSpawn > _jitterPrevention)
            {
                // enter run state
                _direction = GlobalPosition.DirectionTo(_spawnPoint);
                if (StateMachine._currentState != RunState)
                    StateMachine.ChangeState(RunState);
            }
            else
            {
                // enter idle state
                GlobalPosition = _spawnPoint;
                _direction = Vector2.Zero;
                if(StateMachine._currentState != IdleState)
                    StateMachine.ChangeState(IdleState);
            }
        }
        
        StateMachine.Update(delta);
    }

    private Vector2 GetDirection()
    {
        return _direction;
    }
    
    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }

    public void Heal(float heal)
    {
        _health.Heal(heal);
    }
}
