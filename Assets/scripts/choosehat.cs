using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class choosehat : MonoBehaviour
{
    public bool isChoiceChanged; // ables to switch animation when needed and avoids to freeze the current one
    public Animator anim;
    public int switchesHat = 0;

    // NEW WAY
    public string currentHat;
    public float[] hatCoords;

    public bool NoHat;
    // Start is called before the first frame update
    void Start()
    {
        NoHat = true;
        isChoiceChanged=false;
        anim=GetComponent<Animator>();
        //getting all the hats in the list (gotta find a more efficient way)
    }
    void Update(){
        if (switchesHat>0)
        {
            NoHat = false;
            isChoiceChanged = true;
            switchesHat = 0;
        }
        if (!NoHat)
        {
            transform.localPosition = new Vector3(hatCoords[0],hatCoords[1],hatCoords[2]);
            transform.localScale = new Vector3(hatCoords[3],hatCoords[4],hatCoords[5]);
        }
        if(isChoiceChanged){
            anim.Play(currentHat,0,0f);
            isChoiceChanged=false;
        }
        
    }

    public void SwitchHat(string name, float[] coords)
    {
        hatCoords = coords;
        currentHat = name;
        isChoiceChanged = true;
        NoHat = false;
        switchesHat += 1;
    }
}
