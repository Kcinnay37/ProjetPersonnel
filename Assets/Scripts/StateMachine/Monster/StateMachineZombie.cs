using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineZombie : StateMachine
{
    [SerializeField] EnumMonster m_Data;

    public override void AddInitialsStatesAndData()
    {
        AddCurrDataStorage(EnumStatesMonster.stat);

        AddCurrState(EnumStatesMonster.brain);
        AddCurrState(EnumStatesMonster.movement);
    }

    public override void InitAllStatesAndData()
    {
        AddNewDataStorage(EnumStatesMonster.stat, new DataStorageZombieStat(this));

        AddNewState(EnumStatesMonster.brain, new StateZombieBrain(this));
        AddNewState(EnumStatesMonster.movement, new StateZombieMovement(this));
        AddNewState(EnumStatesMonster.patrol, new StateZombiePatrol(this));
        AddNewState(EnumStatesMonster.attack, new StateZombieAttack(this));
        AddNewState(EnumStatesMonster.dead, new StateZombieDead(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
