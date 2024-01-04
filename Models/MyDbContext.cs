//su dung entity framework
using Microsoft.EntityFrameworkCore;
//doc noi dung file appsettings.json
using Microsoft.Extensions.Configuration;
//thao tac voi file, thu muc
using System.IO;
namespace Project.Models
{
    public class MyDbContext : DbContext //tuong ung voi database net19_project
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //tao doi tuong de doc thong tin cua file appsettings.json
            var builder = new ConfigurationBuilder();
            //set duong dan cua file appsettings.json
            builder.SetBasePath(Directory.GetCurrentDirectory());
            //add file appsettings.json vao doi tuong builder
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            //doc chuoi ket noi o trong file appsettings.json
            string strDbConnectString = configuration.GetConnectionString("DBConnectString").ToString();
            //ket noi voi csdl thong qua chuoi ket noi
            optionsBuilder.UseSqlServer(strDbConnectString);
        }
        public DbSet<ItemUser> Users { get; set; } //<=> table Users trong csdl
        public DbSet<ItemCategory> Categories { get; set; } //<=> table Categories trong csdl
        public DbSet<ItemProduct> Products { get; set; } //<=> table Products trong csdl
        public DbSet<ItemCategoriesProducts> CategoriesProducts { get; set; } //<=> table CategoriesProducts trong csdl
        public DbSet<ItemNews> News { get; set; }//<=> table News
        public DbSet<ItemRating> Rating { get; set; }//<=> table Rating
        public DbSet<ItemCustomers> Customers { get; set; }//<=> table Customers
        public DbSet<ItemOrders> Orders { get; set; }//<=> table Orders
        public DbSet<ItemOrdersDetail> OrdersDetail { get; set; }//<=> table OrdersDetail
        public DbSet<ItemAdv> Adv { get; set; }//<=> table Adv
        public DbSet<ItemTag> Tags { get; set; }
        public DbSet<ItemTagProduct> TagsProducts { get; set; }
        public DbSet<ItemSlide> Slides { get; set; }//<=> table Slides
        public DbSet<ItemBlog> Blogs { get; set; }//<=> table Slides       
    }
}
