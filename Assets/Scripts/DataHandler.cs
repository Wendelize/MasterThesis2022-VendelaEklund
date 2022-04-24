using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Result { Sizes, NrOfRooms, DT}
public class DataCollector : MonoBehaviour
{
    string totResult;
    string[] sizeResult = new string[8];
    string[] nrOfRoomResult = new string[20];
    string[] runtimeResult = new string[20];

    public static void AddResultToString(Result result, int a, Time time)
    {
        var temp = Time.realtimeSinceStartup;
        //MyExpensiveFunction();
        print("Time for MyExpensiveFunction: " + (Time.time - temp).ToString("f6"));
    }

    void SizesQ()
    {
        // 1.5 X 1.5
        Vector3 p0 = new Vector3(-0.75f, 0, -0.75f),       //  p2_____p1
                p1 = new Vector3(0.75f, 0, -0.75f),        //   |      |
                p2 = new Vector3(0.75f, 0, 0.75f),         //   |      |
                p3 = new Vector3(-0.75f, 0, 0.75f);        //  p3_____p0

        // 1.5 X 2
        p0 = new Vector3(-0.75f, 0, -1f);
        p1 = new Vector3(0.75f, 0, -1f);
        p2 = new Vector3(0.75f, 0, 1f);
        p3 = new Vector3(-0.75f, 0, 1f);

        // 2 x 2
        p0 = new Vector3(-1f, 0, -1f);
        p1 = new Vector3(1f, 0, -1f);
        p2 = new Vector3(1f, 0, 1f);
        p3 = new Vector3(-1f, 0, 1f);

        // 2 X 3
        p0 = new Vector3(-1f, 0, -1.5f);
        p1 = new Vector3(1f, 0, -1.5f);
        p2 = new Vector3(1f, 0, 1.5f);
        p3 = new Vector3(-1f, 0, 1.5f);

        // 2.5 X 2.5
        p0 = new Vector3(-1.25f, 0, -1.25f);
        p1 = new Vector3(1.25f, 0, -1.25f);
        p2 = new Vector3(1.25f, 0, 1.25f);
        p3 = new Vector3(-1.25f, 0, 1.25f);

        // 3 X 3
        p0 = new Vector3(-1.5f, 0, -1.5f);
        p1 = new Vector3(1.5f, 0, -1.5f);
        p2 = new Vector3(1.5f, 0, 1.5f);
        p3 = new Vector3(-1.5f, 0, 1.5f);

        // 3 X 4
        p0 = new Vector3(-1.5f, 0, -2f);
        p1 = new Vector3(1.5f, 0, -2f);
        p2 = new Vector3(1.5f, 0, 2f);
        p3 = new Vector3(-1.5f, 0, 2f);

        // 4 X 4
        p0 = new Vector3(-2f, 0, -2.5f);
        p1 = new Vector3(2f, 0, -2.5f);
        p2 = new Vector3(2f, 0, 2.5f);
        p3 = new Vector3(-2f, 0, 2.5f);


    }

    void SizesPerformance()
    {
        // 2 X 2
        Vector3 p0 = new Vector3(-1f, 0, -1f),       //  p2_____p1
                p1 = new Vector3(1f, 0, -1f),        //   |      |
                p2 = new Vector3(1f, 0, 1f),         //   |      |
                p3 = new Vector3(-1f, 0, 1f);        //  p3_____p0

        // 4 X 4
        p0 = new Vector3(-0.75f, 0, -1f);
        p1 = new Vector3(0.75f, 0, -1f);
        p2 = new Vector3(0.75f, 0, 1f);
        p3 = new Vector3(-0.75f, 0, 1f);

        // 6 x 6
        p0 = new Vector3(-1f, 0, -1f);
        p1 = new Vector3(1f, 0, -1f);
        p2 = new Vector3(1f, 0, 1f);
        p3 = new Vector3(-1f, 0, 1f);

        // 8 X 8
        p0 = new Vector3(-1f, 0, -1.5f);
        p1 = new Vector3(1f, 0, -1.5f);
        p2 = new Vector3(1f, 0, 1.5f);
        p3 = new Vector3(-1f, 0, 1.5f);

        // 10 X 10
        p0 = new Vector3(-1.25f, 0, -1.25f);
        p1 = new Vector3(1.25f, 0, -1.25f);
        p2 = new Vector3(1.25f, 0, 1.25f);
        p3 = new Vector3(-1.25f, 0, 1.25f);

        // 3 X 3
        p0 = new Vector3(-1.5f, 0, -1.5f);
        p1 = new Vector3(1.5f, 0, -1.5f);
        p2 = new Vector3(1.5f, 0, 1.5f);
        p3 = new Vector3(-1.5f, 0, 1.5f);

        // 3 X 4
        p0 = new Vector3(-1.5f, 0, -2f);
        p1 = new Vector3(1.5f, 0, -2f);
        p2 = new Vector3(1.5f, 0, 2f);
        p3 = new Vector3(-1.5f, 0, 2f);

        // 4 X 4
        p0 = new Vector3(-2f, 0, -2.5f);
        p1 = new Vector3(2f, 0, -2.5f);
        p2 = new Vector3(2f, 0, 2.5f);
        p3 = new Vector3(-2f, 0, 2.5f);


    }






    static Vector3[] custom2 = { p0, p1, p2, p3 };

}
