using System.Collections.Generic;
using System.Data;
using GenModels.DBUtility;
using MySql.Data.MySqlClient;

namespace GenModels.mysql.Test
{
    public class MyProTest
    {
         public static void run1(){
            MySqlParameter[] cmdParms=new MySqlParameter[2];
            cmdParms[0]=new MySqlParameter("id",MySqlDbType.Int32);
            cmdParms[0].Value=1;
            cmdParms[1]=new MySqlParameter("user_count",MySqlDbType.Int32,100);
            cmdParms[1].Direction=ParameterDirection.Output;
            List<MySqlParameter> lsPara=MyDB.ExecuteSql("test.get_person_count",CommandType.StoredProcedure,cmdParms);
            string output=lsPara.Find(x=>x.ParameterName=="user_count").Value.ToString();
            output="";
        }
    }
}