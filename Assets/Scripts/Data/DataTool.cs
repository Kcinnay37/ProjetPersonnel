using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataTool : DataResource
{
    public enum AttackType
    {
        Swipe,
        Stab
    }

    public float distance;
    public int damage;
    public float intervalAttack;
    public float coneRadius;

    public EnumTools instanceType;
    public EnumTools dataType;
    public AttackType attackType;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }
}
