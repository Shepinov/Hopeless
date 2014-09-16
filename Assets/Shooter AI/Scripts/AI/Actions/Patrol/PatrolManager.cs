//attach to patrol manager gameobject
//this script returns the next vector3 to go to, based on waypoints

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolManager : MonoBehaviour {

public Vector3 nextDestination; //the next position to go to
public GameObject[] waypointList; //drag and drop each waypoint into this array
public float criticalDistanceToWaypoint; //how close do we have to be to a waypoint to count as if we've reached it
public NavMesh meshNav = new NavMesh(); //the navmesh we will be using
public bool useOwnNavSystem = false; //whether to use own navmesh; this value is controlled by AIMovementController

private int currentListpos; //the id of the next waypoint to goto

void Update()
{
//set our destination vector3
nextDestination = waypointList[currentListpos].transform.position;

//test if we have to go up on the list, after we reached a checkpoint
if(Vector3.Distance(transform.position, waypointList[currentListpos].transform.position) < criticalDistanceToWaypoint)
{


//this is if we have reached the end of the waypoint list
if(currentListpos > waypointList.Length-2)
{
currentListpos = 0;
}
else
{
//else just increment our position on the list
currentListpos += 1;

}



}

}


//draw lines connecting the waypoints
void OnDrawGizmosSelected ()
{

DrawLinesWithNavigation();
} 


//draw lines using navmesh
void DrawLinesWithNavigation()
{

if(useOwnNavSystem == false)
{
int testId2 = 0;
int testIdPlus2 = 1;


List<Vector3> waypointVector3 = new List<Vector3>();


while(testId2 < waypointList.Length)
{

if(testIdPlus2 >= waypointList.Length)
{
testIdPlus2 = 0;
}
		
NavMeshPath pathMain = new NavMeshPath();

NavMesh.CalculatePath(waypointList[testId2].transform.position, waypointList[testIdPlus2].transform.position, -1, pathMain);


int testId3 = 0;

while(testId3 < pathMain.corners.Length)
{
waypointVector3.Add(pathMain.corners[testId3]);
testId3 += 1;
}

testId2 += 1;
testIdPlus2 += 1;
}


//draw the lines

int testId = 0;
int testIdPlus = 1;

while(testId < waypointVector3.Count)
{

if(testIdPlus >= waypointVector3.Count)
{
testIdPlus = 0;
}

Gizmos.color = Color.magenta;
Gizmos.DrawLine(waypointVector3[testId], waypointVector3[testIdPlus]);

testId += 1;
testIdPlus += 1;
}
}

}



}
