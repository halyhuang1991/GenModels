using System.Data;
using System.Text;
using GenModels.DBUtility;

namespace GenModels.mysql
{
    public class GenModel
    {
        private static string sql=@"SELECT 
  c.TABLE_NAME,         
  c.COLUMN_NAME,          
  case when locate('int',c.DATA_TYPE)>0 then 'int'
  when locate('varchar',c.DATA_TYPE)>0 then 'VARCHAR2'
  when locate('double',c.DATA_TYPE)>0 then 'number'
  when locate('float',c.DATA_TYPE)>0 then 'number'
  else c.DATA_TYPE end DATA_TYPE
  ,              
  c.COLUMN_COMMENT comments,       
  c.IS_NULLABLE nullable,
  c.column_type,
  c.character_maximum_length data_length,
  c.numeric_precision data_precision,
  c.numeric_scale Data_Scale,
  c.column_key iskey
FROM
  INFORMATION_SCHEMA.COLUMNS c
  left join INFORMATION_SCHEMA.TABLES t on c.TABLE_CATALOG=t.TABLE_NAME
  WHERE c.TABLE_SCHEMA ='test' ";
    private static string t_sql=@"select t.TABLE_NAME,t.TABLE_COMMENT from INFORMATION_SCHEMA.TABLES t WHERE TABLE_SCHEMA ='test' ";
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
                ret1.AppendLine("   private " + GetType(DATA_TYPE) + " _" + column_name.ToLower()+";");
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
     private static string GetType(string DATA_TYPE){
            string ret;
            switch (DATA_TYPE)
            {
                case "VARCHAR2":
                    ret = "string";
                    break;
                case "NUMBER":
                    ret = "decimal?";
                    break;
                case "INT":
                    ret = "decimal?";
                    break;
                case "TIMESTAMP":
                    ret = "DateTime?";
                    break;
                case "DATE":
                    ret = "DateTime?";
                    break;
                default:
                    ret = "string";
                    break;
        }
        return ret;
     }
     public static DataTable GenDt(string tablename){
       string v_sql=sql+" and c.Table_Name ='"+tablename.ToUpper().Trim()+"'";
       DataSet ds=MyDB.Query(v_sql);
       return ds.Tables[0];
     }
     public static DataTable GenTbDt(string tablename){
       string v_sql=t_sql+" and t.Table_Name ='"+tablename.ToUpper().Trim()+"'";
       DataSet ds=MyDB.Query(v_sql);
       return ds.Tables[0];
     }
    }
}