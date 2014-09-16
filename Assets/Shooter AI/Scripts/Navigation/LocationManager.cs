// Created by Trevor Blize
// This script is just a base for the Ai to follow to a location given by sub scripts

//attach to patrol manager gameobject
//this script returns the next vector3 to go to, based on current location script

using UnityEngine;
using System.Collections;

public class LocationManager : MonoBehaviour {

    public Vector3 nextDestination; //the next position to go to
    public bool useOwnNavSystem = false; //whether to use own navmesh; this value is controlled by AIMovementController
    public NavMesh meshNav = new NavMesh(); //the navmesh we will be using
    public float criticalDistanceToWaypoint; //how close do we have to be to a waypoint to count as if we've reached it

    protected virtual void Start()
    {

    }

    protected virtual void Update() 
    {

    }
}
