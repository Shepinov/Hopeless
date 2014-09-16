using UnityEngine;
using System.Collections;

public class AIMovementController : MonoBehaviour {

	
	public Vector3 destinationPosition; //the vector3 position of the destionation where we have to go
	public GameObject animationManager; //the object that controls all animations
	public bool freeze = false; //whether to freeze or not
	public bool coverNavmesh = false; //whether the determined cover position is navmesh or raycast; used by function findclosesthalfcover
	public float animationSpeedFactor = 5f; //this controls how the speed gets transformed into data for the animations
	public bool useOwnNavSystem = false; //whether to use own navigation system
	public float minDistanceToDestination = 0.9f; //the min distance to destination to stop if we're running in circles
	public float framesCriticalCheck = 60f; //the critical amount of frames to stop trying to get to the destination	
	public float checksCriticalUntilStop = 2f; //the amount of checks until we stop	
	
	
	private NavMeshAgent agent;
	private Vector3 velocity;
	private float angle;
	private float prevSpeed; //the previous speed before the freeze
	private float prevAngSpeed; //the previous angular speed before the freeze
	private float prevTurnSpeed;
	private Vector3 prevPosition; //the previous position
	
	public float testPosFrames = 0f; //the amount of frames; used for finding when to debug position
	public float checksTested = 0f; //the amount of checks tested
	public bool frozen = false; //whether we're frozen or not	
	
	

	void Awake()
	{
		animationManager = GetComponent<AIStateManager>().animationManager;
		agent = GetComponent<NavMeshAgent>();
		
		prevAngSpeed = agent.angularSpeed;
		prevSpeed = agent.speed;
	}
	
	
	void Update()
	{
		animationManager = GetComponent<AIStateManager>().animationManager;

		//this is whether you use own nav system or not
		if(GetComponent<AIStateManager>().patrolManager.GetComponent<PatrolManager>() != null &&
		 useOwnNavSystem != GetComponent<AIStateManager>().patrolManager.GetComponent<PatrolManager>().useOwnNavSystem)
		{
			GetComponent<AIStateManager>().patrolManager.GetComponent<PatrolManager>().useOwnNavSystem = useOwnNavSystem;
		}
		
		//this is for smooth movement in the animations for the ai
		Vector3 velocity2 = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;
		velocity = ( new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(prevPosition.x, 0f, prevPosition.z) )/Time.deltaTime;
		float angle2 = Mathf.Atan2(velocity2.x, velocity2.z) * 180.0f / 3.14159f;
		//smooth out movement
		angle = Mathf.Lerp(angle, angle2, 0.5f);
		
		
		
		//set the correct variables for the animations
		animationManager.GetComponent<Animator>().SetFloat("Speed", velocity.magnitude );
		animationManager.GetComponent<Animator>().SetFloat("AngularSpeed", angle * Mathf.Deg2Rad);
		animationManager.GetComponent<Animator>().SetBool("Crouch", GetComponent<AIStateManager>().crouching );
		
		
		
		
		
		//this is for trying to find out if we're getting to the destination or not
		testPosFrames += 1f; 
		if(testPosFrames > framesCriticalCheck)
		{

			testPosFrames = 0f;
			checksTested += 1f;
			
			if(Vector3.Distance(transform.position, destinationPosition) < minDistanceToDestination && frozen == false)
			{
				//Debug.Log("Stop");
				
				Freeze();
							
			}
			
			if(frozen == true && Vector3.Distance(transform.position, destinationPosition) > minDistanceToDestination)
			{
				//Debug.Log("Go again");
				Defreeze();
			}
			
		}
		
		//set some variables, MUST BE LAST
		prevPosition = transform.position;
		

	}





	//update to a new target to go to
	public void SetNewDestination(Vector3 newDestination)
	{
		
		if(Vector3.Distance(transform.position, newDestination) < minDistanceToDestination)
		{
			//Debug.Log("Too close");
			return;
		}
		
		Defreeze();
		agent.updatePosition = true;
		destinationPosition = newDestination;
		agent.SetDestination(destinationPosition);
	}
	
	
	
	//these functions fully freeze movement
	public void Freeze()
	{
			
					
		SetNewDestination( transform.position);
		frozen = true;
		agent.Stop(true);
		agent.speed = 0f;
		agent.angularSpeed = 0f;
		
	}
	
	
	public void Defreeze()
	{
		
		frozen = false;
		agent.updatePosition = true;
		agent.Resume();
		
		SetSpeed( prevSpeed);
		agent.angularSpeed = prevAngSpeed;
			
	}
	
	
	
	//this sets the speed for the nav mesh agent
	public void SetSpeed(float speedTarget)
	{
		GetComponent<NavMeshAgent>().speed = speedTarget;
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
		NavMeshPath path = new NavMeshPath();
		if(agent.enabled)
			agent.CalculatePath(targetPosition, path);
		
		// Create an array of points which is the length of the number of corners in the path + 2.
		Vector3 [] allWayPoints = new Vector3[path.corners.Length + 2];
		
		// The first point is the enemy's position.
		allWayPoints[0] = transform.position;
		
		// The last point is the target position.
		allWayPoints[allWayPoints.Length - 1] = targetPosition;
		
		// The points inbetween are the corners of the path.
		for(int i = 0; i < path.corners.Length; i++)
		{
			allWayPoints[i + 1] = path.corners[i];
		}
		
		// Create a float to store the path length that is by default 0.
		float pathLength = 0;
		
		// Increment the path length by an amount equal to the distance between each waypoint and the next.
		for(int i = 0; i < allWayPoints.Length - 1; i++)
		{
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}
		
		return pathLength;
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
		
		prevTurnSpeed = agent.angularSpeed;
		agent.angularSpeed = 0f;
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
		
		agent.angularSpeed = prevTurnSpeed;
		prevTurnSpeed = -1;
	}
	
	void OnDrawGizmosSelected ()
	{
		

		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(destinationPosition, 0.7f);
		
	} 
	
}
