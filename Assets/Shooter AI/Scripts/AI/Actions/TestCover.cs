//test

using UnityEngine;
using System.Collections;

public class TestCover : MonoBehaviour {

private Vector3 test;

void Start()
{
StartCoroutine("GoTOPosition");
test = transform.position;
}

void Update()
{
transform.LookAt(test);

if(Vector3.Distance(transform.position, test) > 1f)
{
//transform.position = Vector3.MoveTowards(transform.position, test, 5*Time.deltaTime);
rigidbody.MovePosition(Vector3.MoveTowards(transform.position, test, 5*Time.deltaTime));
}


RaycastHit hit2;
Debug.DrawRay(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position - transform.position, Color.yellow);
if (Physics.Raycast(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position - transform.position, out hit2))
{
if(hit2.transform.tag == "Player")
{
GetComponent<SearchCover>().FindCoverPostion( GetComponent<SearchCover>().eyePosition, false);
test = GetComponent<SearchCover>().coverPostion;
}
}

}


IEnumerator GoTOPosition()
{



GetComponent<SearchCover>().FindCoverPostion( GetComponent<SearchCover>().eyePosition, false);
test = GetComponent<SearchCover>().coverPostion;

yield return new WaitForSeconds(0.8f);

Reset();
}

void Reset()
{
StartCoroutine("GoTOPosition");
}



void OnDrawGizmos()
{

Gizmos.color = Color.yellow;
Gizmos.DrawSphere(test, 0.5f);
}

}
