using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static OVRBoundary;

public class Utilities : MonoBehaviour
{

    static Vector3 pp0 = new Vector3(2.33f, 0, -1),  //  p2_____p1
            pp1 = new Vector3(2.33f, 0, 1),  //   |      |
            pp2 = new Vector3(-2.33f, 0, 1),  //   |      |
            pp3 = new Vector3(-2.33f, 0, -1);  //  p3_____p0

    static Vector3 p0 = new Vector3(-1.6f, 0, -0.3f),  //  p2_____p1
                p1 = new Vector3(0.6f, 0, -1.1f),      //   |      |
                p2 = new Vector3(1.8f, 0, 2.1f),       //   |      |
                p3 = new Vector3(-0.4f, 0, 2.9f);      //  p3_____p0

    static Vector3[] customPlayareaPoints = { pp0, pp1, pp2, pp3 };
    static Vector3[] custom2 = { p0, p1, p2, p3 };

    public static Vector3[]? GeneratePlayareaPoints()
    {
        Vector3[] points = { };
        if (OVRManager.boundary.GetConfigured())
            points = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        else
            points = null;
        return points;
    }

    public static GameObject BuildTile(Vector3[] playareaPoints, Material material, GameObject parent = null, int tileID = -1)
    {
        // Create gameobject and add components
        GameObject floor = new GameObject("Tile" + tileID);
        MeshFilter mf = floor.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = floor.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshCollider mc = floor.AddComponent(typeof(MeshCollider)) as MeshCollider;

        // Randomize material
        Material mat = material;
        mr.material = mat;

        // Create mesh
        Mesh m = new Mesh();
        m.vertices = playareaPoints;
        m.uv = new Vector2[]
        {new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};

        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        mf.mesh = m;
        mc.sharedMesh = mf.mesh;
        m.RecalculateBounds();
        m.RecalculateNormals();

        // Set parent
        floor.transform.parent = parent.transform;

        return floor;
    }

    public static GameObject BuildWall(Vector3 point1, Vector3 point2, Material material, float wallHeight, GameObject parent)
    {
        // Create gameobject and add components
        GameObject wall = new GameObject("Wall");
        MeshFilter mf = wall.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = wall.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshCollider mc = wall.AddComponent(typeof(MeshCollider)) as MeshCollider;

        // Randomize material
        Material mat = material;
        Tiling(ref mat, point1, point2, wallHeight);
        mr.material = mat;

        // Create mesh
        Mesh m = new Mesh();
        m.vertices = new Vector3[]
        {
            point1,
            new Vector3(point1.x, point1.y+wallHeight, point1.z),
            new Vector3(point2.x, point2.y+wallHeight, point2.z),
            point2
        };
        m.uv = new Vector2[]
        {new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};

        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        mf.mesh = m;
        mc.sharedMesh = mf.mesh;
        m.RecalculateBounds();
        m.RecalculateNormals();

        // Set parent
        wall.transform.parent = parent.transform;

        return wall;
    }

    public static List<GameObject> BuildWalls(Vector3[] playareaPoints, Material material, float wallHeight = 5, GameObject parent = null)
    {
        List<GameObject> walls = new List<GameObject>();
        walls.Add(BuildWall(playareaPoints[3], playareaPoints[2], material, wallHeight, parent));
        walls.Add(BuildWall(playareaPoints[2], playareaPoints[1], material, wallHeight, parent));
        walls.Add(BuildWall(playareaPoints[1], playareaPoints[0], material, wallHeight, parent));
        walls.Add(BuildWall(playareaPoints[0], playareaPoints[3], material, wallHeight, parent));

        return walls;
    }

    public static GameObject BuildTileWall(Vector3[] points, Material material, GameObject parent = null)
    {
        // Create gameobject and add components
        GameObject wall = new GameObject("TileWall");
        MeshFilter mf = wall.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = wall.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Wall w = wall.AddComponent(typeof(Wall)) as Wall;

        // Randomize material
        Material mat = material;
        //WidthHeightTiling(points, mat);
        mr.material = mat;

        // Create mesh
        Mesh m = new Mesh();
        m.vertices = points;
        m.uv = new Vector2[]
        {new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),
         new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)
        };

        m.triangles = new int[] { 0, 1, 2, 0, 2, 3,     // FRONT
                                  1, 5, 6, 1, 6, 2,     // TOP
                                  5, 4, 7, 5, 7, 6,     // BACK
                                  4, 0, 3, 4, 3, 7,     // BOT
                                  3, 2, 6, 3, 6, 7,     // RIGHT
                                  4, 5, 1, 4, 1, 0};    // LEFT
        mf.mesh = m;
        m.RecalculateBounds();
        m.RecalculateNormals();

        // Set parent
        wall.transform.parent = parent.transform;

        return wall;
    }

    public static GameObject BuildPortalWall(Vector3[] points, Material material, ref MeshRenderer mr, GameObject parent = null)
    {
        // Create gameobject and add components
        GameObject portal = new GameObject("PortalScreen");
        MeshFilter mf = portal.AddComponent(typeof(MeshFilter)) as MeshFilter;
        mr = portal.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        // Randomize material
        // FIX PLZ
        Material mat = material;
        //WidthHeightTiling(points, mat);
        mr.material = mat;

        // Create mesh
        Mesh m = new Mesh();
        m.vertices = points;
        m.uv = new Vector2[]
        {new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),
         new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)
        };

        m.triangles = new int[] { 0, 1, 2, 0, 2, 3,     // FRONT
                                  1, 5, 6, 1, 6, 2,     // TOP
                                  5, 4, 7, 5, 7, 6,     // BACK
                                  4, 0, 3, 4, 3, 7,     // BOT
                                  3, 2, 6, 3, 6, 7,     // RIGHT
                                  4, 5, 1, 4, 1, 0};    // LEFT
        mf.mesh = m;
        m.RecalculateBounds();
        m.RecalculateNormals();

        // Set parent
        portal.transform.parent = parent.transform;

        return portal;
    }

    public static List<GameObject> BuildPortalFrame()
    {
        List<GameObject> pillars = new List<GameObject>();
        return pillars;
    }

    public static GameObject BuildPillar(Vector3[] points, Material material, float thickness, GameObject parent = null)
    {
        GameObject pillar = new GameObject("PortalFrame");
        return pillar;
    }

    public static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    public static Vector3[] GetCustomPlayArea()
    {
        return custom2;
    }

    public static Vector3 ABS(Vector3 v)
    {
        Vector3 absV = new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        return absV;
    }

    static void Tiling(ref Material m, Vector3 a, Vector3 b, float wallHeight)
    {
        Vector3 absWidth = ABS(a - b);
        float w = Mathf.Sqrt((absWidth.x * absWidth.x) + (absWidth.z * absWidth.z));
        m.mainTextureScale = new Vector2(w, wallHeight);
    }

    public static void WidthHeightTiling(Vector3[] points, Material mat)
    {
        Vector3 absHeight = points[0] - points[1];
        Vector3 absWidth = points[1] - points[2];

        absHeight = ABS(absHeight);
        absWidth = ABS(absWidth);

        float h = Mathf.Sqrt((absHeight.x * absHeight.x) + (absHeight.z * absHeight.z));
        float w = Mathf.Sqrt((absWidth.x * absWidth.x) + (absWidth.z * absWidth.z));
        w = Mathf.Round(w * 100f) / 100f;
        h = Mathf.Round(h * 100f) / 100f;

        mat.mainTextureScale = new Vector2(w, h);
    }

    public static float CalcDist(Vector3 a, Vector3 b)
    {
        Vector3 absWidth = ABS(a - b);
        return Mathf.Sqrt((absWidth.x * absWidth.x) + (absWidth.z * absWidth.z));
    }

    public static Vector3 CalcTransformPos(Vector3[] points)
    {
        Vector3 res = new Vector3(0,0,0);
        for(int i = 0; i < points.Length; i++)
        {
            res.x += points[i].x;
            res.y += 0;
            res.z += points[i].z;
        }
        res.x /= points.Length;
        res.y /= points.Length;
        res.z /= points.Length;
        return res;
    }

    public static Vector3[] PlayAreaSize(int theSize = -1)
    {
        switch(theSize)
        {
            case 0:
                // 1.5 X 1.5
                p0 = new Vector3(-0.75f, 0, -0.75f);
                p1 = new Vector3(0.75f, 0, -0.75f);
                p2 = new Vector3(0.75f, 0, 0.75f);
                p3 = new Vector3(-0.75f, 0, 0.75f);
                break;
            
            case 1:
                // 1.5 X 2
                p0 = new Vector3(-0.75f, 0, -1f);
                p1 = new Vector3(0.75f, 0, -1f);
                p2 = new Vector3(0.75f, 0, 1f);
                p3 = new Vector3(-0.75f, 0, 1f);
                break;

            case 2:
                // 2 x 2
                p0 = new Vector3(-1f, 0, -1f);
                p1 = new Vector3(1f, 0, -1f);
                p2 = new Vector3(1f, 0, 1f);
                p3 = new Vector3(-1f, 0, 1f);
                break;

            case 3:
                // 2 X 3
                p0 = new Vector3(-1f, 0, -1.5f);
                p1 = new Vector3(1f, 0, -1.5f);
                p2 = new Vector3(1f, 0, 1.5f);
                p3 = new Vector3(-1f, 0, 1.5f);
                break;

            case 4:
                // 2.5 X 2.5
                p0 = new Vector3(-1.25f, 0, -1.25f);
                p1 = new Vector3(1.25f, 0, -1.25f);
                p2 = new Vector3(1.25f, 0, 1.25f);
                p3 = new Vector3(-1.25f, 0, 1.25f);
                break;

            case 5:
                // 3 X 3
                p0 = new Vector3(-1.5f, 0, -1.5f);
                p1 = new Vector3(1.5f, 0, -1.5f);
                p2 = new Vector3(1.5f, 0, 1.5f);
                p3 = new Vector3(-1.5f, 0, 1.5f);
                break;

            case 6:
                // 3 X 4
                p0 = new Vector3(-1.5f, 0, -2f);
                p1 = new Vector3(1.5f, 0, -2f);
                p2 = new Vector3(1.5f, 0, 2f);
                p3 = new Vector3(-1.5f, 0, 2f);
                break;

            case 7:
                // 4 X 4
                p0 = new Vector3(-2f, 0, -2f);
                p1 = new Vector3(2f, 0, -2f);
                p2 = new Vector3(2f, 0, 2f);
                p3 = new Vector3(-2f, 0, 2f);
                break;

            case 8:
                // 4 X 5
                p0 = new Vector3(-2f, 0, -2.5f);
                p1 = new Vector3(2f, 0, -2.5f);
                p2 = new Vector3(2f, 0, 2.5f);
                p3 = new Vector3(-2f, 0, 2.5f);
                break;

            case 9:
                // 5 X 5
                p0 = new Vector3(-2.5f, 0, -2.5f);
                p1 = new Vector3(2.5f, 0, -2.5f);
                p2 = new Vector3(2.5f, 0, 2.5f);
                p3 = new Vector3(-2.5f, 0, 2.5f);
                break;

            case 10:
                // 10 X 10
                p0 = new Vector3(-5f, 0, -5f);
                p1 = new Vector3(5f, 0, -5f);
                p2 = new Vector3(5f, 0, 5f);
                p3 = new Vector3(-5f, 0, 5f);
                break;

            case 11:
                // 20 X 20
                p0 = new Vector3(-10f, 0, -10f);
                p1 = new Vector3(10f, 0, -10f);
                p2 = new Vector3(10f, 0, 10f);
                p3 = new Vector3(-10f, 0, 10f);
                break;

            case 12:
                // 30 X 30
                p0 = new Vector3(-15f, 0, -15f);
                p1 = new Vector3(15f, 0, -15f);
                p2 = new Vector3(15f, 0, 15f);
                p3 = new Vector3(-15f, 0, 15f);
                break;

            case 13:
                // 40 X 40
                p0 = new Vector3(-20f, 0, -20f);
                p1 = new Vector3(20f, 0, -20f);
                p2 = new Vector3(20f, 0, 20f);
                p3 = new Vector3(-20f, 0, 20f);
                break;
        }

        Vector3[] playArea = { p0, p1, p2, p3 };
        return playArea;

    }

    
}