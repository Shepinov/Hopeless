using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//[ExecuteInEditMode]

[HideInInspector]

[CustomEditor( typeof (AIControllerChild))]

public class AIControllerEditor : Editor {

//all the variables are stored as public and are updated automatically


//main objects that are crucial for the ai brain to function
public GameObject patrolManager; //the patrol manager
public GameObject sensorParent; //the parent that holds the sensors
public GameObject ears; //the ears object
public GameObject sight; //the sight object
public GameObject model; //the object that holds the model

//data about self, eg height, health
public Vector3 eyeHeight; //the height of the eyes
public float health; //the amount of health the ai has at the start
public float disengagingHitsToKnockout; //the amount of disengaging hits the ai can take until it gets disengaged
public float radius; //the size of our radius for the navmesh agent
public float height; //our height, for the nav mesh agent

//data about enemies, such as height, tag etc..
public string tagOfEnemy; //the tag of the enemy
public string tagOfBullet; //the tag of the object that shows danger, eg bullets
public Vector3 enemyCriticalHeight; //the height at which the ai should aim
public float initAmmo; //the amount of ammo at the start

//reaction times, how quick does the ai react etc...
public float shockTime; //seconds, how quickly we react to seeing an enemy, reference and manipulated by anderenaline
public float freezeTime; //seconds, how long we freeze when we hear a bullet
public float minCoverTime; //seconds, minimum amount of time to hide in cover
public float maxCoverTime; //seconds, maximum amount of time to hide in cover
public float timeBetweenEnemyChecks; //seconds, amount of time when we check whether the others are a danger or not
public float timeForGivingUpDuringEngagement; //seconds, the amount of time the ai will try and locate the enemy during an engagment before giving up and going back to patrol
public float timeForGivingUpSeeking; //frames, the amount of time the ai will try and locate the enemy if we suddenly see somebody else before giving up and going back to patrol 

//emotion control
public float initAndrenaline; //the amount of andrenaline we start off with
public float initFear; //the amount of fear the ai starts off with
public float chanceForFight; //the percentage for fight or flight instict, reference manipulated by andrenaline

//weapons and engagement
public GameObject weapon; //the weapon object
	public List<GameObject> otherWeapons = new List<GameObject>(); //other weapons
	public List<int> otherWeaponsMelee = new List<int>(); //the other weapons' melee state
public Transform weaponHoldingLocation; //the object where the weapon is held
public List<string> targetCheck = new List<string>(); //add this script to object; once we see the player we will run this script with void "VisualCheck", if it returns true we engage the player
public List<float> targetVisualCheckChance = new List<float>(); //the chance for the visual check
public float distanceToEngageCloseCombatLogic; //the distance at which to engage
public float offsetFactor; //the offset factor

//speed references
public float refSpeedPatrol; //the reference speed for the patrol
public float refSpeedEngage; //the reference speed for when the ai engages
public float refSpeedChase; //the reference speed for chase
public float refSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
public GameObject modelParentOfAllBones; //the parent object that takes in all bones
public HandToUse handToUseInCharacter; //hand to use	

//patrol sutff
public GameObject[] waypointList; //the list that contains all waypoinjts in correct order
public float distanceToWaypointForRegistering; //how far we have to be from the waypoint for it to register that we are at it
public NavMesh navmeshToUse; //the navmesh to use
public float patrolMinDistanceToDestination; //this is the min disance to destination
public float patrolFramesCriticalCheck; //the amount of frames when to check to see if we're at the destination
public float patrolChecksCritical; //the amount of checks at the destination until we're there

//optimisation stuff
public float coverAmountOfRays; //the amount of rays that should be used to sample cover (the more the better, more slower)
public float coverFieldOfView; //the field of view from which we find cove (recommend 360 for most cases)
public float coverDistanceToCheck; //how far should the rays shoot; usually some arbitary large number 
public float patrolTickBarrier; //this is how often the ai brain should check the patroling
public float coverTrueCoverTest; //the amount of tries to take for spotting true cover
public LayerMask coverLayerMask; //the cover layer mask

//melee stuff
public int meleeSetting; //the setting of none,hybrid,only
public float distanceForMeleeAttack; //the distance for melee attack


//this if only for the script to work
public AIControllerChild childScript;


//this is for the different foldouts
public bool showFoldout1 = false;
public bool showFoldout2 = false;
public bool showFoldout3 = false;
public bool showFoldout4 = false;
public bool showFoldout5 = false;
public bool showFoldout6 = false;
public bool showFoldout7 = false;
public bool showFoldout8 = false;
public bool showFoldout9 = false;
public bool showFoldout10 = false;

public bool showFoldout11 = false;

private string[] meleeOptions = {"No melee attack","Ranged weapons and melee attack","Only melee attack"};
private Vector2 multipleWeaponsScroll = Vector2.zero;


void OnEnable()
{
childScript = (AIControllerChild)target;
GetVariablesFromChild();
}

public override void OnInspectorGUI()
{
//this is so that it always updates correct target
childScript = (AIControllerChild)target;


EditorGUILayout.LabelField("AI Property Controller");
EditorGUILayout.Space();


		if(PrefabUtility.GetPrefabType(childScript.gameObject) == PrefabType.Prefab)
		{
			EditorGUILayout.LabelField("Parameters can't be edited using the Overview Panel if the AI is a prefab.");
			return;
		}



showFoldout1 = EditorGUILayout.Foldout(showFoldout1, "Main Referencing Objects");

//main referencing objects
if(showFoldout1)
{
EditorGUILayout.Space();
patrolManager = EditorGUILayout.ObjectField("Patrol Manager" ,patrolManager, typeof(Object), true) as GameObject;
sensorParent = EditorGUILayout.ObjectField("Sensor Parent" ,sensorParent, typeof(Object), true) as GameObject;
ears = EditorGUILayout.ObjectField("Ears Object" ,ears, typeof(Object), true) as GameObject;
sight = EditorGUILayout.ObjectField("Sight Object" ,sight, typeof(Object), true) as GameObject;
model = EditorGUILayout.ObjectField("Model Object" ,model, typeof(Object), true) as GameObject;
EditorGUILayout.Space();
}
		
showFoldout2 = EditorGUILayout.Foldout(showFoldout2, "Data About AI Character");

//Data About AI Character
if(showFoldout2)
{
EditorGUILayout.Space();
eyeHeight = EditorGUILayout.Vector3Field("Relative Eye Height", eyeHeight);
EditorGUILayout.Space();
health = EditorGUILayout.FloatField("Health", health);
EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Disengaging Hits To Knockout");
disengagingHitsToKnockout = EditorGUILayout.FloatField(disengagingHitsToKnockout);
EditorGUILayout.EndHorizontal();
radius = EditorGUILayout.FloatField("Radius", radius);
height = EditorGUILayout.FloatField("Height", height);
EditorGUILayout.Space();
}

showFoldout3 = EditorGUILayout.Foldout(showFoldout3, "Data About Enemies");

//Data About Enemies
if(showFoldout3)
{
EditorGUILayout.Space();
tagOfEnemy = EditorGUILayout.TagField("Tag Of Enemy", tagOfEnemy);
tagOfBullet = EditorGUILayout.TagField("Tag Of Bullet", tagOfBullet);
enemyCriticalHeight = EditorGUILayout.Vector3Field("Enemy Critical Height", enemyCriticalHeight);
EditorGUILayout.Space();
}

showFoldout4 = EditorGUILayout.Foldout(showFoldout4, "Reaction Times");

//Reaction Times
if(showFoldout4)
{
EditorGUILayout.Space();
shockTime = EditorGUILayout.FloatField("Reaction To Enemy", shockTime);
freezeTime = EditorGUILayout.FloatField("Freeze Time To Bullet", freezeTime);
minCoverTime = EditorGUILayout.FloatField("Minimum Time In Cover", minCoverTime);
maxCoverTime = EditorGUILayout.FloatField("Maximum Time In Cover", maxCoverTime);
timeBetweenEnemyChecks = EditorGUILayout.FloatField("Enemy Checks", timeBetweenEnemyChecks);
timeForGivingUpDuringEngagement = EditorGUILayout.FloatField("Giving Up Engage Time", timeForGivingUpDuringEngagement);
timeForGivingUpSeeking = EditorGUILayout.FloatField("(Frames)Give up Seeking", timeForGivingUpSeeking);
EditorGUILayout.Space();
}

showFoldout5 = EditorGUILayout.Foldout(showFoldout5, "Emotion Control");

//Emotion Control
if(showFoldout5)
{
EditorGUILayout.Space();
initAndrenaline = EditorGUILayout.FloatField("Andrenaline", initAndrenaline);
initFear = EditorGUILayout.FloatField("Fear", initFear);
chanceForFight = EditorGUILayout.FloatField("ChanceForFight", chanceForFight);
EditorGUILayout.Space();
}

showFoldout6 = EditorGUILayout.Foldout(showFoldout6, "Weapons And Engagment");

//Weapons And Engagment
if(showFoldout6)
{
EditorGUILayout.Space();

if(GUILayout.Button("Hold No Weapon"))
{
weapon = null;
}
			weaponHoldingLocation = EditorGUILayout.ObjectField("Weapon Holding Location" ,weaponHoldingLocation, typeof(Transform), true) as Transform;
			
			weapon = EditorGUILayout.ObjectField("Main Weapon" ,weapon, typeof(Object), true) as GameObject;
			
			
			if(GUILayout.Button("Add more secondary weapons"))
			{
				otherWeapons.Add( null );
				otherWeaponsMelee.Add( 0);
			}
		
			multipleWeaponsScroll = EditorGUILayout.BeginScrollView( multipleWeaponsScroll, GUILayout.Height(100f), GUILayout.Width(300f));
			
			for(int x = 0; x < otherWeapons.Count; x++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Weapon number: " + (x+1).ToString() );
				
				otherWeapons[x] = EditorGUILayout.ObjectField( otherWeapons[x], typeof(Object), true) as GameObject;
				otherWeaponsMelee[x] = EditorGUILayout.Popup( otherWeaponsMelee[x], meleeOptions);
				
				if(GUILayout.Button("X"))
				{
					otherWeapons.RemoveAt(x);
					otherWeaponsMelee.RemoveAt(x);
				}
				
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Engagement Script");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical();
			if(GUILayout.Button("Add Script"))
			{
				targetCheck.Add("Empty");
				targetVisualCheckChance.Add(0f);
			}
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Name Of Script");
			EditorGUILayout.LabelField("% Chance");
			EditorGUILayout.EndHorizontal();
			for(int x = 0; x < targetCheck.Count; x++)
			{
				EditorGUILayout.BeginHorizontal();
				targetCheck[x] = EditorGUILayout.TextField(targetCheck[x]);
				targetVisualCheckChance[x] = EditorGUILayout.FloatField(targetVisualCheckChance[x]);
				if(GUILayout.Button("X"))
				{
					targetCheck.RemoveAt(x);
					targetVisualCheckChance.RemoveAt(x);
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical(); 
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance For Close Combat Logic");
			distanceToEngageCloseCombatLogic = EditorGUILayout.FloatField(distanceToEngageCloseCombatLogic);
			EditorGUILayout.EndHorizontal();
			initAmmo = EditorGUILayout.FloatField("Amount Of Ammo", initAmmo);
			EditorGUILayout.Space();
			offsetFactor = EditorGUILayout.FloatField("Factor for accuracy", offsetFactor);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
}

showFoldout7 = EditorGUILayout.Foldout(showFoldout7, "Speed References");

//Speed References
if(showFoldout7)
{
EditorGUILayout.Space();
refSpeedPatrol = EditorGUILayout.FloatField("Patrol Reference Speed", refSpeedPatrol);
refSpeedEngage = EditorGUILayout.FloatField("Engage Reference Speed", refSpeedEngage);
refSpeedCover = EditorGUILayout.FloatField("Cover Reference Speed", refSpeedCover);
refSpeedChase = EditorGUILayout.FloatField("Chase Reference Speed", refSpeedChase);
EditorGUILayout.Space();
}

showFoldout8 = EditorGUILayout.Foldout(showFoldout8, "Model Management");

//Model Management
if(showFoldout8)
{
EditorGUILayout.Space();
//modelParentOfAllBones = EditorGUILayout.ObjectField("Parent Of Bones" , modelParentOfAllBones, typeof(Object), true) as GameObject;
handToUseInCharacter = (HandToUse)EditorGUILayout.EnumPopup("Hand To Hold Gun", handToUseInCharacter);
EditorGUILayout.Space();
}

showFoldout9 = EditorGUILayout.Foldout(showFoldout9, "Patrol Management");

//Patrol Management
if(showFoldout9)
{
EditorGUILayout.Space();
EditorGUILayout.LabelField("WAYPOINT MANAGEMENT IN PATROL MANAGER", EditorStyles.boldLabel);
EditorGUILayout.Space();
EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Distance To Register Waypoint");
distanceToWaypointForRegistering = EditorGUILayout.FloatField(distanceToWaypointForRegistering);
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Min Distance To Destination");
patrolMinDistanceToDestination = EditorGUILayout.FloatField(patrolMinDistanceToDestination);
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Frames To Check Distance");
patrolFramesCriticalCheck = EditorGUILayout.FloatField(patrolFramesCriticalCheck);
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Checks Until Registering Destination");
patrolChecksCritical = EditorGUILayout.FloatField(patrolChecksCritical);
EditorGUILayout.EndHorizontal();

EditorGUILayout.Space();
}

showFoldout10 = EditorGUILayout.Foldout(showFoldout10, "Optimisation");

//Optimisation
if(showFoldout10)
{
EditorGUILayout.Space();
coverAmountOfRays = EditorGUILayout.FloatField("Cover: Amount Of Rays", coverAmountOfRays);
coverFieldOfView = EditorGUILayout.FloatField("Cover: Fielf Of View", coverFieldOfView);
coverDistanceToCheck = EditorGUILayout.FloatField("Cover: Distance To Check", coverDistanceToCheck);
patrolTickBarrier = EditorGUILayout.FloatField("Patrol: Frames To Check", patrolTickBarrier);
coverTrueCoverTest = EditorGUILayout.FloatField("Patrol: Extra Checks", coverTrueCoverTest);

/*
EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Cover Layermask: ");
coverLayerMask = EditorGUILayout.MaskField( coverLayerMask, );
EditorGUILayout.EndHorizontal();*/

EditorGUILayout.Space();
}

showFoldout11 = EditorGUILayout.Foldout(showFoldout11, "Melee Settings");
//melee stuff
if(showFoldout11)
{
EditorGUILayout.Space();
meleeSetting = EditorGUILayout.Popup(meleeSetting, meleeOptions);
if(meleeSetting != 0)
{
distanceForMeleeAttack = EditorGUILayout.FloatField("Distance for melee attack", distanceForMeleeAttack);
}
EditorGUILayout.Space();
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


void GetVariablesFromChild()
{

childScript.GetVariablesFromAI();

//main objects that are crucial for the ai brain to function
patrolManager = childScript.patrolManager; //the patrol manager
sensorParent = childScript.sensorParent; //the parent that holds the sensors
ears = childScript.ears; //the ears object
sight = childScript.sight; //the sight object
model = childScript.model; //the object that holds the model

//data about self, eg height, health
eyeHeight = childScript.eyeHeight; //the height of the eyes
health = childScript.health; //the amount of health the ai has at the start
disengagingHitsToKnockout = childScript.disengagingHitsToKnockout; //the amount of disengaging hits the ai can take until it gets disengaged
radius = childScript.radius; //the size of our radius for the navmesh agent
height = childScript.height; //our height, for the nav mesh agent

//data about enemies, such as height, tag etc..
tagOfEnemy = childScript.tagOfEnemy; //the tag of the enemy
tagOfBullet = childScript.tagOfBullet; //the tag of the object that shows danger, eg bullets
enemyCriticalHeight = childScript.enemyCriticalHeight; //the height at which the ai should aim
initAmmo = childScript.initAmmo; //the amount of ammo at the start

//reaction times, how quick does the ai react etc...
shockTime = childScript.shockTime; //seconds, how quickly we react to seeing an enemy, reference and manipulated by anderenaline
freezeTime = childScript.freezeTime; //seconds, how long we freeze when we hear a bullet
minCoverTime = childScript.minCoverTime; //seconds, minimum amount of time to hide in cover
maxCoverTime = childScript.maxCoverTime; //seconds, maximum amount of time to hide in cover
timeBetweenEnemyChecks = childScript.timeBetweenEnemyChecks; //seconds, amount of time when we check whether the others are a danger or not
timeForGivingUpDuringEngagement = childScript.timeForGivingUpDuringEngagement; //seconds, the amount of time the ai will try and locate the enemy during an engagment before giving up and going back to patrol
timeForGivingUpSeeking = childScript.timeForGivingUpSeeking; //frames, the amount of time the ai will try and locate the enemy if we suddenly see somebody else before giving up and going back to patrol 

//emotion control
initAndrenaline = childScript.initAndrenaline; //the amount of andrenaline we start off with
initFear = childScript.initFear; //the amount of fear the ai starts off with
chanceForFight = childScript.chanceForFight; //the percentage for fight or flight instict, reference manipulated by andrenaline

//weapons and engagement
weapon = childScript.weapon; //the weapon object
weaponHoldingLocation = childScript.weaponHoldingLocation; //the object where the weapon is held
otherWeapons = childScript.otherWeapons;
otherWeaponsMelee = childScript.otherWeaponsMelee;
targetCheck = childScript.targetCheck; //the name of the script that return whether the ai should attack or not
targetVisualCheckChance = childScript.targetVisualCheckChance;
distanceToEngageCloseCombatLogic = childScript.distanceToEngageCloseCombatLogic; //the distance at which to engage
offsetFactor = childScript.offsetFactor;

//speed references
refSpeedPatrol = childScript.refSpeedPatrol; //the reference speed for the patrol
refSpeedEngage = childScript.refSpeedEngage; //the reference speed for when the ai engages
refSpeedChase = childScript.refSpeedChase; //the reference speed for chase
refSpeedCover = childScript.refSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
modelParentOfAllBones = childScript.modelParentOfAllBones; //the parent object that takes in all bones
handToUseInCharacter = childScript.handToUseInCharacter;		

//patrol sutff
waypointList = childScript.waypointList; //the list that contains all waypoinjts in correct order
distanceToWaypointForRegistering = childScript.distanceToWaypointForRegistering; //how far we have to be from the waypoint for it to register that we are at it
navmeshToUse = childScript.navmeshToUse; //the navmesh to use
patrolMinDistanceToDestination = childScript.patrolMinDistanceToDestination;
patrolFramesCriticalCheck = childScript.patrolFramesCriticalCheck;
patrolChecksCritical = childScript.patrolChecksCritical;

//optimisation stuff
coverAmountOfRays = childScript.coverAmountOfRays; //the amount of rays that should be used to sample cover (the more the better, more slower)
coverFieldOfView = childScript.coverFieldOfView; //the field of view from which we find cove (recommend 360 for most cases)
coverDistanceToCheck = childScript.coverDistanceToCheck; //how far should the rays shoot; usually some arbitary large number 
patrolTickBarrier = childScript.patrolTickBarrier; //this is how often the ai brain should check the patroling
coverTrueCoverTest = childScript.coverTrueCoverTest; 


//melee stuff
meleeSetting = childScript.meleeSetting;
distanceForMeleeAttack = childScript.distanceForMeleeAttack;

}

void SetVariablesToChild()
{

//main objects that are crucial for the ai brain to function
childScript.patrolManager = patrolManager; //the patrol manager
childScript.sensorParent = sensorParent; //the parent that holds the sensors
childScript.ears = ears; //the ears object
childScript.sight = sight; //the sight object
childScript.model = model; //the object that holds the model

//data about self, eg height, health
childScript.eyeHeight = eyeHeight; //the height of the eyes
childScript.health = health; //the amount of health the ai has at the start
childScript.disengagingHitsToKnockout = disengagingHitsToKnockout; //the amount of disengaging hits the ai can take until it gets disengaged
childScript.radius = radius; //the size of our radius for the navmesh agent
childScript.height = height; //our height, for the nav mesh agent

//data about enemies, such as height, tag etc..
childScript.tagOfEnemy = tagOfEnemy; //the tag of the enemy
childScript.tagOfBullet = tagOfBullet; //the tag of the object that shows danger, eg bullets
childScript.enemyCriticalHeight = enemyCriticalHeight; //the height at which the ai should aim
childScript.initAmmo = initAmmo; //the amount of ammo at the start

//reaction times, how quick does the ai react etc...
childScript.shockTime = shockTime; //seconds, how quickly we react to seeing an enemy, reference and manipulated by anderenaline
childScript.freezeTime = freezeTime; //seconds, how long we freeze when we hear a bullet
childScript.minCoverTime = minCoverTime; //seconds, minimum amount of time to hide in cover
childScript.maxCoverTime = maxCoverTime; //seconds, maximum amount of time to hide in cover
childScript.timeBetweenEnemyChecks = timeBetweenEnemyChecks; //seconds, amount of time when we check whether the others are a danger or not
childScript.timeForGivingUpDuringEngagement = timeForGivingUpDuringEngagement; //seconds, the amount of time the ai will try and locate the enemy during an engagment before giving up and going back to patrol
childScript.timeForGivingUpSeeking = timeForGivingUpSeeking; //frames, the amount of time the ai will try and locate the enemy if we suddenly see somebody else before giving up and going back to patrol 

//emotion control
childScript.initAndrenaline = initAndrenaline; //the amount of andrenaline we start off with
childScript.initFear = initFear; //the amount of fear the ai starts off with
childScript.chanceForFight = chanceForFight; //the percentage for fight or flight instict, reference manipulated by andrenaline

//weapons and engagement
childScript.weapon = weapon; //the weapon object
childScript.weaponHoldingLocation = weaponHoldingLocation; //the object where the weapon is held
childScript.otherWeapons = otherWeapons;
childScript.otherWeaponsMelee = otherWeaponsMelee;
childScript.targetCheck = targetCheck; //the name of the script that return whether the ai should attack or not
childScript.targetVisualCheckChance = targetVisualCheckChance;
childScript.distanceToEngageCloseCombatLogic = distanceToEngageCloseCombatLogic; //the distance at which to engage
childScript.offsetFactor = offsetFactor;

//speed references
childScript.refSpeedPatrol = refSpeedPatrol; //the reference speed for the patrol
childScript.refSpeedEngage = refSpeedEngage; //the reference speed for when the ai engages
childScript.refSpeedChase = refSpeedChase; //the reference speed for chase
childScript.refSpeedCover = refSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
childScript.modelParentOfAllBones = modelParentOfAllBones; //the parent object that takes in all bones
childScript.handToUseInCharacter = handToUseInCharacter;		

//patrol sutff
childScript.waypointList = waypointList; //the list that contains all waypoinjts in correct order
childScript.distanceToWaypointForRegistering = distanceToWaypointForRegistering; //how far we have to be from the waypoint for it to register that we are at it
childScript.navmeshToUse = navmeshToUse; //the navmesh to use
childScript.patrolMinDistanceToDestination = patrolMinDistanceToDestination;
childScript.patrolFramesCriticalCheck = patrolFramesCriticalCheck;
childScript.patrolChecksCritical = patrolChecksCritical;

//optimisation stuff
childScript.coverAmountOfRays = coverAmountOfRays; //the amount of rays that should be used to sample cover (the more the better, more slower)
childScript.coverFieldOfView = coverFieldOfView; //the field of view from which we find cove (recommend 360 for most cases)
childScript.coverDistanceToCheck = coverDistanceToCheck; //how far should the rays shoot; usually some arbitary large number 
childScript.patrolTickBarrier = patrolTickBarrier; //this is how often the ai brain should check the patroling
childScript.coverTrueCoverTest = coverTrueCoverTest; 

//melee stuff
childScript.meleeSetting = meleeSetting;
childScript.distanceForMeleeAttack = distanceForMeleeAttack;

childScript.SetVariablesToAI();
}



}
