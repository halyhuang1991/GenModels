using System.Data;
using System.Text;
using GenModels.DBUtility;

namespace GenModels.oracle
{
    public class GenModel
    {
        private static string sql=@"select a.Table_Name,s.COMMENTS Table_COMMENT,a.column_name,b.comments,a.DATA_TYPE,a.data_length,a.data_precision,a.Data_Scale,A.nullable  
from  user_tab_columns a inner join user_col_comments b on   a.Table_Name = b.Table_Name
    and a.Column_Name = b.Column_Name
    left join all_tab_comments s on a.Table_Name = s.Table_Name";
    private static string t_sql=@"select distinct a.Table_Name,s.COMMENTS Table_COMMENT from  user_tab_columns a inner join all_tab_comments s 
     on a.Table_Name = s.Table_Name";
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
                ret1.AppendLine("   private " + GetType(DATA_TYPE) + " _" + column_name.ToLower());
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
                case "TIMESTAMP(3)":
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
       string v_sql=sql+" where a.Table_Name ='"+tablename.ToUpper().Trim()+"'";
       DataSet ds=OraDB.Query(v_sql);
       return ds.Tables[0];
     }
     public static DataTable GenTbDt(string tablename){
       string v_sql=t_sql+" where a.Table_Name ='"+tablename.ToUpper().Trim()+"'";
       DataSet ds=OraDB.Query(v_sql);
       return ds.Tables[0];
     }
    }
}