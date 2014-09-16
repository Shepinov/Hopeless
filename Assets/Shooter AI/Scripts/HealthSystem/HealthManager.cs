//attach to managing health object

using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

public float health; //the amount of health the character currently has

public float hitRagdollTimeFactor = 0.01f; //the factor with which to calculate the amount of time the ragdoll is activated



	//deduct health 
	public void DeductHealth(float amount)
	{
		health -= amount;


	}
	
	
	
	/// <summary>
	/// Makes the AI fall down.
	/// </summary>
	public void FallDown()
	{
		//Turn into a ragdoll for a short amount of time and then back
		if(health > 1f)
		{
			float amount = Random.Range( 0f, 300f);
			
			GetComponent<AIHealthExecuter>().ragdollObject.GetComponent<RagdollHelper>().ragdolled = true;
			
			StartCoroutine( GoBackToNormal( amount * hitRagdollTimeFactor) );
		}
	}
	
	
	
	/// <summary>
	/// Makes the AI fall down.
	/// </summary>
	public void FallDown(float timeInSeconds)
	{
		//Turn into a ragdoll for a short amount of time and then back

			
			GetComponent<AIHealthExecuter>().ragdollObject.GetComponent<RagdollHelper>().ragdolled = true;
			
			StartCoroutine( GoBackToNormal( timeInSeconds) );
	
	}
	
	
	
	IEnumerator GoBackToNormal(float amountOfTime)
	{
		
		yield return new WaitForSeconds(amountOfTime);
		
		GetComponent<AIHealthExecuter>().ragdollObject.GetComponent<RagdollHelper>().ragdolled = false;
		
		//allow to fire
		yield return new WaitForSeconds(0.7f);
	 	GetComponent<AIHealthExecuter>().parent.GetComponent<AIWeaponController>().allowedToShoot = true;
	}


}
