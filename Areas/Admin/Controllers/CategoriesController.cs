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
    public class CategoriesController : Controller
    {
        public MyDbContext db = new MyDbContext();
        //Tao bien toan cuc doc cac thong so tu file appsetting.son
        public IConfiguration configuration;

        //ham tao la ham mac dinh duoc goi khi claass dc khoi tao
        public CategoriesController(IConfiguration _configuration)
        {
            //su dung dog sau de gam gia tri vao bien toan cuc
            configuration = _configuration; //tu day co the doc duoc cac tham so cua appsetting.json

        }

        public IActionResult Index(int? page)
        {
            //tao do ituong dataTable
            DataTable  dtCategories = new DataTable();
            //Lay chuoi ket noi nam trong appsetting
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            //tu kha using dung de thuc hien brn trong khoi using do, ket thuc thi khoi lenh ben trong se huy
            using(SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //thuc hien truy van, traa ket qua ve bien object
                SqlDataAdapter da = new SqlDataAdapter("select * from categories where ParentId = 0 order by Id desc", conn);
                da.Fill(dtCategories); //dubng sde do du lieu vao DataTable ten la DbCategories
            }
            int current_page = page ?? 1;
            //Dinh nghia so ban ghi tren 1 tranfg
            int record_per_page = 4;
            //do du lieu tu DataTAble co ten dtCategories vao list de phan trang
            List<ItemCategory> listCategories= new List<ItemCategory>();
            foreach (DataRow item in dtCategories.Rows)
            {
                listCategories.Add(new ItemCategory()
                {
                    Id = Convert.ToInt32(item["ID"].ToString()),
                    Name = item["Name"].ToString(),
                    ParentId = Convert.ToInt32(item["ParentId"].ToString())
                });
            }
            return View("Index", listCategories.ToPagedList(current_page, record_per_page));
        }
        //url : /Admin/Categories/Update/id
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //tao dt DataTable
            DataTable dtCategories = new DataTable();
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //thuc hien truy van, traa ket qua ve bien object
                SqlDataAdapter da = new SqlDataAdapter("select * from categories where Id = " + _id, conn);
                da.Fill(dtCategories); //dubng sde do du lieu vao DataTable ten la DbCategories
            }
            DataRow itemCategory = dtCategories.NewRow();
            if (dtCategories.Rows.Count > 0)
            {
                itemCategory = dtCategories.Rows[0];
            }
            //--
            //Liet ke danh muc de triuyen ra View
            int _CurrentId = 0;
            if (itemCategory != null)
            {
                _CurrentId = Convert.ToInt32(itemCategory["ID"]);

            }
            ViewBag.listCategory = db.Categories.Where(item => item.ParentId == 0 && item.Id != _CurrentId).OrderByDescending(item => item.Id).ToList();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/UpdatePost/" + _id;

            // truyen 1 DataRow => 1 hang trong DataTable ra view
            return View("CreateUpdate", itemCategory);
            //--
        }
        //Khi an nut submit thi trang thai Post
        //url : /ADMIN/Categories/UpdaePost/Id
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            string _name = fc["name"].ToString().Trim();
            string _parent_id = fc["parent_id"].ToString().Trim();
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //thuc hien truy van, traa ket qua ve bien object
                conn.Open();
                SqlCommand cmd = new SqlCommand("update Categories set Name=@var_name, ParentId=@var_parent_id where Id=@var_id", conn);
                cmd.Parameters.AddWithValue("@var_name", _name);
                cmd.Parameters.AddWithValue("@var_parent_id", _parent_id);
                cmd.Parameters.AddWithValue("@var_id", _id);
                //thuc thi
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        //url: /Admin/Categories/Create
        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }
        //khi ấn nút submit thì trang sẽ ở trạng thái POST
        //url: /Admin/Categories/UpdatePost/Id
        //ở trạng thái POST thì phải khai báo tag sau
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();
            string _parent_id = fc["parent_id"].ToString().Trim();
            //lay chuoi ket noi -> chuoi nay nam trong file appsettings.json
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //update, delete, insert thì phải open đối tượng kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into Categories(Name, ParentId) values (@var_name,@var_parent_id)", conn);
                cmd.Parameters.AddWithValue("@var_name", _name);
                cmd.Parameters.AddWithValue("@var_parent_id", _parent_id);
                //thực thi truy cấn
                cmd.ExecuteNonQuery();
            }
            //di chuyển đến một url khác
            return Redirect("/Admin/Categories");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay chuoi ket noi -> chuoi nay nam trong file appsettings.json
            string strDbConnectString = configuration.GetConnectionString("DbConnectString").ToString();
            using (SqlConnection conn = new SqlConnection(strDbConnectString))
            {
                //update, delete, insert thì phải open đối tượng kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from Categories where Id=@var_id or ParentId=@var_id", conn);
                cmd.Parameters.AddWithValue("@var_id", _id);
                //thực thi truy cấn
                cmd.ExecuteNonQuery();
            }
            //di chuyển đến một url khác
            return Redirect("/Admin/Categories");
        }

    }
}
