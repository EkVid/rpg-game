using Godot;

namespace tinySwords.scripts;

public partial class Character: CharacterBody2D
{
    protected float Hitpoints{ get; set; } = 100f;
    protected float Speed{ get; set; } = 300f;
    protected AnimatedSprite2D _character;
    protected float AttackPower { get; set; } = 50f;
    protected float DeathDelay { get; set; } = 0.5f;
    
    public override void _Ready()
    {
        base._Ready();
        _character = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    
    public void TakeDamage(float damage)
    {
        Hitpoints -= damage;
        if (Hitpoints <= 0)
        {
            GetTree().CreateTimer(DeathDelay).Timeout += () =>
            {
                Death();
            };
        }
    }
    
    private void Death()
    {
        QueueFree();
    }
}