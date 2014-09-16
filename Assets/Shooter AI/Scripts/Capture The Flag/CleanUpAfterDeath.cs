using UnityEngine;
using System.Collections;

public class Respawner : MonoBehaviour
{

    public string gameManagerTag;
    public string team;
    private GameObject gameManager;
    private SpawnManager spawnManager;

    private bool isDead = false;
    public float respawnTime = 10.0f;

    // Use this for initialization
    void Start()
    {
        if (!gameManager)
        {
            gameManager = GameObject.FindGameObjectWithTag(gameManagerTag); 
            spawnManager = gameManager.GetComponent<SpawnManager>();
        } else
        {
            Debug.LogWarning("Cannot respawn bots without a spawnManagerTag");
        }
    }

    void Update() {

        if (isDead)
        {
            respawnTime -= Time.deltaTime;
        
            if (respawnTime <= 0.0f)
            {
                RemoveBody();
            }
        }
    }
    public void ShooterAiDead()
    {
        isDead = true;
        if (team == "team1")
        {
            spawnManager.SubtractTeam(1);
        } else if (team == "team2")
        {
            spawnManager.SubtractTeam(2);
        }
    }

    void RemoveBody() {
        if (team == "team1")
        {
            spawnManager.Spawn(1);
        } else if (team == "team2")
        {
            spawnManager.Spawn(2);
        }

        Destroy(gameObject.transform.root.gameObject);
    }

}
