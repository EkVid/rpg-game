using System;
using Godot;
using GodotTask;

namespace tinySwords.scripts;

public class HealState: IState
{
    private readonly CharacterBody2D _characterBody2D;
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly Area2D _hitboxArea;
    private readonly float _healAmount;
    private readonly float _healDelay;
    private readonly Action _onHealFinished;

    public HealState(CharacterBody2D characterBody2D, AnimatedSprite2D animatedSprite2D, Area2D hitboxArea, 
        float healAmount, float healDelay, Action onHealFinished)
    {
        _characterBody2D = characterBody2D;
        _animatedSprite2D = animatedSprite2D;
        _hitboxArea = hitboxArea;
        _healAmount = healAmount;
        _healDelay = healDelay;
        _onHealFinished = onHealFinished;
    }

    public void Enter()
    {
        HandleHealAsync().Forget();
    }
    
    private async GDTask HandleHealAsync()
    {
        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
        _animatedSprite2D.Play("attack");
        await GDTask.Delay(TimeSpan.FromSeconds(_healDelay));
        await GDTask.WaitForPhysicsProcess();

        foreach (var node in _hitboxArea.GetOverlappingBodies())
        {
            if (node is Enemy enemy)
                enemy.Heal(_healAmount);
            
        }
    }

    public void Update(double delta) { }

    public void Exit()
    {
        _animatedSprite2D.AnimationFinished -= OnAnimationFinished;
    }
    
    private void OnAnimationFinished()
    {
        _animatedSprite2D.AnimationFinished -= OnAnimationFinished;
        _onHealFinished?.Invoke();
    }
}