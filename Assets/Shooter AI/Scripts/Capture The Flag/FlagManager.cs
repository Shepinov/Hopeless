// Attach this to the ai to detect flag grabs and flap captures
using UnityEngine;
using System.Collections;

public class FlagManager : MonoBehaviour
{

    public GameObject flagPrefab;
    public bool hasFlag = false;
    public GameObject WaypointManager;
    public Vector3 baseCaptureLocation;
    private Vector3 initialFlagLocation;
	
	
	void Update()
	{
		//force the ai to run when it has the flag
		if(hasFlag && GetComponent<AIStateManager>() != null)
		{
			GetComponent<AIStateManager>().currentState = CurrentState.patrol;
			GetComponent<AIMovementSwitcher>().SetNewDestination( baseCaptureLocation );
		}
		
	}
	
	
    void Start()
    {
        initialFlagLocation = GameObject.FindWithTag("Flag").transform.position;

        if (WaypointManager == null)
        {
            Debug.LogWarning("WaypointManager not set, AI cannot go to next location");
        }
    }

    public void FlagPickedUp()
    {
        hasFlag = true;
        Debug.Log(gameObject.name + " has picked up the flag!");
        WaypointManager.SendMessage("SetBaseLocation", baseCaptureLocation);
        WaypointManager.SendMessage("SetLocationState", LocationState.returnFlag);
    }

    void DropFlag()
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        
        foreach (Transform child in allChildren) 
        {
			if ( child.CompareTag( "Flag" ) ) {
                Destroy(child.gameObject);
            }
        }

        Instantiate(flagPrefab, gameObject.transform.position, Quaternion.identity); // create cube to notify user's who has cube
        Debug.Log(gameObject.name + " has dropped the flag!");
    }

    // Receives a message from the Shooter Ai that the player is dead
    public void ShooterAiDead() {
        if (hasFlag)
        {
            DropFlag();
        }
    }

    public void ReturnFlag() {
		
		float spawnRadius = 20f;
		Vector3 newFlagPos = initialFlagLocation;
		for(int x = 0; x < 30; x++)
		{
			newFlagPos = new Vector3( Random.insideUnitCircle.x * spawnRadius, initialFlagLocation.y, Random.insideUnitSphere.z * spawnRadius);
			if( Physics.OverlapSphere( newFlagPos, 1f).Length == 0 )
			{
				continue;
			}
		}
		
        Instantiate(flagPrefab, newFlagPos, Quaternion.identity); // create cube to notify user's who has cube
    }
}
