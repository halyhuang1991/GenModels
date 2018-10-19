
using GenModels.mysql;
using Microsoft.EntityFrameworkCore;
namespace GenModels.oracle
{
    public class CommonDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseOracle("DATA SOURCE=127.0.0.1:1521/tjims;PASSWORD=test;PERSIST SECURITY INFO=True;USER ID=test");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<remark> remark { get; set; }
    }
}