using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;
using System.Collections;
namespace GenModels.DBUtility
{
     public interface IDb{
       
     }
     public class DbClass{
       public static int ExecuteSql(string SQLString){
           Console.WriteLine("error!");
           return 1;
       }
       public int Execute(string SQLString){
            return DbClass.ExecuteSql(SQLString);
       }
     }
    public class OraDB:DbClass
    {
        private static string connectionString = JsonConfig.GetOraConStr();//ConfigurationManager.AppSettings["ConnectionString"];
       
        public static object GetSingle(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
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
                    catch (OracleException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        public static DataSet Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
         public new static int ExecuteSql(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        public static List<OracleParameter> ExecuteSql(string cmdText, CommandType cmdType, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        List<OracleParameter> ResultPara = new List<OracleParameter>();
                        foreach (OracleParameter parm in cmdParms)
                        {
                            if (parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Output || parm.Direction == ParameterDirection.ReturnValue)
                            {
                                ResultPara.Add(parm);
                            }
                        }
                        cmd.Parameters.Clear();
                        return ResultPara;
                    }
                    catch (OracleException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
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
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            PrepareCommand(cmd, conn, trans, CommandType.Text, cmdText, cmdParms);
        }
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
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
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                command.CommandText = storedProcName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (OracleParameter p in parameters)
                {
                    command.Parameters.Add(p);
                }
                OracleDataAdapter sqlDA = new OracleDataAdapter();
                sqlDA.SelectCommand=command;
                sqlDA.Fill(dataSet);
                for (int i = 0; i < dataSet.Tables.Count; i++)
                {
                    dataSet.Tables[i].TableName = tableNames[i];
                }
                connection.Close();
                return dataSet;
            }
        }
        /// 数组批量提交，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的表（key为sql语句，value是该语句的OracleParameter[] 每个值都是一个数组）</param>
        public static void ExecuteTran(SortedList<string, OracleParameter[]> SQLStringList)
        {
            ExecuteTran(SQLStringList, false);
        }
        public static void ExecuteTran(SortedList<string, OracleParameter[]> SQLStringList, bool IsStored)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (KeyValuePair<string, OracleParameter[]> myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
                            cmd.ArrayBindCount = ((object[])(cmdParms[0].Value)).Length;
                            cmd.BindByName = true;
                            if (IsStored)
                            {
                                PrepareCommand(cmd, conn, trans, CommandType.StoredProcedure, cmdText, cmdParms);
                            }
                            else
                            {
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            }
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}