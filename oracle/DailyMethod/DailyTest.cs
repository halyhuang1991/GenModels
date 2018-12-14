using System;

namespace GenModels.oracle.DailyMethod
{
    public class DailyTest
    {
        public static void WriteDes(){
            string ret=GenDescription.GetDes("pcfseas","dte02y,season,seasnd,dte22z,dte23z");
            Console.WriteLine(ret);
        }
    }
}