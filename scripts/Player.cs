using Godot;
using tinySwords.scripts;


public partial class Player : CharacterBody2D, IDamagable
{
    private float _speed = 300f;
    private float _hitpoints = 300f;
    private float _attackPower = 50f;
    private float _attackDelay = 0.5f;
    private float _respawnDelay = 2f;

    private AnimatedSprite2D _animationSprite;
    private Node2D _hitbox;
    private Area2D _hitboxArea;
    private CollisionShape2D _collisionShape2D;
    private Vector2 _spawnPoint;
    private readonly Health _health = new();

    public StateMachine StateMachine { get; } = new();

    public IState IdleState { get; private set; }
    public RunState RunState { get; private set; }
    public AttackState AttackState { get; private set; }
    public DeadState DeadState { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        _health.Hitpoints = _hitpoints;

        _hitbox = GetNode<Node2D>("div");
        _hitboxArea = _hitbox.GetNode<Area2D>("HitBox");
        _animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _spawnPoint = GlobalPosition;

        IdleState = new IdleState(_animationSprite,
            GetDirection, () => StateMachine.ChangeState(RunState), 
            () => StateMachine.ChangeState(AttackState));

        RunState = new RunState(this, _animationSprite, _speed, _hitbox,
            GetDirection, () => StateMachine.ChangeState(IdleState), 
            () => StateMachine.ChangeState(AttackState));
        
        AttackState = new AttackState(this, _animationSprite, _attackDelay,
            _hitboxArea, _attackPower, () => StateMachine.ChangeState(IdleState));
    
        DeadState = new DeadState(this, _animationSprite, _collisionShape2D, _health, _hitpoints,
            _respawnDelay, _spawnPoint, () => StateMachine.ChangeState(IdleState));

        _health.OnDeath += () => StateMachine.ChangeState(DeadState);

        StateMachine.ChangeState(IdleState);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        StateMachine.Update(delta);
    }

    private static Vector2 GetDirection()
    {
        float x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        float y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
        return new Vector2(x,y);
    }

    public void TakeDamage(float damage) => _health.TakeDamage(damage);
    public void Heal(float heal) => _health.Heal(heal);
}