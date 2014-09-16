using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;



public class AIMainCharacterCreationWindow : EditorWindow 
{


public Transform modelObject; //the object that contains the model and is a ragdoll
private string newName; //the name for the new ai
private GameObject weaponObjet; //the weapon

private enum NavSystems { AStar, UnityNavmesh};
private NavSystems navsysToUse = NavSystems.UnityNavmesh;

//this is for creating the instances
private string nameOfObject = "Prefabs/AICharacters/AIEmptyPrefab"; //the name of instance that we use as a prefab for the ai
private string nameOfObjectASTAR = "Prefabs/AICharacters/AIEmptyPrefabASTAR"; //the name of the instance we use as a prefab for the ai with A* pathfinding
private string nameOfAnimator = "AnimatorTemplates/EnemyAnimator"; //the name of the ai animator
private string nameOfReferenceObject = "Prefabs/AICharacters/ReferenceObject"; //the name of the referecing object for the gun

[MenuItem("Tools/Shooter AI/Create New Character")]
private static void showEditor()
{
EditorWindow.GetWindow (typeof (AIMainCharacterCreationWindow), true, "FPS AI Character Creation");
}


//all the gui stuff
void OnGUI()
{
GUI.Label(new Rect(10, 10, 200, 20), "Create New AI Characters", EditorStyles.boldLabel);

EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();

EditorGUILayout.BeginHorizontal();
{
EditorGUILayout.LabelField("Name of new AI");
newName = EditorGUILayout.TextField(newName);
}
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
{
EditorGUILayout.LabelField("Please select with navigation system the AI should use");
navsysToUse = (NavSystems)EditorGUILayout.EnumPopup( "", navsysToUse);
}
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
{
EditorGUILayout.LabelField("Please select the object that contains the model with the ragdoll");
modelObject = EditorGUILayout.ObjectField(modelObject, typeof(Transform), true) as Transform;
}
EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
{
EditorGUILayout.LabelField("Please select FINISHED WEAPON object (optional at this stage)");
weaponObjet = EditorGUILayout.ObjectField(weaponObjet, typeof(Object), true) as GameObject;
}
EditorGUILayout.EndHorizontal();


EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();

if(GUILayout.Button("Create AI") && modelObject != null)
{
CreateNewAI();
}

}





//create new ai
void CreateNewAI()
{

//create the ai from the template and disconnect it from the prefab
GameObject newAI = null;

switch(navsysToUse)
{
case NavSystems.UnityNavmesh: newAI = PrefabUtility.InstantiatePrefab(Resources.Load(nameOfObject) as GameObject) as GameObject; break;
case NavSystems.AStar: newAI = PrefabUtility.InstantiatePrefab(Resources.Load(nameOfObjectASTAR) as GameObject) as GameObject; break;
}

PrefabUtility.DisconnectPrefabInstance(newAI);
newAI.name = newName;

//create the model and set all correct components
var aiModel = Instantiate(modelObject) as Transform;
aiModel.transform.parent = newAI.transform;
aiModel.transform.localPosition = Vector3.zero;
aiModel.transform.localEulerAngles = Vector3.zero;
aiModel.name = "Model";
if(aiModel.GetComponent<Animator>() == null)
{
aiModel.gameObject.AddComponent<Animator>();
}
aiModel.GetComponent<Animator>().runtimeAnimatorController = Resources.Load(nameOfAnimator) as RuntimeAnimatorController;
aiModel.GetComponent<Animator>().applyRootMotion = false;
aiModel.gameObject.AddComponent<RagdollTransitions>();
aiModel.GetComponent<RagdollTransitions>().enabled = false;
aiModel.gameObject.AddComponent<HandIK>();
aiModel.gameObject.AddComponent<RagdollHelper>();
var referenceObject = Instantiate(Resources.Load(nameOfReferenceObject) as GameObject) as GameObject;    //the referecing object
referenceObject.transform.parent = aiModel;
newAI.GetComponentInChildren<ShooterAITransformPositionReference>().objectAsReference = referenceObject.transform;

//set all the correct references
newAI.GetComponent<AIStateManager>().animationManager = aiModel.gameObject;



var rigidObj = aiModel.GetComponentsInChildren<Rigidbody>();
foreach (Rigidbody rb in rigidObj) 
{
if(rb.gameObject.transform.parent.GetComponent<Rigidbody>() == null)
{

aiModel.GetComponent<RagdollTransitions>().boneParent = rb.gameObject.transform.parent.gameObject;


}
}

//this creates the gun
if(weaponObjet != null)
{
var gun = Instantiate(weaponObjet) as GameObject;
gun.transform.parent = newAI.GetComponent<AIWeaponController>().weaponHoldingLocation.transform;
gun.transform.localPosition = Vector3.zero;
gun.transform.localEulerAngles = Vector3.zero;

newAI.GetComponent<AIWeaponController>().weaponHoldingObject = gun.gameObject;
newAI.GetComponent<AIWeaponController>().weaponRotatingLocation = gun.gameObject;
}




//this code is to destroy a unity bug that makes the object still connected to the prefab
GameObject disconnectingObj = newAI.gameObject; 
PrefabUtility.DisconnectPrefabInstance(disconnectingObj);
Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/dummy.prefab");
PrefabUtility.ReplacePrefab(disconnectingObj, prefab, ReplacePrefabOptions.ConnectToPrefab);
PrefabUtility.DisconnectPrefabInstance(disconnectingObj);
AssetDatabase.DeleteAsset("Assets/dummy.prefab");


}



}
