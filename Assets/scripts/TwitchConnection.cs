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
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync("irc.chat.twitch.tv", 6667);
        _streamReader = new StreamReader(_tcpClient.GetStream());
        _streamWriter = new StreamWriter(_tcpClient.GetStream()) {NewLine = "\r\n", AutoFlush = true};
        await _streamWriter.WriteLineAsync("PASS " + Token);
        await _streamWriter.WriteLineAsync("NICK " + accountName);

        ReadMessages();
        JoinChan(accountName);
    }

    public async void WriteInChan(string message, string chanName)
    {
        await _streamWriter.WriteLineAsync($"PRIVMSG #{chanName} :{message}");
    }

    public async void JoinChan(string chanName)
    {
        await _streamWriter.WriteLineAsync("JOIN #" + chanName);
    }

    public async void LeaveChan(string chanName)
    {
        await _streamWriter.WriteLineAsync("PART #" + chanName);
    }

    private string lastLine;
    [SerializeField] public List<string> logs = new List<string>();
    [SerializeField] private int logsIndex;
    [SerializeField] private bool isClearLogs = false;

    private async void ReadMessages()
    {
        logs.Clear();
        logsIndex = 1;
        while (true)
        {
            if (isClearLogs)
            {
                logs.Clear();
                logsIndex = 1;
                isClearLogs = false;
                logs.Add(lastLine);
            }
            lastLine = await _streamReader.ReadLineAsync();
            logs.Add(lastLine);
            Debug.Log(lastLine);
            
            if (lastLine != null && lastLine.StartsWith("PING"))
            {
                lastLine.Replace("PING", "PONG");
                await _streamWriter.WriteLineAsync(lastLine);
            }
        }
    }

    public void ClearLogs()
    {
        isClearLogs = true;
    }

    public bool NewTwitchMessage(out string newMessage) // call this one to check if there is another message to take care of and use the out in the cut message right below
    {
        if (logs.Count < logsIndex)
        {
            newMessage = "";
            ClearLogs();
            return false;
        }

        for (int i = logsIndex; i <= logs.Count; i++)
        {
            if (logs[i-1].Contains("PRIVMSG"))
            {
                logsIndex = i + 1;
                newMessage = logs[i - 1];
                return true;
            }
        }

        logsIndex = logs.Count + 1;
        newMessage = "";
        return false;
    }

    public string[] CutMessage(string rawMessage)
    {
        if (rawMessage== null || !rawMessage.Contains("PRIVMSG"))
        {
            return new string[1] {""};
        }
        string message = rawMessage.Split(':')[2];
        string user = rawMessage.Split(' ')[0].Split('!')[0].Substring(1);
        return new string[2] {user, message};
    }
}
