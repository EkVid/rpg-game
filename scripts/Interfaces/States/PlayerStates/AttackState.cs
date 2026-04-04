using System;
using System.Threading;
using Godot;
using GodotTask;
using tinySwords.scripts;

public class AttackState : IState
{
    private readonly CharacterBody2D _characterBody2D;
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly Area2D _hitboxArea;
    private readonly float _attackPower;
    private readonly float _attackDelay;
    private readonly Action _onAttackFinished;

    public AttackState(CharacterBody2D characterBody2D, AnimatedSprite2D animatedSprite2D,
        float attackDelay, Area2D hitboxArea, float attackPower, Action onAttackFinished)
    {
        _characterBody2D = characterBody2D;
        _animatedSprite2D = animatedSprite2D;
        _attackDelay = attackDelay;
        _hitboxArea = hitboxArea;
        _attackPower = attackPower;
        _onAttackFinished = onAttackFinished;
    }

    public void Enter()
    {
        HandleAttackAsync().Forget();
    }

    private async GDTask HandleAttackAsync()
    {
        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
        _animatedSprite2D.Play("attack");
        await GDTask.Delay(TimeSpan.FromSeconds(_attackDelay));
        await GDTask.WaitForPhysicsProcess();

        foreach (var node in _hitboxArea.GetOverlappingBodies())
        {
            if (node != _characterBody2D && node is IDamagable enemy)
                enemy.TakeDamage(_attackPower);
        }
    }

    public void Update(double delta) { }

    public void Exit()
    {
    }
   
    private void OnAnimationFinished()
    {
        _animatedSprite2D.AnimationFinished -= OnAnimationFinished;
        _onAttackFinished?.Invoke();
    }
}