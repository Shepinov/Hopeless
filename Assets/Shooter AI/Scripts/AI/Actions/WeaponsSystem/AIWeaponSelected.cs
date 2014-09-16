//this script makes the weapon in the correct spot


using UnityEngine;
using System.Collections;

public class AIWeaponSelected : MonoBehaviour {

//main variables
public GameObject objectHoldingGun; //the main ai character holding the gun



//all the same vatriables as the weapon for the player
public float ammoCurrent; //the amount of ammo the current magazine has
public float ammoInNewMagazine; //the ammo in each new magazine
public float magazines = 0; //the amount of magazines

public float rateOfFireMin;
public float rateOfFireMax;
public float rateOfFire; //the rate of fire, seconds between each shot
public float secondsToReload; //the time to reload
public GameObject magazineObjectThrow; //the object to throw off to the side once we've reloaded
public float throwForce; //the force with which to throw the magazine
public GameObject magazineThrowPosition; //the position from where to throw the empty magazine
public string typeOfWeapon; //the tpe of weapon: "semi";"auto"
public Transform ikLeftHand; //the object where to put left hand, IK
public Transform ikRightHand; //the object where to put right hand, IK


private bool allowedToShoot = true; //this is to control the rate of fire
public bool reloading = false; //internal variable for controlling stuff during reloading


	void Awake()
	{
		//setup rate of fire
		if(rateOfFireMin == 0 && rateOfFireMax == 0)
		{
			rateOfFireMin = rateOfFire;
			rateOfFireMax = rateOfFire;
		}
	}


//the weapon has to shoot ai
public void AIShoot(string typeOfAttack)
{

if(typeOfAttack == "ranged")
{
//if we're attacking with ranged fire

//if we have ammo and we can fire
if(ammoCurrent > 0 && allowedToShoot == true)
{

//send fire weapon message
GetComponent<AIWeaponShoot>().AIWeaponFireMain();
//deduct ammo
ammoCurrent -= 1f;
objectHoldingGun.GetComponent<AIWeaponController>().amountOfAmmo -= 1f;
//wait a bit for the time for the next bullet to get in the chamber
allowedToShoot = false;
StartCoroutine("ResetRateOfFire");

}
}
else
{

if(GetComponent<AIWeaponMeleeAttack>().inMeleeAttack == false)
{
//else attack with melee
GetComponent<AIWeaponMeleeAttack>().StartCoroutine("MeleeAttack");
}

}

}

//call this function to reactivate shooting after taking a shot, used for rate of fire
	private IEnumerator ResetRateOfFire() 
	{
		// Maintains original rateOfFire variable incase new Min/Max are not used
		if(rateOfFireMin == 0 && rateOfFireMax ==0 && rateOfFire != 0) {
			yield return new WaitForSeconds(rateOfFire);
		} else {
			yield return new WaitForSeconds(Random.Range(rateOfFireMin, rateOfFireMax));
		}
		allowedToShoot = true;
	}
	




//AI reload function
public IEnumerator AIReload()
{
if(magazines > 0 && reloading == false)
{
reloading = true;

//deactivate shooting
allowedToShoot = false;
//make the reload wait time
yield return new WaitForSeconds(secondsToReload);

			if( magazineObjectThrow != null)
			{
				//create the magazine that gets thrown off to the side
				GameObject magazineCreated = Instantiate( magazineObjectThrow, magazineThrowPosition.transform.position, Quaternion.identity) as GameObject;
				//add force
				magazineCreated.rigidbody.AddForce((transform.right+(transform.up*2f)) * throwForce);
			}
			
//setting everything correct
allowedToShoot = true;
ammoCurrent = ammoInNewMagazine;
reloading = false;
}
}


void Update()
{

if(objectHoldingGun != null)
{
//determine how much ammo/magazines we have, this also automatically compensates for when the magazines are empty, so we don't have to do it in the reload function
magazines = Mathf.Ceil(objectHoldingGun.GetComponent<AIWeaponController>().amountOfAmmo/ammoInNewMagazine);
}

//determine if we need to activate physics or deactivate them, depending whether the ai is holding the gun or not
if(objectHoldingGun == null && GetComponent<Collider>().enabled == false)
{
ActivatePhysics();
}
if(objectHoldingGun != null && GetComponent<Collider>().enabled == true)
{
DeactivatePhysics();
}

}



 
//this is if you want to activate physics back and interact with the environment
void ActivatePhysics()
{
GetComponent<Collider>().enabled = true;
GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
}

//if you want the gun to stop interacting with the environment
void DeactivatePhysics()
{
GetComponent<Collider>().enabled = false;
GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
}




}
