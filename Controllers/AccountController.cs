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

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        //doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        //dang ky thanh vien
        public IActionResult Register()
        {
            return View();
        }
        //khi an nut dang ky
        [HttpPost]
        public IActionResult RegisterPost(IFormCollection fc)
        {
            string _name = fc["name"];
            string _email = fc["email"];
            string _phone = fc["phone"];
            string _address = fc["address"];
            string _password = fc["password"];
            //ma hoa password
            _password = BC.HashPassword(_password);
            //kiem tra xem email da ton tai trong table customers chua, neu chua thi moi cho insert du lieu vao
            int checkMail = db.Customers.Where(item => item.Email == _email).Count();
            if (checkMail == 0)
            {
                ItemCustomers record = new ItemCustomers();
                record.Name = _name;
                record.Email = _email;
                record.Phone = _phone;
                record.Address = _address;
                record.Password = _password;
                //---
                db.Customers.Add(record);
                db.SaveChanges();
            }
            else
            {
                return Redirect("/Account/Register?notify=exists");
            }
            return Redirect("/Account/Login?notify=register-success");
        }
        //dang nhap
        public IActionResult Login()
        {
            return View();
        }
        //khi an nut dang nhap
        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _email = fc["email"];
            string _password = fc["password"];  
            ItemCustomers record = db.Customers.Where(item => item.Email == _email).FirstOrDefault();
            if (record != null && BC.Verify(_password, record.Password) == true)
            {
                //gan bien session de luu du lieu
                HttpContext.Session.SetString("customer_email", record.Email.ToString());
                HttpContext.Session.SetString("customer_id", record.Id.ToString());
            }
            else
                return Redirect("/Account/Login?notify=login-invalid");
            return Redirect("/Home");
        }
        //dang xuat
        public IActionResult Logout()
        {
            //huy cac bien session
            HttpContext.Session.Remove("customer_email");
            HttpContext.Session.Remove("customer_id");
            return Redirect("/Account/Login");
        }
    }
}
