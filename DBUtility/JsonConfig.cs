using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GenModels.DBUtility
{
    public class JsonConfig
    {
        private static string path="E:\\web\\appsettings.json";
        public static string GetOraConStr(){
            string jsonString = File.ReadAllText(path, Encoding.Default);
            JObject jobject = JObject.Parse(jsonString);//解析成json
            return jobject["ConnectionStrings"]["OracleConnection"].ToString();
        }
        public static string GetOraConStr1(){
            string jsonString = File.ReadAllText(path, Encoding.Default);
            JObject jobject = JObject.Parse(jsonString);//解析成json
            return jobject["ConnectionStrings"]["OracleConnection1"].ToString();
        }
    }
}