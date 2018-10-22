using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace GenModels.DBUtility
{
    public class MsOrm
    {
        public string GetInsertSql<T>(T tclass)where T: class{
            Type t=tclass.GetType();
            string tablename=t.Name;
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.Append("INSERT INTO "+tablename+"(");
            List<string> list=new List<string>();
            List<string> ls=new List<string>();
            foreach(var p in t.GetProperties())
            {
                       list.Add(p.Name);
                       var type=p.PropertyType.FullName.ToString().ToLower();
                       var val=p.GetValue(tclass,null).ToString().Replace("'","''");
                       if(type.Contains("decimal")){
                           ls.Add(val);
                       }else{
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
        public string GetUpdSql<T>(T tclass,string keys)where T: class{
            Type t=tclass.GetType();
            string tablename=t.Name;
            StringBuilder stringBuilder = new StringBuilder();
            string[] arrKeys=keys.ToUpper().Split(',');
            stringBuilder.Append("UPDATE " + tablename + " SET ");
            List<string> list = new List<string>();
            List<string> ls = new List<string>();
            foreach (var p in t.GetProperties())
            {
                var type = p.PropertyType.FullName.ToString().ToLower();
                var val = p.GetValue(tclass, null).ToString().Replace("'", "''");
                if (type.Contains("decimal"))
                {
                    list.Add(p.Name + "=" + val);
                    if (arrKeys.Contains<string>(p.Name))
                    {
                        ls.Add(p.Name + "=" + val);
                    }
                }
                else
                {
                    list.Add(p.Name + "='" + val + "'");
                    if (arrKeys.Contains<string>(p.Name))
                    {
                        ls.Add(p.Name + "='" + val + "'");
                    }
                }
            }
            string tmp1=String.Join(",",list);
            string tmp2=String.Join(",",ls);
            stringBuilder.Append(tmp1+" where "+tmp2);
            string v_sql=stringBuilder.ToString();
            return v_sql;
        }
        public bool Insert<T>(T tclass)where T: class{
            string v_sql=GetInsertSql<T>(tclass);
            int ret=MsDB.ExecuteSql(v_sql);
            return true;
        }
         public bool Insert(List<dynamic> aList){
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.AppendLine("BEGIN ");
            string v_sql="";
            foreach(var tclass in aList){
                if(tclass.GetType().isClass){
                  v_sql=GetInsertSql(tclass);
                  stringBuilder.AppendLine(v_sql+";");
                }
                
            }
            stringBuilder.AppendLine("END");
            v_sql=stringBuilder.ToString();
            int ret=MsDB.ExecuteSql(v_sql);
            return true;
         }
        public bool Update<T>(T tclass,string keys)where T: class{
            string v_sql=GetUpdSql<T>(tclass,keys);
            int ret=MsDB.ExecuteSql(v_sql);
            return true;
        }
    }
}