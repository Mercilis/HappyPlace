using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    //at load add the interractable monobehavior to the object?

    public string AssetBundleName { get; set; }
    public PlaceableObject.eObjectState eObjectState { get; set; }
    //is this even needed?
    public Vector3 TransformLocation { get; set; }
    public Quaternion TransformRotation { get; set; }
}
