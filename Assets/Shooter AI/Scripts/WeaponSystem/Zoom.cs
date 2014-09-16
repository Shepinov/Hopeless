//attach to WeaponLocation
//this is a simple zoom script

using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour {

private float baseFOV;
     
    void Start () {
    baseFOV = Camera.main.fieldOfView;
    }
     
void Update () {
if (Input.GetButton("Fire2"))
{
if(Camera.main.fieldOfView > 10)
Camera.main.fieldOfView -= 10;
}
else
{
if(Camera.main.fieldOfView < baseFOV)
Camera.main.fieldOfView += 10;
}
}

}
