using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Shooter AI strategy close together. Used by team.
/// </summary>
public class ShooterAIStrategyCloseTogether : MonoBehaviour {


public float framesCriticalToPutIntoNewPos = 100f; //the amount of frames the script waits until 
public float walkRadius = 3f; //when we want to find a new position for the AI, this is the random radius
public float maxDistanceBetweenTeam = 3f; //the max distance between the team captain and each other soldier



public GameObject teamCaptain;
public Hashtable objectsNotVisisble = new Hashtable();
public List<GameObject> team = new List<GameObject>(); //our team
public float checkStep = 50f; //once how many steps to probe the other team members
public float currentStep = 0f;



void Start()
{
teamCaptain = GetComponent<ShooterAITeamOverview>().currentTeamCaptain;
}

void Update()
{
currentStep += 1f;

if(currentStep > checkStep)
{
ControlTeam();
currentStep = 0f;
}

}



void ControlTeam()
{

//update captain vars
GetComponent<ShooterAITeamOverview>().DetermineTeamCaptain();
GetComponent<ShooterAITeamOverview>().DeterminAliveTeamSoldiers();

teamCaptain = GetComponent<ShooterAITeamOverview>().currentTeamCaptain;
team = GetComponent<ShooterAITeamOverview>().aiCharsAlive;


//determine whose not visible from the team from the team captains perspective; note we do not include FOV, becuase IRL you have a much better undestadment
//of whose behind and around you.
for(int x = 0; x < team.Count; x++)
{


if( Vector3.Distance(teamCaptain.transform.position, team[x].transform.position) > maxDistanceBetweenTeam )
{
float prevValue = 0;

if( objectsNotVisisble[team[x]] != null)
{
prevValue = (float)objectsNotVisisble[team[x]];
}
objectsNotVisisble[team[x]] = prevValue + checkStep;



//if we can't see this team player for a number of frames, send to him to a closer spot
if((float)objectsNotVisisble[team[x]] > framesCriticalToPutIntoNewPos)
{

//find the new spot
Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
randomDirection += teamCaptain.GetComponent<AIMovementController>().destinationPosition;
NavMeshHit hit;
NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
Vector3 finalPosition = hit.position;

team[x].GetComponent<AIMovementController>().SetNewDestination(finalPosition);


}


}

}


}

	


}
