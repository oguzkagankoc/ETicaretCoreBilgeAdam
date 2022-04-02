//class Program
//{
//    static void Main(string[] args)
//    {
//        // kodlar
//    }
//}

// kodlar:
using AppCore.DataAccess.Configs;
using AppCore.MvcWebUI.Utils;
using AppCore.MvcWebUI.Utils.Bases;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Authentication.Cookies;
using MvcWebUI.Settings;

var builder = WebApplication.CreateBuilder(args);

// Bölgesel ayarýn tüm uygulama için ayarlanmasý, eðer parametresiz CultureInfo constuctor'ý kullanýlýrsa tr-TR üzerinden ayarlanýr.
// Önce cultureUtil objesi new'lenir, daha sonra AddCulture methoduyla builder servisleri konfigüre edilir, son olarak da aþaðýda olduðu gibi app.UseRequestLocalization methodu içinde UseCulture methodu çaðrýlýr.
CultureUtilBase cultureUtil = new CultureUtil(); // Ýngilizce için parametre en-US gönderilmelidir.
builder.Services.Configure(cultureUtil.AddCulture());

// Add services to the container.
builder.Services.AddControllersWithViews(); // MVC web uygulamasý

#region Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(config =>
    {
        config.LoginPath = "/Hesaplar/Giris";
        config.AccessDeniedPath = "/Hesaplar/YetkisizIslem";
        config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        config.SlidingExpiration = true;
    });
#endregion

ConnectionConfig.ConnectionString = builder.Configuration.GetConnectionString("ETicaretContext");

#region IoC Container : Inversion of Control Container (Baðýmlýlýklarýn Yönetimi) 
// Alternatif olarak Autofac ve Ninject gibi kütüphaneler de kullanýlabilir.

// Unable to resolve service hatalarý burada çözümlenmelidir.

// AddScoped: istek (request) boyunca objenin referansýný (genelde interface veya abstract class) kullandýðýmýz yerde obje (somut class'tan oluþturulacak) bir kere oluþturulur ve yanýt (response) dönene kadar bu obje hayatta kalýr.
// AddSingleton: web uygulamasý baþladýðýnda objenin referansný (genelde interface veya abstract class) kullandýðýmýz yerde obje (somut class'tan oluþturulacak) bir kere oluþturulur ve uygulama çalýþtýðý (IIS üzerinden uygulama durdurulmadýðý veya yeniden baþlatýlmadýðý) sürece bu obje hayatta kalýr.
// AddTransient: istek (request) baðýmsýz ihtiyaç olan objenin referansýný (genelde interface veya abstract class) kullandýðýmýz her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullanýlýr.

#region 1) IoC Container üzerinden servisler dýþýndaki tüm baðýmlýlýklarýn yönetimi
// Bunun için öncellikle ETicaretContext tipi AddDbContext ile belirtilmeli, daha sonra da her entity için bir repository base ile repository oluþturulup aþaðýdaki þekilde baðýmlýlýklarý yönetilmelidir.
// Bu iþlem doðru ancak zahmetli olduðu için repository ve ETicaretContext baðýmlýlýklarýný burada yönetmek yerine service'lerde repository'leri default constructor üzerinden new'leyeceðiz, dolayýsýyla repository'lerdeki DbContext de new'lenecek. Böylelikle sadece service baðýmlýlýklarýný aþaðýdaki þekilde tanýmlamamýz yeterli olacaktýr.
//builder.Services.AddDbContext<ETicaretContext>(); // ETicaretContext tipindeki parametre kullanýlan her constructor injection'da ETicaretContext tipinde bir obje new'ler.

//builder.Services.AddScoped<KategoriRepoBase, KategoriRepo>();
#endregion

builder.Services.AddScoped<IKategoriService, KategoriService>();
builder.Services.AddScoped<IUrunService, UrunService>();
builder.Services.AddScoped<IMagazaService, MagazaService>();
builder.Services.AddScoped<ISehirService, SehirService>();
builder.Services.AddScoped<IUlkeService, UlkeService>(); 
builder.Services.AddScoped<IHesapService, HesapService>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<IRolService, RolService>();
#endregion

// ASP.NET Core'da appsettings.json dosyasýnda kendi tanýmladýðýmýz bölümlerin okunmasý:
//IConfigurationSection section = builder.Configuration.GetSection("AppSettings");
IConfigurationSection section = builder.Configuration.GetSection(nameof(AppSettings));
section.Bind(new AppSettings());

var app = builder.Build();

app.UseRequestLocalization(cultureUtil.UseCulture());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error"); // controller/action
    app.UseExceptionHandler("/Home/Hata"); // hatalar için bizim oluþturduðumuz aksiyon

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS olarak yönlendirir

app.UseStaticFiles(); // ~/wwwroot klasörü altýndaki css, js, html ve diðer dosyalarýn kullanýlmasýný saðlar

app.UseRouting(); // aþaðýdaki default olarak belirlenen ve ekstra eklenebilecek route kullanýmlarýný saðlar

#region Authentication
app.UseAuthentication(); // sen kimsin?
#endregion

app.UseAuthorization(); // sen iþlem için yetkili misin?

// Þehirler AJAX iþlemi için route tanýmý 1. yöntem, 2. yöntem SehirlerAjaxController.cs altýndadýr.
// default dýþýnda tanýmlanan tüm yeni route'lar default route tanýmý üzerine yazýlmalýdýr, sýra önemlidir.
//app.MapControllerRoute(
//    name: "sehirlerAjax",
//    pattern: "SehirlerAjax/SehirlerGet/{ulkeId?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
