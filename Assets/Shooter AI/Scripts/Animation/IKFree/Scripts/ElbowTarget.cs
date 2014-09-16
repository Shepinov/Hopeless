using UnityEngine;
using System.Collections;

public enum ArmType { Left, Right};

public class ElbowTarget : MonoBehaviour {
	
	
	public ArmType elbowTargetType; //the type of elbow target
	
	public Vector3 offsetEngaged = Vector3.zero; //the offset
	public Vector3 offsetPatrol = Vector3.zero;
	
	
	private ShooterAITransformPositionReference reference;
	private Vector3 finalOffset;
	
	
	void Awake()
	{
		reference = transform.parent.parent.parent.parent.parent.GetComponentInChildren<ShooterAITransformPositionReference>();
		
		if( elbowTargetType == ArmType.Left )
		{
			transform.parent = transform.parent.parent.parent.parent.parent.GetComponentInChildren<Animator>().GetBoneTransform( HumanBodyBones.LeftLowerArm);
			transform.localPosition = Vector3.zero;
			
			//transform.localPosition = transform.parent.transform.right * 0.2f;
		}
		
		if( elbowTargetType == ArmType.Right )
		{
		
			//Debug.Log(transform.parent.parent.parent.parent.parent.GetComponentInChildren<Animator>().avatar.ToString(), gameObject);
			
			transform.parent = transform.parent.parent.parent.parent.parent.GetComponentInChildren<Animator>().GetBoneTransform( HumanBodyBones.RightLowerArm);
			transform.localPosition = Vector3.zero;
			
			
			
			//transform.localPosition = transform.parent.transform.right * 0.2f;
		}
		
		
		
	}
	
	
	void OnGUI()
	{
		if(reference.inResting == true)
		{
			finalOffset = offsetPatrol;
		}
		else
		{
			finalOffset = offsetEngaged;
		}
		
		
		transform.localPosition = new Vector3( 0, 0, 0) + finalOffset;
	}
	
	
}
