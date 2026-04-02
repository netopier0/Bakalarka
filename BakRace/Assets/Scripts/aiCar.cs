using UnityEngine;

public class aiCar : MonoBehaviour
{
    public GameObject wheel1;
    public GameObject wheel2;

    public int currSpeed;
    private int currTurn;
    private int lastTurn;

    private Vector3 startPoint;
    private Quaternion startRotation;

    public LayerMask layerMask;
    RaycastHit[] hits = new RaycastHit[9];
    // float[] angles = new float[8]{30, 45, 60, 75, 105, 120, 135, 150};
    float[] angles = new float[9]{0, 30, 60, 75, 90, 105, 120, 150, 180};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastTurn = 0;
        currTurn = 0;
        currSpeed = 0;
        layerMask = LayerMask.GetMask("WallForAI");
    }

    // Update is called once per frame
    void Update()
    {
        float maxDist = -1f;
        float maxAngle = 0f;
        float secondDist = -1f;
        float secondAngle = 0f;
        Vector3 dirVec = new Vector3(0, 0, 0);
        for(int i = 0; i < angles.Length; i++){
            float angle = angles[i];
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(angle, Vector3.up)* transform.forward, out hits[i], Mathf.Infinity, layerMask)){ 
                // Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * hits[i].distance, Color.yellow); 
                // Debug.Log("Did Hit"); 
                // if (hits[i].distance > 2f){
                //     dirVec += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * hits[i].distance;
                // }
                if (maxDist < hits[i].distance){
                    secondDist = maxDist;
                    secondAngle = maxAngle;
                    maxDist = hits[i].distance;
                    maxAngle = angle;
                    if (Mathf.Abs(angle - transform.eulerAngles.y) < 10){
                        maxDist -= 1;
                    }
                } else if (secondDist < hits[i].distance)
                {
                    secondDist = hits[i].distance;
                    secondAngle = angle;
                    if (Mathf.Abs(angle - transform.eulerAngles.y) < 10){
                        secondDist -= 1;
                    }
                }
            }
        }
        // Debug.DrawRay(transform.position, dirVec, Color.blue);
        // Debug.DrawRay(transform.position, Quaternion.AngleAxis(maxAngle, Vector3.up) * transform.forward * maxDist, Color.red); 
        // Debug.DrawRay(transform.position, Quaternion.AngleAxis(secondAngle, Vector3.up) * transform.forward * secondDist, Color.green); 

        // maxAngle = ((maxAngle - 90) + (secondAngle - 90))/2.0f;
        maxAngle = maxAngle - 90;
        // maxAngle = Vector3.Angle(dirVec, transform.right);
        // if (Vector3.Angle(dirVec, transform.forward) < 90)
        // {
        //     maxAngle *= -1;
        // }
        if (maxAngle > 32)
        {
            maxAngle = 32;
        }
        if (maxAngle < -32)
        {
            maxAngle = -32;
        }
        if (maxAngle > currTurn)
        {
            currTurn ++;
        } else if (maxAngle < currTurn)
        {
            currTurn--;
        }
        currTurn = (int) maxAngle;
        int rot = currTurn - lastTurn;
        lastTurn = currTurn;
        currSpeed = 16 - currTurn / 2;
        wheel1.transform.Rotate(0f, rot, 0f, Space.Self);
        wheel2.transform.Rotate(0f, rot, 0f, Space.Self);
    }

    void OnEnable()
    {
        startPoint = transform.position;
        startRotation = transform.rotation;
    }

    void OnDisable()
    {
        transform.rotation = startRotation;
        transform.position = startPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ciel"){
            GameManager.gm.endStateRace(false);
        }
    }
}
