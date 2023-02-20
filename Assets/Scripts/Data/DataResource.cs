using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataResource : ScriptableObject
{
    public Sprite image;
    public EnumStatesPlayer state;
    public int maxStack;

    public virtual object GetInstanceType() { return null; }
    public virtual object GetDataType() { return null; }
}
