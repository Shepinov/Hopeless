using UnityEngine;
using System.Collections;

/// <summary>
/// AI movement switcher which controls which functions go to which manager
/// </summary>
public class AIMovementSwitcher : MonoBehaviour {
	
	
	public Vector3 destinationPosition;
	public Vector3 velocity;
	public float speed;
	public bool frozen = false;
	public bool allowedToMove = true; //whether allowed to move or not
	public float minDistanceToDestination = 2f; 
	
	
	void Update()
	{
		//control whether we're engaged or not
		if( allowedToMove == false )
		{
			frozen = true;
			speed = 0f;
			SetSpeed( 0f);
		}
		
		
		if(GetComponent<AIMovementController>() != null)
		{
			destinationPosition = GetComponent<AIMovementController>().destinationPosition;
			velocity = GetComponent<NavMeshAgent>().velocity;
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			destinationPosition = GetComponent<AIMovementControllerASTAR>().destinationPosition;
			velocity = GetComponent<ShooterAIPathFinder>().CalculateVelocity( GetComponent<ShooterAIPathFinder>().GetFeetPosition() );
		}
		
		//frozen
		if(GetComponent<AIMovementController>() != null)
		{
			frozen = GetComponent<AIMovementController>().frozen;
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			frozen = GetComponent<AIMovementControllerASTAR>().freeze;
		}
		
		
		//min distance
		if(GetComponent<AIMovementController>() != null)
		{
			minDistanceToDestination = GetComponent<AIMovementController>().minDistanceToDestination;
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			minDistanceToDestination = GetComponent<AIMovementControllerASTAR>().minDistanceToDestination;
		}
		
	}
	
	
	
	
	
	public void SetNewDestination(Vector3 newDestination)
	{
		
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().SetNewDestination( newDestination);
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().SetNewDestination( newDestination);
		}
		
		
	}
	
	
	public void Freeze()
	{
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().Freeze();
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().Freeze();
		}
	}
	
	
	public void Defreeze()
	{
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().Defreeze();
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().Defreeze();
		}
	}
	
	
	
	public void SetSpeed(float speedTarget)
	{
		speed = speedTarget;
		
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().SetSpeed( speedTarget);
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().SetSpeed( speedTarget);
		}
	}
	
	
	
	public Vector3 FindClosestHalfCover()
	{
		if(GetComponent<AIMovementController>() != null)
		{
			return GetComponent<AIMovementController>().FindClosestHalfCover();
			
		}
		else
		{
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			return GetComponent<AIMovementControllerASTAR>().FindClosestHalfCover();
		}
		else
		{
				return Vector3.zero;
		}
		}
		
	}
	
		
	public float CalculatePathLength(Vector3 targetPosition)
	{
		if(GetComponent<AIMovementController>() != null)
		{
			return GetComponent<AIMovementController>().CalculatePathLength( targetPosition);
			
		}
		else
		{
			if(GetComponent<AIMovementControllerASTAR>() != null)
			{
				return GetComponent<AIMovementControllerASTAR>().CalculatePathLength( targetPosition);
			}
			else
			{
				return -1f;
			}
		}
		
	}
	
	
	/// <summary>
	/// Turns off rotation.
	/// </summary>
	public void TurnOffRotation()
	{
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().TurnOffRotation();
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().TurnOffRotation();
		}
	}
	
	
	/// <summary>
	/// Turns on rotation.
	/// </summary>
	public void TurnOnRotation()
	{
		if(GetComponent<AIMovementController>() != null)
		{
			GetComponent<AIMovementController>().TurnOnRotation();
			
		}
		if(GetComponent<AIMovementControllerASTAR>() != null)
		{
			GetComponent<AIMovementControllerASTAR>().TurnOnRotation();
		}
	}
		
	
}
