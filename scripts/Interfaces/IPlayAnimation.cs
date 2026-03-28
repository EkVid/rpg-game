using Godot;

namespace tinySwords.scripts;

public interface IPlayAnimation
{
    public void UpdateAnimation(Vector2 direction, AnimatedSprite2D sprite, bool isAttacking);
    public void PlayAnimation(string animation, AnimatedSprite2D sprite);
}