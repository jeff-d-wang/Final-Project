using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;
using Complete;

public class EnemyScoutAI : MonoBehaviour
{
    public Transform player;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public GameObject centralAI;

    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    public float maxHealth = 150.0f;
    float currentHealth;
    float damage = 25.0f;
    float fireRate = 1;
    float rotSpeed = 5.0f;
    float damaged = 1.0f;

    float visibleRange = 100.0f;
    float shotRange = 60.0f;

    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.

    void Start()
    {
        currentHealth = maxHealth;
        SetHealthUI();

        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0; //for a little buffer
    }

    void Update()
    {
        if(currentHealth < 100.0f)
        {
            agent.speed = 7f;
        }
        else
        {
            agent.speed = 4.0f;
        }
    }



    public void setCurrentHealth(float newHealth)
    {
        currentHealth = newHealth;

        SetHealthUI();
    }

    public void changeCurrentHealth(float health)
    {
        currentHealth += health;

        // Change the UI elements appropriately.
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = currentHealth;
        print(m_Slider.value);

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, currentHealth / maxHealth);
        print(m_FillImage.color);
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    //Comparision Tasks

    [Task]
    public bool SeePlayer()
    {
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool seeWall = false;

        Debug.DrawRay(this.transform.position, distance, Color.red);

        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            seeWall |= hit.collider.gameObject.tag == "wall";
        }

        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        }
        return (distance.magnitude < visibleRange && !seeWall);
    }

    [Task]
    public bool IsHealthLessThan(float health)
    {
        return this.currentHealth < health;
    }

    [Task]
    public bool WasLastPlayerSightedLessThan(int seconds)
    {
        return centralAI.GetComponent<CentralAI>().getTimeSincePlayerSpotted() < seconds;
    }

    [Task]
    public bool Explode()
    {
        Destroy(this.gameObject);
        return true;
    }

    [Task]
    public bool IsDistanceToPlayerLessThan(float minDist)
    {
        Vector3 distance = player.transform.position - this.transform.position;
        return (distance.magnitude < minDist);
    }


    //Movement

    [Task]
    public void MoveToDestination()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void SetDestination(float x, float z)
    {
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void SetTargetDestination()
    {
        agent.SetDestination(target);
        Task.current.Succeed();
    }

    [Task]
    public void TargetPlayer()
    {
        target = player.transform.position;
        Task.current.Succeed();
    }


    [Task]
    public void Roam()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void SearchForPlayer()
    {
        Vector3 dest = centralAI.GetComponent<CentralAI>().getLastPlayerLocation() + new Vector3(Random.Range(-5, 5), this.transform.position.y, Random.Range(-10, 10));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void ExactSearchForPlayer()
    {
        Vector3 dest = centralAI.GetComponent<CentralAI>().getLastPlayerLocation();
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void PickHealStation()
    {
        agent.stoppingDistance = 0;
        print("working");
        Vector3[] healStations = centralAI.GetComponent<CentralAI>().getHealStations();
        float maxDistanceFromPlayer = Vector3.Distance(healStations[0], centralAI.GetComponent<CentralAI>().getLastPlayerLocation());
        int bestIndex = 0;
        for (int i = 1; i < healStations.Length; i++)
        {
            if (maxDistanceFromPlayer < Vector3.Distance(healStations[i], centralAI.GetComponent<CentralAI>().getLastPlayerLocation()))
            {
                maxDistanceFromPlayer = Vector3.Distance(healStations[i], centralAI.GetComponent<CentralAI>().getLastPlayerLocation());
                bestIndex = i;
            }
        }
        Vector3 dest = healStations[bestIndex];
        agent.SetDestination(dest);
        print("at health station");
        print(dest);
        Task.current.Succeed();
    }

    [Task]
    public void Retreat()
    {
        Vector3 awayFromPlayer = Vector3.Normalize(this.transform.position - player.transform.position);
        Vector3 dest = Vector3.zero;
        if (Random.Range(0, 2) == 2)
        {
            dest = this.transform.position + (awayFromPlayer * Random.Range(4, 7));
        }
        else
        {
            dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 10));
        }
        agent.SetDestination(dest);
        Task.current.debugInfo = string.Format("angle={0} {1} {2}",dest.x, dest.y, dest.z);
        Task.current.Succeed();
    }


    //Rotation


    [Task]
    public bool Turn(float angle)
    {
        var p = this.transform.position +
                Quaternion.AngleAxis(angle, Vector3.up) *
                this.transform.forward;
        target = p;
        return true;
    }

    [Task]
    public bool ShotLinedUp()
    {
        agent.stoppingDistance = shotRange - 5;
        Vector3 distance = target - this.transform.position;
        return (distance.magnitude < shotRange &&
            Vector3.Angle(this.transform.forward, distance) < 5.0f);
    }

    [Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction),
                                        Time.deltaTime * rotSpeed);
        //Debug.Log(Vector3.Angle(this.transform.forward, direction));
        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("angle={0}",
                Vector3.Angle(this.transform.forward, direction));
        }
        if (Vector3.Angle(this.transform.forward, direction) < 4.0f)
        {
            bulletSpawn.transform.LookAt(player);
            Task.current.Succeed();
        }
    }


    //Other


    [Task]
    public bool Snipe()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab,
                                bulletSpawn.transform.position,
                                bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(
                                bullet.transform.forward * 30);
        agent.stoppingDistance = 0;
        GetComponent<AudioSource>().Play(0);
        return true;
    }

    [Task]
    public void UpdatePlayerLocation()
    {
        centralAI.GetComponent<CentralAI>().setLastPlayerLocation(new Vector3(player.position.x, player.position.y, player.position.z));
        Task.current.Succeed();
    }


}

