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
                    print("already in database");
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
            con.Close();
        }

        return myInventory;
    }

    public string GetShop()
    {
        string shop="";
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT obj_name, obj_cost FROM objects";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    shop += " -" + _reader.GetString(0) + " costs : " + _reader.GetInt32(1).ToString()+" ";
                }
            }
            con.Close();
        }
        return shop;
    }

    public List<string[]> GetWholeShop()
    {
        List<string[]> shopIntels = new List<string[]>();
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT obj_name, obj_cost, OTName FROM objects AS o INNER JOIN objectTypes AS ot ON ot.OTid = o.type_id_ob";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    string[] item = {_reader.GetString(0),_reader.GetInt32(1).ToString(),_reader.GetString(2) };
                    shopIntels.Add(item);
                }
            }
            con.Close();
        }

        return shopIntels;
    }

    public void BuyItem(string item, int newMoney, string username)
    {
        int idViewer=0;
        int idObject=0;
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT view_id FROM viewers WHERE view_name = '" + username + "'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    idViewer = _reader.GetInt32(0);
                }
            }
            stm = "SELECT obj_id FROM objects WHERE obj_name = '" + item + "'";
            using (var cmd2 = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader2 = cmd2.ExecuteReader();
                while (_reader2.Read())
                {
                    idObject = _reader2.GetInt32(0);
                }
            }

            using (var cmd3 = new SqliteCommand(con))
            {
                cmd3.CommandText = "INSERT INTO hasObject(id_viewer_ho, id_object_ho) VALUES(@idView, @idObj)";
                cmd3.Parameters.AddWithValue("@idView", idViewer);
                cmd3.Parameters.AddWithValue("@idObj", idObject);
                cmd3.Prepare();
                cmd3.ExecuteNonQuery();
            }
            con.Close();
        }
        SetValue(username, newMoney, "view_gold");
    }

    public float[] GetCoords(string objName)
    {
        float[] coords = new float[]{};
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT localPosition_x, localPosition_y, localPosition_z, localScale_x, localScale_y, localScale_z FROM objects WHERE obj_name='"+objName+"'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    coords = new float[] {_reader.GetFloat(0),_reader.GetFloat(1),_reader.GetFloat(2), _reader.GetFloat(3), _reader.GetFloat(4), _reader.GetFloat(5) };
                    
                }
            }
            con.Close();
        }

        return coords;
    }

    public void EquipObj(string objName, string viewName)
    {
        int idObj = 0;
        int ObjType = 0;
        int idView = 0;
        List<int> typesWear = new List<int>();
        List<int> idsHO = new List<int>();
        bool isTaskDone = false;
        using (var con = new SqliteConnection(pathway))
        {
            
            con.Open();
            string stm = "SELECT obj_id, type_id_ob FROM objects WHERE obj_name = '" + objName + "'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    idObj = _reader.GetInt32(0);
                    ObjType = _reader.GetInt32(1);
                }   
            }

            stm = "SELECT view_id FROM viewers WHERE view_name = '" + viewName + "'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    idView = _reader.GetInt32(0);
                }   
            }

            stm =
                "SELECT type_id_ob, idOE FROM objects AS o INNER JOIN ObjectEquipped AS h ON o.obj_id = h.idObjectOE WHERE h.idViewerOE = " + idView;
            using (var cmd = new SqliteCommand(stm, con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    typesWear.Add(_reader.GetInt32(0));
                    idsHO.Add(_reader.GetInt32(1));
                }
            }
            con.Close();
        }

        for (int i = 0; i < typesWear.Count; i++)
        {
            if (typesWear[i]==ObjType)
            {
                updateOE(idsHO[i],idView,idObj);
                isTaskDone = true;
            }
        }
        if (!isTaskDone)
        {
            using (var con = new SqliteConnection(pathway))
            {
                con.Open();
                using (var cmd = new SqliteCommand(con))
                {
                    cmd.CommandText = "INSERT INTO ObjectEquipped (idViewerOE,idObjectOE) VALUES (@idView, @idObj)";
                    cmd.Parameters.AddWithValue("@idView", idView);
                    cmd.Parameters.AddWithValue("@idObj", idObj);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }

    public void updateOE(int idOE, int viewer, int obj)
    {
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            using (var cmd = new SqliteCommand(con))
            {
                cmd.CommandText = "UPDATE ObjectEquipped SET idViewerOE = "+ viewer+", idObjectOE = "+ obj + " WHERE idOE = "+idOE;
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }

    public List<string> CheckObjEquipped(string username)
    {
        List<string> objects = new List<string>();
        int id_user = 0;
        using (var con = new SqliteConnection(pathway))
        {
            con.Open();
            string stm = "SELECT view_id FROM viewers WHERE view_name = '"+username+"'";
            using (var cmd = new SqliteCommand(stm,con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    id_user = _reader.GetInt32(0);
                }
            }

            stm = "SELECT obj_name, OTName FROM ObjectEquipped AS e INNER JOIN objects AS o ON e.idObjectOE = o.obj_id INNER JOIN objectTypes AS t ON o.type_id_ob = t.OTid WHERE e.idViewerOE = " + id_user;
            using (var cmd = new SqliteCommand(stm, con))
            {
                SqliteDataReader _reader = cmd.ExecuteReader();
                while (_reader.Read())
                {
                    objects.Add(_reader.GetString(0));
                    objects.Add(_reader.GetString(1));
                }
            }
            con.Close();
        }

        return objects;
    }
}
