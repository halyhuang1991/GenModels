using System.Data;
using System.IO;
using System.Text;
using GenModels.DBUtility;

namespace GenModels.mssql
{
    public class Genmodel
    {
        private static string sql=@"Select t.name Table_Name,
isnull(f.value,'') Table_COMMENT,
c.name column_name,c.length data_length,c.xprec data_precision,c.xscale Data_Scale
,c.isnullable nullable,ISNULL(comm.text, '') AS defaulttext,
(case when c.xtype=56 then 'int' 
when c.xtype=167 then 'varchar'
when c.xtype=231 then 'nvarchar'
when c.xtype=108 then 'number'
when c.xtype=40 then 'date'
when c.xtype=189 then 'datestamp'
else  '' end) DATA_TYPE,cast(ep.[value]as varchar(100)) AS comments,
CASE WHEN EXISTS ( SELECT 1
FROM dbo.sysindexes si
INNER JOIN dbo.sysindexkeys sik ON si.id = sik.id
AND si.indid = sik.indid
INNER JOIN dbo.syscolumns sc ON sc.id = sik.id
AND sc.colid = sik.colid
INNER JOIN dbo.sysobjects so ON so.name = si.name
AND so.xtype = 'PK'
WHERE sc.id = c.id
AND sc.colid = c.colid ) THEN 1
ELSE 0
END iskey
 FROM SysColumns c 
 left join SysObjects t on c.id=t.id 
 LEFT JOIN sys.extended_properties AS ep ON c.id = ep.major_id
AND c.colid = ep.minor_id
AND ep.name = 'MS_Description' 
left join sys.extended_properties f on t.id=f.major_id and f.minor_id=0
LEFT JOIN dbo.syscomments comm ON c.cdefault = comm.id
  WHERE t.XType='U'";

        private static string t_sql="SELECT t.NAME Table_Name,isnull(f.value,'') Table_COMMENT FROM sys.tables AS t left join sys.extended_properties f on t.object_id=f.major_id and f.minor_id=0";
        public static string GenOneModel(string tablename){
            DataTable dtCol = GenDt(tablename);//获取表名 表备注 列名 备注 列数据类型 列长度 
            DataTable dtTb = GenTbDt(tablename);//获取表名 表备注 
            if (dtCol.Rows.Count == 0 || dtTb.Rows.Count == 0) return "";
            StringBuilder ret = new StringBuilder();
            ret.AppendLine("using System; ");
            ret.AppendLine("namespace Models {");
            ret.AppendLine(" [Serializable] ");
            ret.AppendLine("  public partial class " + tablename.ToUpper() + " { ");
            StringBuilder ret1 = new StringBuilder();
            StringBuilder ret2 = new StringBuilder();
            foreach (DataRow dr in dtCol.Rows)
            {
                string column_name = dr["column_name"].ToString().Trim();
                string DATA_TYPE = dr["DATA_TYPE"].ToString();
                if(DATA_TYPE=="")continue;
                string IsKey = dr["iskey"].ToString();
                ret1.AppendLine("   private " + GetType(DATA_TYPE) + " _" + column_name.ToLower()+";");
                if(IsKey=="1") ret2.AppendLine("    [Key]");
                ret2.AppendLine("   "+GetDs(dr));
                ret2.AppendLine("   public " + GetType(DATA_TYPE) + " " + column_name.ToUpper() + "{");
                ret2.AppendLine("   set { _" + column_name.ToLower() + " = value; }");
                ret2.AppendLine("   get { return _" + column_name.ToLower() + "; }");
                ret2.AppendLine("       }");
            }
            ret.AppendLine(ret1.ToString());
            ret.AppendLine(ret2.ToString());
            ret.AppendLine("   }");
            ret.AppendLine("}");
            return ret.ToString();
     }
      private static string GetDs(DataRow dr){
          string DATATYPE=dr["DATA_TYPE"].ToString();
          string Comments=dr["Comments"].ToString();string Datalength=dr["Data_length"].ToString();
          string Data_precision=dr["Data_precision"].ToString();string Data_Scale=dr["Data_Scale"].ToString();
          string Nullable=dr["Nullable"].ToString();
          string ret="";
          ret="[Des("+DATATYPE+","+Comments+","+Datalength+","+Data_precision+","+Data_Scale+","+Nullable+")]";
          return ret;

      }
     private static string GetType(string DATA_TYPE){
            string ret;
            switch (DATA_TYPE.ToUpper())
            {
                case "VARCHAR":
                    ret = "string";
                    break;
                 case "NVARCHAR":
                    ret = "string";
                    break;
                case "NUMBER":
                    ret = "decimal?";
                    break;
                 case "INT":
                    ret = "decimal?";
                    break;
                case "DATESTAMP":
                    ret = "";
                    break;
                case "DATE":
                    ret = "DateTime?";
                    break;
                 case "DATETIME":
                    ret = "DateTime?";
                    break;
                default:
                    ret = "string";
                    break;
        }
        return ret;
     }
     public static DataTable GenDt(string tablename){
       string v_sql=sql+" and t.name ='"+tablename.ToUpper().Trim()+"'";
       DataSet ds=MsDB.Query(v_sql);
       return ds.Tables[0];
     }
     public static DataTable GenTbDt(string tablename){
       string v_sql=t_sql+" where t.NAME ='"+tablename.ToUpper().Trim()+"'";
       DataSet ds=MsDB.Query(v_sql);
       return ds.Tables[0];
     }
     public static void WriteFile(string tablename){
          tablename=tablename.ToUpper().Trim();
          string ret=GenOneModel(tablename);
          string path=@"D:\C\github\GenModels\Models\"+tablename+".cs";
          if(File.Exists(path)){
            File.Delete(path);
          }
          File.AppendAllText(path,ret);
     }

        
    }
}