using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkComponent : MonoBehaviour
{
    public GameObject level;
    public List<PortalComponent> portals = new List<PortalComponent>();
    public int activePortal;
    public Shader shader;
    public Grid grid;
    public Material floorMat;
    public Material wallMat;
    public Material tileWallMat;
    List<GameObject> portalGO = new List<GameObject>();

    public void GenerateLevel(ref Vector3[] playArea)
    {
        level = new GameObject("Level");
        level.transform.parent = this.transform;

        // CREATE GRID
        InitGrid(ref playArea);

        // CREATE MAZE
        int[] reg = new int[grid.tiles.Length];
        for (int i = 0; i < grid.tiles.Length; i++)
            reg[i] = i;
        grid.MazeCreation(ref reg, ref grid.tiles,true);

        // CREATE WALLS AROUND PLAYAREA
        Utilities.BuildWalls(grid.playareaPoints, wallMat, 5, level);
    }

    public void SetLevelLayer(int layerID, bool setPortalLayer)
    {
        level.layer = layerID;

        foreach (Transform child in level.transform)
            child.gameObject.layer = layerID;
        
        if(setPortalLayer)
            foreach (PortalComponent p in portals)
                p.portal.gameObject.layer = layerID;
        else
            foreach (PortalComponent p in portals)
                p.portal.gameObject.layer = 0;
    }

    public PortalComponent GetActivePortal(Vector3 playerPos)
    {
        activePortal = 0;
        float distance = portals[0].OffsetToPortal(playerPos);

        if (portals.Count > 1)
        {
            float d = portals[1].OffsetToPortal(playerPos);
            if (d < distance)
                activePortal = 1;
        }

        return portals[activePortal];
    }

    void InitGrid(ref Vector3[] playArea)
    {
        grid = new Grid();
        grid.shader = shader;
        grid.floorMat = floorMat;
        grid.wallMat = tileWallMat;
        grid.parent = level;
        grid.GenerateGrid(ref playArea);
    }

    public void AvoidRender()
    {
        // HEY MIGHT NEED TO FIX DIZ BITCH
        for(int i = 0; i < grid.walls.Count; i++)
        {
            // Vector mellan wall och portal
            Vector3 pos = grid.walls[i].pos - portals[0].pos;
            //Vector3 v = portals[0].pos + portals[0].forward;
            pos.y = 0;
            // Dot mellan pos och forward på portal
            float val = Vector3.Dot(pos, portals[0].forward);
            if (val <= 0f /*&& !grid.walls[i].hasPortal*/)
                grid.walls[i].SetVisable(false);
        }
    }

    public void RenderAll()
    {
        for (int i = 0; i < grid.walls.Count; i++)
        {
            if(!grid.walls[i].hasPortal)
                grid.walls[i].SetVisable(true);
        }
    }

    public int OtherChunk()
    {
        if (activePortal == 0)
            return -1;
        else
            return +1;
    }

    public void DeleteWallsToCreatePortal(int tileId)
    {
        for(int i = 0; i < grid.walls.Count; i++)
        {
            Wall w = grid.walls[i];
            if(w.tilesSorted[0] == tileId || w.tilesSorted[1] == tileId)
            {
                grid.walls.RemoveAt(i);
            }
        }
    }

    public void InactivateChunk()
    {
        level.SetActive(false);

        //foreach (PortalComponent p in portals)
        //    p.portal.gameObject.SetActive(false);
    }

    public void ActivateChunk()
    {
        //foreach (Transform child in level.transform)
        //  child.gameObject.SetActive(true);

        level.SetActive(true);

        foreach (PortalComponent p in portals)
            p.portal.gameObject.SetActive(true);

        
    }

    public void AddPortal(ref Wall w)
    {
        GameObject portal = new GameObject("Portal");
        PortalComponent pc = portal.AddComponent(typeof(PortalComponent)) as PortalComponent;
        pc.transform.parent = level.transform;
        pc.CreatePortal(ref w, shader, portal);

        portals.Add(pc);
        portalGO.Add(portal);
    }
}
