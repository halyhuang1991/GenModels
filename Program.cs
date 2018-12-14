using System;
using System.Data;
using GenModels.DBUtility;
using System.IO;
using System.Collections.Generic;
using GenModels.DBUtility.ORM;
using System.Data.Common;
using GenModels.mssql.Test;

namespace GenModels
{
    class Program
    {
        static void Main(string[] args)
        {
            mysql.Test.MyProTest.run1();
            Console.WriteLine("Hello World!");
        }
        private static void WriteModel(){
            //GenModels.mssql.Genmodel.WriteFile("book");
            GenModels.mysql.GenModel.WriteFile("score");
        }
         private static void TestMysql(){
             DBUtility.ORM.MyOrm my=DBUtility.ORM.MyOrm.New;
              Models.SCORE SCORE=new Models.SCORE();
              SCORE.ID=444;
              SCORE.NAME ="232";
              my.Insert<Models.SCORE>(SCORE);



         }
        private static void TestTransation(){
           Models.BOOK book=new Models.BOOK();
           book.ID=342;
           book.NAME="sds";
           Models.BOOK book1=new Models.BOOK();
           book1=book;
           book.NAME="sds23";
            DBUtility.ORM.SqlTrans tran =new DBUtility.ORM.SqlTrans();
           //tran.Insert<Models.BOOK>(book);
           tran.Update<Models.BOOK>(book1);
           tran.Submit();
           //-----------------------------------------
            DBUtility.ORM.SqlTrans tran1 = new DBUtility.ORM.SqlTrans(new DBUtility.ORM.MyOrm());
            Models.SCORE SCORE = new Models.SCORE();
            SCORE.ID = 2;
            SCORE.NAME = "232";
            tran1.Update<Models.SCORE>(SCORE);
            Models.SCORE SCORE1 = new Models.SCORE();
            SCORE1.ID = 60;
            SCORE1.NAME = "232";
            tran1.Insert<Models.SCORE>(SCORE1);
            tran1.Submit();
            //----------------------------------------
            OrmClass orm=new OrmClass();
            string ret=orm.GetPageSql("id", "id,name", 1, 10, " id >0", "book");

        }
    }
}
