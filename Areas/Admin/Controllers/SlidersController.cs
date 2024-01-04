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
    //Khai bao dong sau de nhan biet tren url day la Area Admin
    [Area("Admin")]
    //kiem tra login
    //[CheckLogin]
    public class SlidersController : Controller
    {
        //khai bao bien toan cuc de thao tac csdl (de su dung trong tat ca cac ham)
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //quy dinh so ban ghi tren mot trang
            int _RecordPerPage = 20;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;
            //lay tat ca cac ban ghi trong bang Adv
            List<ItemSlide> _ListRecord = db.Slides.OrderByDescending(item => item.Id).ToList();
            //return Content(_ListRecord.Count.ToString());
            //truyen gia tri ra view co phan trang
            return View("Index", _ListRecord.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemSlide record = db.Slides.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Sliders/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Title = fc["Title"].ToString().Trim();
            string _SubTitle = fc["SubTitle"].ToString().Trim();
            string _Info = fc["Info"].ToString().Trim();
            string _Link = fc["Link"].ToString().Trim();
            //---
            int _id = id ?? 0;
            //lay ban ghi tuong ung voi id truyen vao
            var record = db.Slides.Where(item => item.Id == _id).FirstOrDefault();
            //update ban ghi
            record.Name = _Name;
            record.Title = _Title;
            record.SubTitle = _SubTitle;
            record.Info = _Info;
            record.Link = _Link;
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
                if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo));
                }
                //upload anh moi
                //lay thoi gian gan vao ten file -> tranh cac file co ten trung ten voi file se upload
                var timestap = DateTime.Now.ToFileTime();
                _FileName = timestap + "_" + _FileName;
                //lay duong dan cua file
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Slides", _FileName);
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
            return Redirect("/Admin/Sliders");
        }
        //create
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Sliders/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Title = fc["Title"].ToString().Trim();
            string _SubTitle = fc["SubTitle"].ToString().Trim();
            string _Info = fc["Info"].ToString().Trim();
            string _Link = fc["Link"].ToString().Trim();
            //---
            //lay ban ghi tuong ung voi id truyen vao
            ItemSlide record = new ItemSlide();
            //update ban ghi
            record.Name = _Name;
            record.Title = _Title;
            record.SubTitle = _SubTitle;
            record.Info = _Info;
            record.Link = _Link;
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
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Slides", _FileName);
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
            db.Slides.Add(record);
            //cap nhat ban ghi
            db.SaveChanges();
            //---
            return Redirect("/Admin/Sliders");
        }
        //xoa ban ghi
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay ban ghi tuong ung voi id truyen vao
            var record = db.Slides.Where(item => item.Id == _id).FirstOrDefault();
            //xoa anh
            if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo));
            }
            //xoa ban ghi
            db.Slides.Remove(record);
            //cap nhat csdl
            db.SaveChanges();
            return Redirect("/Admin/Sliders");
        }
    }
}
