//attach to anything that is ammo

using UnityEngine;
using System.Collections;

public class AmmoMain : MonoBehaviour {


public float ammoInside; //the ammount of ammo this object gives
public string playerTag; //the tag of the player

private bool playAnim = false;

public void PlayFinalAnimation()
{
playAnim = true;
}


void Update()
{

if(playAnim == true)
{
GetComponent<BoxCollider>().enabled = false;
transform.position = Vector3.MoveTowards(transform.position, GameObject.FindGameObjectWithTag(playerTag).transform.position, 0.3f);

if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag(playerTag).transform.position) < 1f)
{
Destroy(gameObject);
}

}



}

}
