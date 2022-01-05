using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private TwitchConnection _twitchConnection;

    private int countdown;
    public int golds;
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
}
