using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    GameObject wall;
    public List<int> tilesSorted, tiles;
    public Side side;
    public Vector3[] corners = new Vector3[8];
    public bool built = false;
    public Material material;

    public bool hasPortal;

    public Vector3[] wallWidth;
    public Vector3[] wallDepth;
    public Vector3 start;
    public Vector3 unitH, unitW;
    public Vector3 pos;
    public float depth = 0.08f;
    public float height = 1f;

    public void AddTileID(int tileID1, int tileID2)
    {
        tilesSorted = new List<int>();
        tiles = new List<int>();
        tiles.Add(tileID1);
        tiles.Add(tileID2);
        tilesSorted.Add(tileID1);
        tilesSorted.Add(tileID2);
        tilesSorted.Sort();
    }

    public void AddCorners(Vector3 a)
    {
        corners[0] = a;
    }

    public void CalcWallCorners(Vector3 uW, Vector3 uH, float[] tileSize)
    {
        unitW = uW;
        unitH = uH;

        start = corners[0];

        wallWidth = new Vector3[2];
        wallDepth = new Vector3[2];

        wallWidth[0] = uW * tileSize[0];
        wallWidth[1] = uH * tileSize[1];

        wallDepth[0] = depth * uW;
        wallDepth[1] = depth * uH;

        Vector3 h = new Vector3(0f, height, 0f);

        switch (side)
        {
            case Side.North:
                corners[0] = start + (wallDepth[1] / 2);
                corners[1] = start + (wallDepth[1] / 2) + h;
                corners[2] = start + (wallDepth[1] / 2) + h - wallWidth[0];
                corners[3] = start + (wallDepth[1] / 2) - wallWidth[0];
                corners[4] = start - (wallDepth[1] / 2);
                corners[5] = start - (wallDepth[1] / 2) + h;
                corners[6] = start - (wallDepth[1] / 2) + h - wallWidth[0];
                corners[7] = start - (wallDepth[1] / 2) - wallWidth[0];
                break;

            case Side.East:
                corners[0] = start - (wallDepth[0] / 2);
                corners[1] = start - (wallDepth[0] / 2) + h;
                corners[2] = start - (wallDepth[0] / 2) + h - wallWidth[1];
                corners[3] = start - (wallDepth[0] / 2) - wallWidth[1];
                corners[4] = start + (wallDepth[0] / 2);
                corners[5] = start + (wallDepth[0] / 2) + h;
                corners[6] = start + (wallDepth[0] / 2) + h - wallWidth[1];
                corners[7] = start + (wallDepth[0] / 2) - wallWidth[1];
                break;

            case Side.South:
                corners[0] = start - (wallDepth[1] / 2);
                corners[1] = start - (wallDepth[1] / 2) + h;
                corners[2] = start - (wallDepth[1] / 2) + h + wallWidth[0];
                corners[3] = start - (wallDepth[1] / 2) + wallWidth[0];
                corners[4] = start + (wallDepth[1] / 2);
                corners[5] = start + (wallDepth[1] / 2) + h;
                corners[6] = start + (wallDepth[1] / 2) + h + wallWidth[0];
                corners[7] = start + (wallDepth[1] / 2) + wallWidth[0];
                break;

            case Side.West:
                corners[0] = start + (wallDepth[0] / 2);
                corners[1] = start + (wallDepth[0] / 2) + h;
                corners[2] = start + (wallDepth[0] / 2) + h + wallWidth[1];
                corners[3] = start + (wallDepth[0] / 2) + wallWidth[1];
                corners[4] = start - (wallDepth[0] / 2);
                corners[5] = start - (wallDepth[0] / 2) + h;
                corners[6] = start - (wallDepth[0] / 2) + h + wallWidth[1];
                corners[7] = start - (wallDepth[0] / 2) + wallWidth[1];
                break;
        }

        pos = Utilities.CalcTransformPos(corners);
    }

    public void Build(GameObject parent, Material mat)
    {
        material = mat;
        // WOULD LOVE IF WE COULD ROTATE WALL N AND E INWARDS TO TILE
        wall = Utilities.BuildTileWall(corners, material, parent);
        built = true;
    }

    public void WallIsPortal()
    {
        hasPortal = true;
        SetVisable(false);
    }

    public void SetVisable(bool b)
    {
        wall.SetActive(b);
    }

    public void InitWall(ref Wall w, ref Tile t, int neighbourID, Vector3 uW, Vector3 uH, Side s)
    {
        float[] wh;
        switch (s)
        {
            case Side.North:
                w.side = s;
                w.AddTileID(t.tileID, neighbourID);
                w.AddCorners(t.corners[2]);
                wh = new float[2];
                wh[0] = t.width;
                wh[1] = t.height;
                w.CalcWallCorners(uW, uH, wh);
                break;

            case Side.East:
                w.side = s;
                w.AddTileID(t.tileID, neighbourID);
                w.AddCorners(t.corners[2]);
                wh = new float[2];
                wh[0] = t.width;
                wh[1] = t.height;
                w.CalcWallCorners(uW, uH, wh);
                break;

            case Side.South:
                w.side = s;
                w.AddTileID(t.tileID, neighbourID);
                w.AddCorners(t.corners[0]);
                wh = new float[2];
                wh[0] = t.width;
                wh[1] = t.height;
                w.CalcWallCorners(uW, uH, wh);
                break;

            case Side.West:
                w.side = s;
                w.AddTileID(t.tileID, neighbourID);
                w.AddCorners(t.corners[0]);
                wh = new float[2];
                wh[0] = t.width;
                wh[1] = t.height;
                w.CalcWallCorners(uW, uH, wh);
                break;
        }
    }
}
