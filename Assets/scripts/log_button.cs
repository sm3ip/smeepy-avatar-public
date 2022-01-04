using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class log_button : MonoBehaviour
{
    [SerializeField] private TwitchConnection twitchConnect;
    public string path;
    public GameObject menu; // get the menu
    public bool on_screen= true; // check if the menu is visible
    public GameObject path_input, t_chat; // get the input field and the twitch chat object
    public string[] file; // create an array of string to store the file
    private TwitchChat script_t_chat; // links twitch chat script
    private database script_db; // links to the db script
 
    void Start(){
        script_t_chat = t_chat.GetComponent<TwitchChat>(); // gets the twitch chat script
        script_db = GameObject.Find("Main Camera").GetComponent<database>(); // gets the database script
    }

    public void Clicked()
    {
        // called by clicking on the top-right hand corner button and enable/disable the login menu
        menu.SetActive(on_screen);
        on_screen = !on_screen;
    }

    public void loginStart()
    {
        /*
        the file used to connect is composed of three lines :
        line 1 : username
        line 2 : channel name
        line 3 : Twitch Chat OAuth Password
        */
        // called by clicking the start menu : get the input fields values and modify them in the twitch chat script
        path =path_input.GetComponent<TMP_InputField>().text;
        script_db.LocateDatabase(path); // gives the folder path to the database script
        file=readFile1(path+"stream.txt"); // reads the file and stores it in an array of strings
        //script_t_chat.mod_co_values(file[0], file[1], file[2]); // call the void to modify the login values
        
        // NEW
        twitchConnect.SetupVars(file[0], file[2]);
    }

    public string[] readFile1( string path)
    {
        // reads a file and stores it's values
        string[] lines = System.IO.File.ReadAllLines(path);
        return lines;
    }
}