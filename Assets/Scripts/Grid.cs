using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Region { A , B , none , activeRegion, unactiveRegion}
public enum Side { North , East , South , West }

public class Grid : MonoBehaviour
{
    // GRID DATA
    public Vector3[] playareaPoints = new Vector3[4];
    public List<Wall> walls = new List<Wall>();
    public Tile[] tiles;
    public float width, height;
    public int rows, columns;
    public Vector3 unitW, unitH;
    public bool sizeCheck;
    Vector3 gridOrigo;

    // MISC DATA
    public Shader shader;
    public GameObject parent;
    public Material floorMat;
    public Material wallMat;

    public void GenerateGrid(ref Vector3[] playarea)
    {
        // PLAYAREA POINTS
        string bug = "========== PLAYAREA POINTS =============\n";
        for (int i = 0; i < playarea.Length; i++){
            playareaPoints[i] = playarea[i];
            bug+="Point " + i + " : " + playarea[i].ToString()+ "\n";
        }
        Debug.Log(bug);

        // ORIGO
        gridOrigo = playarea[3];

        // UNIT VECTORS
        unitH = Vector3.Normalize(playareaPoints[2] - playareaPoints[3]);
        unitW = Vector3.Normalize(playareaPoints[0] - playareaPoints[3]);

        // PLAYAREA CALCULATIONS
        PlayareaSizeCheck();

        // GENERATE TILES AND GRID
        sizeCheck = (width >= 1.5) || (height >= 1.5);
        if (sizeCheck)
            CalcGridResolutionFast();
        else
            Debug.Log("_________________!PLAYAREA IS TOO SMALL!_________________________\n");
    }

    public void PlayareaSizeCheck()
    {
        Vector3 absHeight = playareaPoints[0] - playareaPoints[1];
        Vector3 absWidth = playareaPoints[1] - playareaPoints[2];

        absHeight = Utilities.ABS(absHeight);
        absWidth = Utilities.ABS(absWidth);

        float h = Mathf.Sqrt((absHeight.x * absHeight.x) + (absHeight.z * absHeight.z));
        float w = Mathf.Sqrt((absWidth.x * absWidth.x) + (absWidth.z * absWidth.z));
        width = Mathf.Round(w * 100f)/100f;
        height = Mathf.Round(h * 100f) / 100f;
    }

    void CalcGridResolutionFast()
    {
        float w, h;
        float minimalTileSize = 0.5f;// 4f;
        w = width / minimalTileSize;
        h = height / minimalTileSize;
        columns = (int)w;
        rows = (int)h;

        float tileWidth = minimalTileSize + ((width% minimalTileSize) / columns);
        float tileHeight = minimalTileSize + ((height% minimalTileSize) / rows);

        InitializeTiles(tileWidth, tileHeight);
    }

    void InitializeTiles(float tw, float th)
    {
        int index = 0;
        int nrOfTiles = rows * columns;
        tiles = new Tile[nrOfTiles];
        Vector3[] tilePos = new Vector3[4];
        Vector3 uw = tw * unitW;
        Vector3 uh = th * unitH;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                tilePos[0] = gridOrigo + (j*uw) + (i * uh);
                tilePos[1] = gridOrigo + (j*uw) + ((i+1) * uh);
                tilePos[2] = gridOrigo + ((j+1)*uw) + ((i+1) * uh);
                tilePos[3] = gridOrigo + ((j+1)*uw) + (i * uh);
                index = columns * i + j;
                tiles[index] = new Tile();
                tiles[index].CreateTile(tilePos);
                tiles[index].width = tw;
                tiles[index].height = th;
                tiles[index].tileID = index;
            }
        }
        AddTileNeighbours();
    }

    void AddTileNeighbours()
    {
        int amountOfNeighbours = 4;
        int A, B, C, D, index;
        //      A
        //   D  i  B
        //      C

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                index = columns * i + j;

                amountOfNeighbours = 4;
                if (j == 0 || j == columns-1)
                {
                    amountOfNeighbours--;
                }
                if (i == 0 || i == rows-1)
                {
                    amountOfNeighbours--;
                }

                A = index + columns;
                B = index + 1;
                C = index - columns;
                D = index - 1;

                // THIS IS DISGUSTING... DONT H8 T8
                if(i==0 && j==0)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = A;
                    tiles[index].neighboursID[1] = B;
                }
                else if(i==0 && j==columns-1)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = A;
                    tiles[index].neighboursID[1] = D;
                }
                else if(i==rows-1 && j==0)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = B;
                    tiles[index].neighboursID[1] = C;
                }
                else if(i==rows-1 && j==columns-1)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = C;
                    tiles[index].neighboursID[1] = D;
                }
                else if (j == 0)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = A;
                    tiles[index].neighboursID[1] = B;
                    tiles[index].neighboursID[2] = C;
                }
                else if (j == columns-1)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = A;
                    tiles[index].neighboursID[1] = C;
                    tiles[index].neighboursID[2] = D;
                }
                else if (i == 0)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = A;
                    tiles[index].neighboursID[1] = B;
                    tiles[index].neighboursID[2] = D;
                }
                else if (i == rows-1)
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = B;
                    tiles[index].neighboursID[1] = C;
                    tiles[index].neighboursID[2] = D;
                }
                else
                {
                    tiles[index].InitNeighbours(amountOfNeighbours);
                    tiles[index].neighboursID[0] = A;
                    tiles[index].neighboursID[1] = B;
                    tiles[index].neighboursID[2] = C;
                    tiles[index].neighboursID[3] = D;
                }
            }
        }
    }

    void RoomWall(ref List<Wall> mwL, ref Wall w)
    {
        bool addThisWall = true;
        for (int i = 0; i < mwL.Count && addThisWall; i++)
        {
            if (w.tilesSorted[0] == mwL[i].tilesSorted[0] && w.tilesSorted[1] == mwL[i].tilesSorted[1])
                addThisWall = false;
        }

        if (addThisWall)
            mwL.Add(w);
    }

    public void MazeCreation(ref int[] region, ref Tile[] grid, bool visualize)
    {        
        // 1. Collect all cells into single region(ref Tile[] region)
        for (int i = 0; i < region.Length; i++)
            grid[region[i]].region = Region.activeRegion;

        // 2. Split region in two new ones
        List<int> regionA = new List<int>();
        List<int> regionB = new List<int>();

        // 2.1. Choose two random cells as seeds for new region.
        int startTileA = Random.Range(0, region.Length ),
             startTileB = Random.Range(0, region.Length );

        while (startTileA == startTileB)
            startTileB = Random.Range(0, region.Length );

        regionA.Add(region[startTileA]);
        regionB.Add(region[startTileB]);
        grid[regionA[0]].region = Region.A;
        grid[regionB[0]].region = Region.B;

        // Set S
        List<Tile> S = new List<Tile>();
        S.Add(grid[regionA[0]]);
        S.Add(grid[regionB[0]]);

        // Walls that devide the regions l8er
        List<Wall> wallsToBuild = new List<Wall>();

        int count = 0;
        while (S.Count > 0)
        {
            // 2.2. Choose random from S, remove element from S
            int seedS = Random.Range(0, S.Count );
            Tile tempS = S[seedS];
            S.RemoveAt(seedS);

            // 2.3 if the neighbor is not already associated with a subregion,
            // add it to S, and respective subregion
            int nrOfNeighbours = tempS.neighboursID.Length;

            for (int i = 0; i < nrOfNeighbours; i++)
            {
                int neighbourID = tempS.neighboursID[i];
                if (grid[neighbourID].region == Region.activeRegion)
                {
                    grid[neighbourID].region = tempS.region;
                    S.Add(grid[neighbourID]);

                    if (grid[neighbourID].region == Region.A)
                        regionA.Add(neighbourID);
                    else if (grid[neighbourID].region == Region.B)
                        regionB.Add(neighbourID);
                }
                   
                // ADD WALLS TO LISTS
                if (grid[neighbourID].region != tempS.region && grid[neighbourID].region != Region.unactiveRegion)
                {
                    Wall w = new Wall();
                    Side s = Side.North;

                    if (grid[neighbourID].tileID == (tempS.tileID + 1))
                        s = Side.East;
                    else if (grid[neighbourID].tileID == (tempS.tileID - columns))
                        s = Side.South;
                    else if (grid[neighbourID].tileID == (tempS.tileID - 1))
                        s = Side.West;

                    w.InitWall(ref w, ref tempS, grid[neighbourID].tileID, unitW, unitH, s);
                    RoomWall(ref wallsToBuild, ref w);
                }
            }
            count += 1;
        }

        // VISUALIZE
        if(visualize)
            for (int i = 0; i < region.Length; i++)
                Utilities.BuildTile(grid[region[i]].corners, floorMat, parent, region[i]);

        while (wallsToBuild.Count > 1)
        {
            int rand = Random.Range(0, wallsToBuild.Count);
            Wall w = wallsToBuild[rand];
            wallsToBuild.RemoveAt(rand);
            w.Build(parent, wallMat);

            walls.Add(w);
        }

        int[] a = regionA.ToArray();
        int[] b = regionB.ToArray();

        for (int i = 0; i < grid.Length; i++)
            grid[i].region = Region.unactiveRegion;

        if (a.Length > Mathf.Min(rows, columns))
            MazeCreation(ref a, ref grid, false);

        for (int i = 0; i < grid.Length; i++)
            grid[i].region = Region.unactiveRegion;

        if (b.Length > Mathf.Min(rows, columns))
            MazeCreation(ref b, ref grid, false);
    }
}