using UnityEngine;
using System.Collections;


/// <summary>
///IK manager for the free version.
/// </summary>
public class IKFreeManager : MonoBehaviour {
	
	
	public bool activatedFreeIK = false; //whether to activate the ik or not
	
	public Transform leftArm, rightArm; //the arms which contain their resprective parts
	public GameObject model; //the main model
	
	
	private int framesToCheck = 60; //once how many frames to reset the variables
	private int framesChecked = 0; //the frames checked
	
	public HandToUse handToUse; //the hand to use
	
	
	
	void Awake () 
	{
		
		model = transform.parent.parent.parent.GetComponentInChildren<HandIK>().gameObject;
		framesToCheck += Random.Range(-10, 10);
		
		
		#if UNITY_EDITOR
		//if we have free version of unity, so no IK
		
		if(UnityEditorInternal.InternalEditorUtility.HasPro() == false)
		{
			activatedFreeIK = true;
			
			
		}
		#endif
		
		if(activatedFreeIK == true)
		{
			//set the ik vars correctly
			SetVariablesCorrectly();
		}
		else
		{
			//deactivate children
			leftArm.gameObject.SetActive( false );
			rightArm.gameObject.SetActive( false );
		}	
	
	}
	
	
	
	void Update()
	{
		
		if(activatedFreeIK == false)
		{
			return;
		}
		
		if(model.GetComponent<HandIK>() == null)
		{
			Destroy( this);
		}
		
		//optimisation to check vars correctly
		framesChecked += 1;
		if(framesChecked > framesToCheck)
		{
			framesChecked = 0;
			SetVariablesCorrectly();
			
		}
		
		//hand to use
		if(model.GetComponent<HandIK>() != null)
		{
			handToUse = model.GetComponent<HandIK>().handToUseInCharacter;
		}
		
		switch(handToUse)
		{
		case HandToUse.BothHands: leftArm.gameObject.SetActive(true); rightArm.gameObject.SetActive(true); break;
		case HandToUse.LeftHand: leftArm.gameObject.SetActive(true); rightArm.gameObject.SetActive(false); break;
		case HandToUse.RightHand: leftArm.gameObject.SetActive(false); rightArm.gameObject.SetActive(true); break;
		case HandToUse.NoHands: leftArm.gameObject.SetActive(false); rightArm.gameObject.SetActive(false); break;
		}
		
		
	}
	
	
	
	
	/// <summary>
	/// Sets the variables on the IK parts correctly.
	/// </summary>
	public void SetVariablesCorrectly()
	{
		
		if(model.GetComponent<Animator>() == null)
		{
			return;
		}
		
		
		IKLimb lArm = leftArm.GetComponent<IKLimb>();
		IKLimb rArm = rightArm.GetComponent<IKLimb>();
		
		
		//left arm
		lArm.upperArm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftUpperArm);
		lArm.forearm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerArm);
		lArm.hand = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);
		lArm.target = model.GetComponent<HandIK>().leftHandObj;
		
		//rightarm
		rArm.upperArm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightUpperArm);
		rArm.forearm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightLowerArm);
		rArm.hand = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
		rArm.target = model.GetComponent<HandIK>().rightHandObj;
		
		
		/*
		//assign some variables
		IKControl lArm = leftArm.GetComponent<IKControl>();
		IKControl lHand = leftHand.GetComponent<IKControl>();
		IKControl rArm = rightArm.GetComponent<IKControl>();
		IKControl rHand = rightHand.GetComponent<IKControl>();
		
		
		//start with the left arm
		lArm.forearm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerArm);
		lArm.hand = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerArm);
		lArm.target = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);
		
		//then the left hand
		lHand.forearm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerArm);
		lHand.hand = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);
		lHand.target = model.GetComponent<HandIK>().leftHandObj;
		
		//now the right arm
		rArm.forearm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightLowerArm);
		rArm.hand = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightLowerArm);
		rArm.target = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
		
		//then the right hand
		rHand.forearm = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightLowerArm);
		rHand.hand = model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
		rHand.target = model.GetComponent<HandIK>().rightHandObj;*/
		
	}
	
	
	
	
}
