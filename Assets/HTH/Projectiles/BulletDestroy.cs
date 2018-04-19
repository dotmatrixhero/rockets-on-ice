using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : MonoBehaviour {

    public Action<GameObject> OnDeath;
    public BulletFireScript parent;
    public bool timeout = false;
    public GameObject bulletowner;
    bool passedself = false;



    // Use this for initialization
	void OnEnable () {
        Invoke("Destroy", 2f);
        passedself = false;
        
	}

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject != bulletowner.gameObject) {
            CancelInvoke();
            gameObject.SetActive(false);
            PlayaController pc = other.gameObject.GetComponent<PlayaController>();
            if (other.tag.Equals("Player"))
            {
          
             
                    pc.TakeDamage(1);
                
            }
            OnDeath(this.gameObject);

        }
    }

    // Update is called once per frame
    void Destroy () {
        timeout = true;
        gameObject.SetActive(false);
        OnDeath(this.gameObject);	
	}



    void OnDisable()
    {
        CancelInvoke();
    }
}
