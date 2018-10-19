using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace GenModels.DBUtility
{
    public class MyDB
    {
        private static string connectionString = ConfigurationManager.AppSettings["MyConnectionString"];
       
        public static object GetSingle(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        public static DataSet Query(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    // MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    // command.Fill(ds, "ds");
                    MySqlCommand cmd = new MySqlCommand(SQLString,connection);
                    MySqlDataReader reader=cmd.ExecuteReader();
                    DataTable dt=new DataTable();
                    myclass my=new myclass();
                    //dt.Load(reader);
                    my.MyFill(dt, reader);
                    ds.Tables.Add(dt);
                  

                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public static List<MySqlParameter> ExecuteSql(string cmdText, CommandType cmdType, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        List<MySqlParameter> ResultPara = new List<MySqlParameter>();
                        foreach (MySqlParameter parm in cmdParms)
                        {
                            if (parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Output || parm.Direction == ParameterDirection.ReturnValue)
                            {
                                ResultPara.Add(parm);
                            }
                        }
                        cmd.Parameters.Clear();
                        return ResultPara;
                    }
                    catch (MySqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (trans != null)
                cmd.Transaction = trans;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            PrepareCommand(cmd, conn, trans, CommandType.Text, cmdText, cmdParms);
        }
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string[] tableNames)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = storedProcName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (MySqlParameter p in parameters)
                {
                    command.Parameters.Add(p);
                }
                // MySqlDataAdapter sqlDA = new MySqlDataAdapter();
                // sqlDA.Fill(dataSet);
                // for (int i = 0; i < dataSet.Tables.Count; i++)
                // {
                //     dataSet.Tables[i].TableName = tableNames[i];
                // }
                //dataSet.Load(command.ExecuteReader(),LoadOption.OverwriteChanges,tableNames);
                myclass myclass=new myclass();
                myclass.SelectCommand= command;
                myclass.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }
        
    }
     public class myclass : System.Data.Common.DbDataAdapter
    {
        protected override int Fill(DataTable dataTable, IDataReader dataReader)
        {
            return base.Fill(dataTable, dataReader);
        }
 
        public int MyFill(DataTable dataTable, IDataReader dataReader)
        {
            return Fill(dataTable, dataReader);
        }
        protected override int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
        {
            return base.Fill(dataSet, srcTable, dataReader,startRecord,maxRecords);
        }
    }
}