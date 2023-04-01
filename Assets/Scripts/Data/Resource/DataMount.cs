using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataMount : DataResource
{
    public EnumMount instanceType;
    public EnumMount dataType;

    public float mountSpeed;
    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }
}
