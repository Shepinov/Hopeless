//this is the main ai auditory script, return "hearingDistance" true, if the player is within earshot distance and is moving, else false
//attach to a child of the audiotery sensor

using UnityEngine;
using System.Collections;

	public class Auditory : MonoBehaviour {
	
	public float hearingDistance; //the distance that the ai can hear
	public bool canHearTarget; //wherther we can hear our target
	public GameObject target; //the target, probably the player
	public string tagOfTarget2; //this is a second target; best used for stuff like bullets etc.
	public GameObject mainParent; //our main parent
	public Vector3 lastHeardPosition; //the last position we heard the player
	public Vector3 lastHeardPositionSecondary; //the last position we heard the secondary target; in this case its actually the velocity as we're measuring bullets here and it would harden the calculations if we used only positions
	public bool canHearTarget2 = false; //whether we can hear target 2
	public float hearingDistanceTarget2; //the hearing distance for the second target
	public GameObject animatorObject; //the animator object
	
	
	private bool namehash; //whether we're moving or not
	private GameObject closest; //for the closest enemy
	
	//optimisation
	private float currentFrame = 0f;
	private float frameBarrier = 50f;
	private float distanceToEnemy = 10000f;
	
	
	void Start()
	{
	
		frameBarrier += Random.Range(-20f, 20f);
	}
	
	
	void Update()
	{
		
		
		currentFrame += 1f;
		
		if(currentFrame > frameBarrier && target != null)
		{
			distanceToEnemy = CalculatePathLength(target.transform.position);
		}
		
		if(target != null && distanceToEnemy < hearingDistance)
		{
			canHearTarget = true;
			lastHeardPosition = target.transform.position;
		}
		else
		{
			canHearTarget = false;
		}
		
		
		//closest secondary target
		if(currentFrame > frameBarrier)
		{
			GameObject closestT2 = FindClosestBullet(tagOfTarget2);
			currentFrame = 0f;
			
			
			if(closestT2 != null)
			{
				//if we can hear our secondary target
				if(CalculatePathLength(closestT2.transform.position) < hearingDistanceTarget2)
				{
					lastHeardPositionSecondary = closestT2.transform.position;
					canHearTarget2 = true;
					
				}
				else
				{
					canHearTarget2 = false;
				}
				
			}
			else
			{
				canHearTarget2 = false;
			}
			
		}
		
		
		
	}
	
	
	
	
	
	
	public float CalculatePathLength (Vector3 targetPosition)
	{
		return mainParent.GetComponent<AIMovementSwitcher>().CalculatePathLength(targetPosition);
	}
	
	
	
	//find closest enemy
	GameObject FindClosestEnemy(string tagToUse) {
		
		
		return mainParent.GetComponent<AIStateManager>().sight.GetComponent<SightFindClosestObject>().FindOptimalEnemy(tagToUse);
		
		
	}	
	
	
	GameObject FindClosestBullet(string tagToUse)
	{
		
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(tagToUse);
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
		
		return closest;
	}
	
	
	
	void OnDrawGizmosSelected ()
	{
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere( transform.position, hearingDistance);
		
	}

}
