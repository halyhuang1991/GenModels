using System; 
namespace Models {
 [Serializable] 
  public partial class BOOK {
        private decimal? _id;
        private string _name;
        private decimal? _booknum;

        [Key]
        public decimal? ID
        {
            set { _id = value; }
            get { return _id; }
        }
        public string NAME
        {
            set { _name = value; }
            get { return _name; }
        }
        public decimal? BOOKNUM
        {
            set { _booknum = value; }
            get { return _booknum; }
        }

   }
}
