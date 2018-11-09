using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
//using IBM.Data.DB2;
using IBM.Data.DB2.Core;
//using IBM.EntityFrameworkCore;
namespace GenModels.DBUtility
{
  
    public class IBMDB:DbClass
    {
        private static string connectionString = ConfigurationManager.AppSettings["db2ConnectionString"];
       
        public static object GetSingle(string SQLString)
        {
            using (DB2Connection connection = new DB2Connection(connectionString))
            {
                using (DB2Command cmd = new DB2Command(SQLString, connection))
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
                    catch (DB2Exception e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        public static DataSet Query(string SQLString)
        {
            using (DB2Connection connection = new DB2Connection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    // DB2DataAdapter command = new DB2DataAdapter(SQLString, connection);
                    // command.Fill(ds, "ds");
                    DB2Command cmd = new DB2Command(SQLString,connection);
                    DB2DataReader reader=cmd.ExecuteReader();
                    DataTable dt=new DataTable();
                    myclass my=new myclass();
                    //dt.Load(reader);
                    my.MyFill(dt, reader);
                    ds.Tables.Add(dt);
                }
                catch (DB2Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
         public new static int ExecuteSql(string SQLString)
        {
            using (DB2Connection connection = new DB2Connection(connectionString))
            {
                using (DB2Command cmd = new DB2Command(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (DB2Exception E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        public static List<DB2Parameter> ExecuteSql(string cmdText, CommandType cmdType, params DB2Parameter[] cmdParms)
        {
            using (DB2Connection connection = new DB2Connection(connectionString))
            {
                connection.Open();
                using (DB2Command cmd = new DB2Command())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        List<DB2Parameter> ResultPara = new List<DB2Parameter>();
                        foreach (DB2Parameter parm in cmdParms)
                        {
                            if (parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Output || parm.Direction == ParameterDirection.ReturnValue)
                            {
                                ResultPara.Add(parm);
                            }
                        }
                        cmd.Parameters.Clear();
                        return ResultPara;
                    }
                    catch (DB2Exception E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        private static void PrepareCommand(DB2Command cmd, DB2Connection conn, DB2Transaction trans, CommandType cmdType, string cmdText, DB2Parameter[] cmdParms)
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
                foreach (DB2Parameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        private static void PrepareCommand(DB2Command cmd, DB2Connection conn, DB2Transaction trans, string cmdText, DB2Parameter[] cmdParms)
        {
            PrepareCommand(cmd, conn, trans, CommandType.Text, cmdText, cmdParms);
        }
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (DB2Connection conn = new DB2Connection(connectionString))
            {
                conn.Open();
                using (DB2Transaction trans = conn.BeginTransaction())
                {
                    DB2Command cmd = new DB2Command();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            DB2Parameter[] cmdParms = (DB2Parameter[])myDE.Value;
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
       
        public static void ExecuteSqlTran(List<string> SQLStringList)
        {
            using (DB2Connection conn = new DB2Connection(connectionString))
            {
                conn.Open();
                using (DB2Transaction trans = conn.BeginTransaction())
                {
                    DB2Command cmd = new DB2Command();
                    try
                    {
                        cmd.Transaction =trans;
                        cmd.Connection=conn;
                        //循环
                        foreach (string sql in SQLStringList)
                        {
                            string cmdText = sql;
                            cmd.CommandText=cmdText;
                            int val = cmd.ExecuteNonQuery();
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
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string[] tableNames)
        {
            using (DB2Connection connection = new DB2Connection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                DB2Command command = new DB2Command();
                command.Connection = connection;
                command.CommandText = storedProcName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DB2Parameter p in parameters)
                {
                    command.Parameters.Add(p);
                }
                // DB2DataAdapter sqlDA = new DB2DataAdapter();
                // sqlDA.Fill(dataSet);
                // for (int i = 0; i < dataSet.Tables.Count; i++)
                // {
                //     dataSet.Tables[i].TableName = tableNames[i];
                // }
                myclass myclass=new myclass();
                myclass.SelectCommand= command;
                myclass.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

    }
}