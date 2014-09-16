//this is the main sight script
//attach to main sight sensor child object

using UnityEngine;
using System.Collections;

public class Sight : MonoBehaviour {
	
	
	public bool canSeeObject = false; //whether we can see the specified object
	public bool canSeeObjectFromWeapon = false; //whether we can see the enemy from the perspective of our weapon
	public GameObject objectToSeek; //the object that we are seeking
	public float fieldOfView; //the field of view
	public Vector3 lastSeenLocation; //the last position we saw our target
	public GameObject mainParent; //our global parent
	public bool attackAI; //should we attack ai?
	public Vector3 eyeLevel; //the level of our eyes
	public float lastSeenTime; //the last time we saw the enemy
	public float maxSeeDistance = 9999999f; //the max seeing distance
	
	private NavMeshAgent agent; //the agent
	

	void Awake()
	{
		if(mainParent.GetComponent<AIMovementController>() != null)
		{
			agent = mainParent.GetComponent<NavMeshAgent>();
		}
	}
	
	
	void Update()
	{
		//test if we can see the object we're seeking
		canSeeObject = false;
		
		if(objectToSeek != null)
		{
			CanSeeObjectTest();
			CanSeeObjectFromWeaponTest();
			
			if(attackAI)
			{
				CanSeeAI();
			}
			
		}
	}



	public void CanSeeObjectTest()
	{
		
		RaycastHit hit;
		//we have to make the eye level smaller or else the ai wont see standard players
		Vector3 rayToCheck = objectToSeek.transform.position - transform.position + eyeLevel/2f;
		
		//if the object is wihtin our field of view and is closer than the max distance
		if((Vector3.Angle(rayToCheck, transform.forward)) < fieldOfView/2f && rayToCheck.magnitude < maxSeeDistance)
		{
			
			
			//send out the ray
			if(Physics.Raycast(transform.position, rayToCheck, out hit))
			{

				//if the object we see is the one we're seeking	
				int layersToCheck = 25;
				Transform testObject = hit.transform;
				
				for(int currentLayer = 0; currentLayer < layersToCheck; currentLayer ++)
				{
					if(testObject.gameObject == objectToSeek)
					{
						FoundCorrectObject();
						continue;
					}
					else
					{
						if(testObject.parent != null)
						{
							testObject = testObject.parent;
						}
					}

				}
			}
			
			
		}
	}
	
	
	
	
	
	public void CanSeeObjectFromWeaponTest()
	{
		
		if(mainParent.GetComponent<AIWeaponController>().weaponHoldingObject == null)
		{
			canSeeObjectFromWeapon = false;
			return;
		}
		
		canSeeObjectFromWeapon = false;
		
		RaycastHit hit;
		//we have to make the eye level smaller or else the ai wont see standard players
		Vector3 initPos = mainParent.GetComponent<AIWeaponController>().weaponHoldingObject.transform.position;
		
		Vector3 rayToCheck = (objectToSeek.transform.position + eyeLevel/4f) - initPos;
		
		//if the object is wihtin our field of view and is closer than the max distance
		if((Vector3.Angle(rayToCheck, transform.forward)) < fieldOfView/2f && rayToCheck.magnitude < maxSeeDistance)
		{
			
			
			//send out the ray
			if(Physics.Raycast( initPos, rayToCheck, out hit))
			{
				
				//Debug.Log( hit.collider.gameObject, hit.collider.gameObject );
				//Debug.DrawRay( initPos, rayToCheck, Color.red);
				
				
					//if the object we see is the one we're seeking	
					int layersToCheck = 25;
					Transform testObject = hit.transform;
					
					for(int currentLayer = 0; currentLayer < layersToCheck; currentLayer ++)
					{
						if(testObject.gameObject == objectToSeek)
						{
							canSeeObjectFromWeapon = true;
							continue;
						}
						else
						{
							if(testObject.parent != null)
							{
								testObject = testObject.parent;
							}
						}
	
					}
				}
				
				
			}
	}
	
	
	
	


	//found correct object
	void FoundCorrectObject()
	{
		//we can see the object we're seeking
		canSeeObject = true;
		//set the last seen position
		lastSeenLocation = objectToSeek.transform.position;
		//set the time correctly
		lastSeenTime = Time.time;
	}
	
	
	
	//this function is to spot other ai
	public void CanSeeAI()
	{
		
		Vector3 rayToCheck = objectToSeek.transform.position - transform.position + eyeLevel;
		
		//if the enemy is right next to us, we can see him
		if(rayToCheck.magnitude < mainParent.GetComponent<AIStateManager>().patrolManager.GetComponent<LocationManager>().criticalDistanceToWaypoint)
		{
			FoundCorrectObject();
		}
		
		
		//if the object is wihtin our field of view
		if((Vector3.Angle(rayToCheck, transform.forward)) < fieldOfView/2f)
		{
			
			
			//Debug.DrawRay(transform.position, rayToCheck, Color.black);
			
			
			//test it using navmesh raycasting
			if(mainParent.GetComponent<AIMovementController>().useOwnNavSystem == false)
			{
				NavMeshHit hit2;
				if(!agent.Raycast(objectToSeek.transform.position + eyeLevel, out hit2)) 
				{
					
					
					FoundCorrectObject();
					
				}
				
			}
		}
	}
	
	
	
	void OnDrawGizmosSelected ()
	{
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum( transform.position + eyeLevel, fieldOfView, maxSeeDistance, 0f, 1f);
		
	}
	
}
