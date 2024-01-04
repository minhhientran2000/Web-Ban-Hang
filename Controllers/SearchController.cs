 using Microsoft.AspNetCore.Mvc;
//thao tac voi IFormCollection
using Microsoft.AspNetCore.Http;
//doi tuong ma hoa password -> co the gan vao mot bien de su dung bien nay
using BC = BCrypt.Net.BCrypt;
//de nhin thay cac class trong folder Models thi phai using dong duoi
using Project.Models;
//doi tuong phan trang
using X.PagedList;
//su dung kieu List
using System.Collections.Generic;
//de nhin thay file CheckLogin.cs trong thu muc Attributes
using Project.Areas.Admin.Attributes;
using System.Security.Cryptography;
using System.Data;//sử dụng cho các đối tượng: DataTable, SqlConnection, DataAdapter, DataCommand...
using Microsoft.Data.SqlClient;
namespace Project.Controllers
{
    public class SearchController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult SearchPrice(int? page)
        {
            //---
            //lấy biến truyền từ url
            double fromPrice = !String.IsNullOrEmpty(Request.Query["fromPrice"]) ? Convert.ToDouble(Request.Query["fromPrice"]) : 0;
            double toPrice = !String.IsNullOrEmpty(Request.Query["toPrice"]) ? Convert.ToDouble(Request.Query["toPrice"]) : 0;
            //---
            //truyền biến CategoryId ra View
            ViewBag.fromPrice = fromPrice;
            ViewBag.toPrice = toPrice;
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 6;
            //lấy tất cả các bản ghi trong table Products
            List<ItemProduct> list_record = db.Products.Where(item => (item.Price - (item.Price * item.Discount) / 100) >= fromPrice && (item.Price - (item.Price * item.Discount) / 100) <= toPrice).OrderByDescending(item => item.Id).ToList();
            //---
            //truyền giá trị ra view có phân trang
            return View("SearchPrice", list_record.ToPagedList(current_page, record_per_page));

        }
        public IActionResult Tag(int? page, int? id)
        {
            //---
            int _id = id ?? 0;
            //---
            //truyền biến CategoryId ra View
            ViewBag._id = id;
            //---
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 6;
            //lấy tất cả các bản ghi trong table Products
            List<ItemProduct> list_record = (from tag in db.Tags join tag_product in db.TagsProducts on tag.Id equals tag_product.TagId join product in db.Products on tag_product.ProductId equals product.Id where tag.Id == _id select product).ToList();
            //---
            //truyền giá trị ra view có phân trang
            return View("SearchTag", list_record.ToPagedList(current_page, record_per_page));
        }
        public IActionResult SearchName(int? page)
        {
            //---
            //lấy biến truyền từ url
            string key = !String.IsNullOrEmpty(Request.Query["key"]) ? Request.Query["key"] : "";

            //---
            //truyền biến CategoryId ra View
            ViewBag.key = key;

            //return Content(CategoryId.ToString());
            //---
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 6;
            //lấy tất cả các bản ghi trong table Products
            List<ItemProduct> list_record = db.Products.Where(item => item.Name.Contains(key) || item.Description.Contains(key) || item.Content.Contains(key)).OrderByDescending(item => item.Id).ToList();
            //---
            //truyền giá trị ra view có phân trang
            return View("SearchName", list_record.ToPagedList(current_page, record_per_page));
        }
        public string Ajax()
        {
            //lấy biến truyền từ url
            string key = !String.IsNullOrEmpty(Request.Query["key"]) ? Request.Query["key"] : "";
            //lấy tất cả các bản ghi trong table Products
            List<ItemProduct> list_record = db.Products.Where(item => item.Name.Contains(key) || item.Description.Contains(key) || item.Content.Contains(key)).OrderByDescending(item => item.Id).ToList();
            //---
            string str = "";
            foreach (var item in list_record)
                str = str + "<li><a href='/Products/Detail/" + item.Id + "'><img src='/Upload/Products/" + item.Photo + "'>" + item.Name + "</a></li>";
            return str;
        }
    }
}
