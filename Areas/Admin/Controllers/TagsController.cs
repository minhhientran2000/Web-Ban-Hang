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
using System.Data;  //su dung cho cac doi tuong Datatable, sqlConection,./...
using Microsoft.Data.SqlClient;


namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    //Goi Atributes CheckLogin de kiem tra dang nhap
    [CheckLogin]
    public class TagsController : Controller
    {
        public MyDbContext db = new MyDbContext();
     

        public IActionResult Index(int? page)
        {
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 4;
            //lấy tất cả các bản ghi trong table Products
            List<ItemTag> list_record = db.Tags.OrderByDescending(item => item.Id).ToList();
            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));

        }
        
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemTag record = db.Tags.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Tags/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        //Khi an nut submit thi trang thai Post
        
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            //lấy một bản ghi
            ItemTag record = db.Tags.Where(item => item.Id == _id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _name;              
                db.SaveChanges();
               
            }
            
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        
        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Tags/CreatePost";
            return View("CreateUpdate");
        }
        //khi ấn nút submit thì trang sẽ ở trạng thái POST
        //url: /Admin/Tags/UpdatePost/Id
        //ở trạng thái POST thì phải khai báo tag sau
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            
            //lay mot ban ghi
            ItemTag record = new ItemTag();
            record.Name = _name;
           
            
            //them ban ghi vao table Products
            db.Tags.Add(record);
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemTag record = db.Tags.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.Tags.Remove(record);
            //cap nhat lai table Products
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }

    }
}

