using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataTool : DataResource
{
    public float distance;
    public int damage;
    public float intervalAttack;

    public EnumTools instanceType;
    public EnumTools dataType;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }
}
