using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Complete;
using Panda;

public class CentralAI : MonoBehaviour
{
    public GameObject player;
    TankHealth tank;
    public int timeSincePlayerSpotted;
    public Vector3 lastPlayerLocation;
    public Vector3[] healStations = new Vector3[2];
    // Start is called before the first frame update
    void Start()
    {
        timeSincePlayerSpotted = 120;
        InvokeRepeating("updateTimeSincePlayerSpotted", 0.0f, 1.0f);
        tank = player.GetComponent<TankHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getLastPlayerLocation()
    {
        return lastPlayerLocation;
    }

    public int getTimeSincePlayerSpotted()
    {
        return timeSincePlayerSpotted;
    }

    public Vector3[] getHealStations()
    {
        return healStations;
    }

    public float getPlayerHealth()
    {
        return tank.GetCurrentHealth();
    }

    public void setLastPlayerLocation(Vector3 loc)
    {
        lastPlayerLocation = loc;
        timeSincePlayerSpotted = 0;
    }

    private void updateTimeSincePlayerSpotted()
    {
        timeSincePlayerSpotted++;
    }
}
