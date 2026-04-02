using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum gameMode {Endless, Race};
    public static GameManager gm = null;
    public GameObject roadGeneratorObj;
    public gameMode currGameMode;
    private RoadGenerator roadGen;
    public GameObject menuObject;
    public GameObject raceObject;
    public GameObject aiCarObject;

    private GameObject menuCloneObject;
    private GameObject raceCloneObject;
    private GameObject aiCarCloneObject;

    public GameObject menuScriptObject;
    private float timeStart;

    void Awake()
    {
        if (gm == null){
            gm = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roadGen = roadGeneratorObj.GetComponent<RoadGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startRace(int length)
    {
        gm.currGameMode = gameMode.Race;
        menuObject.SetActive(false);
        raceCloneObject = Instantiate(raceObject);
        aiCarCloneObject = Instantiate(aiCarObject);
        raceCloneObject.SetActive(true);
        aiCarCloneObject.SetActive(true);
        raceCloneObject.transform.Find("RoadGenerator").gameObject.GetComponent<RoadGenerator>().generateTrack(length, true);
        timeStart = Time.time;
    }

    public void startEndless()
    {
        gm.currGameMode = gameMode.Endless;
        menuObject.SetActive(false);
        raceCloneObject = Instantiate(raceObject);
        raceCloneObject.SetActive(true);
        roadGen = raceCloneObject.transform.Find("RoadGenerator").gameObject.GetComponent<RoadGenerator>();
        roadGen.generateTrack(3, false);
        timeStart = Time.time;
    }

    public void endStateEndless(int score)
    {
        float t = Time.time - timeStart;
        menuObject.SetActive(true);
        Destroy(raceCloneObject);
        menuScriptObject.GetComponent<MenuScript>().setTextField("Score: " + score);
    }
    public void endStateRace(bool playerWin)
    {
        menuObject.SetActive(true);
        Destroy(raceCloneObject);
        Destroy(aiCarCloneObject);
        if (playerWin)
        {
            menuScriptObject.GetComponent<MenuScript>().setTextField("You Win");
        } else
        {
            menuScriptObject.GetComponent<MenuScript>().setTextField("You Lose");
        }
    }
    

    public void nextRoad()
    {
        roadGen.generateNextTrackBlock();
    }
}
