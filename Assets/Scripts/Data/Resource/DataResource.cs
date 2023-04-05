using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataResource : ScriptableObject
{
    public Sprite image;
    public EnumStatesPlayer state;
    public int maxStack;
    public Vector2 offsetCheckInGround;
    public Vector3 sizeInWorld;

    public virtual object GetInstanceType() { return null; }
    public virtual object GetDataType() { return null; }
}
