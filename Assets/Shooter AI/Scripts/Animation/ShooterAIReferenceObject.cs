//this is to set up the referecing object


using UnityEngine;
using System.Collections;

public class ShooterAIReferenceObject : MonoBehaviour {
	
private Animator animator;

void Awake()
{

animator = transform.parent.GetComponent<Animator>();


transform.parent = animator.GetBoneTransform(HumanBodyBones.Spine).transform;

}



}
