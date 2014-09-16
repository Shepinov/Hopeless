//attach to anything that needs to travel with the object it was created

using UnityEngine;
using System.Collections;

public class TravelWithObject : MonoBehaviour {

public GameObject objectToTravelWith; //the object to travel with

void Update()
{

transform.position = objectToTravelWith.transform.position;
}



}
