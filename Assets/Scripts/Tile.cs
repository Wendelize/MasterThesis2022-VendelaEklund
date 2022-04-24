using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3[] corners = new Vector3[4];
    public float width, height;
    public int[] neighboursID;
    public Region region = Region.none;
    public int tileID;

    public void CreateTile(Vector3[] startPos)
    {
        corners[0] = startPos[0];
        corners[1] = startPos[1];
        corners[2] = startPos[2];
        corners[3] = startPos[3];
    }

    public void InitNeighbours(int nrOf)
    {
        neighboursID = new int[nrOf];
    }
}
