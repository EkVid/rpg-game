using Godot;

namespace tinySwords.scripts;

public partial class Character: CharacterBody2D
{
    protected float Hitpoints{ get; set; } = 100f;
    protected float Speed{ get; set; } = 300f;
    protected AnimatedSprite2D AnimationSprite;
    protected float AttackPower { get; set; } = 50f;
    protected float AttackDelay { get; set; } = 0.5f;
    
    public override void _Ready()
    {
        base._Ready();
        AnimationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    
    public void TakeDamage(float damage)
    {
        Hitpoints -= damage;
        if (Hitpoints <= 0) 
            Death();
    }
    
    private void Death()
    {
        QueueFree();
    }
}