using Godot;

namespace tinySwords.scripts;

public class RunState: IState
{
    
    private readonly CharacterBody2D _characterBody2D;
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly float _speed;
    private readonly Node2D _hitbox;
    
    public RunState(CharacterBody2D character, AnimatedSprite2D sprite, float speed, Node2D hitbox)
    {
        _characterBody2D = character;
        _animatedSprite2D = sprite;
        _speed = speed;
        _hitbox = hitbox;
    }
    
    public void Enter()
    {
        _animatedSprite2D.Play("run");
    }

    public void Update(double delta)
    {
        float x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        float y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
        Vector2 direction = new Vector2(x,y);
        
        if (direction != Vector2.Zero)
        {
            _characterBody2D.Velocity = direction.Normalized() * _speed;
        }
        else
        {
            _characterBody2D.Velocity = Vector2.Zero;
        }
        
        if (x != 0)
        {
            _hitbox.Scale = new Vector2(x * Mathf.Abs(_hitbox.Scale.X), _hitbox.Scale.Y);
            _animatedSprite2D.FlipH = x < 0;
        }

        _characterBody2D.MoveAndSlide();
    }

    public void Exit() { }
}