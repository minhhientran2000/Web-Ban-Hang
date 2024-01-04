using Microsoft.AspNetCore.Mvc;
//su dung doi tuong thao tac IFormCollection
using Microsoft.AspNetCore.Http;
//nhin thay cac file .cs ben trong folder Models
using Project.Models;
//su dung entity framework
using Microsoft.EntityFrameworkCore;
//phan trang
using X.PagedList;
//nhin thay file CheckLogin.cs tron thu muc Attributes
using Project.Areas.Admin.Attributes;
//doi tuong thao tac file
using System.IO;
//su dung kieu List
using System.Collections.Generic;
//doi tuong ma hoa password
using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace project.Controllers
{
    public class WishlistController : Controller
    {
        //khoi tao doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            string json_wishlist = "";
            List<ItemProduct> products = new List<ItemProduct>();
            //lấy chuỗi json, convert sang kiểu List
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("wishlist")))
            {
                json_wishlist = HttpContext.Session.GetString("wishlist");
                products = JsonConvert.DeserializeObject<List<ItemProduct>>(json_wishlist);
            }
            //return Content(products.Count.ToString());
            return View("Index",products);
        }
        //thêm sản phẩm vào wishlist
        public IActionResult Add(int? id)
        {
            int _id = id ?? 0;
            //lấy sản phẩm tương ứng với id
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            //---
            string json_wishlist = "";
            List<ItemProduct> products = new List<ItemProduct>();
            //lấy chuỗi json, convert sang kiểu List
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("wishlist")))
            {
                json_wishlist = HttpContext.Session.GetString("wishlist");
                products = JsonConvert.DeserializeObject<List<ItemProduct>>(json_wishlist);
                //kiểm tra xem product đã có trong danh sách chưa, nếu chưa thì thêm mới
                var check = products.Where(item => item.Id == record.Id).Count();
                if (check == 0)
                    products.Add(record);
            } 
            else
            {
                products.Add(record);
            }    
            //convert lại thành chuỗi json để lưu vào session
            json_wishlist = JsonConvert.SerializeObject(products);
            //set thông tin vào chuỗi session wishlist
            HttpContext.Session.SetString("wishlist",json_wishlist);
            //---
            return Redirect("/Wishlist");
        }
        public IActionResult Remove(int? id)
        {
            int _id = id ?? 0;
            //lấy sản phẩm tương ứng với id
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            //---
            string json_wishlist = "";
            List<ItemProduct> products = new List<ItemProduct>();
            //lấy chuỗi json, convert sang kiểu List
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("wishlist")))
            {
                json_wishlist = HttpContext.Session.GetString("wishlist");
                products = JsonConvert.DeserializeObject<List<ItemProduct>>(json_wishlist);
            }
            //xóa sản phẩm khỏi danh sách
            //---
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Id == id)
                {
                    products.RemoveAt(i);
                }
            }            
            //---
            //convert lại thành chuỗi json để lưu vào session
            json_wishlist = JsonConvert.SerializeObject(products);
            //set thông tin vào chuỗi session wishlist
            HttpContext.Session.SetString("wishlist", json_wishlist);
            //---
            return Redirect("/Wishlist");
        }
    }
}
