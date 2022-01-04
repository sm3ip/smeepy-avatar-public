using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    [SerializeField] private TwitchConnection twitchConnect;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }

    public void ConnectTwitch()
    {
        twitchConnect.ConnectToTwitch();
    }
}
