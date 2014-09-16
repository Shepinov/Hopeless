using UnityEngine;
using System.Collections;

public class BulletMeleeScript : MonoBehaviour {
	
	public float extraHitValue = 30f; //the hit value
	
	
	void OnTriggerEnter(Collider collider)
	{
		collider.gameObject.SendMessage( "Damage", extraHitValue, SendMessageOptions.DontRequireReceiver );
		
		Destroy( gameObject);
	}
	
}
