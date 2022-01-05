using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            string stm="SELECT view_id FROM viewers WHERE view_name = '"+name+"'";
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
                }
            }
            con.Close();
            if (doCreate)
            {
                con.Open();
                using (var cmd= new SqliteCommand(con))
                {
                    cmd.CommandText = "INSERT INTO viewers(view_name, view_level, view_exp, view_gold) VALUES(@name, @level, @exp, @gold)";
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
            string stm = "SELECT "+type+" FROM viewers WHERE view_name ='" + username + "'";
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
                cmd.CommandText = "UPDATE viewers SET "+type+" = "+stock+" WHERE view_name = '"+username+"'";
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }

    public int HowMuch(string username, string type)
    {
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT " + type + " FROM viewers WHERE view_name ='" + username + "'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    return _reader.GetInt32(0);
                }
            }
            con.Close();
        }

        return 0;
    }

    public void SetValue(string username, int value, string type)
    {
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            using (var cmd = new SqliteCommand(con))
            {
                cmd.CommandText = "UPDATE viewers SET " + type + " = " + value + " WHERE view_name = '" + username + "'";
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }

    public List<string> GetInventory(string username, string objectType)
    {
        List<string> myInventory = new List<string>();
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT obj_name FROM objects AS o INNER JOIN objectTypes AS ot ON o.type_id_ob = ot.OTid INNER JOIN hasObject AS ho ON o.obj_id = ho.id_object_ho INNER JOIN viewers AS v ON ho.id_viewer_ho = v.view_id"+" WHERE ot.OTName ='"+objectType+"' AND v.view_name ='"+username+"'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    myInventory.Add(_reader.GetString(0));
                }
            }
            // find a way to good inner join
            con.Close();
        }

        return myInventory;
    }
}
