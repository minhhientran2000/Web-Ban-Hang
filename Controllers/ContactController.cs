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
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;
using MailKit.Security;

namespace Project.Controllers
{
    public class ContactController : Controller
    {
        
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("customer_email")))
            {
                
                /*ViewBag.email = db.Customers.Where(item => item.Email == (HttpContext.Session.GetString("customer_email")).Select(item.Email));
                ViewBag.name = db.Customers.First().Name;*/
                List<ItemCustomers> cus = db.Customers.OrderByDescending(item => item.Id).ToList();
                foreach(var item in cus)
                {
                    if (item.Email == HttpContext.Session.GetString("customer_email"))
                    {
                        ViewBag.email = item.Email;
                        ViewBag.name = item.Name;
                        ViewBag.phone = item.Phone;
                    }
                }
                return View("Index", cus);
            }
            return View("Index");
        }
        [HttpPost]
        public IActionResult SendMail(string mess)
        {
            List<ItemCustomers> cus = db.Customers.OrderByDescending(item => item.Id).ToList();
            foreach (var item in cus)
            {
                if (item.Email == HttpContext.Session.GetString("customer_email"))
                {
                    ViewBag.email = item.Email;
                    ViewBag.name = item.Name;
                    ViewBag.phone = item.Phone;
                }
            }
            string body = mess;
            var message = new MimeMessage();        
           /* message.From.Add(MailboxAddress.Parse(ViewBag.email));*/
            message.From.Add(new MailboxAddress("Noreply my site", "anjapan12345@gmail.com"));
            message.To.Add(MailboxAddress.Parse("anjapan12345@gmail.com"));          
            message.Subject = "Contact email";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "<h4>Hi, my name is: "+ViewBag.name+"</h4>" + "<br/>" + "<h4>My phone: " + ViewBag.phone + "</h4>" + "<br/>" + "<h4>My email: " +ViewBag.Email + "</h4>" + "<br/>" +body };
            using var client= new SmtpClient();
           /* client.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            client.Authenticate("brisa.pfeffer@ethereal.email", "yyVPMc7w7PaEuMxBSc");*/
            client.Connect("smtp.gmail.com");
            client.Authenticate("anjapan12345@gmail.com", "uxmaiejumuozjsqb");
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();           
            return View("Index");
        }
    }
}
