#pragma warning disable

using UnityEngine;
using System.Collections;

public class CTF : MonoBehaviour {

    public string team1Tag = "aiTeam1";
    public string team2Tag = "aiTeam2";

    public string FlagTag;
    public string RedBaseTag;
    public string BlueBaseTag;
    
    private GameObject flag;
    private GameObject redBase;
    private GameObject blueBase;

    private ScoreBoard scoreBoard;

	void Start () {

        scoreBoard = GetComponent<ScoreBoard>();

        if (FlagTag != null)
        {
            flag = GameObject.FindGameObjectWithTag(FlagTag); 
        }
        
        if (RedBaseTag != null)
        {
            redBase = GameObject.FindGameObjectWithTag(RedBaseTag); 
        }
        
        if (BlueBaseTag != null)
        {
            blueBase = GameObject.FindGameObjectWithTag(BlueBaseTag); 
        }
	}
	
    public Vector3 FlagLocation() {
        if (flag == null)
        {
            flag = GameObject.FindGameObjectWithTag(FlagTag); 
        }
        return flag.transform.position;
    }

    public Vector3 RedBaseLocation() {
        return redBase.transform.position;
    }

    public Vector3 BlueBaseLocation() {
        return blueBase.transform.position;
    }
}
