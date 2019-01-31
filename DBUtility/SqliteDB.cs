using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
namespace GenModels.DBUtility
{
    public class SQLiteDB:DbClass
    {
        private static string connectionString = ConfigurationManager.AppSettings["SQLiteConnectionString"];
       
        public static object GetSingle(string SQLiteString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLiteString, connection))
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
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        public static DataSet Query(string SQLiteString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLiteString, connection);
                    command.Fill(ds, "ds");
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public static int ExecuteSQL(string SQLiteString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLiteString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        public static List<SQLiteParameter> ExecuteSQL(string cmdText, CommandType cmdType, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        List<SQLiteParameter> ResultPara = new List<SQLiteParameter>();
                        foreach (SQLiteParameter parm in cmdParms)
                        {
                            if (parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Output || parm.Direction == ParameterDirection.ReturnValue)
                            {
                                ResultPara.Add(parm);
                            }
                        }
                        cmd.Parameters.Clear();
                        return ResultPara;
                    }
                    catch (SQLiteException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, CommandType cmdType, string cmdText, SQLiteParameter[] cmdParms)
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
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            PrepareCommand(cmd, conn, trans, CommandType.Text, cmdText, cmdParms);
        }
        public static void ExecuteSQLTran(Hashtable SQLiteStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLiteStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                         trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        public static void ExecuteSQLTran(List<string> SQLiteStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    try
                    {
                        cmd.Transaction =trans;
                        cmd.Connection=conn;
                        //循环
                        foreach (string SQLite in SQLiteStringList)
                        {
                            string cmdText = SQLite;
                            cmd.CommandText=cmdText;
                            int val = cmd.ExecuteNonQuery();
                        }
                         trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string[] tableNames)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = storedProcName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (SQLiteParameter p in parameters)
                {
                    command.Parameters.Add(p);
                }
                SQLiteDataAdapter SQLiteDA = new SQLiteDataAdapter();
                SQLiteDA.SelectCommand=command;
                SQLiteDA.Fill(dataSet);
                for (int i = 0; i < dataSet.Tables.Count; i++)
                {
                    if(tableNames.Length-1>=i)
                    dataSet.Tables[i].TableName = tableNames[i];
                }
                connection.Close();
                return dataSet;
            }
        }

    }
}