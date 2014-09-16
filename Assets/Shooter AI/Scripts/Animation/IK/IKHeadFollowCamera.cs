//this script makes the head turn with the camera
//use the "WeaponLocationZoom" object for the camera2 variable
//attach to model controller



using UnityEngine;
using System.Collections;

public class IKHeadFollowCamera : MonoBehaviour {

public GameObject camera2;


private Animator animator;


void Awake()
{
animator = GetComponent<Animator>();
}



void OnAnimatorIK(int layerIndex)
{
if(animator)
{
//head rotation
animator.SetLookAtWeight(1f,0.3f,0.6f,1.0f,0.5f);
animator.SetLookAtPosition(camera2.transform.position);

}

}


}
