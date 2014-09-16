using UnityEngine;
using System.Collections;


[AddComponentMenu("Shooter AI/Drop Objects After Death") ]

/// <summary>
/// Drops a number of prefabs after death.
/// </summary>
public class SAIDropPrefabs : MonoBehaviour {
	
	public Transform[] objectDropList; //the list of objects that should be dropped when the ai dies
	public bool selectRandomPrefabFromList = true; //whether to select a random prefab from a list
	public Vector3 relativeDropOffset = Vector3.zero; //the relative offset where dropped objects will be created
	
	
	
	public void ShooterAiDead()
	{
		
		if(selectRandomPrefabFromList == true)
		{
			
			Instantiate( objectDropList[ (int)Random.Range(0, objectDropList.Length) ], transform.position + relativeDropOffset, Quaternion.identity );
			
		}
		else
		{
			
			foreach(Transform drop in objectDropList)
			{
				Instantiate( drop, transform.position + relativeDropOffset, Quaternion.identity );
			}
			
		}
		
	}
	
	
	
}
