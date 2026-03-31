using Godot;

namespace tinySwords.scripts;

public partial class Monk: CharacterBody2D, IDamagable
{
    private readonly Health _health = new Health();
    private AnimatedSprite2D _animationSprite;


    public override void _Ready()
    {
        base._Ready();
        _animationSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _health.OnDeath += async () =>
        {
            PlayAnimation("dead", _animationSprite);
            await ToSignal(_animationSprite, AnimatedSprite2D.SignalName.AnimationFinished);
            QueueFree();
        };
    }
    
    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage);
    }
    
    public void PlayAnimation(string animation, AnimatedSprite2D sprite)
    {
        if(sprite.Animation != animation || sprite.Animation == "attack") 
            sprite.Play(animation);
    }
}