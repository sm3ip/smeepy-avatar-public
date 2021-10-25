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
    // all the hat sprites 
    public Sprite hat0;
    public Sprite hat1;
    public Sprite hat2;
    public Sprite hat3;
    public Sprite hat4;
    // list containing all the sprites
    public List<Sprite> hat;
    public SpriteRenderer spriterenderer;
    // Start is called before the first frame update
    void Start(){
        spriterenderer = gameObject.GetComponent<SpriteRenderer>();
        //getting all the hats in the list (gotta find a more efficient way)
        hat= new List<Sprite>();
        hat.Add(hat0);
        hat.Add(hat1);
        hat.Add(hat2);
        hat.Add(hat3);
        hat.Add(hat4);
    }
    public void chooseMyHat(int numb){// called by chat command to switch hat
        spriterenderer = gameObject.GetComponent<SpriteRenderer>();
        if(numb>=0 && numb<=hat.Count-1){
            spriterenderer.sprite = hat[numb];
        }  
    }
}
