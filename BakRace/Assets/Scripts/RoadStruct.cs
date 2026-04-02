using UnityEngine;

public class RoadStruct : MonoBehaviour
{

    public float xsize;
    public float ysize;
    public float zsize;
    public GameObject prevRoad;
    public GameObject nextRoad;

    public float yrotation;
    public bool destroying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car"){
            if (GameManager.gm.currGameMode == GameManager.gameMode.Endless){
                destroying = true;
                Destroy(this.gameObject, 1f);
                GameManager.gm.nextRoad();
            }
        }
    }
}
