using UnityEngine;
using System.Collections;


[AddComponentMenu("Shooter AI/Clean-Up after death")]

/// <summary>
/// Cleans up the AI after death.
/// </summary>
public class CleanUpAfterDeath : MonoBehaviour
{


    private bool isDead = false;
    
    public float cleanupTime = 10.0f;


    void Update() {

        if (isDead)
        {
			cleanupTime -= Time.deltaTime;
        
			if (cleanupTime <= 0.0f)
            {
                RemoveBody();
            }
        }
    }
    
    public void ShooterAiDead()
    {
        isDead = true;
    }

    void RemoveBody() {
		
		if(gameObject != null)
		{
			Destroy(gameObject);
		}
    }

}
