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
    public class AdvController : Controller
    {
        public MyDbContext db = new MyDbContext();


        public IActionResult Index(int? page)
        {
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 4;
            //lấy tất cả các bản ghi trong table Products
            List<ItemAdv> list_record = db.Adv.OrderByDescending(item => item.Id).ToList();
            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));

        }

        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemAdv record = db.Adv.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Adv/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        //Khi an nut submit thi trang thai Post

        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            int _Position = Convert.ToInt32(fc["Position"].ToString().Trim());
            //---
            int _id = id ?? 0;
            //lay ban ghi tuong ung voi id truyen vao
            var record = db.Adv.Where(item => item.Id == _id).FirstOrDefault();
            //update ban ghi
            record.Name = _Name;
            record.Position = _Position;
            //---
            //lay thong tin o the file co type="file"
            string _FileName = "";
            try
            {
                //lay ten file
                _FileName = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!String.IsNullOrEmpty(_FileName))
            {
                //xoa anh cu
                if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo));
                }
                //upload anh moi
                //lay thoi gian gan vao ten file -> tranh cac file co ten trung ten voi file se upload
                var timestap = DateTime.Now.ToFileTime();
                _FileName = timestap + "_" + _FileName;
                //lay duong dan cua file
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Adv", _FileName);
                //upload file
                using (var stream = new FileStream(_Path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Photo trong csdl
                record.Photo = _FileName;
            }
            //---
            //cap nhat ban ghi
            db.SaveChanges();
            //---
            return Redirect("/Admin/Adv");
        }

        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Adv/CreatePost";
            return View("CreateUpdate");
        }
        //khi ấn nút submit thì trang sẽ ở trạng thái POST
        //url: /Admin/Tags/UpdatePost/Id
        //ở trạng thái POST thì phải khai báo tag sau
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            int _Position = Convert.ToInt32(fc["Position"].ToString().Trim());
            //---
            //lay ban ghi tuong ung voi id truyen vao
            ItemAdv record = new ItemAdv();
            //update ban ghi
            record.Name = _Name;
            record.Position = _Position;
            //---
            //lay thong tin o the file co type="file"
            string _FileName = "";
            try
            {
                //lay ten file
                _FileName = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!String.IsNullOrEmpty(_FileName))
            {
                //upload anh moi
                //lay thoi gian gan vao ten file -> tranh cac file co ten trung ten voi file se upload
                var timestap = DateTime.Now.ToFileTime();
                _FileName = timestap + "_" + _FileName;
                //lay duong dan cua file
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Adv", _FileName);
                //upload file
                using (var stream = new FileStream(_Path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
            }
            //update gia tri vao cot Photo trong csdl
            record.Photo = _FileName;

            //---
            //tham ban ghi vao csdl
            db.Adv.Add(record);
            //cap nhat ban ghi
            db.SaveChanges();
            //---
            return Redirect("/Admin/Adv");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay ban ghi tuong ung voi id truyen vao
            var record = db.Adv.Where(item => item.Id == _id).FirstOrDefault();
            //xoa anh
            if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Adv", record.Photo));
            }
            //xoa ban ghi
            db.Adv.Remove(record);
            //cap nhat csdl
            db.SaveChanges();
            return Redirect("/Admin/Adv");
        }

    }
}
