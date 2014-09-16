//this is to actually execute the melee attack

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIWeaponMeleeAttack : MonoBehaviour {

public bool inMeleeAttack = false; //whether we'rew curently in melee or not
public GameObject optionalMeleeBulletObject; //the optional bullet object to use instead of the normal bullet

private List<AnimationState> animClips = new List<AnimationState>();



void Start()
{

foreach (AnimationState clip in animation)
{
animClips.Add(clip);
}

}	


//this function needs to be called to execute the actual attack
public IEnumerator MeleeAttack()
{
//activate arms
var beforeHands = GetComponent<AIWeaponSelected>().objectHoldingGun.GetComponent<AIControllerChild>().model.GetComponent<HandIK>().handToUseInCharacter;
GetComponent<AIWeaponSelected>().objectHoldingGun.GetComponent<AIControllerChild>().model.GetComponent<HandIK>().handToUseInCharacter = HandToUse.BothHands;

inMeleeAttack = true;
//create the bullet
GameObject meleeAttackBullet;
if(optionalMeleeBulletObject != null)
{
meleeAttackBullet = Instantiate(optionalMeleeBulletObject, GetComponent<AIWeaponShoot>().bulletPosition.transform.position, Quaternion.identity) as GameObject;
}
else
{
meleeAttackBullet = Instantiate(GetComponent<AIWeaponShoot>().bulletToUse, GetComponent<AIWeaponShoot>().bulletPosition.transform.position, Quaternion.identity) as GameObject;
}
//deactivate it's mesh renderer, if it has one
if(meleeAttackBullet.gameObject.GetComponent<MeshRenderer>() != null)
{
meleeAttackBullet.gameObject.GetComponent<MeshRenderer>().enabled = false;
}
//deativate physx, if it has enabled
if(meleeAttackBullet.gameObject.GetComponent<Rigidbody>() != null)
{
meleeAttackBullet.gameObject.GetComponent<Rigidbody>().isKinematic = true;
}
//make it a child of the bullet position, as it has to swing with the attack
meleeAttackBullet.gameObject.transform.parent = GetComponent<AIWeaponShoot>().bulletPosition.transform;
//find and activate the correct animation
animation.clip = animClips[FindCorrectAnimationClip()].clip;
animation.Play();

		//Debug.LogError(meleeAttackBullet.transform.parent, meleeAttackBullet.transform.parent);

//wait until the animation ends, then destroy the bullet
yield return new WaitForSeconds(animation.clip.length);

Destroy(meleeAttackBullet.gameObject);
inMeleeAttack = false;

//deactivate arms
if(GetComponent<AIWeaponSelected>().objectHoldingGun != null)
		{
			GetComponent<AIWeaponSelected>().objectHoldingGun.GetComponent<AIControllerChild>().model.GetComponent<HandIK>().handToUseInCharacter = beforeHands;
		}
}



//CUSTOMIZE THIS TO SELECT ANIMATION CLIP
int FindCorrectAnimationClip()
{

//by default the animation clip will be selected randomly
int num = Mathf.FloorToInt(Random.Range(0, animClips.Count));
return num;

}


}
