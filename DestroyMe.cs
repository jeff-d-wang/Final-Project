using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class DestroyMe : MonoBehaviour {

    public float damage;

    void OnCollisionEnter(Collision col)
	{
        if (col.gameObject.tag == "Player")
        {
            print("ya yeet you dumb Jonders");
            col.gameObject.GetComponent<TankHealth>().TakeDamage(damage);
        }
        Destroy(this.gameObject);
	}
}
