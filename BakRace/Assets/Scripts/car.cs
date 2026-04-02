using UnityEngine;
using System;
using TMPro;
// using System.IO.Ports;

public class car : MonoBehaviour
{
    public GameObject serialCommObj;
    private SerialReader serialComm;
    public GameObject wheel1;
    public GameObject wheel2;
    public int currSpeed;
    private int currTurn;
    private int lastTurn;
    private Vector3 startPoint;
    private Quaternion startRotation;

    private int hits = 3;
    private int score = 0;
    private bool imunity = false;
    private TMP_Text textFieldLives;

    void Start()
    {
        // Application.targetFrameRate = 50;
        serialComm = serialCommObj.GetComponent<SerialReader>();
        lastTurn = 0;
        currTurn = 0;
        currSpeed = 0;
        if (GameManager.gm.currGameMode == GameManager.gameMode.Endless)
        {
            gameObject.transform.Find("Canvas").gameObject.SetActive(true);
            textFieldLives = gameObject.transform.Find("Canvas").Find("Lives").GetComponent<TMP_Text>();
            textFieldLives.text = "Životy: " + hits;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Testing using keyboard
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     wheel1.transform.Rotate(0f, -30f, 0f, Space.Self);
        //     wheel2.transform.Rotate(0f, -30f, 0f, Space.Self);
        //     currTurn = -30;
        // } else if (Input.GetKeyDown(KeyCode.D))
        // {
        //     wheel1.transform.Rotate(0f, 30f, 0f, Space.Self);
        //     wheel2.transform.Rotate(0f, 30f, 0f, Space.Self);
        //     currTurn = 30;
        // }
        // if (Input.GetKeyUp(KeyCode.A))
        // {
        //     currTurn = 0;
        //     wheel1.transform.Rotate(0f, 30f, 0f, Space.Self);
        //     wheel2.transform.Rotate(0f, 30f, 0f, Space.Self);
        // } else if (Input.GetKeyUp(KeyCode.D))
        // {
        //     currTurn = 0;
        //     wheel1.transform.Rotate(0f, -30f, 0f, Space.Self);
        //     wheel2.transform.Rotate(0f, -30f, 0f, Space.Self);
        // }
        // lastTurn = currTurn;
        

        // >>>>> Arduino Code:
        if (SerialReader.sr.getConnected())
        {
            parseLine();
            int rot = currTurn - lastTurn;
            lastTurn = currTurn;
            wheel1.transform.Rotate(0f, rot, 0f, Space.Self);
            wheel2.transform.Rotate(0f, rot, 0f, Space.Self);            
        }
        // <<<<< Arduino Code;
    }

    void OnEnable()
    {
        startPoint = transform.position;
        startRotation = transform.rotation;
    }

    void OnDisable()
    {
        wheel1.transform.Rotate(0f, -lastTurn, 0f, Space.Self);
        wheel2.transform.Rotate(0f, -lastTurn, 0f, Space.Self);    
        transform.rotation = startRotation;
        transform.position = startPoint;
        
        hits = 3;
        score = 0;
    }


    private void parseLine()
    {
        string line = serialComm.getLatestLine();
        if (line.Length != 6) 
        {
            return;
        }
        try {
        int i = int.Parse(line.Substring(1,2));
        if (line[0] == 'B'){
            i = i * -1;
        }
        currSpeed = i;

        i = int.Parse(line.Substring(4,2));
        if (line[3] == 'L'){
            i = i * -1;
        }
        currTurn = i;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log(line);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.gm.currGameMode == GameManager.gameMode.Endless)
        {
            if (collision.gameObject.tag == "Walls")
            {
                if (imunity)
                {
                    return;
                }
                imunity = true;
                hits = hits - 1;
                textFieldLives.text = "Životy: " + hits;
                if (hits <= 0)
                {
                    imunity = false;
                    GameManager.gm.endStateEndless(score);
                    return;
                }
                GameObject block = collision.gameObject.transform.parent.gameObject;
                if (block.GetComponent<RoadStruct>().destroying)
                {
                    block = block.GetComponent<RoadStruct>().nextRoad;
                }
                transform.rotation = block.transform.rotation;
                transform.position = block.transform.position;
                if (block.name.StartsWith("RoadLeft"))
                {
                    transform.Rotate(0f, 180f, 0f, Space.Self);
                } else if (!block.name.StartsWith("RoadRight"))
                {
                    transform.Rotate(0f, -90f, 0f, Space.Self);
                }
                if (block.name.StartsWith("RoadLeft2Fin"))
                {
                    transform.Translate(new Vector3(7.5f, 0f, 7.5f));
                } else if (block.name.StartsWith("RoadRight2Fin"))
                {
                    transform.Translate(new Vector3(7.5f, 0f, -7.5f));
                }
                Rigidbody carRb = gameObject.GetComponent<Rigidbody>();
                carRb.linearVelocity = Vector3.zero;
                carRb.angularVelocity = Vector3.zero;
                transform.Translate(new Vector3(0f, 2f, 0f));
                imunity = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ciel"){
            GameManager.gm.endStateRace(true);
        }
        if (other.gameObject.tag == "ScorePoint"){
            score = score + 1;
            Destroy(other.gameObject);
        }
    }
}
