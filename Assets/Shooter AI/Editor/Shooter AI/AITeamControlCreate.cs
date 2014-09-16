using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;


public class AITeamControlCreate : EditorWindow {


private string newName; //the name for the new ai
private string nameOfObject = "Prefabs/TeamControllers/TeamController"; //the name of instance that we use as a prefab for the ai

[MenuItem("Tools/Shooter AI/Create New Team Controller")]
private static void showEditor()
{
EditorWindow.GetWindow (typeof (AITeamControlCreate), true, "Team Controller");
}

//all the gui stuff
void OnGUI()
{
GUI.Label(new Rect(10, 10, 200, 20), "Create New AI Team Controller", EditorStyles.boldLabel);

EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();
EditorGUILayout.Space();

EditorGUILayout.BeginHorizontal();
{
EditorGUILayout.LabelField("Name of new AI controller");
newName = EditorGUILayout.TextField(newName);
}
EditorGUILayout.EndHorizontal();

if(GUILayout.Button("Create AI Contoller"))
{
CreateNewAIController();
}

}

void CreateNewAIController()
{
//create the ai from the template and disconnect it from the prefab
var newAI = PrefabUtility.InstantiatePrefab(Resources.Load(nameOfObject) as GameObject) as GameObject;
PrefabUtility.DisconnectPrefabInstance(newAI);
newAI.name = newName;

//this code is to destroy a unity bug that makes the object still connected to the prefab
GameObject disconnectingObj = newAI.gameObject; 
PrefabUtility.DisconnectPrefabInstance(disconnectingObj);
Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/dummy.prefab");
PrefabUtility.ReplacePrefab(disconnectingObj, prefab, ReplacePrefabOptions.ConnectToPrefab);
PrefabUtility.DisconnectPrefabInstance(disconnectingObj);
AssetDatabase.DeleteAsset("Assets/dummy.prefab");

}



}
