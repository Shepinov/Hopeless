using UnityEngine;
using System.Collections;
 
public class DontGoThroughThings : MonoBehaviour 
{
	public LayerMask layerMask; //make sure we aren't in this layer 
	public float skinWidth = 0.1f; //probably doesn't need to be changed 
 
	private float minimumExtent; 
	private float partialExtent; 
	private float sqrMinimumExtent; 
	private Vector3 previousPosition; 
	private Rigidbody myRigidbody; 
 
 
	//initialize values 
	void Awake() 
	{ 
	   myRigidbody = rigidbody; 
	   previousPosition = myRigidbody.position; 
	   minimumExtent = Mathf.Min(Mathf.Min(collider.bounds.extents.x, collider.bounds.extents.y), collider.bounds.extents.z); 
	   partialExtent = minimumExtent * (1.0f - skinWidth); 
	   sqrMinimumExtent = minimumExtent * minimumExtent; 
	} 
 
	void FixedUpdate() 
	{ 
	   //have we moved more than our minimum extent? 
	   Vector3 movementThisStep = myRigidbody.position - previousPosition; 
	   float movementSqrMagnitude = movementThisStep.sqrMagnitude;
 
	   if (movementSqrMagnitude > sqrMinimumExtent) 
		{ 
	      float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
	      RaycastHit hitInfo; 

			Debug.DrawRay(previousPosition, movementThisStep, Color.red);

	      //check for obstructions we might have missed 
	      if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
			{
			
			//check if we hit our target
				//hitInfo.collider.gameObject.SendMessageUpwards("Damage", GetComponent<BulletHealthExtras>().extraHitValue, SendMessageOptions.DontRequireReceiver);
			
			
	         myRigidbody.position = hitInfo.point - (movementThisStep/movementMagnitude)*partialExtent; 

				
				if(GetComponent<BulletHealthExtras>() != null && hitInfo.transform != null && hitInfo.transform != transform)
				{
					//Debug.Log("Hit 2");
					GetComponent<BulletHealthExtras>().hitPoint = hitInfo.point;
					GetComponent<BulletHealthExtras>().hitNormal = hitInfo.normal;
					GetComponent<BulletHealthExtras>().StartCoroutine("BulletHit", hitInfo.transform);
				}
			}
} 
 
	   previousPosition = myRigidbody.position; 
	}
}