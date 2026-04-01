using Godot;

namespace tinySwords.scripts;

public class IdleState(AnimatedSprite2D animatedSprite2D) : IState
{
    public void Enter()
    {
        animatedSprite2D.Play("idle");
    }

    public void Update(double delta) { }

    public void Exit() { }
}