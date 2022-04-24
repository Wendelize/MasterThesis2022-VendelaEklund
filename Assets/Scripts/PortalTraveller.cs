using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public GameObject graphicsObject;
    public Vector3? previousOffsetFromPortal { get; set; }

    //=======================
    // When we "teleport":
    // * Change active world (WorldHandler)
    // * 
    //=======================
    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
}
