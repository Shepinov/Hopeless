using UnityEngine;
using System.Collections;

public class ReloadAnimation : MonoBehaviour {
	
	
	public string animationName = "Reloading"; 
	
	private AIWeaponSelected weapon;
	private HandToUse handUsed;
	private bool prevWeaponState = false;
	
	
	void Awake()
	{
		weapon = transform.parent.GetComponent<AIWeaponController>().weaponHoldingObject.GetComponent<AIWeaponSelected>();
	}
	
	
	void Update()
	{
		
		if(weapon.reloading == true && prevWeaponState == false)
		{
			//play anim
			GetComponent<Animator>().Play(animationName);
			
			//get and set hands correctly
			handUsed = GetComponent<HandIK>().handToUseInCharacter;
			GetComponent<HandIK>().handToUseInCharacter = HandToUse.BothHands;
			
		}
		
		if(weapon.reloading == false && prevWeaponState == true)
		{
			//reset stuff
			GetComponent<HandIK>().handToUseInCharacter = handUsed;
		}
	
		
		//for late use
		prevWeaponState = weapon.reloading;
	}
	
	
	
}
