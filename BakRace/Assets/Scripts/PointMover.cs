using UnityEngine;

public class PointMover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up * 50 * Time.deltaTime);
        Vector3 newPos = new Vector3(0, 2f, 0) + Vector3.up * Mathf.Cos(Time.timeSinceLevelLoad)/4;
        newPos.x += gameObject.transform.position.x;
        newPos.z += gameObject.transform.position.z; 
        gameObject.transform.position = newPos;
    }
}
