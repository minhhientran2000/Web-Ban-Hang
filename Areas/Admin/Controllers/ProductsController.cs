using Microsoft.AspNetCore.Mvc;
//thao tac voi IFormCollection
using Microsoft.AspNetCore.Http;
//doi tuong ma hoa password -> co the gan vao mot bien de su dung bien nay
using BC = BCrypt.Net.BCrypt;
//de nhin thay cac class trong folder Models thi phai using dong duoi
using Project.Models;
//doi tuong phan trang
using X.PagedList;
//su dung kieu List
using System.Collections.Generic;
//de nhin thay file CheckLogin.cs trong thu muc Attributes
using Project.Areas.Admin.Attributes;
using System.Security.Cryptography;
using System.Data;//sử dụng cho các đối tượng: DataTable, SqlConnection, DataAdapter, DataCommand...
using Microsoft.Data.SqlClient;


namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class ProductsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //lấy trang hiện tại
            int current_page = page ?? 1;
            //định nghĩa số bản ghi trên một trang
            int record_per_page = 20;
            //lấy tất cả các bản ghi trong table Products
            List<ItemProduct> list_record = db.Products.OrderByDescending(item => item.Id).ToList();
            //truyền giá trị ra view có phân trang
            return View("Index", list_record.ToPagedList(current_page, record_per_page));
        }
        //mặc định trạng thái của trang là GET
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Products/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        //khi an nut submit thi trang thai la POST
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            int _id = id ?? 0;
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            double _price = Convert.ToDouble(fc["price"].ToString().Trim());
            double _discount = Convert.ToDouble(fc["discount"].ToString().Trim());
            int _hot = fc["hot"] != "" && fc["hot"] == "on" ? 1 : 0;
            string _description = fc["description"].ToString().Trim();
            string _content = fc["content"].ToString().Trim();
            //lấy một bản ghi
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _name;
                record.Price = _price;
                record.Discount = _discount;
                record.Hot = _hot;
                record.Description = _description;
                record.Content = _content;
                //lay thong tin cua file
                string _file_name = "";
                try
                {
                    //lay ten file
                    _file_name = Request.Form.Files[0].FileName;
                }
                catch {; }
                if (!string.IsNullOrEmpty(_file_name))
                {
                    //upload anh moi
                    var timestamp = DateTime.Now.ToFileTime();
                    _file_name = timestamp + "_" + _file_name;
                    //lay duong dan cua file
                    string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", _file_name);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    //update gia tri vao cot Photo trong csdl
                    record.Photo = _file_name;
                    //cập nhật lại table
                    db.SaveChanges();
                }
                db.SaveChanges();

                //CategoryProduct
                //liệt kê các checkbox để lấy giá trị insert vào table CategoriesProducts
                List<ItemCategory> list_categories = db.Categories.ToList();
                //xóa các bản ghi trong table CategoriesProducts có id tương ứng với sản phẩm này (trước khi insert dữ liệu mới)
                List<ItemCategoriesProducts> list_category_product = db.CategoriesProducts.Where(item => item.ProductId == _id).ToList();
                foreach (var item in list_category_product)
                {
                    db.CategoriesProducts.Remove(item);
                }
                db.SaveChanges();
                foreach(var itemCategory in list_categories)
                {
                    string formName = "category_" + itemCategory.Id;
                    if (!String.IsNullOrEmpty(Request.Form[formName]))
                    {
                        ItemCategoriesProducts recordCategoryProduct = new ItemCategoriesProducts();
                        recordCategoryProduct.CategoryId = itemCategory.Id;
                        recordCategoryProduct.ProductId = _id;
                        //thêm bản ghi vào table
                        db.CategoriesProducts.Add(recordCategoryProduct);
                        db.SaveChanges();
                    }
                }


                //TagProduct
                List<ItemTagProduct> list_tag_product = db.TagsProducts.Where(item => item.ProductId == _id).ToList();
                foreach (var item_tag_product in list_tag_product)
                {
                    db.TagsProducts.Remove(item_tag_product);
                }
                db.SaveChanges();
                //--

                //lay cac gia tri da select o select box
                List<string> list_id_tags = Request.Form["tags"].ToList();
                foreach (var tag_id in list_id_tags)
                {
                    ItemTagProduct record_tag_product = new ItemTagProduct();
                    record_tag_product.TagId = Convert.ToInt32(tag_id);
                    record_tag_product.ProductId = _id;
                    db.TagsProducts.Add(record_tag_product);
                    db.SaveChanges();
                }

            }
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Products/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            //lấy dữ liệu của thẻ form thông qua đối tượng fc
            string _name = fc["name"].ToString().Trim();
            double _price = Convert.ToDouble(fc["price"].ToString().Trim());
            double _discount = Convert.ToDouble(fc["discount"].ToString().Trim());
            int _hot = fc["hot"] != "" && fc["hot"] == "on" ? 1 : 0;
            string _description = fc["description"].ToString().Trim();
            string _content = fc["content"].ToString().Trim();
            //lay mot ban ghi
            ItemProduct record = new ItemProduct();
            record.Name = _name;
            record.Price = _price;
            record.Discount = _discount;
            record.Hot = _hot;
            record.Description = _description;
            record.Content = _content;
            //lay thong tin cua file
            string _file_name = "";
            try
            {
                //lay ten file
                _file_name = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!string.IsNullOrEmpty(_file_name))
            {
                //upload anh moi
                var timestamp = DateTime.Now.ToFileTime();
                _file_name = timestamp + "_" + _file_name;
                //lay duong dan cua file
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", _file_name);
                //upload file
                using (var stream = new FileStream(_Path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Photo trong csdl
                record.Photo = _file_name;
                //cập nhật lại table
                db.SaveChanges();
            }
            //them ban ghi vao table Products
            db.Products.Add(record);
            db.SaveChanges();

            //CategoryProdyct
            int insert_id = record.Id;
            List<ItemCategory> list = db.Categories.ToList();
            foreach (var itemCate in list)
            {
                string formName = "category_" + itemCate.Id;
                if (!string.IsNullOrEmpty(Request.Form[formName]))
                {
                    ItemCategoriesProducts recordCateProduct = new ItemCategoriesProducts();
                    recordCateProduct.CategoryId = itemCate.Id;
                    recordCateProduct.ProductId = insert_id;
                    db.CategoriesProducts.Add(recordCateProduct);
                    db.SaveChanges();
                }
            }


            //TagProduct
            List<string> lst_id_tags = Request.Form["tags"].ToList();
            foreach (var tag_id in lst_id_tags)
            {
                ItemTagProduct record_tag_product = new ItemTagProduct();
                record_tag_product.TagId = Convert.ToInt32(tag_id);
                record_tag_product.ProductId = insert_id;
                db.TagsProducts.Add(record_tag_product);
                db.SaveChanges();
            }





            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
        //xoa ban ghi
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.Products.Remove(record);
            //cap nhat lai table Products
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Index");
        }
    }
}
