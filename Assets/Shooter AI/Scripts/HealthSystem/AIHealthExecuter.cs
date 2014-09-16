//this script does eveything that the ai needs to do, eg death


using UnityEngine;
using System.Collections;

public class AIHealthExecuter : MonoBehaviour {

public GameObject parent;
public string state = "alive"; //the current state: "alive", "disengaged", "dead"
public float disengagingHitsToKnockout; //the amount of disengaging shots to take until initating knockout
public GameObject ragdollObject; //ragdoll obj
public GameObject sensors; //the sensors parent

public float disengagingHits = 0;

void Update()
{

if(GetComponent<HealthManager>().health <= 0)
{
CharacterDead();
}

ragdollObject = parent.GetComponent<AIStateManager>().animationManager;

}


//what to do if we get knocked out
public void Knockout()
{

//ragdollify
GetComponent<HealthManager>().FallDown();

//don't allow to fire anymore
parent.GetComponent<AIWeaponController>().allowedToShoot = false;


disengagingHits += 1f;

//if we have gone over the amount of disengaging hits to knockout
if(disengagingHits > (disengagingHitsToKnockout-1f))
{
//what to do if we get a lot of critical hits
state = "disengaged";

			//send to everybody that we're dead
			gameObject.SendMessageUpwards("ShooterAiDead", SendMessageOptions.DontRequireReceiver);

//reset vars
ResetVariables();


//Debug
if(parent.GetComponent<AIStateManager>().debug)
{
Debug.Log("Disengaged");
}

			//disable other scripts
			if(parent.GetComponent<AIMovementController>() != null)
			{
				Destroy(parent.GetComponent<AIMovementController>());
				Destroy(parent.GetComponent<NavMeshAgent>());
				
			}
			if(parent.GetComponent<AIMovementControllerASTAR>() != null)
			{
				Destroy(parent.GetComponent<AIMovementControllerASTAR>());
				Destroy(parent.GetComponent<ShooterAIPathFinder>());
				Destroy(parent.GetComponent<Seeker>());
				
			}
			
			Destroy(parent.GetComponent<AIControllerChild>());
			Destroy(parent.GetComponent<SearchCover>());
			Destroy(parent.GetComponent<TestCover>());
			Destroy( parent.GetComponent<AIMovementSwitcher>() );
			Destroy(parent.GetComponent<AIStateManager>());
			Destroy(parent.GetComponent<AIWeaponController>());
			Destroy(ragdollObject.GetComponent<Animator>());
			Destroy(ragdollObject.GetComponent<HandIK>());
			Destroy(sensors.gameObject);
			
			
			
			
			//reset tag to avoid ai shooting at bodies
			parent.transform.tag = "Untagged";

			
			//drop weapon
			parent.GetComponent<AIWeaponController>().DropWeapon();
			
			//turn on ragdoll
			ragdollObject.GetComponent<RagdollTransitions>().EnableRagdoll();
			
			//destory self
			Destroy(gameObject);


}


}



//when we die
public void CharacterDead()
{

state = "dead";

//reset vars
ResetVariables();

		//send to everybody that we're dead
		gameObject.SendMessageUpwards("ShooterAiDead", SendMessageOptions.DontRequireReceiver);
		
		
//Debug
if(gameObject.activeSelf == true)
{
if(parent.GetComponent<AIStateManager>().debug)
{
Debug.Log("Dead");
}
}


		//disable other scripts
		if(parent.GetComponent<AIMovementController>() != null)
		{
			Destroy(parent.GetComponent<AIMovementController>());
			Destroy(parent.GetComponent<NavMeshAgent>());
			
		}
		if(parent.GetComponent<AIMovementControllerASTAR>() != null)
		{
			Destroy(parent.GetComponent<AIMovementControllerASTAR>());
			Destroy(parent.GetComponent<ShooterAIPathFinder>());
			Destroy(parent.GetComponent<Seeker>());
			
		}
		
		Destroy(parent.GetComponent<AIControllerChild>());
		Destroy(parent.GetComponent<SearchCover>());
		Destroy(parent.GetComponent<TestCover>());
		Destroy( parent.GetComponent<AIMovementSwitcher>() );
		Destroy(parent.GetComponent<AIStateManager>());
		Destroy(parent.GetComponent<AIWeaponController>());
		Destroy(ragdollObject.GetComponent<HandIK>());
		Destroy(ragdollObject.GetComponent<Animator>());
		Destroy(sensors.gameObject);
		
		
		
		

//reset tag to avoid ai shooting at bodies
parent.transform.tag = "Untagged";

//drop weapon
parent.GetComponent<AIWeaponController>().DropWeapon();

//destory self
Destroy(gameObject);

//change into ragdoll
ragdollObject.GetComponent<RagdollTransitions>().EnableRagdoll();


}


//pass onto our brain that we are hit
public void DeductHealth()
{
parent.GetComponent<AIStateManager>().AiHit();

}





/// <summary>
/// Resets the variables, such as tags
/// </summary>
public void ResetVariables()
{
//set the tag to null, as we are dead/knockedout so we cant influence ai decision making
parent.tag = null;
}


}
