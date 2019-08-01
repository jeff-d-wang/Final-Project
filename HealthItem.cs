using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class HealthItem : MonoBehaviour
{
    private int health;
    private float cooldown;
    BoxCollider col;

    public GameObject[] healableObjects;
    GameObject current;

    // Start is called before the first frame update
    private void Start()
    {
        health = 50;
        cooldown = 0.0f;
        col = this.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
            GetComponent<Renderer>().enabled = false;
        }
        else
        {
            col.enabled = true;
            GetComponent<Renderer>().enabled = true;
            transform.Rotate(0.0f, 10.0f, 0.0f);
            for (int i = 0; i < healableObjects.Length; i++)
            {
                if (healableObjects[i] != null)
                {
                    current = healableObjects[i];
                    if (Vector3.Distance(current.transform.position, transform.position) < 2)
                    {

                        switch (current.tag)
                        {
                            case "Player":
                                TankHealth tank = current.GetComponent<TankHealth>();
                                tank.SetCurrentHealth(tank.GetCurrentHealth() + (health / 2.0f));
                                cooldown = 10.0f;
                                break;
                            case "Scout":
                                current.GetComponent<EnemyScoutAI>().changeCurrentHealth(health);
                                cooldown = 5.0f;
                                break;
                            case "Aggessor":
                                col.enabled = true;
                                current.GetComponent<EnemyAggressorAI>().changeCurrentHealth(health);
                                cooldown = 5.0f;
                                break;
                            case "Ambusher":
                                col.enabled = true;
                                current.GetComponent<EnemyAmbusherAI>().changeCurrentHealth(health);
                                cooldown = 5.0f;
                                break;
                        }
                    }
                }
            }
        }
    }
}
