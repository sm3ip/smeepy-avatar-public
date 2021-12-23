﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class choosehat : MonoBehaviour
{
    public int choice; // which animation is chosed
    public bool isChoiceChanged; // ables to switch animation when needed and avoids to freeze the current one
    public Animator anim;
    // all the hat animation names 
    public string hat0;
    public string hat1;
    public string hat2;
    public string hat3;
    public string hat4;
    // list containing all the strings
    public List<string> hat;
    // Start is called before the first frame update
    void Start(){
        isChoiceChanged=false;
        anim=GetComponent<Animator>();
        //getting all the hats in the list (gotta find a more efficient way)
        hat= new List<string>();
        hat.Add(hat0);
        hat.Add(hat1);
        hat.Add(hat2);
        hat.Add(hat3);
        hat.Add(hat4);
        choice=0;
    }
    void Update(){
        if(isChoiceChanged){
            anim.Play(hat[choice],0,0f);
            isChoiceChanged=false;
        }
        
    }
    public void chooseMyHat(int numb){// called by chat command to switch hat
        if(numb>=0 && numb<=hat.Count-1){
            choice =numb;
            isChoiceChanged=true;
        }  
    }
}
