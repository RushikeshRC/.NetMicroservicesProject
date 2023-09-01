using Mango.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {   
        }

        public DbSet<Coupon> Coupons { get; set; } //coupons will be the table name in the database on coupon class  

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);  //on the base class we have to pass modelBuilder which we have as a parameter here

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                CouponCode = "10OFF",
                DiscountAmount = 10,
                MinAmount = 20,
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 2,
                CouponCode = "20OFF",
                DiscountAmount = 20,
                MinAmount = 40,
            });
        }
    }
}
