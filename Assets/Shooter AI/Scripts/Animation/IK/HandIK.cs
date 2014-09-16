//this is the main hand ik
//attach to main model
//to activate/deactivate ik, just set the ikactive variable to true/false


using UnityEngine;
using System.Collections;


public enum HandToUse{LeftHand, RightHand, BothHands, NoHands}


public class HandIK : MonoBehaviour {

public bool ikActiveGlobal = true; //whether the ik is active in any way at all
public bool ikActive = false; //whether it is active at this moment
public Transform rightHandObj = null;
public Transform leftHandObj = null;
public float currentWeight;

//this is for head rotation
public bool headShouldLook = false; //whether the head should look towards the "headLook" or not
public Vector3 headLook; //the position at which our head should look
//public float maxHeadAngle = 45f; //the max angle at which the head can look

//this is to select which hand we're holding

public HandToUse handToUseInCharacter;

	



private Animator animator;
private float lookAtWeight = 0f; //weight with which we're looking


void Start () 
{
animator = GetComponent<Animator>();

#if UNITY_EDITOR
//if we have free version of unity, so no IK

if(UnityEditorInternal.InternalEditorUtility.HasPro() == false)
{
ikActiveGlobal = false;
}

#endif
}

 //a callback for calculating IK
void OnAnimatorIK()
{
		//rounding errors
		if(lookAtWeight < 0.1f)
		{
			lookAtWeight = 0f;
		}
		
		
		

	      if(animator && ikActiveGlobal == true) {
			
                        //if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) {
				
				//animator.SetLookAtWeight(1f);
				//make it look in the right position
				if(headShouldLook)
				{
					animator.SetLookAtWeight(lookAtWeight);
					animator.SetLookAtPosition( Vector3.Lerp( animator.GetBoneTransform(HumanBodyBones.Head).transform.position, headLook, 0.1f) );
				}
				else
					{
					//animator.SetLookAtPosition(transform.position + transform.forward + );
					animator.SetLookAtWeight(lookAtWeight);
				}
				
				
				//weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
				if(handToUseInCharacter == HandToUse.RightHand || handToUseInCharacter == HandToUse.BothHands)
					{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,currentWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,currentWeight);
				}
				
				if(handToUseInCharacter == HandToUse.LeftHand || handToUseInCharacter == HandToUse.BothHands)
					{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,currentWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,currentWeight);
					}


			        //set the position and the rotation of the right hand where the external object is
				if(rightHandObj != null || leftHandObj != null) {
					
					if(handToUseInCharacter == HandToUse.RightHand || handToUseInCharacter == HandToUse.BothHands)
					{
					animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
					}
					
					if(handToUseInCharacter == HandToUse.LeftHand || handToUseInCharacter == HandToUse.BothHands)
					{
					animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHandObj.rotation);
						}
				}					

			}

			//if the IK is not active, set the position and rotation of the hand back to the original position
			else {			
				if(handToUseInCharacter == HandToUse.RightHand || handToUseInCharacter == HandToUse.BothHands)
					{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,currentWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,currentWeight);	
					}

					if(handToUseInCharacter == HandToUse.LeftHand || handToUseInCharacter == HandToUse.BothHands)
					{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,currentWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,currentWeight);		
					}

			}
		}
	}




void Update()
{
//this is for smooth transitions
if(ikActive && currentWeight < 0.95f  && ikActiveGlobal == true)
{
currentWeight = Mathf.Lerp(currentWeight , 1f, 0.1f);
}

if(ikActive == false && currentWeight > 0.05f && ikActiveGlobal == true)
{
currentWeight = Mathf.Lerp(currentWeight , 0f, 0.1f);
}


if(headShouldLook)
{
lookAtWeight = Mathf.Lerp(lookAtWeight, 1f, 0.1f);
}
else
{
lookAtWeight = Mathf.Lerp(lookAtWeight, 0f, 0.1f);
}



}


	  
}

