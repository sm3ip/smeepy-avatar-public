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
    // twitch chat variables
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;
    public string username, password, channelName; // https://twitchapps.com/tmi to get the oauth key
    public GameObject character; // the chibi prefab
    public GameObject g_name; // the chibi's nametag
    public GameObject tp; // the spawn game object
    public int time = 0; // countdown between each twitch connexion
    public GameObject viewer_entity; // the chibi gameobject
    public GameObject viewer_entity_hugged; // the gameobject hugged by the chibi
    public string hugged_name; // the name of the gameobject hugged by the chibi
    public int nbMessage; // the number in the message (to choose the hat)
    public bool can_connect; // check if there are values in the input fields to connect to twitch
    public bool didConnect; // check if it connected at least once
    void Start(){
        // set up some standard values
        can_connect = false;
        didConnect = false;
    }
    void Update(){
        if (can_connect){ // check if there are values in the input fields
            Connect();
            ReadChat();
        }
        if (didConnect){
            if (!twitchClient.Connected) { //check if is connected
                Connect();
            }
            else if (time >= 28800) {  // check if timed out (the twitch connexion lags sometimes)
                Connect();
                time = 0; // get the countdown 
            }
            ReadChat(); // get the chat messages each frame
            time += 1;
        }
    }

    public void mod_co_values(string txt_username, string channel, string oauth){ // gets the input field values to login
        password = oauth;
        username = txt_username;
        channelName = channel;
        can_connect = true;
        didConnect = true;
    }
    private void Connect(){  // Connects to the twitch chat
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());
        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
        can_connect = false;
    }
    private void ReadChat(){ // Gets the last message
        if (twitchClient.Available > 0){ // if there is an unread message
            var message = reader.ReadLine(); //Read in the current message
            print(message);       // weirdly enough printing this thing makes it work fine but without it it doesn't work at all
            if (message.Contains("PRIVMSG")){ // check if it's a chat message from a viewer
                //Get the users name by splitting it from the string
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);
                //Get the users message by splitting it from the string
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                checkUserExist(chatName, message); // call the check user void ( to not create clones)
            }
        }
    }

    private void checkUserExist(string name, string message){ // the way used to tell if the viewer already has a chibi in-game
        if (GameObject.Find(name) == null){ // check if there is no instance of the viewer's chibi
            g_name = Instantiate(character, new Vector2(tp.transform.position.x, 295.139f), Quaternion.identity); // instantiate a new chibi at the tp position
            g_name.name = name; // gets viewer name to the chibi
        }else{
            ChatCommand(name, message); //  calls the chat command void if the user already exist in the scene
        }
    }
    private void ChatCommand(string name, string message){
        viewer_entity = GameObject.Find(name); // finds the already existing chibi
        viewer_entity.GetComponent<rng_mov>().BackFromTheVoid(); // calls the back from the void method so it's cooldown is back to normal
        switch(message){
            case "!jump": // recognises the jump command
                viewer_entity.GetComponent<rng_mov>().Jump();   // makes it jump
                break;
            case string a when a.Contains("!hug"):  // recognises the hug command
                for (int i = 5; i < message.Length; i++){ // gets the hugged chibi name
                    if(message[i]!='@'){  // removes the @
                        hugged_name += message[i].ToString().ToLower(); // gets the char in the var and lowers it
                    }  
                }
                if (GameObject.Find(hugged_name)){  //  checks if the hugged chibi exists
                    viewer_entity_hugged = GameObject.Find(hugged_name); // gets it into a container
                    viewer_entity.GetComponent<rng_mov>().Hug();  // call their hug animations
                    viewer_entity_hugged.GetComponent<rng_mov>().Hugged(name);
                }
                break;
            case string b when b.Contains("!lurk"): // recognises the lurk command
                viewer_entity.GetComponent<rng_mov>().isLurking = true;    // gets it lurking
                break;
            case string c when c.Contains("!unlurk"):  // recognises unlurk command
                viewer_entity.GetComponent<rng_mov>().isLurking = false;   // gets it out of lurking
                break;
            case string d when d.Contains("!hat"):  // recognises the hat command
                message = message.Substring(4); // gets the number of the hat
                if (Int32.TryParse(message, out nbMessage)){  // check if it's parsable
                    viewer_entity.GetComponent<rng_mov>().GoChooseMyHat(nbMessage);  // gets it a new hat
                }
                break;
        }
        // reset the containers so it doesn't lag later on
        viewer_entity = null; 
        viewer_entity_hugged = null;
        hugged_name = null;
    }
}
