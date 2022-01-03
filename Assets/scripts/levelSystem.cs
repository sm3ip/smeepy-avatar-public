using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelSystem : MonoBehaviour
{
    [SerializeField] public TwitchConnection twitchConnect;
    public int exp;
    public int expOutOfLvl;
    public int currentlvl;
    public int newLvl;
    public string username;
    public int[] levels;
    private database db_script;
    // Start is called before the first frame update
    void Start()
    {
        db_script = GameObject.Find("Main Camera").GetComponent<database>();
        levels = new int[13] {750, 935, 1175, 1470, 1830, 2300, 2900, 3600, 4500, 5600, 7000, 8700, 10000 };
        username = gameObject.name;
        CheckLvl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckLvl()
    {
        currentlvl=db_script.HowMuch(username, "level");
        newLvl = currentlvl;
        exp = db_script.HowMuch(username, "exp");
        expOutOfLvl = exp;
        for (int i = 0; i < newLvl; i++)
        {
            expOutOfLvl -= levels[i];
        }

        for (int j = newLvl; j < levels.Length; j++)
        {
            if (expOutOfLvl>=levels[j])
            {
                expOutOfLvl = -levels[j];
                newLvl += 1;
            }
        }

        while (expOutOfLvl>=levels[levels.Length-1])
        {
            expOutOfLvl -= levels[levels.Length-1];
            newLvl += 1;
        }
        db_script.SetValue(username,newLvl,"level");
        if (newLvl>currentlvl)
        {
            currentlvl = newLvl;
            twitchConnect.WriteInChan("Congrats @"+username+" you've just leveled up (new level : "+newLvl+" )", twitchConnect.accountName);
        }
    }
}
