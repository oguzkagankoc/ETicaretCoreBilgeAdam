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

// B�lgesel ayar�n t�m uygulama i�in ayarlanmas�, e�er parametresiz CultureInfo constuctor'� kullan�l�rsa tr-TR �zerinden ayarlan�r.
// �nce cultureUtil objesi new'lenir, daha sonra AddCulture methoduyla builder servisleri konfig�re edilir, son olarak da a�a��da oldu�u gibi app.UseRequestLocalization methodu i�inde UseCulture methodu �a�r�l�r.
CultureUtilBase cultureUtil = new CultureUtil(); // �ngilizce i�in parametre en-US g�nderilmelidir.
builder.Services.Configure(cultureUtil.AddCulture());

// Add services to the container.
builder.Services.AddControllersWithViews(); // MVC web uygulamas�

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

#region IoC Container : Inversion of Control Container (Ba��ml�l�klar�n Y�netimi) 
// Alternatif olarak Autofac ve Ninject gibi k�t�phaneler de kullan�labilir.

// Unable to resolve service hatalar� burada ��z�mlenmelidir.

// AddScoped: istek (request) boyunca objenin referans�n� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak) bir kere olu�turulur ve yan�t (response) d�nene kadar bu obje hayatta kal�r.
// AddSingleton: web uygulamas� ba�lad���nda objenin referansn� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak) bir kere olu�turulur ve uygulama �al��t��� (IIS �zerinden uygulama durdurulmad��� veya yeniden ba�lat�lmad���) s�rece bu obje hayatta kal�r.
// AddTransient: istek (request) ba��ms�z ihtiya� olan objenin referans�n� (genelde interface veya abstract class) kulland���m�z her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullan�l�r.

#region 1) IoC Container �zerinden servisler d���ndaki t�m ba��ml�l�klar�n y�netimi
// Bunun i�in �ncellikle ETicaretContext tipi AddDbContext ile belirtilmeli, daha sonra da her entity i�in bir repository base ile repository olu�turulup a�a��daki �ekilde ba��ml�l�klar� y�netilmelidir.
// Bu i�lem do�ru ancak zahmetli oldu�u i�in repository ve ETicaretContext ba��ml�l�klar�n� burada y�netmek yerine service'lerde repository'leri default constructor �zerinden new'leyece�iz, dolay�s�yla repository'lerdeki DbContext de new'lenecek. B�ylelikle sadece service ba��ml�l�klar�n� a�a��daki �ekilde tan�mlamam�z yeterli olacakt�r.
//builder.Services.AddDbContext<ETicaretContext>(); // ETicaretContext tipindeki parametre kullan�lan her constructor injection'da ETicaretContext tipinde bir obje new'ler.

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

// ASP.NET Core'da appsettings.json dosyas�nda kendi tan�mlad���m�z b�l�mlerin okunmas�:
//IConfigurationSection section = builder.Configuration.GetSection("AppSettings");
IConfigurationSection section = builder.Configuration.GetSection(nameof(AppSettings));
section.Bind(new AppSettings());

var app = builder.Build();

app.UseRequestLocalization(cultureUtil.UseCulture());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error"); // controller/action
    app.UseExceptionHandler("/Home/Hata"); // hatalar i�in bizim olu�turdu�umuz aksiyon

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS olarak y�nlendirir

app.UseStaticFiles(); // ~/wwwroot klas�r� alt�ndaki css, js, html ve di�er dosyalar�n kullan�lmas�n� sa�lar

app.UseRouting(); // a�a��daki default olarak belirlenen ve ekstra eklenebilecek route kullan�mlar�n� sa�lar

#region Authentication
app.UseAuthentication(); // sen kimsin?
#endregion

app.UseAuthorization(); // sen i�lem i�in yetkili misin?

// �ehirler AJAX i�lemi i�in route tan�m� 1. y�ntem, 2. y�ntem SehirlerAjaxController.cs alt�ndad�r.
// default d���nda tan�mlanan t�m yeni route'lar default route tan�m� �zerine yaz�lmal�d�r, s�ra �nemlidir.
//app.MapControllerRoute(
//    name: "sehirlerAjax",
//    pattern: "SehirlerAjax/SehirlerGet/{ulkeId?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
