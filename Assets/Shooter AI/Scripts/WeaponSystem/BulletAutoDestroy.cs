//this script auto destroys the bullet once it notices its stopped moving

using UnityEngine;
using System.Collections;

public class BulletAutoDestroy : MonoBehaviour {

public float amountOfFramesStopped = 0f; //the amount of frames the bullet has stopped moving
public float criticalAmountOfFramesStopped = 60f; //the critical amount fo frames for the bullet to stop
public float minCritSpeed = -10f; //the minimum critical speed
public float maxCritSpeed = 10f; //the maximum critical speed 
public float criticalDistance = 500f; //the critical distance after which to auto destruct
public float bugOutDistance = 5f; //this is the distance t destory if we bug out
public float criticalAmountOfFramesBugged = 30f; //the critical amoutn of frames in bug mode to stop

private Vector3 initPos; //the init position
private Vector3 testPos; //this is the position to test whether we're bugging out on one spot or not	
private float framesBuggedOut = 0f; //the amount of frames bugged

void Start()
{
initPos = transform.position;
}



void FixedUpdate()
{

if(Vector3.Distance(transform.position, testPos) < bugOutDistance)
{
framesBuggedOut += 1f;
}
else
{
framesBuggedOut = 0f;
}
testPos = transform.position;

if(framesBuggedOut > criticalAmountOfFramesBugged)
{
Destroy(gameObject);
}



Rigidbody rb = GetComponent<Rigidbody>();

if(rb.velocity.magnitude < maxCritSpeed && rb.velocity.magnitude > minCritSpeed)
{
amountOfFramesStopped += 1f;
}
else
{
amountOfFramesStopped = 0f;
}


if(amountOfFramesStopped > criticalAmountOfFramesStopped)
{
			Destroy( audio, audio.clip.length - audio.time);
}


if(Vector3.Distance(initPos, transform.position) > criticalDistance)
{
			Destroy( audio, audio.clip.length - audio.time);
}


}


}
