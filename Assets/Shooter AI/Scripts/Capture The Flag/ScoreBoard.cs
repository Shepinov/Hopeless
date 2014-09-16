#pragma warning disable

using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

    int team1Kills = 0; // Team 1 kills
    int team2Kills = 0; // Team 2 kills
    
    int team1Captures = 0;
    int team2Captures = 0;

    public void UpdateKills(int team) {
        if (team == 1)
        {
            team2Kills++;
        } else
        {
            team1Kills++;
        }
    }

    public void UpdateCaptures(int team) {
        if (team == 1)
        {
            team1Captures++;
        } else
        {
            team2Captures++;
        }
    }

    void OnGUI () {
        GUI.Label (new Rect (25,25,100,50), "Team 2");
        GUI.Label (new Rect (25,50,100,50), "Kills: " + team2Kills);
        GUI.Label (new Rect (25,75,100,50), "Captures: " + team2Captures);
        
        GUI.Label (new Rect(Screen.width - 200, 25, 200, 20), "Team 1");
        GUI.Label (new Rect(Screen.width - 200, 50, 200, 20), "Kills: " + team1Kills);
        GUI.Label (new Rect(Screen.width - 200, 75, 200, 20), "Captures: " + team1Captures);
    }

}
