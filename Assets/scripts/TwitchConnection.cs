using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu]
public class TwitchConnection : ScriptableObject
{
    public string accountName;
    public string Token;

    private TcpClient _tcpClient;
    private StreamReader _streamReader;
    private StreamWriter _streamWriter;

    public void SetupVars(string acName, string token)
    {
        accountName = acName;
        Token = token;
        ConnectToTwitch();
    }

    public async void ConnectToTwitch()
    {
        Debug.Log("Connecting.......");
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync("irc.chat.twitch.tv", 6667);
        _streamReader = new StreamReader(_tcpClient.GetStream());
        _streamWriter = new StreamWriter(_tcpClient.GetStream()) {NewLine = "\r\n", AutoFlush = true};
        await _streamWriter.WriteLineAsync("PASS " + Token);
        await _streamWriter.WriteLineAsync("NICK " + accountName);

        ReadMessages();
    }

    public string lastLine;

    private async void ReadMessages()
    {
        while (true)
        {
            lastLine = await _streamReader.ReadLineAsync();
            Debug.Log(lastLine);
        }
    }
}
