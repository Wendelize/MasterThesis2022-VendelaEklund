using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Portal connection
public class PortalConnector : MonoBehaviour
{
    public PortalComponent thisPortal; 
    public PortalComponent otherPortal;

    bool checkPortalsExist()
    {
        bool portalsExist = true;
        if (thisPortal == null || otherPortal == null)
            portalsExist = false;
        return portalsExist;

    }

    // Draw gizmo
    void OnDrawGizmos()
    {
        if (thisPortal != null
        && otherPortal != null)
        {
            if (Vector3.Distance(thisPortal.transform.localPosition, otherPortal.transform.localPosition) > 0.01f)
            {
                Gizmos.color = Color.blue;
            }
            else if (Quaternion.Angle(thisPortal.transform.rotation, otherPortal.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up)) > 0.01f)
            {
                Gizmos.color = Color.yellow;
            }
            else if (Vector3.Distance(thisPortal.transform.lossyScale, otherPortal.transform.lossyScale) > 0.01f)
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(thisPortal.transform.position + Vector3.up, otherPortal.transform.position + Vector3.up);
            Gizmos.color = Color.white;
        }
    }
}
