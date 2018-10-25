using System;
using System.Collections.Generic;

namespace GenModels.DBUtility.ORM
{
    public class SqlTrans
    {
         private OrmClass orm;
        List<string> SQLStringList;
        public SqlTrans()
        {
            orm = OrmClass.New;
            SQLStringList = new List<string>();
        }
         public SqlTrans(OrmClass anyorm)
        {
            orm = anyorm;
            SQLStringList = new List<string>();
        }
        public void Insert<T>(T tclass) where T : class
        {
            string v_sql = orm.GetInsertSql<T>(tclass);
            SQLStringList.Add(v_sql);
        }
        public void Insert(List<dynamic> aList)
        {
            foreach (var tclass in aList)
            {
                string v_sql = orm.GetInsertSql(tclass);
                SQLStringList.Add(v_sql);
            }
        }
        public void Update<T>(T tclass, string keys = "") where T : class
        {
            string v_sql = orm.GetUpdSql<T>(tclass, keys);
            SQLStringList.Add(v_sql);
        }
        public void Update(Dictionary<dynamic, string> dicClass)
        {
            string v_sql = "";
            foreach (var item in dicClass)
            {
                v_sql = orm.GetUpdSql(item.Key, item.Value);
                SQLStringList.Add(v_sql);
            }

        }
        public void Delete<T>(T tclass, string keys = "") where T : class
        {
            string v_sql = orm.GetDelSql<T>(tclass, keys);
            SQLStringList.Add(v_sql);
        }
        public void Delete(Dictionary<dynamic, string> dicClass)
        {
            string v_sql = "";
            foreach (var item in dicClass)
            {
                v_sql = orm.GetDelSql(item.Key, item.Value);
                SQLStringList.Add(v_sql);
            }
        }
        public void Submit()
        {
            if (this.SQLStringList.Count == 0) return;
            switch(orm.GetType().Name.ToLower()){
                case "myorm":
                    MyDB.ExecuteSqlTran(this.SQLStringList);
                    break;
                case "oraorm":
                    OraDB.ExecuteSqlTran(this.SQLStringList);
                    break;
                case "msorm":
                    MsDB.ExecuteSqlTran(this.SQLStringList);
                    break;
                default:
                    throw new Exception("null!");
                    //break;
            }
            
        }
    }
}