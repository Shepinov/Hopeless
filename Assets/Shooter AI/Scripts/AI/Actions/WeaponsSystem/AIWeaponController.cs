//attach to main ai object
//this is the controller for stuff like ammo etc..

using UnityEngine;
using System.Collections;

public class AIWeaponController : MonoBehaviour {
	
	public bool masterAllowedToShoot = true; //a permament variable controlling whether we're allowed to shoot or not
	public bool allowedToShoot = true; //whether allowed to shoot or not
	public float amountOfAmmo; //the amount of ammo we have, must be a large number at the start
	public GameObject weaponHoldingObject; //the weapon object that we're holding, insert here the weapon that we're holding
	public Transform weaponHoldingLocation; //the object where the weapon is to be held
	public GameObject weaponRotatingLocation; //where the pivot object is of the main weapon
	public float maxGunAngle = 45f; //the max angle at which the gun can freely rotate
	
	public Vector3 debugAimingPos; //the position at which we're aiming
	
	
	void Awake()
	{
		//put the weapon in the correct position
		if(weaponHoldingObject != null)
		{
			weaponHoldingObject.transform.parent = weaponHoldingLocation.transform;
			weaponHoldingObject.transform.localPosition = new Vector3(0,0,0);
			weaponHoldingObject.GetComponent<AIWeaponSelected>().objectHoldingGun = gameObject;
		}
		
	}
	
	//drop weapon, for example when we're dead or incapacitated
	public void DropWeapon()
	{
		//put the weapon in the correct position
		if(weaponHoldingObject != null)
		{
			weaponHoldingObject.transform.parent = null;
			weaponHoldingObject.GetComponent<AIWeaponSelected>().objectHoldingGun = null;
			weaponHoldingObject = null;
		}
	}
	
	
	
	//if we should shoot/attack; "ranged","melee" for type of attack
	public void ShootAi(string typeOfAttack)
	{
		if( masterAllowedToShoot == false)
		{
			return;
		}
		
		if(allowedToShoot == false)
		{
			return;
		}
		
		if(weaponHoldingObject == null)
		{
			return;
		}
		
		gameObject.SendMessage("AIShoot", typeOfAttack, SendMessageOptions.DontRequireReceiver);
		weaponHoldingObject.SendMessage("AIShoot", typeOfAttack);
		
	}
	
	
	public void AimAtTarget(Vector3 posToAim)
	{
		
		Vector3 finalAimingPos = posToAim;
		
		if( Vector3.Angle(posToAim, weaponRotatingLocation.transform.parent.forward) < maxGunAngle)
		{
	
			weaponRotatingLocation.transform.eulerAngles = Vector3.Lerp(weaponRotatingLocation.transform.eulerAngles, posToAim, 0.01f);
			
		}
		else
		{
			GetComponent<AIStateManager>().turnToEnemyWeaponAngle = true;
		}
		
		
		if(GetComponent<AIStateManager>().turnToEnemyWeaponAngle == true && Vector3.Angle(posToAim, weaponRotatingLocation.transform.parent.forward) < maxGunAngle)
		{
			GetComponent<AIStateManager>().turnToEnemyWeaponAngle = false;
		}
		
		
		weaponRotatingLocation.transform.LookAt(finalAimingPos);
		
		debugAimingPos = posToAim;
		
	}
	
	
	
	void Update()
	{
		WeaponConfigurations();
	}
	
	public void WeaponConfigurations()
	{
		//reload if we have no ammo left
		if(weaponHoldingObject != null && weaponHoldingObject.GetComponent<AIWeaponSelected>().ammoCurrent == 0f)
		{
			weaponHoldingObject.GetComponent<AIWeaponSelected>().StartCoroutine("AIReload");
		}
		
		//ik stuff here
		if(weaponHoldingObject != null)
		{
			//set all the correct variables
			GetComponent<AIStateManager>().animationManager.GetComponent<HandIK>().ikActive = true;
			GetComponent<AIStateManager>().animationManager.GetComponent<HandIK>().leftHandObj = weaponHoldingObject.GetComponent<AIWeaponSelected>().ikLeftHand;
			GetComponent<AIStateManager>().animationManager.GetComponent<HandIK>().rightHandObj = weaponHoldingObject.GetComponent<AIWeaponSelected>().ikRightHand;
		}
		else
		{
			GetComponent<AIMovementController>().animationManager.GetComponent<HandIK>().ikActive = false;
		}
		
		
	}
	
	
	void OnDrawGizmosSelected ()
	{
		
		//test
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(debugAimingPos, 0.7f);
		
	} 
	



}
