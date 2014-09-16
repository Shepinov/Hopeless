#pragma warning disable

// Created by Trevor Blize
// HOW TO USE: 
// Step 1: Create an empty gameObject and rename it SpawnManager
// Step 2: Create a bunch of empty gameObjects where the AI's will spawn make sure the seperate the teams
// Step 3: Assign Team1 prefab with the Team1 AI's prefab and the same for Team2
using UnityEngine;
using System.Collections;

[AddComponentMenu("Shooter AI/Generic Spawn Manager") ]

public class GenericSpawnManager : MonoBehaviour
{
    public Transform[] Team1Spawns; // Team 1 spawn points
    public GameObject Team1; // Team 1 AI prefab to spawn
	public string tagOfAI; //the tag of the ai
	
    public int maxTeam = 6; // Max amount of bots on a team
    
    public int current1 = 0; // Current team 1 active AI's

    private ScoreBoard scoreBoard;

    void Update()
    {
        //get the amount of AI
        current1 = GameObject.FindGameObjectsWithTag( tagOfAI).Length;
        
        //spawn as many as needed
        while(current1 < maxTeam)
        {
			Spawn();
			
        }
       
    }

	
    public void Spawn()
    {
		
		// Spawn team
		Transform team1 = Team1Spawns [Random.Range(0, Team1Spawns.Length)];
		Vector3 pos = team1.position + 1.5f * Vector3.up + Random.insideUnitSphere * 3f;
		Transform bot = Instantiate(Team1, pos, Quaternion.identity) as Transform;
       
    }
    
}
