using System.Collections.Generic;
using System.Data;
using System.Linq;
using GenModels.DBUtility;

namespace GenModels.oracle.DailyMethod
{
    public class GenHtml
    {
        private static string GetHtmlFromDatatable(int count,string[] tables=null,string columns=null)
        {
            string v_sql = @"select a.column_name,b.comments,a.DATA_TYPE from  user_tab_columns a left join user_col_comments b on   a.Table_Name = b.Table_Name
    and a.Column_Name = b.Column_Name ";

            int ii = 0;
            foreach (string str in tables)
            {
                if (ii == 0)
                {
                    v_sql += " where a.Table_Name = '" + str.ToUpper() + "'";
                }
                else
                {
                    v_sql += " or a.Table_Name = '" + str.ToUpper() + "'";
                }
                ii++;
            }
            System.Data.DataTable dtt = new System.Data.DataTable();
            dtt =OraDB.Query(v_sql).Tables[0];
            if (columns != null)
            {
                foreach(DataRow dr in dtt.Rows)
                {
                    if (!columns.ToUpper().Split(',').Contains<string>(dr["column_name"].ToString().Trim()))
                    {
                        dr.Delete();
                    }
                }
                dtt.AcceptChanges();
                DataTable dtC = dtt.Clone();
                foreach(string str in columns.ToUpper().Split(','))
                {
                    DataRow[] drs = dtt.Select("column_name='" + str + "'");
                    if (drs.Length > 0)
                    {
                        dtC.ImportRow(drs[0]);
                    }
                }
                dtC.AcceptChanges();
                dtt = dtC;
            }
            string DialogRow = "<div class=\"Dialog-Row\">\r\n{0}\r\n<div class=\"clear\">\r\n</div>\r\n</ div > ";
            string DialogColumn = " <div class=\"Dialog-Column\">\r\n<div class=\"title\">\r\n<span><%=GetFunText(\"lab{0}\",\"{1}\") %></span>\r\n</div>\r\n<div class=\"input\">\r\n<input type=\"text\" id=\"txt{0}\"  {2} />\r\n</div><div class=\"clear\"></div>\r\n</div>";
            List<string> lsStr = new List<string>();
            int i = 1;
            string rows = "";
            string NoFilter = "creusr, credte, lstupdusr, lstupddte".ToUpper();
            foreach (DataRow dr in dtt.Rows)
            {
                if (NoFilter.IndexOf(dr["column_name"].ToString().Trim()) < 0)
                {
                    string comments = dr["comments"].ToString().Trim() == "" ? dr["column_name"].ToString() : dr["comments"].ToString().Trim();
                    string str = string.Format(DialogColumn, dr["column_name"].ToString(), comments,"");
                    if (dr["DATA_TYPE"].ToString().Trim() == "DATE")
                    {
                        str= string.Format(DialogColumn, dr["column_name"].ToString(), comments, "class='calendar'");
                    }
                    
                    if (i % count == 0)
                    {
                        rows += str + "\r\n";
                        string one = string.Format(DialogRow, rows);
                        lsStr.Add(one);
                        rows = "";
                    }
                    else
                    {
                        rows += str + "\r\n";
                    }
                    i += 1;
                }



            }
            string result = string.Join("\r\n", lsStr.ToArray());
            return result;
        }
    }
}