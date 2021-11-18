using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class log_button : MonoBehaviour
{
    public GameObject menu; // get the menu
    public bool on_screen= true; // check if the menu is visible
    public GameObject path_input, t_chat; // get the input field and the twitch chat object
    public string[] file; // create an array of string to store the file
    private TwitchChat script_t_chat; // links twitch chat script
 
    void Start(){
        script_t_chat = t_chat.GetComponent<TwitchChat>(); // gets the twitch chat script
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
        file=readFile1(path_input.GetComponent<TMP_InputField>().text); // reads the file and stores it in an array of strings
        script_t_chat.mod_co_values(file[0], file[1], file[2]); // call the void to modify the login values
    }

    public string[] readFile1( string path)
    {
        // reads a file and stores it's values
        string[] lines = System.IO.File.ReadAllLines(path);
        return lines;
    }
}