//attach to each child object of the helathsystem manager to detect bullets

using UnityEngine;
using System.Collections;

public class Detecthit : MonoBehaviour {

public GameObject healthSystemManager; //the gameobject that is the manager
public float hitValue; //the amount of hit points are deducted if a bullet hits this area; this is the base value without extras from the bullet (so that different bullets deduct different amounts of health)
public bool criticalArea; //where one shot in this area kills instantly
public bool disablingArea; //area where a shot contributes to the disabling cycle
public bool debug; //whether to debug or not



//if something hits us
void OnTriggerEnter(Collider other)
{

if(other.transform.tag == healthSystemManager.GetComponent<DetectHitManager>().tagOfBullet)
{
if(debug)
{
Debug.Log("Hit" + collider.gameObject);
}

//if we get hit by a bullet
//pass on the parameters for our hit on
if(other.GetComponent<BulletHealthExtras>() != null)
{
healthSystemManager.GetComponent<DetectHitManager>().BulletHitArea( hitValue + other.GetComponent<BulletHealthExtras>().extraHitValue, criticalArea, disablingArea);

//tell the bullet to destroy
other.GetComponent<BulletHealthExtras>().StartCoroutine("BulletHit", transform);
}
else
{
healthSystemManager.GetComponent<DetectHitManager>().BulletHitArea(hitValue, criticalArea, disablingArea);
}

}
}

//this is for other scripts, eg. UFPS to use natively
public void Damage(float extraDamage)
{
healthSystemManager.GetComponent<DetectHitManager>().BulletHitArea(hitValue + extraDamage, criticalArea, disablingArea);
}
public void ApplyDamage(float extraDamage)
{
 healthSystemManager.GetComponent<DetectHitManager>().BulletHitArea(hitValue + extraDamage, criticalArea, disablingArea);
}


}
