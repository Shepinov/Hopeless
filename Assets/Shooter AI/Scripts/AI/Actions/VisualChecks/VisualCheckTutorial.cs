using UnityEngine;
using System.Collections;

public class VisualCheckTutorial : MonoBehaviour {


void VisualCheck()
{


float randomNumber = Random.Range(0f, 100f);

if(randomNumber > 90f)
{
SendMessage("Engage");
Debug.Log("Engage");
}
else
{
SendMessage("StepDown");
Debug.Log("Stepped Down");
}


Destroy(this);
}


}
