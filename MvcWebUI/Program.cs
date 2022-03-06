//class Program
//{
//    static void Main(string[] args)
//    {
//        // kodlar
//    }
//}

// kodlar:
using Business.Services;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Repositories;
using DataAccess.Repositories.Bases;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // MVC web uygulamasý

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

//builder.Services.AddScoped<KategoriRepositoryBase, KategoriRepository>();
#endregion

builder.Services.AddScoped<IKategoriService, KategoriService>();
builder.Services.AddScoped<IUrunService, UrunService>();
#endregion

var app = builder.Build();

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

app.UseAuthorization(); // sen iþlem için yetkili misin?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
