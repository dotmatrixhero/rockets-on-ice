
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour {
    public float restartdelay = 3f;
    bool resetTimer = false;
    float timetoreset = 0f;

    public int hp1;
    public int hp2;

    int finalhp1 = -10000;
    int finalhp2 = -10000;
    Text hp1obj;
    Text hp2obj;
    
    // Use this for initialization
	void Start () {
        Text[] hudtexts = GetComponentsInChildren<Text>();	
        foreach (Text hudtext in hudtexts)
        {
            if (hudtext.tag == "HP1")
            {
                hp1obj = hudtext;
            } else if (hudtext.tag == "HP2")
            {
                hp2obj = hudtext;
            }
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        hp1obj.text = hp1.ToString();
        hp2obj.text = hp2.ToString();
        if (resetTimer)
        {

            if (hp1 > finalhp1)
            {

                finalhp1 = hp1;
            }
            else if (hp2 > finalhp2)
            {
                finalhp2 = hp2;
            }

            
            hp1obj.text = finalhp1.ToString();
            hp2obj.text = finalhp2.ToString();
            timetoreset += Time.deltaTime;

            if (timetoreset > restartdelay)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        
    }

    public void GameOver()
    {
        GetComponent<Animator>().SetTrigger("GameOver");
        resetTimer = true;
    }
}
