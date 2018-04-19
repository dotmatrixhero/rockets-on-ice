using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This was refined movement + world space boost
public class PlayaController2 : MonoBehaviour {

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
       // Debug.Log(currentfuel);

    }

    void Move()
    {
        var chup = Input.GetAxisRaw("Vertical") *-1;
        var chright = Input.GetAxisRaw("Horizontal") *-1;
       
        Vector3 brake = rigidbody.velocity;
        if (chup > 0)
        {
            if (rigidbody.velocity.z < 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 95 / 100, 0, rigidbody.velocity.z * 9 / 10);

            }
            upchargeforce = upchargeforce * exponentialchargerate;
            upchargeforce = Mathf.Min(upchargeforce, maxcharge);
    
            upvector = new Vector3(0, 0, 1 * upchargeforce);
        }
        if (chup < 0)
        {
            if (rigidbody.velocity.z > 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 95 / 100, 0, rigidbody.velocity.z * 9 / 10);


            }
            downchargeforce *= exponentialchargerate;
            downchargeforce = Mathf.Min(downchargeforce, maxcharge);
            downvector = new Vector3(0, 0, -1 * downchargeforce);
        }
        if (chright > 0)
        {
            if (rigidbody.velocity.x > 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 9 / 10, 0, rigidbody.velocity.z * 95 / 100);

            }
            rightchargeforce *= exponentialchargerate;
            rightchargeforce = Mathf.Min(rightchargeforce, maxcharge);
            rightvector = new Vector3(1 * rightchargeforce, 0, 0);
        }
        if (chright  < 0)
        {
            if (rigidbody.velocity.x < 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 9 / 10, 0, rigidbody.velocity.z * 95 / 100);

            }
            leftchargeforce *= exponentialchargerate;
            leftchargeforce = Mathf.Min(leftchargeforce, maxcharge);
            leftvector = new Vector3(-1 * leftchargeforce, 0, 0);
        }

        rigidbody.velocity = brake;
        ReleaseMovement();
    }

    void ReleaseMovement()
    {
        var chup = Input.GetAxisRaw("Vertical");
        var chright = Input.GetAxisRaw("Horizontal");
        var allmove = upvector + downvector + rightvector + leftvector;
        var goup = (chup == 0 && chright == 0 && upvector != Vector3.zero);
        var godown = (chup == 0 && chright == 0 && downvector != Vector3.zero);
        var goleft = (chup == 0 && chright == 0 & leftvector != Vector3.zero);
        var goright = (chup == 0 && chright == 0 & rightvector != Vector3.zero);
        if (goup)
        {

            StartCoroutine("StartMove", upvector);
            upchargeforce = startcharge;
            upvector = new Vector3(0, 0, 0);
        }
        if (godown)
        {


            StartCoroutine("StartMove", downvector);
            downchargeforce = startcharge;
            downvector = new Vector3(0, 0, 0);
        }
        if (goleft)
        {


            StartCoroutine("StartMove", leftvector);
            leftchargeforce = startcharge;
            leftvector = new Vector3(0, 0, 0);
        }
        if (goright)
        {


            StartCoroutine("StartMove", rightvector);
            rightchargeforce = startcharge;
            rightvector = new Vector3(0, 0, 0);
        }
    }

    void AugmentMove()
    {
        ReleaseMovement();

        var chup = Input.GetAxis("Vertical");
        var chright = Input.GetAxis("Horizontal");
        if (chup != 0 || chright != 0)
        {
            var go = Vector3.zero;
            var normalv = rigidbody.velocity.normalized;
            var input = new Vector3(chright, 0, chup);


            go = (normalv);
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.z))
            {
                if (input.x > 0 && normalv.z > 0)
                {
                    go = new Vector3(go.z, 0, -1 * go.x);
                }
                else if (input.x > 0 && normalv.z < 0)
                {
                    go = new Vector3(go.z * -1, 0, go.x);
                }
                else if (input.x < 0 && normalv.z > 0)
                {
                    go = new Vector3(go.z * -1, 0, go.x);
                }
                else if (input.x < 0 && normalv.z < 0)
                {
                    go = new Vector3(go.z, 0, -1 * go.x);
                }
            }
            else
            {
                if (input.z > 0 && normalv.x > 0)
                {
                    go = new Vector3(go.z * -1, 0, go.x);
                }
                else if (input.z > 0 && normalv.x < 0)
                {
                    go = new Vector3(go.z, 0, -1 * go.x);
                }
                else if (input.z < 0 && normalv.x > 0)
                {
                    go = new Vector3(go.z, 0, -1 * go.x);
                }
                else if (input.z < 0 && normalv.x < 0)
                {
                    go = new Vector3(go.z * -1, 0, go.x);
                }
            }
            if (currentfuel > 0 && go != Vector3.zero)
            {
                Debug.Log(normalv.x+ ""+ normalv.z + "//go " + go);

                rigidbody.AddForce(go * fuelboost);
                //currentfuel -= .1F;
                timeoflastfueluse = Time.time;
                timesincelastfueluse = 0;

            }
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
