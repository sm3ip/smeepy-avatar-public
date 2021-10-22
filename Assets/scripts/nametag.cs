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
    // Start is called before the first frame update
    void Start()
    {
        isLurking=false;
        mov=parent.GetComponent<rng_mov>();
        name_=mov.gameObject.name; // gets the viewer username
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLurking){
            name_=mov.gameObject.name; // diplays name when not lurking
        }
        else{
            name_="";// hides name when lurking
        }
        GetComponent<TextMesh>().text = name_; // diplays the viewer's username
    }

}
