using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof (AIWeaponControllerChild))]
public class AIWeaponControllerEditor : Editor {


//all the editable variables

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


//this is for the different foldouts
public bool showFoldout1 = false;
public bool showFoldout2 = false;
public bool showFoldout3 = false;
//public bool showFoldout4 = false;
// bool showFoldout5 = false;
//public bool showFoldout6 = false;
//public bool showFoldout7 = false;
//public bool showFoldout8 = false;
//public bool showFoldout9 = false;
// bool showFoldout10 = false;

//this if only for the script to work
public AIWeaponControllerChild childScript;

void OnEnable()
{
childScript = (AIWeaponControllerChild)target;
GetVariablesFromChild();
}

public override void OnInspectorGUI()
{
//this is so that it always updates correct target
childScript = (AIWeaponControllerChild)target;


EditorGUILayout.LabelField("Weapon Controller");
EditorGUILayout.Space();


showFoldout1 = EditorGUILayout.Foldout(showFoldout1, "Main Referencing Objects");

//main referencing objects
if(showFoldout1)
{
EditorGUILayout.Space();
bulletToUse = EditorGUILayout.ObjectField("Bullet Object" ,bulletToUse, typeof(Object), true) as GameObject;
bulletPosition = EditorGUILayout.ObjectField("Bullet Position Creation" ,bulletPosition, typeof(Object), true) as GameObject;
muzzleFlashManager = EditorGUILayout.ObjectField("Flash Managing Object" ,muzzleFlashManager, typeof(Object), true) as GameObject;
magazineObjectThrow = EditorGUILayout.ObjectField("Empty Magazine Object" ,magazineObjectThrow, typeof(Object), true) as GameObject;
magazineThrowPosition = EditorGUILayout.ObjectField("Magazine Creation Object" ,magazineThrowPosition, typeof(Object), true) as GameObject;
ikLeftHand = EditorGUILayout.ObjectField("IK Left Hand" ,ikLeftHand, typeof(Transform), true) as Transform;
ikRightHand = EditorGUILayout.ObjectField("IK Right Hand" ,ikRightHand, typeof(Transform), true) as Transform;
EditorGUILayout.Space();
}



showFoldout2 = EditorGUILayout.Foldout(showFoldout2, "Ammo Properties");

//ammo stuff
if(showFoldout2)
{
EditorGUILayout.Space();
ammoCurrent = EditorGUILayout.FloatField("Ammo In Current Magazine", ammoCurrent);
ammoInNewMagazine = EditorGUILayout.FloatField("Ammo In New Magazine", ammoInNewMagazine);
magazines = EditorGUILayout.FloatField("Amount Of Magazines", magazines);
secondsToReload = EditorGUILayout.FloatField("Seconds To Reload", secondsToReload);
throwForce = EditorGUILayout.FloatField("Magazine Throw Force", throwForce);
EditorGUILayout.Space();

}

showFoldout3 = EditorGUILayout.Foldout(showFoldout3, "Shooting Properties");

//shoot stuff
if(showFoldout3)
{
shootForce = EditorGUILayout.FloatField("Shoot Force", shootForce);
recoilVertical = EditorGUILayout.FloatField("Recoil Vertical", recoilVertical);
recoilHorizontal = EditorGUILayout.FloatField("Recoil Horizontal", recoilHorizontal);
recoilSpeedMax = EditorGUILayout.FloatField("Recoil Max Speed", recoilSpeedMax);
}

if(GUI.changed)
{

SetVariablesToChild();

}
else
{
GetVariablesFromChild();
}

}

public void GetVariablesFromChild()
{
childScript.GetVariablesFromWeapon();

ammoCurrent = childScript.ammoCurrent;
ammoInNewMagazine = childScript.ammoInNewMagazine;
magazines = childScript.magazines;
rateOfFire = childScript.rateOfFire;
secondsToReload = childScript.secondsToReload;
magazineObjectThrow = childScript.magazineObjectThrow;
throwForce = childScript.throwForce;
magazineThrowPosition = childScript.magazineThrowPosition;
ikLeftHand = childScript.ikLeftHand;
ikRightHand = childScript.ikRightHand;
bulletToUse = childScript.bulletToUse;
bulletPosition = childScript.bulletPosition;
shootForce = childScript.shootForce;
recoilVertical = childScript.recoilVertical;
recoilHorizontal = childScript.recoilHorizontal;
recoilSpeedMax = childScript.recoilSpeedMax;
muzzleFlashManager = childScript.muzzleFlashManager;


}
	
public void SetVariablesToChild()
{

childScript.ammoCurrent =ammoCurrent;
childScript.ammoInNewMagazine=ammoInNewMagazine;
childScript.magazines=magazines;
childScript.rateOfFire=rateOfFire;
childScript.secondsToReload=secondsToReload;
childScript.magazineObjectThrow=magazineObjectThrow;
childScript.throwForce=throwForce;
childScript.magazineThrowPosition=magazineThrowPosition;
childScript.ikLeftHand=ikLeftHand;
childScript.ikRightHand=ikRightHand;
childScript.bulletToUse=bulletToUse;
childScript.bulletPosition=bulletPosition;
childScript.shootForce=shootForce;
childScript.recoilVertical=recoilVertical;
childScript.recoilHorizontal=recoilHorizontal;
childScript.recoilSpeedMax=recoilSpeedMax;
childScript.muzzleFlashManager=muzzleFlashManager;

childScript.SetVariablesToWeapon();
}

}
