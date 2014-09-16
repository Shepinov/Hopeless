//this is a script that facilitates the "ai controller editor" main editor script, for functions such  as setting the variabkes etc..

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIControllerChild : MonoBehaviour {


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
public HandToUse handToUseInCharacter;

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
	


//this gets all the vairables to correct values
public void GetVariablesFromAI()
{

		//to avoid complicating stuff in the prefab
		#if UNITY_EDITOR
		if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab)
		{
			return;
		}
		#endif


if(gameObject.activeSelf == true)
{

//main objects that are crucial for the ai brain to function
patrolManager = GetComponent<AIStateManager>().patrolManager; //the patrol manager
sensorParent = GetComponent<AIStateManager>().ears.transform.parent.gameObject; //the parent that holds the sensors
ears = GetComponent<AIStateManager>().ears; //the ears object
sight = GetComponent<AIStateManager>().sight; //the sight object
model = GetComponent<AIStateManager>().animationManager;

//data about self, eg height, health
eyeHeight = GetComponent<SearchCover>().eyePosition; //the height of the eyes
health = GetComponent<AIStateManager>().healthManager.GetComponent<HealthManager>().health; //the amount of health the ai has at the start
disengagingHitsToKnockout = GetComponentInChildren<AIHealthExecuter>().disengagingHitsToKnockout; //the amount of disengaging hits the ai can take until it gets disengaged

if(GetComponent<NavMeshAgent>() != null)
{
radius = GetComponent<NavMeshAgent>().radius; //the size of our radius for the navmesh agent
height = GetComponent<NavMeshAgent>().height; //our height, for the nav mesh agent
}



//data about enemies, such as height, tag etc..
tagOfEnemy = GetComponent<AIStateManager>().tagToAvoidPrimary; //the tag of the enemy
tagOfBullet = GetComponent<AIStateManager>().tagToAvoidSecondary; //the tag of the object that shows danger, eg bullets
enemyCriticalHeight = GetComponent<AIStateManager>().enemyHeight; //the height at which the ai should aim
initAmmo = GetComponent<AIWeaponController>().amountOfAmmo; //the amount of ammo at the start

//reaction times, how quick does the ai react etc...
shockTime = GetComponent<AIStateManager>().shockTime; //seconds, how quickly we react to seeing an enemy, reference and manipulated by anderenaline
freezeTime = GetComponent<AIStateManager>().freezeTime; //seconds, how long we freeze when we hear a bullet
minCoverTime = GetComponent<AIStateManager>().minCoverTime; //seconds, minimum amount of time to hide in cover
maxCoverTime = GetComponent<AIStateManager>().maxCoverTime; //seconds, maximum amount of time to hide in cover
timeBetweenEnemyChecks = GetComponent<AIStateManager>().timeUntilNextCheck; //seconds, amount of time when we check whether the others are a danger or not
timeForGivingUpDuringEngagement = GetComponent<AIStateManager>().investigationTime; //seconds, the amount of time the ai will try and locate the enemy during an engagment before giving up and going back to patrol
timeForGivingUpSeeking = GetComponent<AIStateManager>().timeBarrierEngageToInvestigate; //frames, the amount of time the ai will try and locate the enemy if we suddenly see somebody else before giving up and going back to patrol 

//emotion control
initAndrenaline = GetComponent<AIStateManager>().andrenaline; //the amount of andrenaline we start off with
initFear = GetComponent<AIStateManager>().fear; //the amount of fear the ai starts off with
chanceForFight = GetComponent<AIStateManager>().chanceForFight; //the percentage for fight or flight instict, reference manipulated by andrenaline

//weapons and engagement
weapon = GetComponent<AIWeaponController>().weaponHoldingObject; //the weapon object
			otherWeapons = GetComponent<AIStateManager>().otherWeapons; //other weapons
			otherWeaponsMelee = GetComponent<AIStateManager>().otherWeaponsMelee; //other weapons melee
weaponHoldingLocation = GetComponent<AIWeaponController>().weaponHoldingLocation; //the object where the weapon is held
targetCheck = GetComponent<AIStateManager>().targetVisualCheck; //the name of the script that return whether the ai should attack or not
targetVisualCheckChance = GetComponent<AIStateManager>().targetVisualCheckChance;
distanceToEngageCloseCombatLogic = GetComponent<AIStateManager>().distanceToStopWalking; //the distance at which to engage
offsetFactor = GetComponent<AIStateManager>().offsetFactor;


//speed references
refSpeedPatrol = GetComponent<AIStateManager>().maxSpeedPatrol; //the reference speed for the patrol
refSpeedEngage = GetComponent<AIStateManager>().maxSpeedEngage; //the reference speed for when the ai engages
refSpeedChase = GetComponent<AIStateManager>().maxSpeedChase; //the reference speed for chase
refSpeedCover = GetComponent<AIStateManager>().maxSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
modelParentOfAllBones = model.GetComponent<RagdollTransitions>().boneParent; //the parent object that takes in all bones
handToUseInCharacter = model.GetComponent<HandIK>().handToUseInCharacter;


//patrol sutff
			if(patrolManager.GetComponent<PatrolManager>() != null)
			{
				waypointList = patrolManager.GetComponent<PatrolManager>().waypointList; //the list that contains all waypoinjts in correct order
				distanceToWaypointForRegistering = patrolManager.GetComponent<PatrolManager>().criticalDistanceToWaypoint; //how far we have to be from the waypoint for it to register that we are at it
				navmeshToUse = patrolManager.GetComponent<PatrolManager>().meshNav; //the navmesh to use
			}
			
			if(GetComponent<AIMovementController>() != null)
			{
				patrolMinDistanceToDestination = GetComponent<AIMovementController>().minDistanceToDestination;
				patrolFramesCriticalCheck = GetComponent<AIMovementController>().framesCriticalCheck;
				patrolChecksCritical = GetComponent<AIMovementController>().checksCriticalUntilStop;
			}
			if(GetComponent<AIMovementControllerASTAR>() != null)
			{
				patrolMinDistanceToDestination = GetComponent<AIMovementControllerASTAR>().minDistanceToDestination;
				patrolFramesCriticalCheck = GetComponent<AIMovementControllerASTAR>().framesCriticalCheck;
				patrolChecksCritical = GetComponent<AIMovementControllerASTAR>().checksCriticalUntilStop;
			}
		

//optimisation stuff
coverAmountOfRays = GetComponent<SearchCover>().amountOfRays; //the amount of rays that should be used to sample cover (the more the better, more slower)
coverFieldOfView = GetComponent<SearchCover>().fieldOfRays; //the field of view from which we find cove (recommend 360 for most cases)
coverDistanceToCheck = GetComponent<SearchCover>().distanceToCheck; //how far should the rays shoot; usually some arbitary large number 
patrolTickBarrier = GetComponent<AIStateManager>().tickBarrier; //this is how often the ai brain should check the patroling
coverTrueCoverTest = GetComponent<SearchCover>().trueCoverTest;


//melee stuff
meleeSetting = GetComponent<AIStateManager>().meleeAttackUsed;
distanceForMeleeAttack = GetComponent<AIStateManager>().meleeAttackDistance;

}


}

//this sets all the variables to the correct values
public void SetVariablesToAI()
{
		//to avoid complicating stuff in the prefab
		#if UNITY_EDITOR
		if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab)
		{
			return;
		}
		#endif


//main objects that are crucial for the ai brain to function
GetComponent<AIStateManager>().patrolManager = patrolManager; //the patrol manager
GetComponentInChildren<AIHealthExecuter>().sensors = sensorParent; //the parent that holds the sensors
GetComponent<AIStateManager>().ears = ears; //the ears object
GetComponent<AIStateManager>().sight = sight; //the sight object
		
			GetComponent<AIStateManager>().animationManager = model; //the object that holds the model


//data about self, eg height, health
GetComponent<SearchCover>().eyePosition = eyeHeight; //the height of the eyes
GetComponentInChildren<HealthManager>().health = health; //the amount of health the ai has at the start
GetComponentInChildren<AIHealthExecuter>().disengagingHitsToKnockout = disengagingHitsToKnockout; //the amount of disengaging hits the ai can take until it gets disengaged

		if(GetComponent<NavMeshAgent>() != null)
		{
			GetComponent<NavMeshAgent>().radius = radius; //the size of our radius for the navmesh agent
			GetComponent<NavMeshAgent>().height = height; //our height, for the nav mesh agent
		}
		
//data about enemies, such as height, tag etc..
GetComponent<AIStateManager>().tagToAvoidPrimary = tagOfEnemy; //the tag of the enemy
GetComponent<AIStateManager>().tagToAvoidSecondary = tagOfBullet; //the tag of the object that shows danger, eg bullets
GetComponent<AIStateManager>().enemyHeight = enemyCriticalHeight; //the height at which the ai should aim
GetComponent<AIWeaponController>().amountOfAmmo = initAmmo; //the amount of ammo at the start

//reaction times, how quick does the ai react etc...
GetComponent<AIStateManager>().shockTime = shockTime; //seconds, how quickly we react to seeing an enemy, reference and manipulated by anderenaline
GetComponent<AIStateManager>().freezeTime = freezeTime; //seconds, how long we freeze when we hear a bullet
GetComponent<AIStateManager>().minCoverTime = minCoverTime; //seconds, minimum amount of time to hide in cover
GetComponent<AIStateManager>().maxCoverTime = maxCoverTime; //seconds, maximum amount of time to hide in cover
GetComponent<AIStateManager>().timeUntilNextCheck = timeBetweenEnemyChecks; //seconds, amount of time when we check whether the others are a danger or not
GetComponent<AIStateManager>().investigationTime = timeForGivingUpDuringEngagement; //seconds, the amount of time the ai will try and locate the enemy during an engagment before giving up and going back to patrol
GetComponent<AIStateManager>().timeBarrierEngageToInvestigate = timeForGivingUpSeeking; //frames, the amount of time the ai will try and locate the enemy if we suddenly see somebody else before giving up and going back to patrol 

//emotion control
GetComponent<AIStateManager>().andrenaline = initAndrenaline; //the amount of andrenaline we start off with
GetComponent<AIStateManager>().fear = initFear; //the amount of fear the ai starts off with
GetComponent<AIStateManager>().chanceForFight = chanceForFight; //the percentage for fight or flight instict, reference manipulated by andrenaline

//weapons and engagement
GetComponent<AIWeaponController>().weaponHoldingObject = weapon; //the weapon object
GetComponent<AIWeaponController>().weaponHoldingLocation = weaponHoldingLocation; //the object where the weapon is held
		GetComponent<AIStateManager>().otherWeapons = otherWeapons; //other weapons
		GetComponent<AIStateManager>().otherWeaponsMelee = otherWeaponsMelee; //other weapons melee
GetComponent<AIStateManager>().targetVisualCheck = targetCheck; //the name of the script that return whether the ai should attack or not
GetComponent<AIStateManager>().targetVisualCheckChance = targetVisualCheckChance; //the name of the script that return whether the ai should attack or not
GetComponent<AIStateManager>().distanceToStopWalking = distanceToEngageCloseCombatLogic; //the distance at which to engage
GetComponent<AIStateManager>().offsetFactor = offsetFactor; //the offset factor

//speed references
GetComponent<AIStateManager>().maxSpeedPatrol = refSpeedPatrol; //the reference speed for the patrol
GetComponent<AIStateManager>().maxSpeedEngage = refSpeedEngage; //the reference speed for when the ai engages
GetComponent<AIStateManager>().maxSpeedChase = refSpeedChase; //the reference speed for chase
GetComponent<AIStateManager>().maxSpeedCover = refSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
model.GetComponent<RagdollTransitions>().boneParent = modelParentOfAllBones; //the parent object that takes in all bones
model.GetComponent<HandIK>().handToUseInCharacter = handToUseInCharacter;

//patrol sutff
patrolManager.GetComponent<PatrolManager>().waypointList = waypointList; //the list that contains all waypoinjts in correct order
patrolManager.GetComponent<PatrolManager>().criticalDistanceToWaypoint = distanceToWaypointForRegistering; //how far we have to be from the waypoint for it to register that we are at it
patrolManager.GetComponent<PatrolManager>().meshNav = navmeshToUse; //the navmesh to use
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().minDistanceToDestination = patrolMinDistanceToDestination;
			GetComponent<AIMovementController>().framesCriticalCheck = patrolFramesCriticalCheck;
			GetComponent<AIMovementController>().checksCriticalUntilStop = patrolChecksCritical;	
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().minDistanceToDestination = patrolMinDistanceToDestination;
			GetComponent<AIMovementControllerASTAR>().framesCriticalCheck = patrolFramesCriticalCheck;
			GetComponent<AIMovementControllerASTAR>().checksCriticalUntilStop = patrolChecksCritical;	
		}

//optimisation stuff
GetComponent<SearchCover>().amountOfRays = coverAmountOfRays; //the amount of rays that should be used to sample cover (the more the better, more slower)
GetComponent<SearchCover>().fieldOfRays = coverFieldOfView; //the field of view from which we find cove (recommend 360 for most cases)
GetComponent<SearchCover>().distanceToCheck = coverDistanceToCheck; //how far should the rays shoot; usually some arbitary large number 
GetComponent<AIStateManager>().tickBarrier = patrolTickBarrier; //this is how often the ai brain should check the patroling
GetComponent<SearchCover>().trueCoverTest = coverTrueCoverTest;

//melee stuff
GetComponent<AIStateManager>().meleeAttackUsed = meleeSetting;
GetComponent<AIStateManager>().meleeAttackDistance = distanceForMeleeAttack;
		if(GetComponent<AIMovementController>() != null && distanceForMeleeAttack < GetComponent<AIMovementController>().minDistanceToDestination)
		{
			GetComponent<AIMovementController>().minDistanceToDestination = distanceForMeleeAttack - 0.3f;	
		}

		if(GetComponent<AIMovementControllerASTAR>() != null && GetComponent<AIMovementControllerASTAR>().minDistanceToDestination > distanceForMeleeAttack)
		{
			GetComponent<AIMovementControllerASTAR>().minDistanceToDestination = distanceForMeleeAttack - 0.3f;	
		}

}

//try and disconnect prefab, if we're one
public void PrefabReset()
{

#if UNITY_EDITOR
if(PrefabUtility.GetPrefabType(gameObject) != PrefabType.None && PrefabUtility.GetPrefabType(gameObject) != PrefabType.Prefab)
{
PrefabUtility.DisconnectPrefabInstance(gameObject);
}
#endif

}


}
