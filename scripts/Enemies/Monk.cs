using System;
using System.Linq;
using Godot;
using GodotTask;

namespace tinySwords.scripts;

public partial class Monk: Enemy
{
    private float _healDelay = 0.5f;
    private float _amountToHeal = 10;
    
    private bool _teamInRange = false;
    private bool _teamInHealingRange = false;
    
    public IState HealState { get; private set; }
    
    
    public override void _Ready()
    {
        HealState = new HealState(this, _animationSprite,  _hitboxArea, _amountToHeal, _healDelay, 
            () => StateMachine.ChangeState(IdleState));

        ChaseBox.BodyEntered += (Node2D body) =>
        {
            if (body is Player)
                PlayerInRange = true;
            else if(body != this && body is Enemy)
                _teamInRange = true;
        };

        ChaseBox.BodyExited += (Node2D body) =>
        {
            if (body is Player)
                PlayerInRange = false;
            else if(body!= this && body is Enemy)
                _teamInRange = false;
        };

        _hitboxArea.BodyEntered += (Node2D body) =>
        {
            if (body != this && body is Enemy)
                _teamInHealingRange = true;
        };

        _hitboxArea.BodyExited += (Node2D body) =>
        {
            if (body != this && body is Enemy)
                _teamInHealingRange = false;
        }; 
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (IsDead) return;
        
        if ((Health.Hitpoints < 100 && StateMachine._currentState != AttackState) || _teamInHealingRange)
        {
            // enter heal state
            if(StateMachine._currentState != HealState) 
                StateMachine.ChangeState(HealState);
        }
        
        else if (_teamInRange)
        {
            if (ChaseBox.GetOverlappingBodies().Where(node => node != this && node is Enemy teammate).MinBy((body) =>
                    GlobalPosition.DistanceTo(body.GlobalPosition)) is Node2D allyNode2D)
            {
                // enter Run State
                Direction = GlobalPosition.DirectionTo(allyNode2D.GlobalPosition);
                if (StateMachine._currentState != RunState)
                    StateMachine.ChangeState(RunState);      
            }
        }

        else
        {
            if(StateMachine._currentState != IdleState) 
                StateMachine.ChangeState(IdleState);
        }
        
        StateMachine.Update(delta);
    }
}