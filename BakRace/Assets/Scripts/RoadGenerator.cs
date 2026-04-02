using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{

    public Transform roadParent;
    public GameObject[] roadTiles = new GameObject[5];
    private GameObject lastRoad;

    public GameObject startRoad;
    public GameObject finishRoad;
    public GameObject[] buildings = new GameObject[7];

    public int trackLen;
    private List<Vector2> trackCoord = new List<Vector2>();
    private List<GameObject> track = new List<GameObject>();

    struct cityBlock
    {
        public Vector2 position;
        public GameObject build;
    }

    private List<cityBlock> cityBlocks = new List<cityBlock>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetRoads()
    {
        foreach(Transform child in roadParent)
        {
            Destroy(child.gameObject);
        }
        trackCoord = new List<Vector2>();
        track = new List<GameObject>();
        cityBlocks = new List<cityBlock>();
        lastRoad = null;
        return;
    }

    public void generateTrack(int length, bool ending = true){
        while (track.Count == 0)
        {
            for (int i = 0; i < length; i++)
            {
                generateNextTrackBlock();
                if (track.Count == 0){break;}
            }
        }
        if (ending)
        {
            generateFinal();
        }
    }

    public void generateNextTrackBlock(){
        GameObject inst;
        if (lastRoad == null){
            inst = Instantiate(startRoad, new Vector3(0,0,0), Quaternion.identity);
            inst.transform.parent = roadParent;
            lastRoad = inst;
            track.Add(inst);
            if (GameManager.gm.currGameMode != GameManager.gameMode.Endless)
            {
                trackCoord.Add(new Vector2(0, 0));
                trackCoord.Add(new Vector2(1, 0));
                buildAround(inst);
            }
            return;
        }
        
        RoadStruct lastRoadStruct = lastRoad.GetComponent<RoadStruct>();
        

        int[] nextTile = generateOrder(3);
        int newTileIndex = -1;
        
        if (GameManager.gm.currGameMode != GameManager.gameMode.Endless)
        {
            Vector2 dir = trackCoord[trackCoord.Count-1] - trackCoord[trackCoord.Count-2];
            for (int i = 0; i < 3; i++){
                if (newTileIndex != -1) {break;}
                newTileIndex = nextTile[i];
                if (newTileIndex == 0){
                    if (trackCoord.Contains(trackCoord[trackCoord.Count-1] + dir)){
                        newTileIndex = -1;
                    } else {
                        trackCoord.Add(trackCoord[trackCoord.Count-1] + dir);
                    }   
                } else if (newTileIndex == 1){ // Prava
                    if (trackCoord.Contains(trackCoord[trackCoord.Count-1] + new Vector2(-dir.y, dir.x))){
                        newTileIndex = -1;
                    } else {
                        trackCoord.Add(trackCoord[trackCoord.Count-1] + new Vector2(-dir.y, dir.x));
                    }
                } else if (newTileIndex == 2){ // Lava
                    if (trackCoord.Contains(trackCoord[trackCoord.Count-1] + new Vector2(dir.y, -dir.x))){
                        newTileIndex = -1;
                    } else {
                        trackCoord.Add(trackCoord[trackCoord.Count-1] + new Vector2(dir.y, -dir.x));
                    }
                }
            }

            if (newTileIndex == -1){
                resetRoads();
                return;
            }
        } else {
            newTileIndex = nextTile[0];
        }
        
        if (newTileIndex != 0)
        {
            newTileIndex += 2 * Random.Range(0, 2);
        }     

        inst = Instantiate(roadTiles[newTileIndex], lastRoad.transform.position, lastRoad.transform.rotation);

        lastRoadStruct.nextRoad = inst;

        Vector3 translDir = new Vector3(
            lastRoadStruct.xsize,
            lastRoadStruct.ysize,
            lastRoadStruct.zsize
        );

        inst.transform.Translate(translDir);

        inst.transform.Rotate(0, lastRoadStruct.yrotation, 0, Space.Self);

        if (GameManager.gm.currGameMode != GameManager.gameMode.Endless)
        {
            buildAround(inst);
        } else
        {
           int preset = Random.Range(1, 7);
            if (preset < 4)
            {
                inst.transform.Find("Collectible" + preset).gameObject.SetActive(true);
            } 
        }

        inst.transform.parent = roadParent;
        track.Add(inst);
        lastRoad = inst;
    }

    void generateFinal()
    {
        RoadStruct lastRoadStruct = lastRoad.GetComponent<RoadStruct>();

        GameObject inst = Instantiate(finishRoad, lastRoad.transform.position, lastRoad.transform.rotation);

        Vector3 translDir = new Vector3(
            lastRoadStruct.xsize,
            lastRoadStruct.ysize,
            lastRoadStruct.zsize
        );

        inst.transform.Translate(translDir);

        inst.transform.Rotate(0, lastRoadStruct.yrotation+180, 0, Space.Self);
        for (int i = -1; i < 2; i++){
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                addBuilding(inst, trackCoord[trackCoord.Count-1] + new Vector2(i, j));
            }
        }
        buildAround(inst);

        inst.transform.parent = roadParent;
        lastRoad = inst;
    }

    void fillEmpty()
    {
        int minX = 0, maxX = 0, minY = 0, maxY = 0;
        foreach (Vector2 vec2 in trackCoord) {
            if (vec2.x < minX) {
                minX = (int) vec2.x;
            } else if (vec2.x > maxX) {
                maxX = (int) vec2.x;
            }
            if (vec2.y < minY) {
                minY = (int) vec2.y;
            } else if (vec2.y > maxY) {
                maxY = (int) vec2.y;
            }
        }

        for (int i = minX-1; i <= maxX+1; i++)
        {
            for (int j = minY-1; j <= maxY+1; j++)
            {
                if (!trackCoord.Contains(new Vector2(i, j)))
                {
                    //TODO rotate
                    GameObject inst = Instantiate(buildings[Random.Range(0, 7)]);
                    inst.transform.Translate(new Vector3(j*40, 0, i*40));
                    inst.transform.parent = roadParent;
                }
            }
        }
    }

    int[] generateOrder(int count){
        int[] result = new int[count];
        for (int i = 0; i < count; i++){
            result[i] = i;
        }

        for (int i = 0; i < count*5; i++)
        {
            int randomIndex = Random.Range(0, count);
            int randomIndex2 = Random.Range(0, count);
            if (randomIndex == randomIndex2)
            {
                continue;
            }
            int temp = result[randomIndex2];
            result[randomIndex2] = result[randomIndex];
            result[randomIndex] = temp;
        }

        return result;
    }

    private void buildAround(GameObject roadBlock)
    {
        removeBuilding(trackCoord[trackCoord.Count-1]);
        removeBuilding(trackCoord[trackCoord.Count-2]);
        for (int i = -1; i < 2; i++){
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                addBuilding(roadBlock, trackCoord[trackCoord.Count-2] + new Vector2(i, j));
            }
        }
    }

    private void addBuilding(GameObject roadBlock, Vector2 pos)
    {
        cityBlock cB = cityBlocks.Find( b => b.position.x == pos.x && b.position.y == pos.y);
        if (cB.build != null)
        {
            cB.build.transform.parent = roadBlock.transform;
            return;
        }
        if (trackCoord.Contains(pos))
        {
            return;
        }
        cB = new cityBlock();
        
        cB.build = Instantiate(buildings[Random.Range(0, 7)]);
        cB.build.transform.Translate(new Vector3(pos.y*40, 0, pos.x*40));
        cB.build.transform.Rotate(0, Random.Range(0, 4)*90, 0, Space.Self);
        cB.build.transform.parent = roadBlock.transform;
        cB.position = pos;
        cityBlocks.Add(cB);
    }

    private void removeBuilding(Vector2 pos)
    {
        cityBlock cB = cityBlocks.Find( b => b.position.x == pos.x && b.position.y == pos.y);
        if (cB.build == null)
        {
            return;
        }
        Destroy(cB.build);
        cityBlocks.Remove(cB);
    }

}
