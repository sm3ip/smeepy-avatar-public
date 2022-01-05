using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelSystem : MonoBehaviour
{
    [SerializeField] public TwitchConnection twitchConnect;
    public int countdown;
    public int exp;
    public int expOutOfLvl;
    public int currentlvl;
    public int newLvl;
    public string username;
    public int[] levels;
    private database db_script;
    private rng_mov mov_script;
    private nametag name_script;
    // Start is called before the first frame update
    void Start()
    {
        db_script = GameObject.Find("Main Camera").GetComponent<database>();
        mov_script = gameObject.GetComponent<rng_mov>();
        countdown = 0;
        levels = new int[13] {750, 935, 1175, 1470, 1830, 2300, 2900, 3600, 4500, 5600, 7000, 8700, 10000 };
        username = gameObject.name;
        name_script = gameObject.GetComponentInChildren<nametag>();
        CheckLvl();
    }

    // Update is called once per frame
    void Update()
    {
        if (mov_script.timeNoTalk>0 && !mov_script.isLurking)
        {
            if (countdown > 0)
            {
                countdown -= 1;
            }
            else
            {
                MoreExp(1);
                CheckExpOutOfLvl();
                countdown = 3600;
            }
        }
    }

    public void MoreExp(int amount)
    {
        db_script.INeedMore(amount,gameObject.name,"view_exp");
        expOutOfLvl += amount;
    }
    public void CheckExpOutOfLvl()
    {
        if (currentlvl<13)
        {
            if (expOutOfLvl>levels[currentlvl])
            {
                CheckLvl();
            }
        }
        else
        {
            if (expOutOfLvl>levels[levels.Length-1])
            {
                CheckLvl();
            }
        }
    }

    void CheckLvl()
    {
        currentlvl=db_script.HowMuch(username, "view_level");
        newLvl = currentlvl;
        exp = db_script.HowMuch(username, "view_exp");
        expOutOfLvl = exp;
        for (int i = 0; i < newLvl; i++)
        {
            expOutOfLvl -= levels[i];
        }

        for (int j = newLvl; j < levels.Length; j++)
        {
            if (expOutOfLvl>=levels[j])
            {
                expOutOfLvl -=levels[j];
                newLvl += 1;
            }
        }

        while (expOutOfLvl>=levels[levels.Length-1])
        {
            expOutOfLvl -= levels[levels.Length-1];
            newLvl += 1;
        }
        db_script.SetValue(username,newLvl,"view_level");
        if (newLvl>currentlvl)
        {
            currentlvl = newLvl;
            name_script.level = newLvl;
            twitchConnect.WriteInChan("Congrats @"+username+" you've just leveled up (new level : "+newLvl+" )", twitchConnect.accountName);
        }
    }
}
