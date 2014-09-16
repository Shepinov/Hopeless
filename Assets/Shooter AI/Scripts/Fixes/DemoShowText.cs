using UnityEngine;
using System.Collections;

public class DemoShowText : MonoBehaviour {

public string textToDisplay; //the text to display


void OnGUI()
{

GUI.Label( new Rect(20,20, 400f, 150f), textToDisplay);
GUI.Label(new Rect(20, 200f, 200f, 200f), "Press R to restart");

}

void Update()
{

if(Input.GetKeyDown(KeyCode.R))
{
Application.LoadLevel(Application.loadedLevel);
}

}



}
