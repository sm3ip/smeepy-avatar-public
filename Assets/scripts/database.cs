using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class database : MonoBehaviour
{
    public string pathway;

    public bool doCreate = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LocateDatabase(string path){
        pathway="URI=file:"+path+"database.db"; // gets the database location from the log_button script
    }
    public void FirstAppears(string name){
        
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm="SELECT id FROM viewers WHERE name = '"+name+"'";
            using (var cmd = new SqliteCommand(stm, con))
            {
                SqliteDataReader reader_ = cmd.ExecuteReader();
                int id=0;
                while(reader_.Read()){
                    id=reader_.GetInt32(0);
                } 
                if(id<1)
                {
                    doCreate = true;
                }else
                {
                    doCreate = false;
                    print("already in database");
                }
            }
            con.Close();
            if (doCreate)
            {
                con.Open();
                using (var cmd= new SqliteCommand(con))
                {
                    cmd.CommandText = "INSERT INTO viewers(name, level, exp, gold) VALUES(@name, @level, @exp, @gold)";
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@level", 0);
                    cmd.Parameters.AddWithValue("@exp", 0);
                    cmd.Parameters.AddWithValue("@gold", 0);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    
                }
                con.Close();
                
            }
        }
        
        
    }

    public void INeedMore(int amount, string username, string type)
    {
        using (var con = new SqliteConnection(pathway))
        {
            int stock = 0;
            con.Open();
            string stm = "SELECT "+type+" FROM viewers WHERE name ='" + username + "'";
            using (var cmd= new SqliteCommand(stm, con))
            {
                SqliteDataReader reader_ = cmd.ExecuteReader();
                
                while (reader_.Read())
                {
                    stock = reader_.GetInt32(0);
                }
            }
            con.Close();
            stock += amount;
            con.Open();
            using (var cmd= new SqliteCommand(con))
            {
                cmd.CommandText = "UPDATE viewers SET "+type+" = "+stock+" WHERE name = '"+username+"'";
                print(username);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }
}
