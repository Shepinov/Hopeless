using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
	public float time = 0.04f;
	
	private float timer;
	
	void Start ()
	{
		timer = time;
		StartCoroutine("Flicker");
	}
	
	IEnumerator Flicker()
	{
		while(true)
		{
			light.enabled = !light.enabled;
			
			do
			{
				timer -= Time.deltaTime;
				yield return null;
			}
			while(timer > 0);
			timer = time;
		}
	}
}
