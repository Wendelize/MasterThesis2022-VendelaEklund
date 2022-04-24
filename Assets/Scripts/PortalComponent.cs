using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality for portal:
// * Handles portal travel
// * 
public class PortalComponent : MonoBehaviour
{
    public GameObject portal;
    public MeshRenderer screen;
    public Vector3[] corners = new Vector3[8];
    float portalWidth;
    public Vector3 pos, forward;

    // Portal Connector
    PortalConnector portalCon;

    void Start()
    {
        portalWidth = Utilities.CalcDist(corners[0], corners[2]);
        pos = Utilities.CalcTransformPos(corners);
    }

    public void CreatePortal(ref Wall w, Shader s, GameObject parent)
    {
        Vector3 h = new Vector3(0f, w.height, 0f);

        switch (w.side)
        {
            case Side.North:
                corners[0] = w.start + (w.wallDepth[1] / 2);
                corners[1] = w.start + (w.wallDepth[1] / 2) + h;
                corners[2] = w.start + (w.wallDepth[1] / 2) + h - w.wallWidth[0];
                corners[3] = w.start + (w.wallDepth[1] / 2) - w.wallWidth[0];
                corners[4] = w.start;
                corners[5] = w.start + h;
                corners[6] = w.start + h - w.wallWidth[0];
                corners[7] = w.start - w.wallWidth[0];
                forward = w.unitH;
                break;

            case Side.East:
                corners[0] = w.start - (w.wallDepth[0] / 2);
                corners[1] = w.start - (w.wallDepth[0] / 2) + h;
                corners[2] = w.start - (w.wallDepth[0] / 2) + h - w.wallWidth[1];
                corners[3] = w.start - (w.wallDepth[0] / 2) - w.wallWidth[1];
                corners[4] = w.start;
                corners[5] = w.start + h;
                corners[6] = w.start + h - w.wallWidth[1];
                corners[7] = w.start - w.wallWidth[1];
                forward = -w.unitW;
                break;

            case Side.South:
                corners[0] = w.start - (w.wallDepth[1] / 2);
                corners[1] = w.start - (w.wallDepth[1] / 2) + h;
                corners[2] = w.start - (w.wallDepth[1] / 2) + h + w.wallWidth[0];
                corners[3] = w.start - (w.wallDepth[1] / 2) + w.wallWidth[0];
                corners[4] = w.start;
                corners[5] = w.start + h;
                corners[6] = w.start + h + w.wallWidth[0];
                corners[7] = w.start + w.wallWidth[0];
                forward = -w.unitH;
                break;

            case Side.West:
                corners[0] = w.start + (w.wallDepth[0] / 2);
                corners[1] = w.start + (w.wallDepth[0] / 2) + h;
                corners[2] = w.start + (w.wallDepth[0] / 2) + h + w.wallWidth[1];
                corners[3] = w.start + (w.wallDepth[0] / 2) + w.wallWidth[1];
                corners[4] = w.start;
                corners[5] = w.start + h;
                corners[6] = w.start + h + w.wallWidth[1];
                corners[7] = w.start + w.wallWidth[1];
                forward = w.unitW;
                break;
        }
        Material m = new Material(s);
        m.color = new Vector4(1, 0, 0, 1);
        portal = Utilities.BuildPortalWall(corners, m, ref screen, parent);
    }

    public bool SideOfPortalVR(ref string str, ref GameObject player, ref OVRControllerCustom customController)
    {
        // Calc offset between portal and player position
        Vector3 offsetFromPortal = player.transform.position - pos;

        // Debugging
        string s = "";
        s += "Offset to portal : " + offsetFromPortal.x + ", "
                + offsetFromPortal.y + ", " +
                +offsetFromPortal.z + "\n";
        str += s;

        if (OffsetToPortal(player.transform.position) > (portalWidth / 2f))
            return false;

        // Solution for bug with previousPosition
        if (customController.previousOffsetFromPortal == null)
            customController.previousOffsetFromPortal = offsetFromPortal;

        // Check which side of the portal the player is on (Two options; behind / in front)
        int currentPortalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, forward));
        int oldPortalSide = System.Math.Sign(Vector3.Dot(customController.previousOffsetFromPortal.Value, forward));

        if (currentPortalSide != oldPortalSide)
        {
            // Not forward portal
            customController.previousOffsetFromPortal = offsetFromPortal;
            return true;
        }
        else
        {
            // Forward portal
            customController.previousOffsetFromPortal = offsetFromPortal;
            return false;
        }
    }

    public bool SideOfPortalPC(ref string str, ref GameObject player, ref FPSController customController)
    {
        // Calc offset between portal and player position
        Vector3 offsetFromPortal = player.transform.position - pos;

        if (OffsetToPortal(player.transform.position) > (portalWidth / 2f))
            return false;

        // Solution for bug with previousPosition
        if (customController.previousOffsetFromPortal == null)
            customController.previousOffsetFromPortal = offsetFromPortal;

        // Debugging
        string s = "";
        s += "Offset to portal : " + offsetFromPortal.x + ", "
              + offsetFromPortal.y + ", " +
              +offsetFromPortal.z + "\n";
        str += s;

        // Check which side of the portal the player is on (Two options; behind / in front)
        int currentPortalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, forward));
        int oldPortalSide = System.Math.Sign(Vector3.Dot(customController.previousOffsetFromPortal.Value, forward));

        if (currentPortalSide != oldPortalSide)
        {
            // Went through portal
            customController.previousOffsetFromPortal = offsetFromPortal;
            return true;
        }
        else
        {
            // Forward portal
            customController.previousOffsetFromPortal = offsetFromPortal;
            return false;
        }
    }

    public float OffsetToPortal(Vector3 playerPos)
    {
        Vector3 p = new Vector3(pos.x, 0, pos.z);
        float offsetToPortal = Utilities.CalcDist(p, new Vector3(playerPos.x, 0,playerPos.z));
        return offsetToPortal;
    }
}  