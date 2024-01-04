using Project.Models;
using System.Collections.Generic;
using System.Linq;


namespace Project.MyClass
{
    public class Helper
    {
        public static MyDbContext db = new MyDbContext();
        public static List<ItemAdv> GetAdv(int position)
        {
            List<ItemAdv> list_adv = db.Adv.Where(item => item.Position == position).ToList();
            return list_adv;
        }
        //lay ten san pham
        public static string GetCategoryname(int _CategoryId)
        {
            ItemCategory record = db.Categories.Where(item => item.Id == _CategoryId).FirstOrDefault();
            return record.Name;

        }
    }
}
