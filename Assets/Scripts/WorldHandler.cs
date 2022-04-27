using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldHandler : MonoBehaviour
{
    // Cunks data
    public List<ChunkComponent> chunksInWorld;
    public int nrOfChunks;
    public int activeChunk = 0;
    int nextChunk = 1;
    int oldChunk = -1;
    public List<Material> materialList = new List<Material>();
    public bool changedChunk = false;

    // Misc
    public Vector3[] playareaPoints;  
    public Shader shader;
    int activeChunks = 2;

    // Player data
    public bool PlayWithVR;
    GameObject playerParentNode;
    GameObject playerObject;
    OVRControllerCustom playerControllerVR;
    FPSController playerControllerPC;

    // Debugging
    public GameObject r_hand;
    public GameObject l_hand;
    public Text displayTextR; 
    public Text displayText;
    string textR, textL;

    // Test Data
    bool wordInitalized = false;
    int nrOfPlayAreaSizesTest = 2;
    int nrOfIterations = 3;
    string testSizeResult = "";



    void Start()
    {
        // HOW TO RUN TESTS
        // START A LOOP THAT RUNNS THROUGH ALL DIFFERENT SIZES
        for (int i = 0; i < nrOfPlayAreaSizesTest; i++)
        {
            testSizeResult += "TestSize " + i+", ";
            // FETCH PLAYAREA SIZE
            playareaPoints = Utilities.PlayAreaSize(i);

            for (int j = 0; j < nrOfIterations; j++)
            {
                // CREATE WORLD WITH TEST SIZE
                DoEverything();

                // PUT A TIMER ON GRID CREATION, MAZECREATION, PORTAL PLACEMENT
                for (int k = 0; k < nrOfChunks; k++)
                {
                    testSizeResult += chunksInWorld[i].gridAndMaze;
                    testSizeResult += chunksInWorld[i].portalPlacement;
                }

                // DELETE EVERYTHING BEFORE RUNNING AGAIN
                KillEverything();
            }

        }

        Debug.Log(testSizeResult);

    }

    public void DoEverything()
    {
        // FETCH PLAYAREA POINTS
        //if (PlayWithVR)
        //    playareaPoints = Utilities.GetCustomPlayArea();//GeneratePlayareaPoints();
        //else
        //    playareaPoints = Utilities.GetCustomPlayArea();

        // CREATE CHUNKS
        CreateWorld(nrOfChunks);

        // CREATE PROTAL
        for (int i = 0; i < nrOfChunks - 1; i++)
            PortalPlacement(i);

        // CHECK VR or PC
        VRorPC();

        // SET LAYER TO CHUNKS
        for (int i = 1; i < nrOfChunks - 1; i++)
            StartLayerAllChunks(i);
        if (nrOfChunks > 1)
            SetChunkLayers();

        wordInitalized = true;
    }

    void KillEverything()
    {
        for(int i = 0; i < nrOfChunks; i++)
        {
            GameObject temp = chunksInWorld[0].gameObject;
            chunksInWorld.RemoveAt(0);
            Destroy(temp);
        }
        wordInitalized = false;
    }

    void LateUpdate()
    {
        if (wordInitalized)
        {
            ResetTextDisplay();

            textR = "HELLO CUTIE! :3\n";
            textL = "ERHMAGHERD!!!\n";

            if (nrOfChunks > 1)
                PortalTravel();

            for (int i = 0; i < nrOfChunks; i++)
            {
                Color col = new Color();
                ChunkComponent c = chunksInWorld[i];

                if (i % 2 == 0)
                    col = Color.cyan;
                else
                    col = Color.red;

                for (int j = 0; j < c.portals.Count; j++)
                    Debug.DrawLine(c.portals[j].pos, c.portals[j].pos + c.portals[j].forward, col, Time.deltaTime);
            }
            Debug.DrawLine(playerObject.transform.position, (playerObject.transform.position + playerObject.transform.forward), Color.white, Time.deltaTime);

            DebugText();
            WriteToDisplay(textL, true);
            WriteToDisplay(textR, false);

            Debug.Log(textR);
            Debug.Log(textL);
        }
    }

    // "Teleportation" / Portal Travelling
    void PortalTravel()
    {
        int otherChunk;
        if (activeChunk == 0)
            otherChunk = nextChunk;
        else 
            otherChunk = activeChunk + chunksInWorld[activeChunk].OtherChunk();

        // Fetch active portal 
        PortalComponent activePortal = chunksInWorld[activeChunk].GetActivePortal(playerObject.transform.position);
        bool wentThroughPortal = false;

        // Check if player is in front of portal
        if (PlayWithVR)
            wentThroughPortal = activePortal.SideOfPortalVR(ref textR, ref playerObject, ref playerControllerVR);
        else
            wentThroughPortal = activePortal.SideOfPortalPC(ref textR, ref playerObject, ref playerControllerPC);

        textR += "WENT THROUGH PORTAL : " + wentThroughPortal + "\n";

        // "Teleportation" 
        if (wentThroughPortal)
        { 
            SwapChunks(ref activeChunk, ref otherChunk);
            SetChunkLayers();
            Debug.Log("WENT THROUGH PORTAL");
        }
    }

    private void ResetTextDisplay()
    {
        displayText.text = "";
        displayTextR.text = "";
    }

    void WriteToDisplay(string s, bool leftHand)
    {
        if (leftHand)
            displayText.text = s;
        else
            displayTextR.text = s;
    }

    void DebugText()
    {
        textL += "PLAYER POS : ";
        textL += playerObject.transform.position.x.ToString() + ", ";
        textL += playerObject.transform.position.y.ToString() + ", ";
        textL += playerObject.transform.position.z.ToString() + "\n";

        textL += "ACTIVE CHUNK : " + activeChunk + "\n";

    }

    void CreateWorld(int nrOfChunks)
    {
        // Here we create and add new chunks to world
        for (int i = 0; i < nrOfChunks; i++)
        {
            GameObject chunk = new GameObject("Chunk" + i);
            ChunkComponent cc = chunk.AddComponent(typeof(ChunkComponent)) as ChunkComponent;
            cc.shader = shader;
            cc.floorMat = materialList[0];
            cc.wallMat = materialList[2];
            cc.tileWallMat = materialList[6];
            cc.GenerateLevel(ref playareaPoints);
            chunk.transform.parent = this.transform;
            chunksInWorld.Add(cc);
            Debug.Log("HELLO");
        }
    }

    void PortalPlacement(int index)
    {

        var timer = Time.realtimeSinceStartup;
        bool portalPlacementFound = false;

        int nrOfWallsInChunk = chunksInWorld[index].grid.walls.Count;
        int nrOfWallsInNextChunk = chunksInWorld[index + 1].grid.walls.Count;
        string bug = "========= PORTAL PLACEMENT ==========\n";

        for (int i = 0; i < nrOfWallsInChunk && !portalPlacementFound; i++)
        {
            Wall w = chunksInWorld[index].grid.walls[i];
            if (!w.hasPortal) {
                {
                    for (int j = 0; j < nrOfWallsInNextChunk && !portalPlacementFound; j++)
                    {
                        Wall w2 = chunksInWorld[index + 1].grid.walls[j];
                        if (!w2.hasPortal)
                        {
                            if (w.tilesSorted[0] == w2.tilesSorted[0] && w.tilesSorted[1] == w2.tilesSorted[1])
                            {
                                bug += "MATCH FOUND\n";
                                if (w.tiles[0] == w2.tiles[1])
                                {
                                    bug += "AMAZING MATCH!!!\n";
                                    chunksInWorld[index].AddPortal(ref w);
                                    chunksInWorld[index + 1].AddPortal(ref w2);

                                    chunksInWorld[index].grid.walls[i].WallIsPortal();
                                    chunksInWorld[index + 1].grid.walls[j].WallIsPortal();

                                    portalPlacementFound = true;
                                }
                                else
                                {
                                    bug += "FIX AMAZING MATCH\n";
                                    Side s = Side.North;
                                    if (w.side == Side.North)
                                        s = Side.South;
                                    else if (w.side == Side.East)
                                        s = Side.West;
                                    else if (w.side == Side.West)
                                        s = Side.East;

                                    bug += "Tiles: " + i + " , " + j + "\n"; 

                                    Wall wTemp = new Wall();
                                    wTemp.InitWall(ref wTemp, ref chunksInWorld[index + 1].grid.tiles[w2.tiles[1]], w2.tiles[0], w2.unitW, w2.unitH, s);
                                    wTemp.Build(chunksInWorld[index + 1].level, chunksInWorld[index + 1].tileWallMat);
                                    wTemp.SetVisable(false);

                                    chunksInWorld[index + 1].grid.walls[j].SetVisable(false);
                                    chunksInWorld[index + 1].grid.walls.RemoveAt(j);
                                    chunksInWorld[index + 1].grid.walls.Insert(j, wTemp);

                                    chunksInWorld[index].AddPortal(ref w);
                                    chunksInWorld[index + 1].AddPortal(ref wTemp);

                                    chunksInWorld[index].grid.walls[i].WallIsPortal();
                                    chunksInWorld[index + 1].grid.walls[j].WallIsPortal();

                                    portalPlacementFound = true;
                                }
                            }
                            else
                            {
                                bug += "NO MATCH FOUND\n";
                            }
                        }
                    }
                }
            }
        }

        Debug.Log(bug);

        if (!portalPlacementFound)
        {
            Debug.Log("REGENERATE MAZE FOR CHUNK " + (index + 1) + "\n");
            // Regenerate Maze
            Destroy(chunksInWorld[index + 1].level.gameObject);
            chunksInWorld[index + 1].GenerateLevel(ref playareaPoints);
            PortalPlacement(index);
        }
        chunksInWorld[index].portalPlacement = "PORTAL CREATION: " + (Time.realtimeSinceStartup - timer).ToString("f6") + ", ";
    }

    void VRorPC()
    {
        if (PlayWithVR)
        {
            // Fetch VRHeadset
            playerParentNode = transform.Find("VRHeadset").gameObject;
            playerControllerVR = playerParentNode.transform.GetComponentInChildren<OVRControllerCustom>();
            playerObject = GameObject.Find("CenterEyeAnchor");

            // Disable PC
            GameObject go = transform.Find("PCPlayer").gameObject;
            go.SetActive(false);
        }
        else
        {
            // Fetch ComputerPlayer
            playerParentNode = transform.Find("PCPlayer").gameObject;
            playerControllerPC = playerParentNode.transform.GetComponentInChildren<FPSController>();
            playerObject = GameObject.Find("CameraPlayerPC").gameObject;

            // Disable VR
            GameObject go = transform.Find("VRHeadset").gameObject;
            go.SetActive(false);
        }
    }

    void SetChunkLayers()
    {
        nextChunk = activeChunk + 1;
        oldChunk = activeChunk - 1;

        for (int i = 0; i < nrOfChunks; i++)
        {
            if ((i == activeChunk) || (i == oldChunk) || (i == nextChunk))
                chunksInWorld[i].ActivateChunk();
            else
                chunksInWorld[i].InactivateChunk();
        }

        chunksInWorld[activeChunk].SetLevelLayer(10, false);    //LAYER A
        Material mat = new Material(chunksInWorld[activeChunk].portals[chunksInWorld[activeChunk].activePortal].screen.material);
        chunksInWorld[activeChunk].RenderAll();

        if (activeChunk == 0)   // FIRST CHUNK 
        {
            chunksInWorld[nextChunk].SetLevelLayer(11, true);    // LAYER B
            chunksInWorld[activeChunk].portals[0].screen.material = materialList[4];
            chunksInWorld[nextChunk].portals[0].screen.material = materialList[3];
            chunksInWorld[nextChunk].AvoidRender();
        }
        else if(activeChunk == (nrOfChunks-1))  // LAST CHUNK
        {
            chunksInWorld[oldChunk].SetLevelLayer(12, true);    // LAYER C
            chunksInWorld[activeChunk].portals[0].screen.material = materialList[5];
            chunksInWorld[oldChunk].portals[0].screen.material = materialList[3];
            chunksInWorld[oldChunk].AvoidRender();
        }
        else  // MIDDLE CHUNK
        {
            chunksInWorld[nextChunk].SetLevelLayer(11, true);   // LAYER B
            chunksInWorld[activeChunk].portals[1].screen.material = materialList[4];
            chunksInWorld[nextChunk].portals[0].screen.material =  materialList[3];
            chunksInWorld[nextChunk].AvoidRender();

            chunksInWorld[oldChunk].SetLevelLayer(12, true);    // LAYER C
            chunksInWorld[activeChunk].portals[0].screen.material = materialList[5];
            chunksInWorld[oldChunk].portals[0].screen.material = materialList[3];
            chunksInWorld[oldChunk].AvoidRender();
        }

        
    }

    void StartLayerAllChunks(int index)
    {
        chunksInWorld[index+1].SetLevelLayer(11, true);   // LAYER B
        chunksInWorld[index + 1].portals[0].screen.material = materialList[3];

        chunksInWorld[index - 1].SetLevelLayer(12, true);    // LAYER C
        chunksInWorld[index - 1].portals[0].screen.material = materialList[3];
    }

    void SwapChunks(ref int currentActiveChunk, ref int currentOldChunk)
    {
        int temp = currentActiveChunk;
        currentActiveChunk = currentOldChunk;
        currentOldChunk = temp;
    }
}
