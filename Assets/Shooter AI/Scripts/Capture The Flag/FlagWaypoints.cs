// Simple example of the new LocationManager

using UnityEngine;
using System.Collections;

public enum LocationState
{
    goToFlag,
    returnFlag,
}

public class FlagWaypoints : LocationManager
{

    private GameObject gameManager;
    private CTF _ctf;
    private Vector3 _baseLocation;
    public LocationState currentState = LocationState.goToFlag;

    protected override void Start()
    {
        base.Start(); // initialize base class
        gameManager = GameObject.FindGameObjectWithTag("GameManager"); 
        _ctf = gameManager.GetComponent<CTF>();
    }

    protected override void Update()
    {
        base.Update();
    }

    void LateUpdate()
    {
        if (currentState == LocationState.goToFlag)
        {
            // Find and goto flag location
            nextDestination = _ctf.FlagLocation();
        } else if (currentState == LocationState.returnFlag)
        {
            // flag holder brings the flag too the baseCapture of his team
            nextDestination = _baseLocation;
        } 
    }

    public void SetLocationState(LocationState state)
    {
        currentState = state;
    }

    public void SetBaseLocation(Vector3 baseLocation)
    {
        _baseLocation = baseLocation;
    }
}
