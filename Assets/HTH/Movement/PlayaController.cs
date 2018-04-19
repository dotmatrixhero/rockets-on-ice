using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayaController : MonoBehaviour {

    public int playernum;

    public int maxhp;
    int currenthp;

    public float minspeed = 200;
    public float currentspeed = 20;

    public float startcharge = 200;
    public float exponentialchargerate = 1.06F;
    public float maxcharge = 4000F;

    public float maxfuel = 1F;
    public float fuelwait = 200F;
    public float fuelrechargerate = .1F;
    public int fuelboost = 4000;

    public bool inverseMove = false;
    public Vector3 gundir;

    float currentfuel;
    float timesincelastfueluse;
    float timeoflastfueluse;

    float timeofslidestart;
    float timesinceslidestart;

    float upchargeforce = 500;
     float downchargeforce = 500;
     float leftchargeforce = 500;
     float rightchargeforce = 500;
    Rigidbody rigidbody;

    public Vector3 upvector;
    public Vector3 rightvector;
    public Vector3 leftvector;
    public Vector3 downvector;

    
    GamepadDevice gamepad;

    GamepadInput _input;

    public GamepadInput input
    {
        get
        {
            if (!_input)
                _input = GetComponent<GamepadInput>();
            return _input;
        }
    }


    // Use this for initialization
    void Start()
    {

        gamepad = input.gamepads[playernum];
        rigidbody = GetComponent<Rigidbody>();
        currentfuel = maxfuel;
        timeoflastfueluse = Time.time;
        timeofslidestart = 0F;
        currenthp = maxhp;
        if (playernum == 0)
        {

            GameObject.FindGameObjectWithTag("HUD").GetComponent<GameOverManager>().hp1 = currenthp;
        }
        else if (playernum == 1)
        {

        
            GameObject.FindGameObjectWithTag("HUD").GetComponent<GameOverManager>().hp2 = currenthp;
        }

    }
	
	// Update is called once per frame
	void Update () {
        var slide = gamepad.GetButton(GamepadButton.LeftBumper) || gamepad.GetButton(GamepadButton.RightBumper);
        if (slide)
            PowerSlide();
        else
        {
            timesincelastfueluse += Time.time - timeoflastfueluse;
            if (timesincelastfueluse > fuelwait)
            {
                StartCoroutine("Refuel");
            }
            timeofslidestart = 0;
            timesinceslidestart = 0;
        }
        Move();

          // Debug.Log(currentfuel);

    }

    void Move()
    {
        var aimup = gamepad.GetAxis(GamepadAxis.RightStickY) ;
        var aimright = gamepad.GetAxis(GamepadAxis.RightStickX);

        gundir = new Vector3(aimright, 0, aimup);
        

        int inverse = 1;
        if (inverseMove)
        {
            inverse = -1;
        }
            var chup = gamepad.GetAxis(GamepadAxis.LeftStickY) * inverse;
            var chright = gamepad.GetAxis(GamepadAxis.LeftStickX) * inverse;

        
        Vector3 brake = rigidbody.velocity;
        
        if (chup > 0)
        {
            if (rigidbody.velocity.z < 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 95 / 100, 0, rigidbody.velocity.z * 95 / 100);

            }
            upchargeforce = upchargeforce * exponentialchargerate;
            upchargeforce = Mathf.Min(upchargeforce, maxcharge);
    
            upvector = new Vector3(0, 0, chup * upchargeforce);
        }
        if (chup < 0)
        {
            if (rigidbody.velocity.z > 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 95 / 100, 0, rigidbody.velocity.z * 95 / 100);


            }
            downchargeforce *= exponentialchargerate;
            downchargeforce = Mathf.Min(downchargeforce, maxcharge);
            downvector = new Vector3(0, 0, chup * downchargeforce);
        }
        if (chright > 0)
        {
            if (rigidbody.velocity.x < 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 95 / 100, 0, rigidbody.velocity.z * 95 / 100);

            }
            rightchargeforce *= exponentialchargerate;
            rightchargeforce = Mathf.Min(rightchargeforce, maxcharge);
            rightvector = new Vector3(chright * rightchargeforce, 0, 0);
        }
        if (chright  < 0)
        {
            if (rigidbody.velocity.x > 0)
            {
                brake = new Vector3(rigidbody.velocity.x * 95 / 100, 0, rigidbody.velocity.z * 95 / 100);

            }
            leftchargeforce *= exponentialchargerate;
            leftchargeforce = Mathf.Min(leftchargeforce, maxcharge);
            leftvector = new Vector3(chright * leftchargeforce, 0, 0);
        }
        rigidbody.velocity = brake;
        ReleaseMovement();
    }

    void ReleaseMovement()
    {
        var chup = gamepad.GetAxis(GamepadAxis.LeftStickY);x`
        var chright = gamepad.GetAxis(GamepadAxis.LeftStickX);
        var allmove = upvector + downvector + rightvector + leftvector;
        var goup = (chup == 0 && upvector != Vector3.zero);
        var godown = (chup == 0 && downvector != Vector3.zero);
        var goleft = ( chright == 0 & leftvector != Vector3.zero);
        var goright = (chright == 0 & rightvector != Vector3.zero);
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

    void PowerSlide()
    {
        ReleaseMovement();

        Vector3 perp = Vector3.zero;
        if (gamepad.GetButton(GamepadButton.LeftBumper))
        {
            perp = new Vector3(-1*rigidbody.velocity.z, 0, rigidbody.velocity.x);
        }

        if (gamepad.GetButton(GamepadButton.RightBumper)) {
            perp = new Vector3(rigidbody.velocity.z, 0, rigidbody.velocity.x * -1);
        }
        rigidbody.AddForce(perp);
        
        if (timesinceslidestart > 1.5)
            rigidbody.velocity = rigidbody.velocity * 1.8f / timesinceslidestart;
        else if (timesinceslidestart > 1)
          rigidbody.velocity = rigidbody.velocity*1.1f /timesinceslidestart;
       

        if (timeofslidestart == 0) {
            timeofslidestart = Time.time;
        } else
        {
            timesinceslidestart = Time.time - timeofslidestart;
        }
        /*
        var chup = Input.GetAxis("Vertical");
        var chright = Input.GetAxis("Horizontal");
        if (chup != 0 || chright != 0)
        {
            var go = Vector3.zero;
            var normalv = rigidbody.velocity.normalized;
            var input = new Vector3(chright, 0, chup);


            go = (normalv);
 
            if (currentfuel > 0 && go != Vector3.zero)
            {
                Debug.Log(normalv.x+ ""+ normalv.z + "//go " + go);

                rigidbody.AddForce(go * fuelboost);
                //currentfuel -= .1F;
                timeoflastfueluse = Time.time;
                timesincelastfueluse = 0;

            }
        }*/
    }

    public void TakeDamage(int i)
    {
        Debug.Log(currenthp);
        currenthp = currenthp - i;
        StartCoroutine(Blink(.5f, .25f/currenthp));

        if (playernum == 0)
        {

            GameObject.FindGameObjectWithTag("HUD").GetComponent<GameOverManager>().hp1 = currenthp;
        } else if (playernum ==1 )
            GameObject.FindGameObjectWithTag("HUD").GetComponent<GameOverManager>().hp2 = currenthp;
        {

        }
        if (currenthp <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GameOverManager>().GameOver();

    }


 
    IEnumerator Blink(float waitTime, float rapid)
    {
        var endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.enabled = false;
            yield return new WaitForSeconds(rapid);
            renderer.enabled = true;
            yield return new WaitForSeconds(rapid);
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
