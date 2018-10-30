namespace GenModels.Attibutes
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class Des:System.Attribute
    {
        readonly string datatype;readonly string comments;readonly string datalength;
        readonly string data_precision;readonly string data_scale;readonly string nullable;
        public Des(string DATATYPE,string Comments,string Datalength,string Data_precision,string Data_Scale,string Nullable){
            this.datatype=DATATYPE;this.comments=Comments;this.datalength=Datalength;
            this.data_precision=Data_precision;
            this.data_scale=Data_Scale;this.nullable=Nullable;
        }
        public string DATATYPE{
            get{
                return this.datatype;
            }
        }
        public string Comments{
            get{
                return this.comments;
            }
        }
        
        public string Datalength{
            get{
                return this.datalength;
            }
        }
        public string Data_Precision{
            get{
                return this.data_precision;
            }
        }
         public string Data_Scale{
            get{
                return this.data_scale;
            }
        }
        public string Nullable{
            get{
                return this.nullable;
            }
        }
    }
}