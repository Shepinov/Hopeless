//attach to sight
//this script updates the correct variables on the sight organ, for example finding the closest enemy
//you can specify a second team so that the script includes that team in its algorithms



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SightFindClosestObject : MonoBehaviour {
	
	public string tagToAvoid; //the tag to avoid
	public string tagToAvoid2 = ""; //the second group to find;
	
	
	private GameObject closest = null;
	private List<GameObject> visibleEnemies = new List<GameObject>(); //the visible enemies; used inside script
	
	//optimisation
	private float currentFrame = 0f;
	private float frameBarrierAfterWhichToCheckEnemies = 50f;
	
	
	void Start()
	{
		//add the randomness so that each enemy checks at its own interval; against bottlenecking the CPU
		frameBarrierAfterWhichToCheckEnemies += Random.Range(-20f, 20f);
		
	}
	
	
	void Update()
	{
		//optimsation
		currentFrame += 1f;
		
		if(currentFrame > frameBarrierAfterWhichToCheckEnemies)
		{
			ManageOptimalEnemies();
			currentFrame = 0;
		}
		
	}
	
	
	void ManageOptimalEnemies()
	{
		
		//get all the correct tags
		tagToAvoid = GetComponent<Sight>().mainParent.GetComponent<AIStateManager>().tagToAvoidPrimary;
		//tagToAvoid2 = GetComponent<Sight>().mainParent.GetComponent<AIStateManager>().tagToAvoidSecondary;
		
		
		if((tagToAvoid != null && tagToAvoid2 == "") || (GameObject.FindGameObjectsWithTag(tagToAvoid).Length > 0 && GameObject.FindGameObjectsWithTag(tagToAvoid2).Length == 0))
		{
			
			//find the closest gameobject to avoid
			if(GameObject.FindGameObjectsWithTag(tagToAvoid).Length != 0)
			{
				
				GetComponent<Sight>().objectToSeek = FindOptimalEnemy(tagToAvoid);
				
			}
			else
			{
				StartCoroutine("SetToNull");
			}
		}

		

		//if we have to find the closest object from both teams
		if(tagToAvoid != null && tagToAvoid2 != "")
		{
			
			GameObject team1closest = FindOptimalEnemy(tagToAvoid);
			GameObject team2closest = FindOptimalEnemy(tagToAvoid2);
			

			if(GameObject.FindGameObjectsWithTag(tagToAvoid).Length != 0 && GameObject.FindGameObjectsWithTag(tagToAvoid2).Length != 0)
			{
				if(Vector3.Distance(transform.position, team1closest.transform.position) < Vector3.Distance(transform.position, team2closest.transform.position))
				{
					GetComponent<Sight>().objectToSeek = team1closest;
				}
				else
				{
					GetComponent<Sight>().objectToSeek = team2closest;
				}
				
			}
		}
		
		
	}
	
	
	
	//at the end of the frame, reset the sight
	IEnumerator SetToNull()
	{
		yield return new WaitForFixedUpdate();
		GetComponent<Sight>().objectToSeek = null;
	}
	
	

	//find closest enemy
	public GameObject FindOptimalEnemy (string tagToUse) {
		
		
		
		visibleEnemies.Clear();
		
		closest = null;
		
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(tagToUse);
		
		
		for(int x = 0; x < gos.Length; x++)
		{
			
			if(GetComponent<Sight>().mainParent.GetComponent<AIStateManager>().CheckIfFree( gos[x] ) == true)
			{
				visibleEnemies.Add( gos[x] );
			}
			
		}			



		if(visibleEnemies.Count == 0)
		{
			
			
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
			
		}
		else
		{
			
			float distance = Mathf.Infinity;
			Vector3 position = transform.position;
			
			foreach (GameObject go in visibleEnemies) 
			{
				Vector3 diff = go.transform.position - position;
				float curDistance = diff.sqrMagnitude;
				
				if( go.GetComponent<AIStateManager>() != null && go.GetComponent<AIStateManager>().currentState == CurrentState.engage && curDistance < distance + 10f )
				{	
					closest = go;
					distance = curDistance;
					continue;
				}
				
				if (curDistance < distance) 
				{
					closest = go;
					distance = curDistance;
				}
			}
			
		}


		return closest;
	}
	


}
