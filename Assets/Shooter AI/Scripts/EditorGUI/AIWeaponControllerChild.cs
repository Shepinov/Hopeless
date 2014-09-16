using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class AIWeaponControllerChild : MonoBehaviour {

public float ammoCurrent; //the amount of ammo the current magazine has
public float ammoInNewMagazine; //the ammo in each new magazine
public float magazines = 0; //the amount of magazines
public float rateOfFire; //the rate of fire, seconds between each shot
public float secondsToReload; //the time to reload
public GameObject magazineObjectThrow; //the object to throw off to the side once we've reloaded
public float throwForce; //the force with which to throw the magazine
public GameObject magazineThrowPosition; //the position from where to throw the empty magazine
public Transform ikLeftHand; //the object where to put left hand, IK
public Transform ikRightHand; //the object where to put right hand, IK

public GameObject bulletToUse; //the bullet to use (prefab)
public GameObject bulletPosition; //where to create bullet
public float shootForce; //the force with which to propel the bullet at the start
public float recoilVertical; //the vertical max recoil
public float recoilHorizontal; //the horizonatal max recoil
public float recoilSpeedMax; //the maximum recoil speed
public GameObject muzzleFlashManager; //the muzzle flash manager object

public void GetVariablesFromWeapon()
{

AIWeaponSelected ais = GetComponent<AIWeaponSelected>();
AIWeaponShoot aws = GetComponent<AIWeaponShoot>();

ammoCurrent = ais.ammoCurrent;
ammoInNewMagazine = ais.ammoInNewMagazine;
magazines = ais.magazines;
rateOfFire = ais.rateOfFire;
secondsToReload = ais.secondsToReload;
magazineObjectThrow = ais.magazineObjectThrow;
throwForce = ais.throwForce;
magazineThrowPosition = ais.magazineThrowPosition;
ikLeftHand = ais.ikLeftHand;
ikRightHand = ais.ikRightHand;
bulletToUse = aws.bulletToUse;
bulletPosition = aws.bulletPosition;
shootForce = aws.shootForce;
recoilVertical = aws.recoilVertical;
recoilHorizontal = aws.recoilHorizontal;
recoilSpeedMax = aws.recoilSpeedMax;
muzzleFlashManager = aws.muzzleFlashManager;

}

public void SetVariablesToWeapon()
{
PrefabReset();

AIWeaponSelected ais = GetComponent<AIWeaponSelected>();
AIWeaponShoot aws = GetComponent<AIWeaponShoot>();

ais.ammoCurrent =ammoCurrent;
ais.ammoInNewMagazine=ammoInNewMagazine;
ais.magazines=magazines;
ais.rateOfFire=rateOfFire;
ais.secondsToReload=secondsToReload;
ais.magazineObjectThrow=magazineObjectThrow;
ais.throwForce=throwForce;
ais.magazineThrowPosition=magazineThrowPosition;
ais.ikLeftHand=ikLeftHand;
ais.ikRightHand=ikRightHand;
aws.bulletToUse=bulletToUse;
aws.bulletPosition=bulletPosition;
aws.shootForce=shootForce;
aws.recoilVertical=recoilVertical;
aws.recoilHorizontal=recoilHorizontal;
aws.recoilSpeedMax=recoilSpeedMax;
aws.muzzleFlashManager=muzzleFlashManager;
}

public void PrefabReset()
{
#if UNITY_EDITOR

if(PrefabUtility.GetPrefabType(gameObject) != PrefabType.None)
{
PrefabUtility.DisconnectPrefabInstance(gameObject);
}
#endif

}




}
