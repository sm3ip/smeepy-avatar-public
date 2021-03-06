using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class TwitchChat : MonoBehaviour
{
    [SerializeField] private TwitchConnection _twitchConnection;
    public GameObject character;                                                                                        // the chibi prefab
    public GameObject g_name;                                                                                           // the chibi's nametag
    public GameObject tp;                                                                                               // the spawn game object
    public GameObject viewer_entity;                                                                                    // the chibi gameobject
    public GameObject viewer_entity_hugged;                                                                             // the gameobject hugged by the chibi
    public string hugged_name;                                                                                          // the name of the gameobject hugged by the chibi
    public int nbMessage;                                                                                               // the number in the message (to choose the hat)
    private database script_db;                                                                                         // the var used to link to the database script
    
    void Start(){
        script_db=GameObject.Find("Main Camera").GetComponent<database>();                                              // gets the database script
    }
    void Update(){
        if (_twitchConnection.NewTwitchMessage(out string newMessage))
        {
            checkUserExist(_twitchConnection.CutMessage(newMessage)[0], _twitchConnection.CutMessage(newMessage)[1]); // call the check user void ( to not create clones)
        }
    }
    private void checkUserExist(string name, string message){                                                           // the way used to tell if the viewer already has a chibi in-game
        if (GameObject.Find(name) == null){                                                                             // check if there is no instance of the viewer's chibi
            script_db.FirstAppears(name);                                                                               // calls the setup void if the soft creates a new chibi gameobject
            g_name = Instantiate(character, new Vector2(tp.transform.position.x, 294.65f), Quaternion.identity); // instantiate a new chibi at the tp position
            g_name.name = name;                                                                                         // gets viewer name to the chibi
        }else{
            ChatCommand(name, message);                                                                                 //  calls the chat command void if the user already exist in the scene
        }
    }
    private void ChatCommand(string name, string message){
        viewer_entity = GameObject.Find(name);                                                                          // finds the already existing chibi
        viewer_entity.GetComponent<rng_mov>().BackFromTheVoid();                                                        // calls the back from the void method so it's cooldown is back to normal
        switch(message){
            case "!jump": // recognises the jump command
                viewer_entity.GetComponent<rng_mov>().Jump();                                                           // makes it jump
                break;
            case "!gold": // recognises the gold command
                _twitchConnection.WriteInChan("@"+name+" you have "+viewer_entity.GetComponent<InventorySystem>().golds + " golds", _twitchConnection.accountName);
                break;
            case "!exp": // recognises the exp command
                _twitchConnection.WriteInChan("@"+name+" you have "+viewer_entity.GetComponent<levelSystem>().exp + " exp", _twitchConnection.accountName);
                break;
            case "!lvl":
                _twitchConnection.WriteInChan("@"+name+" you are level "+ viewer_entity.GetComponent<levelSystem>().currentlvl, _twitchConnection.accountName);
                break;
            case "!shop": // recognises the shop command
                _twitchConnection.WriteInChan("@"+name+ script_db.GetShop(), _twitchConnection.accountName);
                break;
            case "!inventory":
                _twitchConnection.WriteInChan("@"+name+ viewer_entity.GetComponent<InventorySystem>().TellInventory(), _twitchConnection.accountName);
                break;
            case string a when a.Contains("!hug"):                                                                      // recognises the hug command
                for (int i = 5; i < message.Length; i++){                                                               // gets the hugged chibi name
                    if(message[i]!='@'){                                                                                // removes the @
                        hugged_name += message[i].ToString().ToLower();                                                 // gets the char in the var and lowers it
                    }  
                }
                if (GameObject.Find(hugged_name)){                                                                      //  checks if the hugged chibi exists
                    viewer_entity_hugged = GameObject.Find(hugged_name);                                                // gets it into a container
                    viewer_entity.GetComponent<rng_mov>().Hug();                                                        // call their hug animations
                    viewer_entity_hugged.GetComponent<rng_mov>().Hugged(name);
                }
                break;
            case string b when b.Contains("!lurk"):                                                                     // recognises the lurk command
                viewer_entity.GetComponent<rng_mov>().isLurking = true;                                                 // gets it lurking
                break;
            case string c when c.Contains("!unlurk"):                                                                   // recognises unlurk command
                viewer_entity.GetComponent<rng_mov>().isLurking = false;                                                // gets it out of lurking
                break;
            case string e when e.StartsWith("!buy "):
                viewer_entity.GetComponent<InventorySystem>().BuyItem(message.Substring(5));
                break;
            case string f when f.StartsWith("!equipHAT "):
                if (viewer_entity.GetComponent<InventorySystem>().IsItemOwned(message.Substring(10)))
                {
                    viewer_entity.GetComponentInChildren<choosehat>().SwitchHat(message.Substring(10),script_db.GetCoords(message.Substring(10)));
                    script_db.EquipObj(message.Substring(10),viewer_entity.name);
                }
                else
                {
                    _twitchConnection.WriteInChan("@"+name+" it looks like you don't own that hat or it doesn't exist ", _twitchConnection.accountName);
                }
                break;
        }
        // reset the containers so it doesn't lag later on
        viewer_entity = null; 
        viewer_entity_hugged = null;
        hugged_name = null;
    }
}
