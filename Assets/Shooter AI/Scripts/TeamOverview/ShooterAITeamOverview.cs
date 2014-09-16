//this script controls teams overview
//and is a child script for the editor drawer


using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.Collections.Generic;



public enum PredefinedStrategies {CloseInTogether, Custom}


public class ShooterAITeamOverview : MonoBehaviour {




public GameObject[] teamOverview; //the objects that are in the team

//these are all the variables that can be changed globally
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
public Transform weaponHoldingLocation; //the object where the weapon is held
public List<string> targetCheck = new List<string>(); //add this script to object; once we see the player we will run this script with void "VisualCheck", if it returns true we engage the player
public List<float> targetVisualCheckChance = new List<float>(); //the chance for the visual check
public float distanceToEngageCloseCombatLogic; //the distance at which to engage

//speed references
public float refSpeedPatrol; //the reference speed for the patrol
public float refSpeedEngage; //the reference speed for when the ai engages
public float refSpeedChase; //the reference speed for chase
public float refSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
public HandToUse handToUseInCharacter;
public bool handToUseChanged = false; //this is for internal use only

//patrol sutff
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

//melee stuff
public int meleeSetting; //the setting of none,hybrid,only
public float distanceForMeleeAttack; //the distance for melee attack
	


//this is to keep the list constant across both scripts
public List<GameObject> aiChars = new List<GameObject>();  //all objects that are in this team
public List<bool> aiCharsSelectedIndex = new List<bool>();   //this list contains the indexes that are selected and not
public List<GameObject> aiCharsAlive = new List<GameObject>(); //all the team objects that are alive


//this is for team behaviour scripts
//NOTE: All behaviours are only executed if the others are not already engaged, except panic which is always executed

/// <summary>
/// The behaviour on enemy sight for the team.
/// </summary>
public string behaviourOnEnemySight;

/// <summary>
/// The behaviour on enemy engagement.
/// </summary>
public string behaviourOnEnemyEngage;

/// <summary>
/// The behaviour when a soldier panics.
/// </summary>
public string behaviourOnPanic;

/// <summary>
/// The behaviour when a soldier chases.
/// </summary>
public string behaviourOnChase; 



//behaviours supporting variables
public bool nonEngaged = true; //this is the variable that says whether anybody is engaged; true if at least 1 ai is engaged
public Vector3 lastSeenLocation; //the last location an enemy was spotted
public float lastSeenTime; //the last seen time
public bool prevCheckAnybodyEngaged = false; //whether at the last check, anybody was engaged



//this is for strategic policies
public int currentTeamCaptainIndex; //the current team captain. this is always the top of the list who isn't dead 
public GameObject currentTeamCaptain; //the current team captain
public string strategyScript; //the script for close combat
public PredefinedStrategies strategy;


//private worker variables
private float behaviourExecuteFramesMin = 20f; //the min amount of frames until we check dehaviour execute command
private float behaviourExecuteCurrentFrame; //the current frame for counting for behaiour frames




void Start()
{

//this starts the assigned strategic script
if(strategyScript != null)
{
gameObject.AddComponent(strategyScript);
}

}



/// <summary>
/// Syncs the AI chars with slave script.
/// </summary>
/// <param name='aiCharsNew'>
/// All the characters
/// </param>
/// <param name='aiCharsSelectedNew'>
/// All the selected characters
/// </param>
public void SyncSlaveWithAIScript(List<GameObject> aiCharsNew, List<bool> aiCharsSelectedNew)
{

aiChars = aiCharsNew;
aiCharsSelectedIndex = aiCharsSelectedNew;

}


/// <summary>
/// Writes the variables to AI finished.
/// </summary>
/// <param name='aiCharacter'>
/// Ai character gameobject
/// </param>
public void WriteVariablesToAIFinished(GameObject aiCharacter)
{

//this is to prevent nullreference errors
if(aiCharacter.GetComponent<AIStateManager>() == null)
{
Debug.LogWarning("AI Team Overview: Object that isn't AI is trying to get parameters set. Check object list in Shooter AI Team Overview");
return;
}



//data about self, eg height, health
if(eyeHeight != Vector3.zero && aiCharacter.GetComponent<SearchCover>().eyePosition != eyeHeight)
{
aiCharacter.GetComponent<SearchCover>().eyePosition = eyeHeight; //the height of the eyes
}
		
if(health != 0 && aiCharacter.GetComponentInChildren<HealthManager>().health != health)
{
aiCharacter.GetComponentInChildren<HealthManager>().health = health; //the amount of health the ai has at the start
}

if(disengagingHitsToKnockout != 0 && aiCharacter.GetComponentInChildren<AIHealthExecuter>().disengagingHitsToKnockout != disengagingHitsToKnockout)
{
aiCharacter.GetComponentInChildren<AIHealthExecuter>().disengagingHitsToKnockout = disengagingHitsToKnockout; //the amount of disengaging hits the ai can take until it gets disengaged
}

if(radius != 0 && aiCharacter.GetComponent<NavMeshAgent>().radius != radius)
{
aiCharacter.GetComponent<NavMeshAgent>().radius = radius; //the size of our radius for the navmesh agent
}

if(height != 0)
{
aiCharacter.GetComponent<NavMeshAgent>().height = height; //our height, for the nav mesh agent
}


//data about enemies, such as height, tag etc..
if(tagOfEnemy != null && tagOfEnemy != "")
{
aiCharacter.GetComponent<AIStateManager>().tagToAvoidPrimary = tagOfEnemy; //the tag of the enemy
}

if(tagOfBullet != null)
{
aiCharacter.GetComponent<AIStateManager>().tagToAvoidSecondary = tagOfBullet; //the tag of the object that shows danger, eg bullets
}
if(enemyCriticalHeight != Vector3.zero)
{
aiCharacter.GetComponent<AIStateManager>().enemyHeight = enemyCriticalHeight; //the height at which the ai should aim
}
if(initAmmo != 0)
{
aiCharacter.GetComponent<AIWeaponController>().amountOfAmmo = initAmmo; //the amount of ammo at the start
}

//reaction times, how quick does the ai react etc...
if(shockTime != 0)
{
aiCharacter.GetComponent<AIStateManager>().shockTime = shockTime; //seconds, how quickly we react to seeing an enemy, reference and manipulated by anderenaline
}
if(freezeTime != 0)
{
aiCharacter.GetComponent<AIStateManager>().freezeTime = freezeTime; //seconds, how long we freeze when we hear a bullet
}
if(minCoverTime != 0)
{
aiCharacter.GetComponent<AIStateManager>().minCoverTime = minCoverTime; //seconds, minimum amount of time to hide in cover
}
if(maxCoverTime != 0)
{
aiCharacter.GetComponent<AIStateManager>().maxCoverTime = maxCoverTime; //seconds, maximum amount of time to hide in cover
}
if(timeBetweenEnemyChecks != 0)
{
aiCharacter.GetComponent<AIStateManager>().timeUntilNextCheck = timeBetweenEnemyChecks; //seconds, amount of time when we check whether the others are a danger or not
}
if(timeForGivingUpDuringEngagement != 0)
{
aiCharacter.GetComponent<AIStateManager>().investigationTime = timeForGivingUpDuringEngagement; //seconds, the amount of time the ai will try and locate the enemy during an engagment before giving up and going back to patrol
}
if(timeForGivingUpSeeking != 0)
{
aiCharacter.GetComponent<AIStateManager>().timeBarrierEngageToInvestigate = timeForGivingUpSeeking; //frames, the amount of time the ai will try and locate the enemy if we suddenly see somebody else before giving up and going back to patrol 
}

//emotion control
if(initAndrenaline != 0)
{
aiCharacter.GetComponent<AIStateManager>().andrenaline = initAndrenaline; //the amount of andrenaline we start off with
}
if(initFear != 0)
{
aiCharacter.GetComponent<AIStateManager>().fear = initFear; //the amount of fear the ai starts off with
}
if(chanceForFight != 0)
{
aiCharacter.GetComponent<AIStateManager>().chanceForFight = chanceForFight; //the percentage for fight or flight instict, reference manipulated by andrenaline
}

//weapons and engagement
if(weapon != null)
{
aiCharacter.GetComponent<AIWeaponController>().weaponHoldingObject = weapon; //the weapon object
}
if(weaponHoldingLocation != null)
{
aiCharacter.GetComponent<AIWeaponController>().weaponHoldingLocation = weaponHoldingLocation; //the object where the weapon is held
}
/*
if(targetCheck != aiCharacter.GetComponent<AIStateManager>().targetVisualCheck)
{
aiCharacter.GetComponent<AIStateManager>().targetVisualCheck = targetCheck; //the name of the script that return whether the ai should attack or not
aiCharacter.GetComponent<AIStateManager>().targetVisualCheckChance = targetVisualCheckChance;
}*/
if(distanceToEngageCloseCombatLogic != 0)
{
aiCharacter.GetComponent<AIStateManager>().distanceToStopWalking = distanceToEngageCloseCombatLogic; //the distance at which to engage
}

//speed references
if(refSpeedPatrol != 0)
{
aiCharacter.GetComponent<AIStateManager>().maxSpeedPatrol = refSpeedPatrol; //the reference speed for the patrol
}
if(refSpeedEngage != 0)
{
aiCharacter.GetComponent<AIStateManager>().maxSpeedEngage = refSpeedEngage; //the reference speed for when the ai engages
}
if(refSpeedChase != 0)
{
aiCharacter.GetComponent<AIStateManager>().maxSpeedChase = refSpeedChase; //the reference speed for chase
}
if(refSpeedCover != 0)
{
aiCharacter.GetComponent<AIStateManager>().maxSpeedCover = refSpeedCover; //the refernce speed at which the ai runs to cover
}

//model stuff
		if(aiCharacter.GetComponent<AIMovementController>().animationManager == null)
		{
			aiCharacter.GetComponent<AIMovementController>().animationManager = aiCharacter.GetComponent<AIStateManager>().animationManager;
		}

if(handToUseInCharacter != aiCharacter.GetComponent<AIMovementController>().animationManager.GetComponent<HandIK>().handToUseInCharacter && handToUseChanged == true)
{
aiCharacter.GetComponent<AIMovementController>().animationManager.GetComponent<HandIK>().handToUseInCharacter = handToUseInCharacter;
}

//patrol sutff
if(distanceToWaypointForRegistering != 0)
{
aiCharacter.GetComponent<AIStateManager>().patrolManager.GetComponent<PatrolManager>().criticalDistanceToWaypoint = distanceToWaypointForRegistering; //how far we have to be from the waypoint for it to register that we are at it
}
if(navmeshToUse != null)
{
aiCharacter.GetComponent<AIStateManager>().patrolManager.GetComponent<PatrolManager>().meshNav = navmeshToUse; //the navmesh to use
}
if(patrolMinDistanceToDestination != 0)
{
aiCharacter.GetComponent<AIMovementController>().minDistanceToDestination = patrolMinDistanceToDestination;
}
if(patrolFramesCriticalCheck != 0)
{
aiCharacter.GetComponent<AIMovementController>().framesCriticalCheck = patrolFramesCriticalCheck;
}
if(patrolChecksCritical != 0)
{
aiCharacter.GetComponent<AIMovementController>().checksCriticalUntilStop = patrolChecksCritical;	
}

//optimisation stuff
if(coverAmountOfRays != 0)
{
aiCharacter.GetComponent<SearchCover>().amountOfRays = coverAmountOfRays; //the amount of rays that should be used to sample cover (the more the better, more slower)
}
if(coverFieldOfView != 0)
{
aiCharacter.GetComponent<SearchCover>().fieldOfRays = coverFieldOfView; //the field of view from which we find cove (recommend 360 for most cases)
}
if(coverDistanceToCheck != 0)
{
aiCharacter.GetComponent<SearchCover>().distanceToCheck = coverDistanceToCheck; //how far should the rays shoot; usually some arbitary large number 
}
if(patrolTickBarrier != 0)
{
aiCharacter.GetComponent<AIStateManager>().tickBarrier = patrolTickBarrier; //this is how often the ai brain should check the patroling
}
if(coverTrueCoverTest != 0)
{
aiCharacter.GetComponent<SearchCover>().trueCoverTest = coverTrueCoverTest;
}

//melee stuff
if(meleeSetting != aiCharacter.GetComponent<AIStateManager>().meleeAttackUsed)
{
aiCharacter.GetComponent<AIStateManager>().meleeAttackUsed = meleeSetting;
}
if(distanceForMeleeAttack != 0)
{
aiCharacter.GetComponent<AIStateManager>().meleeAttackDistance = distanceForMeleeAttack;
}

}


/// <summary>
/// Writes the variables all selected AI
/// </summary>
public void WriteVariablesToAIFinishedAllSelected()
{

//apply to all objects
for( int y = 0; y < aiChars.Count-1; y++)
{

if(aiCharsSelectedIndex[y] == true)
{
WriteVariablesToAIFinished(aiChars[y]);
}

}


}


void Update()
{

//behaviour execute
behaviourExecuteCurrentFrame += 1;
		if(behaviourExecuteCurrentFrame > (behaviourExecuteFramesMin + Random.Range(10f, 50f)) )
		{
			BehaviourExecute();
		}


//determine team variables
DetermineTeamCaptain();

}


/// <summary>
/// Executes all the behaviours
/// </summary>
void BehaviourExecute()
{
//reset vars before cycle
nonEngaged = true;


//apply to all objects
for( int y = 0; y < aiChars.Count-1; y++)
{

//test to see if anybody is engaged
if(aiChars[y].GetComponent<AIStateManager>().currentState == CurrentState.engage)
{
nonEngaged = false;

}




//enemy sight
if(aiChars[y].GetComponent<AIStateManager>().sight.GetComponent<Sight>().lastSeenTime > lastSeenTime && nonEngaged == true && behaviourOnEnemySight != "")
{
//if we've seen the enemy last

//set the time var correct
lastSeenTime = Time.time;
//set the new last seen location
lastSeenLocation = aiChars[y].GetComponent<AIStateManager>().sight.GetComponent<Sight>().lastSeenLocation;
//start the script
gameObject.AddComponent(behaviourOnEnemySight);

}
			
			Debug.Log(nonEngaged + " " + prevCheckAnybodyEngaged);
			
			//ai team engaged
			if(nonEngaged == false && prevCheckAnybodyEngaged == false && behaviourOnEnemyEngage!= "")
			{
				Debug.LogError("works");
				
				//start the engagment script
				gameObject.AddComponent(behaviourOnEnemyEngage);
			}


//panic started
if(aiChars[y].GetComponent<AIStateManager>().panic == true && behaviourOnPanic != "")
{
//start the panic script
gameObject.AddComponent(behaviourOnPanic);
}


//someobdy starts chasing
if(aiChars[y].GetComponent<AIStateManager>().currentState == CurrentState.engage && nonEngaged == true && behaviourOnChase != "")
{
//start the engage script
gameObject.AddComponent(behaviourOnChase);				

}
			
			//set back some vars
			if(aiChars[y].GetComponent<AIStateManager>().currentState == CurrentState.engage)
			{
				
				if(prevCheckAnybodyEngaged == false)
				{
					prevCheckAnybodyEngaged = true;
				}
				
			}


}




//for prev engaged vars, HAS TO BE AT THE END
if(nonEngaged == false && prevCheckAnybodyEngaged == true)
{
prevCheckAnybodyEngaged = false;
}

}


/// <summary>
/// Determines the team captain.
/// </summary>
public void DetermineTeamCaptain()
{


//cycle through all the objects to see the first who is still alive, else return -1/null
currentTeamCaptainIndex = -1;
currentTeamCaptain = null;

for( int x = 0; currentTeamCaptain == null && x < aiChars.Count; x ++)
{

if(aiChars[x].GetComponent<AIStateManager>() != null)
{
currentTeamCaptain = aiChars[x];
currentTeamCaptainIndex = x;

}

}



}

/// <summary>
/// Determins the alive team soldiers. Not automatically updated.
/// </summary>
public void DeterminAliveTeamSoldiers()
{

aiCharsAlive.Clear();
for(int x = 0; x < aiChars.Count; x++)
{

if(aiChars[x].GetComponent<AIStateManager>() != null)
{
aiCharsAlive.Add(aiChars[x]);
}

}


}

}
