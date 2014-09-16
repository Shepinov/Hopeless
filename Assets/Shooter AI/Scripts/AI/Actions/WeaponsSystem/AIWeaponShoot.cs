//attach to each gun the ai can use
//this is the acutal bullet creation and maniuplation process

using UnityEngine;
using System.Collections;

public class AIWeaponShoot : MonoBehaviour {

public GameObject bulletToUse; //the bullet to use (prefab)
public GameObject bulletPosition; //where to create bullet
public float shootForce; //the force with which to propel the bullet at the start
public bool beingFired = false; //whether we're being fired or not
public float recoilVertical; //the vertical max recoil
public float recoilHorizontal; //the horizonatal max recoil
public float recoilSpeedMax; //the maximum recoil speed
public GameObject muzzleFlashManager; //the muzzle flash manager object

//this is for future tests
private float amountOfBulletsToCreate = 1; //how many bullets are created during each shot
private Vector3 shotOffset; //distance between bullets that are created, only use this if you're creating more than 1 bullet simultanously

//recoil vars
/*
private float recoil = 0.0f;
private float maxRecoil_x = -20f;
private float maxRecoil_y = 20f;
private float recoilSpeed = 2f;
*/
//private Vector3 initPos; //this is for recoil


//what to do when the wepon has to fire
	public void AIWeaponFireMain()
	{
		float bulletsCreated = 0;
		
		while(bulletsCreated < amountOfBulletsToCreate)
		{
			//make the bullet
			GameObject bulletCreated = Instantiate( bulletToUse, bulletPosition.transform.position, Quaternion.identity) as GameObject;
			//create muzzleflash
			muzzleFlashManager.GetComponent<MuzzleFlashManager>().CreateMuzzleFlash();
			//add forces
			bulletCreated.rigidbody.AddForce(transform.forward * shootForce);
			//set variables correctly
			if(bulletCreated.GetComponent<BulletHealthExtras>() != null)
			{
				bulletCreated.GetComponent<BulletHealthExtras>().tagOfFriendly = GetComponent<AIWeaponSelected>().objectHoldingGun.transform.tag;
			}
			//for multiple bullets
			bulletsCreated += 1;
		}
		//set the correct variables
		beingFired = true;
		//reset
		StartCoroutine("Reset");
	}
	
	
	IEnumerator Reset()
	{
		yield return new WaitForSeconds(0.1f);
		beingFired = false;
	}




}
