using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using GenModels.DBUtility;
using Models;
namespace GenModels.Sqlite.Test
{
    public class TestSqlitecs
    {
        public partial class UNPROMISE {
            [Key]
            public string NAME{get;set;}
            public string REMARK{get;set;}
        }
        public static void run1(){
             List<SCORE> ls=new List<SCORE>();
             Models.SCORE SCORE=new Models.SCORE();
              SCORE.ID=444;
              SCORE.NAME ="232";
              ls.Add(SCORE);
              Sqlite.SqliteHelper.UpdAddValues<SCORE>(ls);
        }
        public static void run2(){
            string path=@"C:\Users\halyhuang\Desktop\lai.txt";
            FileInfo file=new FileInfo(path);
            if (!file.Exists)return;//file.Create();
            List<UNPROMISE> ls=new List<UNPROMISE>();
            StreamReader sr = new StreamReader(path,Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null) 
            {
                if (line.ToString() != "")
                {
                    UNPROMISE model = new UNPROMISE();
                    model.NAME=line.ToString();
                    model.REMARK="Broken faith enterprise";
                    ls.Add(model);
                }
            }
            Sqlite.SqliteHelper.UpdAddValues<UNPROMISE>(ls);
                
        }
        public static void query2(){
               DataSet dataSet=SqliteHelper.ReadTable("UNPROMISE","");
               foreach(DataRow dr in dataSet.Tables[0].Rows){
                   Console.WriteLine(dr[0].ToString());
               }
        }
    }
}