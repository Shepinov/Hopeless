//this script creates random muzzle flashes at its position
//attach to the object where muzzle flashes will be made

using UnityEngine;
using System.Collections;

public class MuzzleFlashManager : MonoBehaviour {

public GameObject[] muzzleFlashes; //the different muzzle flashes array


public void CreateMuzzleFlash()
{

GameObject muzzleflash;

//create a muzzle flash with picking one random flash out of all the ones given
if(muzzleFlashes.Length > 1)
{
muzzleflash = Instantiate(muzzleFlashes[Mathf.RoundToInt(Random.Range(0, muzzleFlashes.Length - 1f))], transform.position, transform.rotation) as GameObject;
}
else
{
muzzleflash = Instantiate(muzzleFlashes[0], transform.position, transform.rotation) as GameObject;
}

muzzleflash.GetComponent<TravelWithObject>().objectToTravelWith = gameObject;
}


}
