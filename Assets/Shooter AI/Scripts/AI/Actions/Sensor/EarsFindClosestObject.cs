using UnityEngine;
using System.Collections;

public class EarsFindClosestObject : MonoBehaviour {



public string tagToAvoid; //the tag to avoid

//optimisation
private float currentFrame = 0f; //current frame
private float frameBarrier = 150f; //the frame barrier


void Start()
{

frameBarrier += Random.Range(-30f, 30f);

}


void Update()
{
currentFrame += 1f;

if(tagToAvoid != null && currentFrame > frameBarrier )
{
//find the closest gameobject to avoid
if(GameObject.FindGameObjectsWithTag(tagToAvoid) != null)
{



GetComponent<Auditory>().target = FindClosestEnemy(tagToAvoid);
currentFrame = 0f;



//find the correct animator in the enemy
if(GetComponent<Auditory>().target != null)
{

if(GetComponent<Auditory>().target.GetComponentInChildren<Animator>() != null)
{
GetComponent<Auditory>().animatorObject = GetComponent<Auditory>().target.GetComponentInChildren<Animator>().transform.gameObject;
}
}
else
{
GetComponent<Auditory>().animatorObject = null;
}

}

}


//get the correct tags (from our main parent) to avoid
tagToAvoid = GetComponent<Auditory>().mainParent.GetComponent<AIStateManager>().tagToAvoidPrimary;
GetComponent<Auditory>().tagOfTarget2 = GetComponent<Auditory>().mainParent.GetComponent<AIStateManager>().tagToAvoidSecondary;



}


//find closest enemy
GameObject FindClosestEnemy(string tagToUse) {

return GetComponent<Auditory>().mainParent.GetComponent<AIStateManager>().sight.GetComponent<SightFindClosestObject>().FindOptimalEnemy(tagToUse);


}


}
