using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GenModels.DBUtility;

namespace GenModels.mssql.Test
{
    public class TestPro1
    {
        public static void run1(){
            SqlParameter[] cmdParms=new SqlParameter[3];
            cmdParms[0]=new SqlParameter("@GoodsId",SqlDbType.Int);
            cmdParms[0].Value=23;
            cmdParms[1]=new SqlParameter("@name",SqlDbType.VarChar);
            cmdParms[1].Value="23";
            cmdParms[2]=new SqlParameter("@output",SqlDbType.VarChar,100);
            cmdParms[2].Direction=ParameterDirection.Output;
            List<SqlParameter> lsPara=MsDB.ExecuteSql("[dbo].[test_three]",CommandType.StoredProcedure,cmdParms);
            string output=lsPara.Find(x=>x.ParameterName=="@output").Value.ToString();
            output="";
        }
        public static void run2(){
            DataTable dataTable=new DataTable();
            dataTable.Columns.Add("name",typeof(string));
            for(int i=0;i<10;i++){
                DataRow dataRow=dataTable.NewRow();
                dataRow["name"]=i+"";
                dataTable.Rows.Add(dataRow);
            }
            SqlParameter[] cmdParms=new SqlParameter[1];
            cmdParms[0]=new SqlParameter("@ManyRows",SqlDbType.Structured);
            cmdParms[0].Value=dataTable;
            
            List<SqlParameter> lsPara=MsDB.ExecuteSql("[dbo].[InsertMultiRows]",CommandType.StoredProcedure,cmdParms);
            string output="";
        }
    }
}