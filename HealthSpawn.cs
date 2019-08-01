using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class HealthSpawn : MonoBehaviour
{
    public bool enabled;
    public HealthItem obj;
    //Stopwatch stopWatch = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        enabled = true;
        HealthItem healthItem = Instantiate(obj);
        healthItem.transform.parent = this.transform;
    }

    // Update is called once per frame

}
