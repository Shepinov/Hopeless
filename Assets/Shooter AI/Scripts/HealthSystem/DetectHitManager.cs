//attach to the health system manager, all other detect hit scripts rely on this one

using UnityEngine;
using System.Collections;

public class DetectHitManager : MonoBehaviour {

public string tagOfBullet; //the tag of the bullet


void Update()
{
//update tags
tagOfBullet = GetComponent<AIHealthExecuter>().parent.GetComponent<AIStateManager>().tagToAvoidSecondary;

}



	//what to do once somehting hits us
	public void BulletHitArea(float amountHit, bool criticalArea, bool disablingArea)
	{
		
		
		//pass it onto the main health manager
		SendMessage("DeductHealth", amountHit, SendMessageOptions.DontRequireReceiver);
		
		//send it upwards
		SendMessageUpwards("DeductHealth", amountHit, SendMessageOptions.DontRequireReceiver);
		
		
		if(criticalArea)
		{
			SendMessage("CharacterDead");
			SendMessageUpwards("CharacterDead", SendMessageOptions.DontRequireReceiver);
		}
		
		if(disablingArea)
		{
			SendMessage("Knockout");
			SendMessageUpwards("Knockout", SendMessageOptions.DontRequireReceiver);
		}
		
}


}
