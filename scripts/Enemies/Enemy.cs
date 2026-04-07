using Godot;
using tinySwords.scripts;
using tinySwords.scripts.States.EnemyStates;

public partial class Enemy : CharacterBody2D, IDamagable, IHealable
{
    protected AnimatedSprite2D _animationSprite;
    protected AnimatedSprite2D _healSprite;
    protected CollisionShape2D _collisionShape2D;

    public Health Health { get; } = new();
   
    protected Area2D ChaseBox;
    protected Vector2 SpawnPoint;
    
    protected Player Player;
    protected float Speed = 100f;
    
    protected float AttackPower = 50f;
    protected float AttackDelay = 0.5f;
    protected float JitterPrevention = 2f;
    protected float AttackDistance = 60f;

    protected bool IsDead = false;
    protected bool PlayerInRange = false;
    
    protected Node2D _hitbox;
    protected Area2D _hitboxArea;
    
    protected Vector2 Direction = Vector2.Zero;


    public StateMachine StateMachine { get; set; } = new();
        
    public IState IdleState { get; private set; }
    
    public IState RunState { get; private set; }
        
    public IState AttackState  { get; private set; }
        
    public IState DeadState { get; private set; }
    
    public override void _Ready()
    {
        base._Ready();
        Player = GetNode<Player>("%Player");
        _animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        ChaseBox = GetNode<Area2D>("ChaseBox");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _hitbox = GetNode<Node2D>("div");
        _hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
        _healSprite = GetNode<AnimatedSprite2D>("HealAnimation");
        SpawnPoint = GlobalPosition;
        
        IdleState = new IdleStateEnemy(_animationSprite, GetDirection,
            () => StateMachine.ChangeState(RunState));
        
        RunState = new RunStateEnemy(this, _animationSprite, Speed, _hitbox, GetDirection, 
            () => StateMachine.ChangeState(IdleState), 
            () => StateMachine.ChangeState(AttackState));

        AttackState = new AttackState(this, _animationSprite, AttackDelay, _hitboxArea, AttackPower,
            () => StateMachine.ChangeState(IdleState));

        DeadState = new DeadStateEnemy(this, _animationSprite, _collisionShape2D, _hitboxArea);
        
        Health.OnDeath += () =>
        {
            IsDead = true;
            StateMachine.ChangeState(DeadState);
        };
        
        StateMachine.ChangeState(IdleState);
    }

    public override void _PhysicsProcess(double delta) { }

    protected Vector2 GetDirection() => Direction;
    
    public void TakeDamage(float damage) => Health.TakeDamage(damage);

    public void Heal(float heal)
    {
        _healSprite.Play("heal");
        Health.Heal(heal);
    }

    public float GetHealth() => Health.Hitpoints;
}
