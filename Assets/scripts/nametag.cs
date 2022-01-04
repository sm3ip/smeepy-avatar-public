using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nametag : MonoBehaviour
{
    public GameObject parent; // the character's prefab game object
    public rng_mov mov;  // links to the char's rng_mov script
    public string name_; // it's name
    public bool isLurking; // check if viewer is lurking
    public levelSystem lvl_script;

    public int level;
    // Start is called before the first frame update
    void Start()
    {
        
        isLurking=false;
        mov=parent.GetComponent<rng_mov>();
        lvl_script = mov.gameObject.GetComponent<levelSystem>();
        name_=mov.gameObject.name; // gets the viewer username
        level = lvl_script.currentlvl;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLurking){
            name_="("+level+ ") " +mov.gameObject.name; // diplays name and level when not lurking
        }
        else{
            name_="";// hides name when lurking
        }
        GetComponent<TextMesh>().text = name_; // diplays the viewer's username
    }
    
    

}
