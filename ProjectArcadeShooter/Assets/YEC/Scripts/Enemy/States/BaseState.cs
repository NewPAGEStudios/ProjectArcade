using UnityEngine;

public abstract class BaseState{
    public Enemy enemy;
    public StateMachine stateMachine;
    public abstract void Enter();
    public abstract void Perform();
    public abstract void ResetAttack();
    public abstract void Exit();

}