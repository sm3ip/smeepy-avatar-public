using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class log_button : MonoBehaviour
{
    public GameObject menu; // get the menu
    public bool on_screen= true; // check if the menu is visible
    public GameObject channel_input, username_input, oauth_input, t_chat; // get the input fields and the twitch chat object
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
        // called by clicking the start menu : get the input fields values and modify them in the twitch chat script
        script_t_chat.mod_co_values(username_input.GetComponent<TMP_InputField>().text,channel_input.GetComponent<TMP_InputField>().text,oauth_input.GetComponent<TMP_InputField>().text);
    }
}
