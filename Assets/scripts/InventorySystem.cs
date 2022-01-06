using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private TwitchConnection _twitchConnection;

    private int countdown;
    public int golds;
    public List<string[]> shop;
    public List<string> hatInventory;
    public List<string> maskInventory;
    public List<string> appearInventory;
    private database db_script;
    private rng_mov mov_script;
    private nametag name_script;
    // Start is called before the first frame update
    void Start()
    {
        countdown = 3600;
        db_script = GameObject.Find("Main Camera").GetComponent<database>();
        mov_script = gameObject.GetComponent<rng_mov>();
        name_script = gameObject.GetComponentInChildren<nametag>();
        shop = db_script.GetWholeShop();
        CheckInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mov_script.isLurking && mov_script.timeNoTalk>0)
        {
            if (countdown>0)
            {
                countdown -= 1;
            }
            else
            {
             addGold(1);
             countdown = 3600;
            }
        }
    }

    public void CheckInventory()
    {
        golds = db_script.HowMuch(gameObject.name, "view_gold");
        hatInventory = db_script.GetInventory(gameObject.name, "hat");
        maskInventory = db_script.GetInventory(gameObject.name, "mask");
        appearInventory = db_script.GetInventory(gameObject.name, "appear_anim");
    }

    public void addGold(int amount)
    {
        db_script.INeedMore(amount, gameObject.name,"view_gold");
        golds += amount;
    }

    public string TellInventory()
    {
        string inventory = " Your inventory contains : ";
        foreach (var obj in hatInventory)
        {
            inventory += " -"+obj + " ";
        }

        foreach (var obj1 in maskInventory)
        {
            inventory += " -"+obj1 + " ";
        }

        foreach (var obj2 in appearInventory)
        {
            inventory += " -"+obj2 + " ";
        }

        if (inventory==" Your inventory contains : ")
        {
            inventory = " Your inventory is empty, try buying some things and come back later ;3";
        }
        return inventory;
    }

    public bool IsItemOwned(string item)
    {
        return (hatInventory.Contains(item) || maskInventory.Contains(item) || appearInventory.Contains(item));
    }

    public void BuyItem(string item)
    {
        if (IsItemOwned(item))
        {
            _twitchConnection.WriteInChan("@"+gameObject.name+" You already own this item", _twitchConnection.accountName);
        }
        else
        {
            bool itemExist = false;
            string[] itemChosen = new string[]{};
            foreach (var product in shop)
            {
                if (product[0]==item)
                {
                    itemChosen = product;
                    itemExist = true;
                }
                
            }

            if (itemExist && Convert.ToInt32(itemChosen[1])<=golds)
            {
                golds -= Convert.ToInt32(itemChosen[1]);
                db_script.BuyItem(itemChosen[0],golds-Convert.ToInt32(itemChosen[1]), gameObject.name);
                switch (itemChosen[2])
                {
                    case "hat":
                        hatInventory.Add(itemChosen[0]);
                        break;
                    case "mask":
                        maskInventory.Add(itemChosen[0]);
                        break;
                    case "appear_anim":
                        appearInventory.Add(itemChosen[0]);
                        break;
                }
                _twitchConnection.WriteInChan("@"+gameObject.name+" you successfully bought "+itemChosen[0], _twitchConnection.accountName);
            }else if (!itemExist)
            {
                _twitchConnection.WriteInChan("@"+gameObject.name+" It looks like this item doesn't exist",_twitchConnection.accountName);
            }
        }
    }
}
