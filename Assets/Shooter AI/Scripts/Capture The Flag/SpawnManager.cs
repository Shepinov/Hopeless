#pragma warning disable

// Created by Trevor Blize
// HOW TO USE: 
// Step 1: Create an empty gameObject and rename it SpawnManager
// Step 2: Create a bunch of empty gameObjects where the AI's will spawn make sure the seperate the teams
// Step 3: Assign Team1 prefab with the Team1 AI's prefab and the same for Team2
using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public Transform[] Team1Spawns; // Team 1 spawn points
    public GameObject Team1; // Team 1 AI prefab to spawn

    public Transform[] Team2Spawns; // Team 2 spawn Points
    public GameObject Team2; // Team 2 AI prefabs to spawn

    public int maxTeam = 6; // Max amount of bots on a team
    public int current1 = 0; // Current team 1 active AI's
    public int current2 = 0; // Current team 2 active AI's

    private ScoreBoard scoreBoard;

    void Start()
    {
        scoreBoard = GetComponent<ScoreBoard>();

        for (int i = 0; i < maxTeam; i++)
        {
            Spawn(1);
        }

        for (int b = 0; b < maxTeam; b++)
        {
            Spawn(2);
        }
    }

    public void Spawn(int team)
    {
        if (team == 1)
        {
            if (current1 <= maxTeam)
            {
                // Spawn team 1
                Transform team1 = Team1Spawns [Random.Range(0, Team1Spawns.Length)];
                Vector3 pos = team1.position + 1.5f * Vector3.up + Random.insideUnitSphere * 3f;
                Transform bot = Instantiate(Team1, pos, Quaternion.identity) as Transform;
                current1++;
            }
        } else
        {
            if (current2 <= maxTeam)
            {
                // Spawn team 2
                Transform team2 = Team2Spawns [Random.Range(0, Team2Spawns.Length)];
				Vector3 pos = team2.position + 1.5f * Vector3.up + Random.insideUnitSphere * 3f;
                Transform bot = Instantiate(Team2, pos, Quaternion.identity) as Transform;
                current2++;
            }
        }
    }

    public void SubtractTeam(int team)
    {
        if (team == 1)
        {
            current1 = current1 - 1;
        } else if (team == 2)
        {
            current2 = current2 - 1;
        }

        scoreBoard.UpdateKills(team);
    }
}
