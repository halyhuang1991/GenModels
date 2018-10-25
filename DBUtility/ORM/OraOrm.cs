using System;
using System.Collections.Generic;
using System.Text;

namespace GenModels.DBUtility.ORM
{
    public class OraOrm:OrmClass
    {
        static OraOrm _new;
        public new static OraOrm New{
            get{
                if(_new==null){
                    _new=new OraOrm();
                }
                return _new;
            }
        }
         public override bool Insert<T>(T tclass){
            string v_sql=GetInsertSql<T>(tclass);
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Insert(List<dynamic> aList){
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.AppendLine("BEGIN ");
            string v_sql="";
            foreach(var tclass in aList){
                  v_sql=GetInsertSql(tclass);
                  stringBuilder.AppendLine(v_sql+";");
            }
            stringBuilder.AppendLine("END");
            v_sql=stringBuilder.ToString();
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
         }
        public override bool Update<T>(T tclass,string keys="",string updCols=""){
            string v_sql=GetUpdSql<T>(tclass,keys,updCols);
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Update(Dictionary<dynamic, string> dicClass){
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.AppendLine("BEGIN ");
            string v_sql="";
            foreach(var item in dicClass){
                  v_sql=GetUpdSql(item.Key,item.Value);
                  stringBuilder.AppendLine(v_sql+";");
            }
            stringBuilder.AppendLine("END");
            v_sql=stringBuilder.ToString();
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Delete<T>(T tclass,string keys=""){
            string v_sql=GetDelSql<T>(tclass,keys);
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool Delete(Dictionary<dynamic, string> dicClass){
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.AppendLine("BEGIN ");
            string v_sql="";
            foreach(var item in dicClass){
                  v_sql=GetDelSql(item.Key,item.Value);
                  stringBuilder.AppendLine(v_sql+";");
            }
            stringBuilder.AppendLine("END");
            v_sql=stringBuilder.ToString();
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
        }
        public override bool AddOrUpd(Dictionary<dynamic, string> dicClass){
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.AppendLine("BEGIN ");
            string v_sql="";
            foreach (var item in dicClass)
            {
                v_sql = GetDelSql(item.Key, item.Value);
                stringBuilder.AppendLine(v_sql + ";");
                stringBuilder.AppendLine("IF SQL%NOTFOUND THEN");
                v_sql = GetInsertSql(item.Key);
                stringBuilder.AppendLine(v_sql + ";");
                stringBuilder.AppendLine("END IF;");
            }
            stringBuilder.AppendLine("END;");
            v_sql=stringBuilder.ToString();
            int ret=OraDB.ExecuteSql(v_sql);
            return true;
        }
    }
}