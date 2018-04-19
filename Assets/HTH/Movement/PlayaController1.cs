using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This was non-inverse, free movement boost
public class PlayaController1 : MonoBehaviour {

    public float minspeed = 200;
    public float currentspeed = 20;

    public float startcharge = 200;
    public float exponentialchargerate = 1.06F;
    public float maxcharge = 4000F;

    public float maxfuel = 1F;
    public float fuelwait = 200F;
    public float fuelrechargerate = .1F;
    public int fuelboost = 4000;
    float currentfuel;
    float timesincelastfueluse;
    float timeoflastfueluse;
    float upchargeforce = 500;
     float downchargeforce = 500;
     float leftchargeforce = 500;
     float rightchargeforce = 500;
    Rigidbody rigidbody;

    public Vector3 upvector;
    public Vector3 rightvector;
    public Vector3 leftvector;
    public Vector3 downvector;

    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        currentfuel = maxfuel;
        timeoflastfueluse = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        var moveaugment = Input.GetAxis("LShoulder");
        if (moveaugment > 0)
            AugmentMove();
        else
        {
            Move();
            timesincelastfueluse += Time.time - timeoflastfueluse;
            if (timesincelastfueluse > fuelwait)
            {
                StartCoroutine("Refuel");
            }
        }
        Debug.Log(currentfuel);

    }

    void Move()
    {
        var chup = Input.GetAxisRaw("Vertical");
        var chright = Input.GetAxisRaw("Horizontal");

        if (chup > 0)
        {
            upchargeforce = upchargeforce * exponentialchargerate;
            upchargeforce = Mathf.Min(upchargeforce, maxcharge);
    
            upvector = new Vector3(0, 0, 1 * upchargeforce);
        }
        if (chup < 0)
        {

            downchargeforce *= exponentialchargerate;
            downchargeforce = Mathf.Min(downchargeforce, maxcharge);
            downvector = new Vector3(0, 0, -1 * downchargeforce);
        }
        if (chright > 0)
        {

            rightchargeforce *= exponentialchargerate;
            rightchargeforce = Mathf.Min(rightchargeforce, maxcharge);
            rightvector = new Vector3(1 * rightchargeforce, 0, 0);
        }
        if (chright  < 0)
        {
            leftchargeforce *= exponentialchargerate;
            leftchargeforce = Mathf.Min(leftchargeforce, maxcharge);
            leftvector = new Vector3(-1 * leftchargeforce, 0, 0);
        }
        ReleaseMovement();
    }

    void ReleaseMovement()
    {
        var chup = Input.GetAxisRaw("Vertical");
        var chright = Input.GetAxisRaw("Horizontal");

        var goup = (chup == 0 & upvector != Vector3.zero);
        var godown = (chup == 0 & downvector != Vector3.zero);
        var goleft = (chright == 0 & leftvector != Vector3.zero);
        var goright = (chright == 0 & rightvector != Vector3.zero);
        Vector3 brake = rigidbody.velocity;
        if (goup)
        {
            brake = new Vector3(rigidbody.velocity.x*2/3, 0, rigidbody.velocity.z / 2);
            StartCoroutine("StartMove", upvector);
            upchargeforce = startcharge;
            upvector = new Vector3(0, 0, 0);
        }
        if (godown)
        {
            brake = new Vector3(rigidbody.velocity.x*2/3, 0, rigidbody.velocity.z / 2);
            StartCoroutine("StartMove", downvector);
            downchargeforce = startcharge;
            downvector = new Vector3(0, 0, 0);
        }
        if (goleft)
        {
            brake = new Vector3(rigidbody.velocity.x/2, 0, rigidbody.velocity.z*2/3);
            StartCoroutine("StartMove", leftvector);
            leftchargeforce = startcharge;
            leftvector = new Vector3(0, 0, 0);
        }
        if (goright)
        {
            brake = new Vector3(rigidbody.velocity.x/2, 0, rigidbody.velocity.z*2/3);
            StartCoroutine("StartMove", rightvector);
            rightchargeforce = startcharge;
            rightvector = new Vector3(0, 0, 0);
        }
        rigidbody.velocity = brake;
    }

    void AugmentMove()
    {
        ReleaseMovement();

        var chup = Input.GetAxisRaw("Vertical");
        var chright = Input.GetAxisRaw("Horizontal");
        var go = new Vector3(chright * fuelboost, 0, chup * fuelboost);
        if (currentfuel > 0 && go != Vector3.zero)
        {
            if (Mathf.Abs(rigidbody.velocity.x - go.x) < fuelboost/2)
            {
                rigidbody.velocity = new Vector3(go.x/3, 0, rigidbody.velocity.z);
            }
            if (Mathf.Abs(rigidbody.velocity.z - go.z) < fuelboost/2)
            {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, go.z/3);
            }
            rigidbody.AddForce(go);
            currentfuel -= .1F;
            timeoflastfueluse = Time.time;
            timesincelastfueluse = 0;

        }
    }


    IEnumerator StartMove(Vector3 dir)
    {
        for (float f = .5f; f >= 0; f -= 0.1f)
        {
            GetComponent<Rigidbody>().AddForce(dir);
            yield return null;
        }
    }

    IEnumerator Refuel()
    {
        while (currentfuel < maxfuel)
        {
            if (timesincelastfueluse > fuelwait)
            {
                currentfuel += .001F;
                   currentfuel = Mathf.Min(currentfuel, maxfuel);
            } else
            {
                yield break;
            }

            yield return null;
        } 
    }

}
