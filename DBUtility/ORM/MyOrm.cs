using System;
using System.Collections.Generic;
using System.Text;
namespace GenModels.DBUtility.ORM
{
    public class MyOrm:OrmClass
    {
        static MyOrm _new;
        
        public new static MyOrm New{
            get{
                if(_new==null){
                    _new=new MyOrm();
                }
                return _new;
            }
        }
        public override bool Insert<T>(T tclass){
            string v_sql=GetInsertSql<T>(tclass);
            int ret=MyDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Insert(List<dynamic> aList){
            string v_sql = ""; List<string> ls = new List<string>();
            foreach (var tclass in aList)
            {
                v_sql = GetInsertSql(tclass);
                ls.Add(v_sql);
            }

            MyDB.ExecuteSqlTran(ls);
            return true;
         }
        public override bool Update<T>(T tclass,string keys="",string updCols=""){
            string v_sql=GetUpdSql<T>(tclass,keys,updCols);
            int ret=MyDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Update(Dictionary<dynamic, string> dicClass){
            string v_sql = ""; List<string> ls = new List<string>();
            foreach(var item in dicClass){
                  v_sql=GetUpdSql(item.Key,item.Value);
                  ls.Add(v_sql);
            }
            MyDB.ExecuteSqlTran(ls);
            return true;
        }
        public override bool Delete<T>(T tclass,string keys=""){
            string v_sql=GetDelSql<T>(tclass,keys);
            int ret=MyDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Delete(Dictionary<dynamic, string> dicClass){
            string v_sql="";List<string> ls=new List<string>();
            foreach(var item in dicClass){
                  v_sql=GetDelSql(item.Key,item.Value);
                   ls.Add(v_sql);
            }
            MyDB.ExecuteSqlTran(ls);
            return true;
        }
        public string GetAddOrUpdSql<T>(T tclass) where T: class{
            Type t=tclass.GetType();
            string tablename=t.Name;
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.Append("REPLACE INTO "+tablename+"(");
            List<string> list=new List<string>();
            List<string> ls=new List<string>();
            foreach(var p in t.GetProperties())
            {
                       list.Add(p.Name);
                       var type=p.PropertyType.FullName.ToString().ToLower();
                       var value=p.GetValue(tclass,null);
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
        public override bool AddOrUpd(Dictionary<dynamic, string> dicClass){
            string v_sql="";List<string> ls=new List<string>();
            foreach (var item in dicClass)
            {
                v_sql = GetAddOrUpdSql(item.Key);
                ls.Add(v_sql);
            }
            MyDB.ExecuteSqlTran(ls);
            return true;
        }
    }
   
}