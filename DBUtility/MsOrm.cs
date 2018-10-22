using System;
using System.Collections.Generic;
using System.Text;

namespace GenModels.DBUtility
{
    public class MsOrm
    {
        private string GetInsertSql<T>(T tclass)where T: class{
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
        public bool insert<T>(T tclass)where T: class{
            string v_sql=GetInsertSql<T>(tclass);
            int ret=MsDB.ExecuteSql(v_sql);
            return true;
        }
    }
}