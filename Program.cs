var builder = WebApplication.CreateBuilder(args);
/*
    Khai bao de he thong su dung mo hinh mvc
 */
builder.Services.AddControllersWithViews();
//Hoac 
//builder.Services.AddMvc();

//su dung bien session
builder.Services.AddSession();

//----------------------------//
var app = builder.Build();
//app.MapGet("/", () => "Hello World!");
//Khai bao de prj hieu duoc va co the load file trong thu muc wwwroot
app.UseStaticFiles();
//Khai bao de su dung bien Session
app.UseSession();
//Khi tao phan he Area thi phai cau hinh de website nhan biet duoc mvc trong area do
//Cau hinh area phai nam tren cau hinh cua phan he mac dinh
//--
//--
//Cau hinh duong dan
app.MapControllerRoute(name: "area", pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

//
app.Run();


