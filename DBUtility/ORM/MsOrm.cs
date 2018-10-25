using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.Reflection;

namespace GenModels.DBUtility.ORM
{
    //单例 贪婪 模式
    public class MsOrm:OrmClass
    {
        static MsOrm _new;

        public static MsOrm GetNew()
        {
            if (_new == null)
            {
                _new = new MsOrm();
            }
            return _new;
        }


    }
    
}