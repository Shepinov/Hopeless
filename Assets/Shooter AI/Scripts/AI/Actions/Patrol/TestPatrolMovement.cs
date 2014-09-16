//TEST!!!!!!!

using UnityEngine;
using System.Collections;

public class TestPatrolMovement : MonoBehaviour {

public GameObject patrolManager; 


void Update()
{


rigidbody.MovePosition(Vector3.MoveTowards(transform.position, patrolManager.GetComponent<PatrolManager>().nextDestination, 0.5f));

}



}
