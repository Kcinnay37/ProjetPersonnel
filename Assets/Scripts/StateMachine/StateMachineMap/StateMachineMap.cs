using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineMap : StateMachine
{
    public override void AddInitialsStates()
    {
        AddCurrState(EnumStatesMap.manager);
    }

    public override void InitAllStates()
    {
        AddNewStateData(EnumStatesMap.data, new StateMapData(this));
        AddNewStateData(EnumStatesMap.generate, new StateMapGenerate(this));
        AddNewStateData(EnumStatesMap.view, new StateMapView(this));

        AddNewState(EnumStatesMap.manager, new StateMapManager(this));
    }
}
