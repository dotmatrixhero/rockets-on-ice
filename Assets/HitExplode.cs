using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitExplode : MonoBehaviour {

    public Action<GameObject> OnDeath;
    // Use this for initialization
    void OnEnable()
    {
        Invoke("Destroy", .15f);

    }
    // Update is called once per frame
    void Destroy()
    {
        gameObject.SetActive(false);
        OnDeath(this.gameObject);
    }

}
