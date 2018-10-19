using Microsoft.EntityFrameworkCore;
namespace GenModels{
public class BooksContext : DbContext
    {
        //private DbContextOptionsBuilder _optionsBuilder;
        public DbSet<book> book { get; set; } //和表同名
        private string connstr{ get; set; }
        public BooksContext(){
           // _optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=test; User=root;Password=;"); 
           var hostname = "XPHP0004\\HALY";
           var username="haly";
            var password = "admin";
            var connString = $"Data Source={hostname};Initial Catalog=test;User ID={username};Password={password};pooling=false";
           //_optionsBuilder.UseSqlServer(connString); 
           connstr=connString;
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          // optionsBuilder=_optionsBuilder;
          optionsBuilder.UseSqlServer(this.connstr); 
        }
    }
     public class book
    {
        public int id{get;set;}
        public string name{get;set;}
        public int booknum{get;set;}
    }
    
}

