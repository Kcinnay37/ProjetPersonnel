using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataConsumable : DataResource
{
    public EnumConsumables instanceType;
    public EnumConsumables dataType;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }
}
