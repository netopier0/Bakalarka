using UnityEngine;

public class wheel : MonoBehaviour
{
    public GameObject car;
    private car carScript;
    private aiCar aiCarScript;
    public bool ai;
    Rigidbody carRb;

     [SerializeField] float restDistance = 1.5f;
     [SerializeField] float springStrength = 10f;
     [SerializeField] float springDamper = 1.5f;

     [SerializeField] float tireGripFactor = 0.25f;
     [SerializeField] float tireMass = 0.25f;
    
    private float runnerSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carRb = car.GetComponent<Rigidbody>();
        if (ai)
        {
            aiCarScript = car.GetComponent<aiCar>();
        } else
        {
            carScript = car.GetComponent<car>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitRay;
        if (Physics.Raycast(transform.position, -transform.up, out hitRay, restDistance))
        {
            // Springs
            Vector3 springDir = transform.up;

            Vector3 tireWorldVel = carRb.GetPointVelocity(transform.position);

            float offset = restDistance - hitRay.distance;

            float vel = Vector3.Dot(springDir, tireWorldVel);

            float force = ( offset * springStrength ) - ( vel * springDamper );

            // Debug.DrawRay(transform.position, transform.up * force, Color.green);

            carRb.AddForceAtPosition(springDir * force, transform.position);


            // Steering

            Vector3 steeringDir = transform.right;

            tireWorldVel = carRb.GetPointVelocity(transform.position);

            float steeringVel = Vector3.Dot(tireWorldVel, steeringDir);

            float desiredVelChange = -steeringVel * tireGripFactor;

            float desiredAccel = desiredVelChange; // / Time.fixedDeltaTime;

            //Vector3 finalForce = (springDir * force) + (steeringDir * tireMass * desiredAccel);

            // if (steeringVel > 1f || -1f > steeringVel)
            //  {
                carRb.AddForceAtPosition(steeringDir * tireMass * desiredAccel, transform.position);
                //Debug.DrawRay(transform.position, finalForce, Color.red);
            // }

            if (ai)
            {
                carRb.AddForceAtPosition(transform.forward * Time.deltaTime * 2 * aiCarScript.currSpeed, transform.position);
            } else if (GameManager.gm.currGameMode == GameManager.gameMode.Endless)
            {
                if (runnerSpeed < 40){
                    runnerSpeed += 0.05f;
                }
                carRb.AddForceAtPosition(transform.forward * Time.deltaTime * 2 * runnerSpeed, transform.position);
            } else {
                // >>>>> Arduino Code:
                carRb.AddForceAtPosition(transform.forward * Time.deltaTime * 2 * carScript.currSpeed, transform.position);
                // <<<<< Arduino Code:

                // Testing using keyboard
                // if (Input.GetKey(KeyCode.W))
                // {
                //     carRb.AddForceAtPosition(transform.forward * Time.deltaTime * 50, transform.position);
                // } else if (Input.GetKey(KeyCode.S))
                // {
                //     carRb.AddForceAtPosition(-transform.forward * Time.deltaTime * 50, transform.position);
                // }
            }
            
        } else
        {
            // Wheel Not on ground
            // Debug.DrawRay(transform.position, -transform.up * restDistance, Color.black); 
        }
    }


    void OnDisable()
    {
        runnerSpeed = 0f;
        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;
    }
}
