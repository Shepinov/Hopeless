using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;





[HideInInspector]
[CustomEditor( typeof(ShooterAITeamOverview))]

public class AITeamOverviewEditor : Editor {

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


//strategic policies
public string strategyScript; //the script for close combat
public PredefinedStrategies predefinedStrategies; //the predefined strategies
public string closeCombatStrategy = "ShooterAIStrategyCloseTogether";

//this is for the different foldouts inside the different properties
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
public bool showFoldout12 = false;
public bool showFoldout13 = false;
public bool showFoldout14 = false;
public bool showFoldout15 = false;
public bool showFoldout16 = false;
public bool showFoldout17 = false;
public bool showFoldout18 = false;
public bool showFoldout19 = false;


private string[] meleeOptions = {"No melee attack","Ranged weapons and melee attack","Only melee attack"};
	
private Vector2 scrollPosition;

public ShooterAITeamOverview childScript;                                                    


//this part is for selection and is synced across both scripts
public List<GameObject> aiChars = new List<GameObject>();  //all objects that are in this team
public List<bool> aiCharsSelectedIndex = new List<bool>();   //this list contains the indexes that are selected and not
	

//this is for strategic policies; synced
public int currentTeamCaptainIndex; //the current team captain. this is always the top of the list who isn't dead 
public GameObject currentTeamCaptain; //the current team captain



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
/// The behaviour when one soldier panics.
/// </summary>
public string behaviourOnPanic;

/// <summary>
/// The behaviour when one soldier chases.
/// </summary>
public string behaviourOnChase; 



void Update()
{
//this is to sync with the child script
if(aiChars != childScript.aiChars)
{
SyncThisWithSlave();
}


}



void OnEnable()
{
childScript = (ShooterAITeamOverview)target;
SyncThisWithSlave();
LoadValuesToEditor();


//set some vars correct
closeCombatStrategy = "ShooterAIStrategyCloseTogether";
childScript.handToUseChanged = false;


}

public override void OnInspectorGUI()
{
//draw the objects
SelectableObjects();



EditorGUILayout.Space();



//this part is for changing paramters
showFoldout1 = EditorGUILayout.Foldout(showFoldout1, "Data About AI Characters");
EditorGUILayout.Space();
if(showFoldout1)
{		
ChangeParameters();
}

//this part is for the behaviours
showFoldout12 = EditorGUILayout.Foldout(showFoldout12, "Behaviour Data For Team");
EditorGUILayout.Space();
if(showFoldout12)
{		
BehaviourParameters();
}

//this part is for the strategies
showFoldout13 = EditorGUILayout.Foldout(showFoldout13, "Team Strategy");
EditorGUILayout.Space();
if(showFoldout13)
{		
StrategyFoldout();
}


//this has to be last to update all the stuff we did
if(GUI.changed)
{
//sync with slave script
childScript.SyncSlaveWithAIScript(aiChars, aiCharsSelectedIndex);

//apply to all objects
for( int y = 0; y < aiChars.Count-1; y++)
{

if(aiCharsSelectedIndex[y] == true)
{
WriteValuesToAI(aiChars[y]);
}

}

}


}


//this function draws the selectable objects
void SelectableObjects()
{
		
EditorGUILayout.Space();
EditorGUILayout.Space();

//this adds the button to add objects to the list
if(GUILayout.Button("Add Object"))
{
aiChars.Add(childScript.gameObject);
aiCharsSelectedIndex.Add(false);
}	

	
EditorGUILayout.Space();

//this draws the objects
for(int x = 0; x < aiChars.Count-1; x++)
{

if(aiChars[x] == currentTeamCaptain)
{
EditorGUILayout.LabelField("Leader:");
GUI.color = Color.green;
}

EditorGUILayout.BeginHorizontal();

aiChars[x] = EditorGUILayout.ObjectField(aiChars[x], typeof(Object), true) as GameObject;
aiCharsSelectedIndex[x] = EditorGUILayout.Toggle(aiCharsSelectedIndex[x]);





if(GUILayout.Button("Remove Object"))
{
aiChars.RemoveAt(x);
aiCharsSelectedIndex.RemoveAt(x);
}

EditorGUILayout.EndHorizontal();

if(aiChars[x] == currentTeamCaptain)
{
GUI.color = Color.white;
}


}
		

EditorGUILayout.BeginHorizontal();
if(GUILayout.Button("Select All"))
{
for(int xx = 0; xx < aiChars.Count - 1; xx ++)
{
aiCharsSelectedIndex[xx] = true;
}
}

if(GUILayout.Button("Select None"))
{

for(int xx = 0; xx < aiChars.Count - 1; xx ++)
{
aiCharsSelectedIndex[xx] = false;
}

}
EditorGUILayout.EndHorizontal();


}

	

void ChangeParameters()
{
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
weapon = EditorGUILayout.ObjectField("Weapon" ,weapon, typeof(Object), true) as GameObject;

//engagement script stuff
/*
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
EditorGUILayout.Space();*/




EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Distance For Close Combat Logic");
distanceToEngageCloseCombatLogic = EditorGUILayout.FloatField(distanceToEngageCloseCombatLogic);
EditorGUILayout.EndHorizontal();
initAmmo = EditorGUILayout.FloatField("Amount Of Ammo", initAmmo);
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

if(childScript.handToUseChanged == false)
{
childScript.handToUseChanged = true;
}

EditorGUILayout.Space();
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

}


void BehaviourParameters()
{

EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("On Enemy Sight Script");
behaviourOnEnemySight = EditorGUILayout.TextField(behaviourOnEnemySight);
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("On Engagement Script");
behaviourOnEnemyEngage = EditorGUILayout.TextField(behaviourOnEnemyEngage);
EditorGUILayout.EndHorizontal();
		
EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("On Chase Script");
behaviourOnChase = EditorGUILayout.TextField(behaviourOnChase);
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("On Panic Script");
behaviourOnPanic = EditorGUILayout.TextField(behaviourOnPanic);
EditorGUILayout.EndHorizontal();


}
	


//this is the for the strategies
void StrategyFoldout()
{
predefinedStrategies = (PredefinedStrategies)EditorGUILayout.EnumPopup(predefinedStrategies);

if(predefinedStrategies == PredefinedStrategies.Custom)
{
EditorGUILayout.BeginHorizontal();
EditorGUILayout.LabelField("Name Of Custom Strategy Script");
strategyScript = EditorGUILayout.TextField(strategyScript);
EditorGUILayout.EndHorizontal();
}

if(predefinedStrategies == PredefinedStrategies.CloseInTogether)
{
strategyScript = closeCombatStrategy;
}


}





/// <summary>
/// Writes the values to and AI character
/// </summary>
/// <param name='aiCharacterToUse'>
/// Ai character object to use.
/// </param>
public void WriteValuesToAI(GameObject aiCharacterToUse)
{


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
childScript.targetCheck = targetCheck; //the name of the script that return whether the ai should attack or not
childScript.distanceToEngageCloseCombatLogic = distanceToEngageCloseCombatLogic; //the distance at which to engage

//speed references
childScript.refSpeedPatrol = refSpeedPatrol; //the reference speed for the patrol
childScript.refSpeedEngage = refSpeedEngage; //the reference speed for when the ai engages
childScript.refSpeedChase = refSpeedChase; //the reference speed for chase
childScript.refSpeedCover = refSpeedCover; //the refernce speed at which the ai runs to cover

//model stuff
childScript.handToUseInCharacter = handToUseInCharacter;		

//patrol sutff
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

//behaviour parameters
childScript.behaviourOnEnemySight = behaviourOnEnemySight;
childScript.behaviourOnEnemyEngage = behaviourOnEnemyEngage;
childScript.behaviourOnChase = behaviourOnChase;
childScript.behaviourOnPanic = behaviourOnPanic;

childScript.WriteVariablesToAIFinished(aiCharacterToUse);

childScript.strategy = predefinedStrategies;
childScript.strategyScript = strategyScript;
}



/// <summary>
/// Loads the values to this script.
/// </summary>
public void LoadValuesToEditor()
{
childScript.DetermineTeamCaptain();

behaviourOnChase = childScript.behaviourOnChase;
behaviourOnEnemyEngage = childScript.behaviourOnEnemyEngage;
behaviourOnPanic = childScript.behaviourOnPanic;
behaviourOnEnemySight = childScript.behaviourOnEnemySight;

currentTeamCaptain = childScript.currentTeamCaptain;
currentTeamCaptainIndex = childScript.currentTeamCaptainIndex;

predefinedStrategies = childScript.strategy;
strategyScript = childScript.strategyScript;
}

/// <summary> This is to sync this script with the slave script </summary>
public void SyncThisWithSlave()
{
aiChars = childScript.aiChars;
aiCharsSelectedIndex = childScript.aiCharsSelectedIndex;

}


}
