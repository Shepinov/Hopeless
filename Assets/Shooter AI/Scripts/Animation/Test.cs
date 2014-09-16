using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {


void FixedUpdate()
{
GetComponent<Animator>().SetFloat("Speed", Input.GetAxis("Vertical"));
}

}
