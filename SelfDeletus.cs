using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDeletus : MonoBehaviour
{
    public float time;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 20)
        {
            Destroy(this.gameObject);
        }
    }
}
