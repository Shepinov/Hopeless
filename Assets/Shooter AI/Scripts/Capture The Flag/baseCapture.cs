using UnityEngine;
using System.Collections;

public class baseCapture : MonoBehaviour
{
	public float distanceToPickup = 2f; //the distance to capture the base
    public Vector3 startFlagLocation;
    private ScoreBoard scoreBoard;

    void Start() {
        scoreBoard = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ScoreBoard>();
    }
	
	
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
			TryCapture( closest.transform);
		}
	}
	
	
    void OnTriggerEnter(Collider other)
    {
        TryCapture(other.transform);
    }
    
    
    void TryCapture(Transform other)
    {
		try {
			if (other.gameObject.transform.root.gameObject.GetComponent<FlagManager>().hasFlag)
			{
				other.gameObject.transform.root.gameObject.GetComponent<FlagManager>().hasFlag = false;
				
				Transform[] allChildren = other.gameObject.transform.root.gameObject.GetComponentsInChildren<Transform>();
				
				foreach (Transform child in allChildren) 
				{
					if (child.tag == "Flag") {
						Destroy(child.gameObject);
					}
				}
				
				if (other.gameObject.transform.root.tag == "aiTeam1") {
					scoreBoard.UpdateCaptures(1);
				} else {
					scoreBoard.UpdateCaptures(2);
				}
				
				FlagManager flagManager = other.gameObject.transform.root.gameObject.GetComponent<FlagManager>();
				flagManager.ReturnFlag();
				flagManager.WaypointManager.GetComponent<FlagWaypoints>().SetLocationState(LocationState.goToFlag);
			}
		} catch {
		}
    }
    

}
