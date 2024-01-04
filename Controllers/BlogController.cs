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
    public class BlogController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {            
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 9;
            //lấy tất cả các bản ghi trong table Products
            List<ItemBlog> list_record = db.Blogs.OrderByDescending(item => item.Id).ToList();
            //Có thể tiếp tục truy vấn tiếp kết quả đã lấy về

            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));
        }
        public IActionResult Detail(int? id)
        {
            int _id = id ?? 0;
            ItemBlog record = db.Blogs.Where(item => item.Id == _id).FirstOrDefault();
           
            return View("Detail", record);
        }
    }
}
