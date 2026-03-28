using Godot;

namespace tinySwords.scripts;

public class Animations
{
    public void UpdateAnimation(Vector2 direction, AnimatedSprite2D sprite, bool isAttacking)
    {
        if (isAttacking)
            return;
        string anima = direction != Vector2.Zero ? "run" : "idle";
        PlayAnimation(anima, sprite);
    }

    public void PlayAnimation(string animation, AnimatedSprite2D sprite)
    {
        if(sprite.Animation != animation) 
            sprite.Play(animation);
    }
}