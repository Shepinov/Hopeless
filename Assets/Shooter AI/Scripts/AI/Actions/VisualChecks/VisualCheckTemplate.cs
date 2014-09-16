//visual check template
//use as template

using UnityEngine;
using System.Collections;

public class VisualCheckTemplate : MonoBehaviour {


//what we need to check about the player
void VisualCheck()
{


//we then have to sendmessage back either: "Engage" or "StepDown", based on what happens in this void

SendMessage("Engage");


//and then destroy itself at the end
Destroy(this);

}


}
