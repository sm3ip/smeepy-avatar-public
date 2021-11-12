using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class rng_mov : MonoBehaviour
{   
    // chibi's moving boundaries
    public float x_min = 632.74f; 
    public float x_max = 647.04f;
    public int choose;// decides in which direction to go
    public float direction;
    public float goal; // point x where the chibi is going
    public float speed = 0.1f; // moving speed of the chibi
    public bool isMoving = true; // check if the chibi's moving
    public Vector2 position_; // movement vector
    public GameObject huggy; // the other chibi hugged
    public bool hugAction=false; // check if the chibi is moving to hug another one
    public int timeNoTalk; // the amount of time the viewer was "afk"
    public bool isLurking; // check if the viewer is lurking
    public Animator animator; // gets the animator
    public GameObject hat; // the chibi's hat
    public GameObject nametag; // the chibi's nametag
    public choosehat doit; // links to the choosehat script
    public nametag nameScript; // links to the nametag script
    // Start is called before the first frame update
    void Start(){
        // get the chibi's child values
        nametag = gameObject.transform.GetChild(0).gameObject;
        hat = gameObject.transform.GetChild(1).gameObject;
        doit= hat.GetComponent<choosehat>();
        target();// defines chibi's new target
        timeNoTalk=21600; 
        isLurking=false;
    }
    // Update is called once per frame
    void Update(){
        if(transform.position.x-goal>-0.1f && transform.position.x-goal<0.1f && isMoving){
            target(); // check if the chibi reached it's goal and gives it a new one
        }
        if(isMoving){
            Move(); // makes him reach it's goal
        }
        if(hugAction){
                    StartCoroutine(anim(3,"hugg"));    // need an hug animation
        }
        if(timeNoTalk<=0 && !isLurking){ // check if viewer is "afk" and not lurking 
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0f); // makes it transparent
            nameScript= nametag.GetComponent<nametag>(); // change nametag value
            nameScript.isLurking=true;
            hat.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0f); // makes the hat transparent
        }else if(isLurking){ // check if is lurking
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,.5f); // makes it half transparent
            hat.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,.5f);
        }
        else{
            timeNoTalk -=1; // countdown 
        }
    }
    private void target(){ // defines the chibi's new target
            choose+=1;      // if even goes toward right if odd goes toward left
            if(choose%2==1){
                direction=x_min;
            }else if(choose%2==0){
                direction=x_max;
            }
            goal = UnityEngine.Random.Range(gameObject.transform.position.x,direction); // chooses a random x between direction and current position
    }
    IEnumerator anim(int x, string anim){ // animation coroutine
        isMoving=false; // makes it stop moving
        animator.Play(anim); // call the anim animation
        yield return new WaitForSeconds(x); // waits
        isMoving=true; // gets back to moving
        reset(); // resets all values
        target(); // defines a new target
    }
    private void reset(){ // resets important values
        hugAction=false;
        huggy=null;
    }
    private void Move(){// makes it move one step toward goal
        float step = speed * Time.deltaTime;
        position_= new Vector2(goal, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, position_,step);
    }
    public void Hug(){ // waits for the hug
        goal= gameObject.transform.position.x;
        isMoving=false;
        hugAction=true;
    }
    public void Hugged(string name){ // go hug someone else
        huggy=GameObject.Find(name);
        goal=huggy.transform.position.x;
        gameObject.transform.position=huggy.transform.position;
        isMoving=false;
        //StartCoroutine(anim(3,"hugged")); the animation doesn't exist rn
    }

    public void BackFromTheVoid(){ // called when unlurking or when coming back from "afk"
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        nameScript= nametag.GetComponent<nametag>();
        nameScript.isLurking=false;
        hat.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        timeNoTalk=21600;
    }
    public void GoChooseMyHat(int numb){ // called to switch hat
        doit.chooseMyHat(numb);
    }
    public void Jump(){
        StartCoroutine(anim(2,"monster_jump"));
    }
}