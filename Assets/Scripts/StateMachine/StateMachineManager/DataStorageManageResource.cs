using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorageManageResource : DataStorage
{
    public DataStorageManageResource(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public void InstanciateResourceInWorldAt(object dataResource, Vector3 pos, Vector2 velo, int dir)
    {
        DataResource data = (DataResource)Pool.m_Instance.GetData(dataResource);

        GameObject objectResource = Pool.m_Instance.GetObject(data.GetInstanceType());
        objectResource.GetComponent<ResourceInWorld>().InitResource(false, velo, dataResource, data.GetInstanceType());
        objectResource.transform.rotation = Quaternion.identity;
        objectResource.transform.position = pos;

        Vector3 scale = objectResource.transform.localScale;
        if ((dir < 0 && scale.x > 0) || (dir > 0 && scale.x < 0))
        {
            scale.x = -scale.x;
        }
        objectResource.transform.localScale = scale;

        objectResource.SetActive(true);
    }
}
