using Microsoft.AspNetCore.Mvc;

//thao tac voi IFormCollection
using Microsoft.AspNetCore.Http;
//Doi tuong ma hoa Password
using BC = BCrypt.Net.BCrypt;
//de nhin thay cac  class trong Models
using Project.Models;
using X.PagedList;
using System.Collections.Generic;
//de nhin thay file checkLogin tring thu muc Attributes
using Project.Areas.Admin.Attributes;
using System.Security.Cryptography;

namespace Project.Areas.Admin.Controllers
{
    //Khai bao route Admin de nhan dienurl day la Area aDmin
    [Area("Admin")]
    //Goi Atributes CheckLogin de kiem tra dang nhap
    [CheckLogin]
    public class UsersController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //int? page -> neu co gia tri truyen vao ham thi page = giatri
            //Neu ko co gia tri truyen vao ham thi Page = null

            //Lay trang hien tai
            int current_page = page ?? 1;
            int record_per_page = 3;
            //Lay tat ca cac ban ghi trong Users
            List<ItemUser> list_record = db.Users.OrderByDescending(item => item.Id).ToList();
            //truyen gia tri ra View phan trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));
        }
        //mac dinh trang thai trang la GET
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemUser record = db.Users.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Users/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        //Khi an nut submit thi trang thai cua tranbg la POST
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            //cũng có thể sử dụng đối tượng Request.Form["ten the form"]
            string _password = Request.Form["password"].ToString().Trim();
            //lấy một bản ghi
            ItemUser record = db.Users.Where(item => item.Id == id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _name;
                //nếu password không rỗng thì update password
                if (!String.IsNullOrEmpty(_password))
                {
                    //mã hóa password
                    _password = BC.HashPassword(_password);
                    record.Password = _password;
                }
            }
            //cập nhật lại table
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");


        }
        public IActionResult Create(int id)
        {
            ViewBag.action = "/Admin/Users/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            //lay du lieu thong qua doi tuong fc
            string _name = fc["name"].ToString().Trim();
            string _email = fc["email"].ToString().Trim();
            //co the su dung doi tuong Request.Form
            string _password = Request.Form["password"].ToString().Trim();
            //lay mot ban ghi
            _password = BC.HashPassword(_password);
            //lay mot ban ghi
            ItemUser record = new ItemUser();

            record.Name = _name;
            record.Email = _email;
            record.Password = _password;
            db.Users.Add(record);
            //cap nhaat table
            db.SaveChanges();
            //di chuyen den action co ten la Index
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemUser record = db.Users.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.Users.Remove(record);
            //cap nhat lai table Users
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
    }
}
