using System; 
namespace Models {
 [Serializable] 
  public partial class SCORE { 
   private decimal? _id;
   private string _name;
   private decimal? _math;
   private decimal? _english;

    [Key]
   public decimal? ID{
   set { _id = value; }
   get { return _id; }
       }
   public string NAME{
   set { _name = value; }
   get { return _name; }
       }
   public decimal? MATH{
   set { _math = value; }
   get { return _math; }
       }
   public decimal? ENGLISH{
   set { _english = value; }
   get { return _english; }
       }

   }
}
