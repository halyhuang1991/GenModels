using System;
using System.Data;
using GenModels.DBUtility;
using System.IO;
using System.Collections.Generic;

namespace GenModels
{
    class Program
    {
        static void Main(string[] args)
        {
          

           Console.WriteLine("Hello World!");
        }
        private static void WriteModel(){
            GenModels.mssql.Genmodel.WriteFile("book");
        }
        private static void TestTransation(){
           Models.BOOK book=new Models.BOOK();
           book.ID=342;
           book.NAME="sds";
           Models.BOOK book1=new Models.BOOK();
           book1=book;
           book.NAME="sds23";
           MsSqlTran tran=new MsSqlTran();
           //tran.Insert<Models.BOOK>(book);
           tran.Update<Models.BOOK>(book1);
           tran.Submit();
        }
    }
}
