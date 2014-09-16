using UnityEngine;
using System.Collections;

public class AIMovementControllerASTAR : MonoBehaviour {


public Vector3 destinationPosition; //the vector3 position of the destionation where we have to go
public GameObject animationManager; //the object that controls all animations
public bool freeze = false; //whether to freeze or not
public bool coverNavmesh = false; //whether the determined cover position is navmesh or raycast; used by function findclosesthalfcover
public float animationSpeedFactor = 5f; //this controls how the speed gets transformed into data for the animations
public bool useOwnNavSystem = false; //whether to use own navigation system
public float minDistanceToDestination = 1.5f; //the min distance to destination to stop if we're running in circles
public float framesCriticalCheck = 60f; //the critical amount of frames to stop trying to get to the destination	
public float checksCriticalUntilStop = 2f; //the amount of checks until we stop	
public bool setYPosCorrectly = true; //whether to set the y position correctly

private ShooterAIPathFinder agent;
private Vector3 velocity;
private float angle;
public float testPosFrames = 0f; //the amount of frames; used for finding when to debug position
public float checksTested = 0f; //the amount of checks tested
//private bool frozen = false; //whether we're frozen or not	


private int minFrameBetweenNewOrders = 5; //the minimum amount of frames to wait until we're allowed to go to a new position
private int framesPassedSinceLastOrder = 0; //the amount of frames passed since the last order came to move; see variable before
public bool allowedToMoveToNewPos = true; //whether we're allowed to recieve new orders

private float prevTurnSpeed; //the previous turning speed


	void Start()
	{
		agent = GetComponent<ShooterAIPathFinder>();
	}
	
	
	void Update()
	{
		animationManager = GetComponent<AIStateManager>().animationManager;
		
		//this is for smooth movement in the animations for the ai
		float angle2 = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;
		//smooth out movement
		angle = Mathf.Lerp(angle, angle2, 0.5f);
		
		
		//set the correct variables for the animations
		animationManager.GetComponent<Animator>().SetFloat("Speed", agent.CalculateVelocity( agent.GetFeetPosition() ).magnitude );
		animationManager.GetComponent<Animator>().SetFloat("AngularSpeed", angle * Mathf.Deg2Rad);
		animationManager.GetComponent<Animator>().SetBool("Crouch", GetComponent<AIStateManager>().crouching );
		

		
		
		
		//this is for trying to find out if we're getting to the destination or not
		testPosFrames += 1f; 
		if(testPosFrames > framesCriticalCheck)
		{
			testPosFrames = 0f;
			checksTested += 1f;
			
			if(Vector3.Distance(transform.position, destinationPosition) < minDistanceToDestination && checksTested >= checksCriticalUntilStop)
			{
				SetNewDestination(transform.position);
				checksTested = 0;
			}

		}

//this is for freezing
/*
if(Vector3.Distance(destinationPosition, transform.position) > minDistanceToDestination && frozen == true)
{
agent.Resume();
frozen = false;
}*/



		//limit orders
		framesPassedSinceLastOrder += 1;
		if(framesPassedSinceLastOrder > minFrameBetweenNewOrders)
		{
			framesPassedSinceLastOrder = 0;
			allowedToMoveToNewPos = true;
			
		}

		//set the y position correctly
		if(setYPosCorrectly == true)
		{
			
			RaycastHit hit;
			if( Physics.Raycast( transform.position + transform.up, -transform.up * 9999999999f, out hit) )
			{
				
				transform.position = new Vector3( transform.position.x, hit.point.y, transform.position.z );
			}
		}
		
}





	//update to a new target to go to
	public void SetNewDestination(Vector3 newDestination)
	{
		
		if(allowedToMoveToNewPos == true)
		{
			agent.canMove = true;
			destinationPosition = newDestination;
			agent.SearchPath( destinationPosition );
			allowedToMoveToNewPos = false;
		}
		
		
	}

	//these functions fully freeze movement
	public void Freeze()
	{
		//frozen = true;
		agent.canMove = false;
	}

	public void Defreeze()
	{
		//frozen = false;
		agent.canMove = true;
	}
	

	//this sets the speed for the nav mesh agent
	public void SetSpeed(float speedTarget)
	{
		agent.speed = speedTarget;
	}
	

	//this function finds the closest half cover by using navmesh and returns the position
	public Vector3 FindClosestHalfCover()
	{

		//we try and get a cover position, seen from the hips, but not from the eyes ( so a half cover)

		GetComponent<SearchCover>().FindCoverPostion( new Vector3( 0f, 0.3f, 0f), true);

		return GetComponent<SearchCover>().coverPostion;

	}



	//this function is calculate the path length

	public float CalculatePathLength(Vector3 targetPosition)
	{
		// Create a path and set it based on a target position.
		
		if(agent.enabled)
		{
			return -1f;
		}
		
		Pathfinding.Path newPath = GetComponent<Seeker>().StartPath( agent.GetFeetPosition(), targetPosition);
		
		
		return newPath.GetTotalLength();
		
	}
	
	
	/// <summary>
	/// Turns off rotation.
	/// </summary>
	public void TurnOffRotation()
	{
		if(prevTurnSpeed != -1)
		{
			return;
		}
		
		prevTurnSpeed = agent.turningSpeed;
		agent.turningSpeed = 0f;
	}
	
	/// <summary>
	/// Turns on rotation.
	/// </summary>
	public void TurnOnRotation()
	{
		if(prevTurnSpeed == -1)
		{
			return;
		}
		
		agent.turningSpeed = prevTurnSpeed;
		prevTurnSpeed = -1;
		
	}
	
	
	
	void OnDrawGizmosSelected ()
	{


		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(destinationPosition, 0.7f);
		
	} 

}
