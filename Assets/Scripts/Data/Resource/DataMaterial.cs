using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataMaterial : DataResource
{
    public EnumMaterial instanceType;
    public EnumMaterial dataType;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }
}
