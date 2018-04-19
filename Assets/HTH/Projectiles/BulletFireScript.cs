using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFireScript : MonoBehaviour {

    public float firetime = .2F;
    public GameObject bullet;
    public GameObject hit;
    public int poolSize = 20;
    public List<GameObject> activebullets;
    public List<GameObject> deactivebullets;

    public List<GameObject> activehits;
    public List<GameObject> deactivehits;

    // Use this for initialization
    void Start () {
        activebullets = new List<GameObject>();
        deactivebullets = new List<GameObject>();
        activehits = new List<GameObject>();
        deactivehits = new List<GameObject>();
        // HOW IS DEACTIVE HITS GETTING POPULATED WHEN I DON'T ADD ANYTHING TO IT???!?!?!?!
        for (int i = 0;i < poolSize; i++)
        {
            GameObject obj = (GameObject)Instantiate(bullet);
            GameObject hitt = (GameObject)Instantiate(hit);
            hitt.SetActive(false);
            hitt.GetComponent<HitExplode>().OnDeath += DestroyHit;

            obj.SetActive(false);
            obj.GetComponent<BulletDestroy>().bulletowner = this.gameObject.GetComponent<GunAim>().player;
            obj.GetComponent<BulletDestroy>().OnDeath += ExpireBullet;


            deactivebullets.Add(obj);
        }

        InvokeRepeating("Fire", firetime, firetime);	
	}
	
	// Update is called once per frame
	void Fire () {
        if (deactivebullets.Count > 0) {
            int i = 0;
             deactivebullets[i].transform.position = transform.position;
             deactivebullets[i].transform.rotation = transform.rotation;
             deactivebullets[i].transform.Rotate(90, 0, 90);
             deactivebullets[i].SetActive(true);
             (deactivebullets[i].GetComponent<BulletDestroy>()).parent = this;
            activebullets.Add(deactivebullets[i]);
            deactivebullets.RemoveAt(0);

        }
	}

    void ExpireBullet(GameObject bullet)
    {
        int i = 0;
        foreach (GameObject obj in activebullets)
        {
            i++;
            if (obj == bullet)
            {
                if (deactivehits.Count > 0)
                {
                    deactivehits[0].transform.position = bullet.transform.position;
                    deactivehits[0].transform.rotation = bullet.transform.rotation;

                    deactivehits[0].SetActive(true);

                    activehits.Add(deactivehits[0]);
                    deactivehits.RemoveAt(0);
                }

                activebullets.Remove(obj);
                deactivebullets.Add(obj);
               

                
                break;
            }
        }
        
    }

    void DestroyHit(GameObject hit)
    {
        activehits.Remove(hit);
        deactivehits.Add(hit);
    }
}
