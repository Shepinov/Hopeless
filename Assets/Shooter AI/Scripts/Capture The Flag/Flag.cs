using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

    bool pickedUp = false;

	public float distanceToPickup = 2f; //the distance to pickup the flag
    public GameObject hasFlagCube;

    private GameObject cube;
    // Notifies the ai that he has the flag
    
    void Update()
    {
		
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("aiTeam1");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		
		gos = GameObject.FindGameObjectsWithTag("aiTeam2");
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		
		
		if(closest != null && Vector3.Distance(closest.transform.position, transform.position) < distanceToPickup)
		{
			TryPickup( closest.transform);
		}
		
		
    }
    
    
    
    
    
    void OnTriggerEnter(Collider other) 
    {
		
		TryPickup( other.transform);
		
    }
    
    
    void TryPickup( Transform other)
    {
		if (pickedUp == false && other.gameObject.transform.root.tag == "aiTeam1" || other.gameObject.transform.root.tag == "aiTeam2")
		{
			pickedUp = true;
			other.gameObject.SendMessageUpwards("FlagPickedUp");
			
			cube = Instantiate(hasFlagCube, new Vector3(0, 10, 0), Quaternion.identity) as GameObject; // create cube to notify user's who has cube
			cube.transform.parent = other.gameObject.transform.root; // make bot the parent 
			cube.transform.localPosition = new Vector3(0, 2.5f, 0); // place the cube above the bots head that has flag
			cube.gameObject.tag = gameObject.tag; // set cubes tag as the flags tag
			
			Destroy(gameObject); // destroy this flag 
		}
    }
    
    
}
