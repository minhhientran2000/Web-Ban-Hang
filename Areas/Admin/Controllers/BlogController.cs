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
    public class BlogController : Controller
    {
        //khai bao bien toan cuc de thao tac csdl (de su dung trong tat ca cac ham)
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //quy dinh so ban ghi tren mot trang
            int _RecordPerPage = 6;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;
            //lay tat ca cac ban ghi trong bang Adv
            List<ItemBlog> _ListRecord = db.Blogs.OrderByDescending(item => item.Id).ToList();
            //return Content(_ListRecord.Count.ToString());
            //truyen gia tri ra view co phan trang
            return View("Index", _ListRecord.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemBlog record = db.Blogs.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Blog/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            
            string _Title = fc["Title"].ToString().Trim();
            //string _CreateAt = fc["Create_at"].ToString().Trim();
            string _UpdateAt = fc["Update_at"].ToString().Trim();
            string _Description = fc["Description"].ToString().Trim();
            string _Content = fc["Content"].ToString().Trim();
            string _Video = fc["Video"].ToString().Trim();
            //---
            int _id = id ?? 0;
            //lay ban ghi tuong ung voi id truyen vao
            var record = db.Blogs.Where(item => item.Id == _id).FirstOrDefault();
            //update ban ghi
            
            record.Title = _Title;
            //record.Create_at = Convert.ToDateTime( _CreateAt);
            record.Update_at = DateTime.Now;
            record.Description = _Description;
            record.Content = _Content;
            record.Video = _Video;
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
                if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Blog", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Blog", record.Photo));
                }
                //upload anh moi
                //lay thoi gian gan vao ten file -> tranh cac file co ten trung ten voi file se upload
                var timestap = DateTime.Now.ToFileTime();
                _FileName = timestap + "_" + _FileName;
                //lay duong dan cua file
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Blog", _FileName);
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
            return Redirect("/Admin/Blog");
        }
        //create
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Blog/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Title = fc["Title"].ToString().Trim();
            string _CreateAt =  fc["Create_at"].ToString().Trim();
            string _UpdateAt = fc["Update_at"].ToString().Trim();
            string _Description = fc["Description"].ToString().Trim();
            string _Content = fc["Content"].ToString().Trim();
            string _Video = fc["Video"].ToString().Trim();
            //---
            //lay ban ghi tuong ung voi id truyen vao
            ItemBlog record = new ItemBlog();
            //update ban ghi
            record.Title = _Title;
            record.Create_at = DateTime.Now;
            record.Update_at = DateTime.Now;
            record.Description = _Description;
            record.Content = _Content;
            record.Video = _Video;
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
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Blog", _FileName);
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
            db.Blogs.Add(record);
            //cap nhat ban ghi
            db.SaveChanges();
            //---
            return Redirect("/Admin/Blog");
        }
        //xoa ban ghi
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay ban ghi tuong ung voi id truyen vao
            var record = db.Blogs.Where(item => item.Id == _id).FirstOrDefault();
            //xoa anh
            if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Blog", record.Photo)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Blog", record.Photo));
            }
            //xoa ban ghi
            db.Blogs.Remove(record);
            //cap nhat csdl
            db.SaveChanges();
            return Redirect("/Admin/Blog");
        }
    }
}
