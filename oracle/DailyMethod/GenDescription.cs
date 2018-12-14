using System.Collections.Generic;
using System.Data;
using GenModels.DBUtility;

namespace GenModels.oracle.DailyMethod
{
    public class GenDescription
    {
        public static string GetDes(string tables, string cols)
        {
            string v_sql = @"select a.column_name,b.comments,a.DATA_TYPE from  user_tab_columns a left join user_col_comments b on   a.Table_Name = b.Table_Name
    and a.Column_Name = b.Column_Name ";

            int i = 0;
            foreach (string str in tables.ToUpper().Split(','))
            {
                if (i == 0)
                {
                    v_sql += " where a.Table_Name = '" + str.ToUpper() + "'";
                }
                else
                {
                    v_sql += " or a.Table_Name = '" + str.ToUpper() + "'";
                }
                i += 1;
            }
            System.Data.DataTable dtt = new System.Data.DataTable();
            dtt = OraDB.Query(v_sql).Tables[0];
            List<string> ls = new List<string>();
            foreach (string str in cols.ToUpper().Split(','))
            {
                DataRow dr = dtt.Select("COLUMN_NAME='" + str + "'")[0];
                ls.Add(dr["comments"].ToString().Trim());
            }
            string ret = string.Join(",", ls.ToArray());
            return ret;
        }
    }
}