using System.Collections.Generic;
using System.Data;
using GenModels.DBUtility;
using System.Reflection;
using System;
using System.Linq;
using System.Text;

namespace GenModels.Sqlite
{
    public class SqliteHelper
    {
         public static bool HasAttibute(PropertyInfo p,string attibute){
            Attribute[] ab=Attribute.GetCustomAttributes(p);
            if(ab.Length>0){
                return true;
            }else{
                return false;
            }
        }
        public static string GetUpdAddSql<T>(Type t,T model)where T : class{
            string tablename=t.Name;
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.Append("REPLACE INTO "+tablename+"(");
             List<string> list=new List<string>();
            List<string> ls=new List<string>();
            foreach(var p in t.GetProperties())
            {
                       list.Add(p.Name);
                       var type=p.PropertyType.FullName.ToString().ToLower();
                       var value=p.GetValue(model,null);
                       if(type.Contains("decimal")){
                           var val=value==null?"null":value.ToString();
                           ls.Add(val);
                       }else{
                           var val=value==null?"":value.ToString().Replace("'","''");
                           ls.Add("'"+val+"'");
                       }
            }
            string tmp1=String.Join(",",list);
            string tmp2=String.Join(",",ls);
            stringBuilder.Append(tmp1+") VALUES(");
            stringBuilder.Append(tmp2+")");
            string v_sql=stringBuilder.ToString();
            return v_sql;
        }
        public static int CreateTable(string tableName, string[] colNames, string[] colTypes,string[] colKeys)
        {
            string queryString = "CREATE TABLE IF NOT EXISTS " + tableName + "(" + colNames[0] + " " + colTypes[0];
            for (int i = 1; i < colNames.Length; i++)
            {
                queryString += ", " + colNames[i] + " " + colTypes[i];
            }
            if (colKeys.Length > 0)
            {
                queryString += ", PRIMARY KEY(" + string.Join(',', colKeys) + ")";
            }
            queryString += "  ) ";
            return SQLiteDB.ExecuteSQL(queryString);
        }
        public static DataSet ReadTable(string tableName, string where)
        {
            string queryString = "SELECT * from " + tableName;
            if (where.Trim() != "")
            {
                queryString += " where" + where;
            }
            return SQLiteDB.Query(queryString);
        }
        public static void UpdAddValues<T>(List<T> ls) where T : class
        {
            if (ls.Count == 0) return;
            Type t=typeof(T);
            string tbn=t.Name;
            string[] colNames=(from item in t.GetProperties().AsEnumerable() select item.Name).ToArray();
            string[] colTypes=(from item in t.GetProperties().AsEnumerable() select item.PropertyType==typeof(System.String)?"varchar":"double").ToArray();
            string[] colKeys=(from item in t.GetProperties().AsEnumerable() where HasAttibute(item,"Key")  select item.Name).ToArray();
            CreateTable(tbn,colNames,colTypes,colKeys);
            List<string> lss=new List<string>();
            foreach(T model in ls){
                string v_sql=GetUpdAddSql<T>(t,model);
                lss.Add(v_sql);
                
            }
            SQLiteDB.ExecuteSQLTran(lss);
        }
    }
}