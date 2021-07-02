using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiPlayer : MonoBehaviour
{
    public bool isLocalPlayer;
    public double latitude;
    public double longitude;
    public float heading;
    public bool isOnline;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if we are the local player, and GPS is ready, grab GPS data
        if (isLocalPlayer)
        {
            if (GPS_Manager.instance.GPS_ready)
            {
                latitude = GPS_Manager.instance.deviceLAT;
                longitude = GPS_Manager.instance.deviceLNG;
                heading = GPS_Manager.instance.deviceHEADING;
            }
        }
    }
}
