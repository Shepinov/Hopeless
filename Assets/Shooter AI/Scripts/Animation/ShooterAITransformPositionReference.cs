/// <summary>
/// Shooter AI transform position reference.
/// </summary>


using UnityEngine;
using System.Collections;



public class ShooterAITransformPositionReference : MonoBehaviour {
	
	
	public Transform objectAsReference; //the object that is our refernece
	public bool useX;
	public bool useY;
	public bool useZ;
	
	public Vector3 newPos;
	
	
	//for resting position weapon
	private string nameOfRestingPosition = "RestingPosition"; 
	private GameObject restingPosition;
	public bool inResting = false; //whether we're currently resting the weapon or not
	private AIStateManager brain;
	private AIWeaponController weaponController;
	

	void Start()
	{
		objectAsReference.transform.position = transform.position;
		
		//set up the resting position gameobject
		if(transform.Find(nameOfRestingPosition) == null )
		{
			restingPosition = new GameObject( nameOfRestingPosition );
			restingPosition.transform.parent = transform;
			restingPosition.transform.position = transform.position;
			
			restingPosition.transform.localRotation = Quaternion.Euler( new Vector3( 90f, 0f, 0f) );
			restingPosition.transform.localPosition -= new Vector3( 0f, 0.4f, 0f );
			
		}
		else
		{
			restingPosition = transform.Find(nameOfRestingPosition).gameObject;
		}
		
		//set up cache correctly
		brain = transform.parent.parent.GetComponent<AIStateManager>();
		weaponController = transform.parent.parent.GetComponent<AIWeaponController>();
		
	}
	
	
	void Update()
	{
		
		if(useX)
		{
			newPos.x =  objectAsReference.transform.position.x - transform.position.x;
		}
		
		if(useY)
		{
			newPos.y =  objectAsReference.transform.position.y - transform.position.y;
		}
		
		if(useX)
		{
			newPos.z =  objectAsReference.transform.position.z - transform.position.z;
		}
		
		transform.position += newPos;
		
		newPos = Vector3.zero;
		
		//calculate resting and apply
		if(brain != null)
		{
		
		if( brain.currentState == CurrentState.engage || brain.currentState == CurrentState.investigate 
		   || brain.currentState == CurrentState.chase || brain.enabled == false )
		{
			inResting = false;
			
			weaponController.weaponHoldingLocation.transform.position = Vector3.Lerp( weaponController.weaponHoldingLocation.transform.position, transform.position, 0.1f);
			weaponController.weaponHoldingLocation.transform.rotation = Quaternion.Lerp( weaponController.weaponHoldingLocation.transform.rotation, transform.rotation, 0.1f);
		}
		else
		{
			inResting = true;
			
			weaponController.weaponHoldingLocation.transform.position = Vector3.Lerp( weaponController.weaponHoldingLocation.transform.position, restingPosition.transform.position, 0.1f);
			weaponController.weaponHoldingLocation.transform.rotation = Quaternion.Lerp( weaponController.weaponHoldingLocation.transform.rotation, restingPosition.transform.rotation, 0.1f);
		}
		
		}
	}



}
