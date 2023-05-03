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
    public string description;

    public EnumAudios collectSound;
    public EnumAudios dropSound;

    public virtual object GetInstanceType() { return null; }
    public virtual object GetDataType() { return null; }

    public virtual string GetText() { return "Desription: " + description + "\n\n" + "Stat:\n"; }
}
