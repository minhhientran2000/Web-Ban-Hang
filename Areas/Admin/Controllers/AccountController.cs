using Microsoft.AspNetCore.Mvc;

//thao tac voi IFormCollection
using Microsoft.AspNetCore.Http;
//Doi tuong ma hoa Password
using BC = BCrypt.Net.BCrypt;
//de nhin thay cac  class trong Models
using Project.Models;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();

        public IActionResult Login()
        {
            return View("Login");
        }
        //sau khi an nut submit
        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            //ham trim de loai bo khoang trang ben trai va phai cua chuoi

            //lay gia tri cua email tu form kh nhap vao
            string _email = fc["email"].ToString().Trim();
            //lay gia tri cua password tu form
            string _password = fc["password"].ToString().Trim();

            //lay mot ban ghi tuong ung voi email truyen vao
            //Users : anh xa den Users trong MyDbContext.cs
            ItemUser record = db.Users.Where(item => item.Email == _email).FirstOrDefault();
            if (record != null)
            {
                if (BC.Verify(_password, record.Password) == true)
                {
                    //khoi tao bien Session
                    HttpContext.Session.SetString("admin_email", _email);
                    //lay id cua record nay
                    HttpContext.Session.SetString("admin_id", record.Id.ToString());
                    //di chuyen den url /admin/Home
                    return Redirect("/Admin/Home");
                }
            }
            return Redirect("/Admin/Account/Login?notify=invalid");
        }
        //dang xuat
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_email");
            return Redirect("/Admin/Account/Login");
        }

    }
}
