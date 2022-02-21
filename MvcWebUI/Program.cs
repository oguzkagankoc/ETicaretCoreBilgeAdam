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
builder.Services.AddControllersWithViews(); // MVC web uygulaması

#region IoC Container : Inversion of Control Container (Bağımlılıkların Yönetimi) 
// Alternatif olarak Autofac ve Ninject gibi kütüphaneler de kullanılabilir.

// Unable to resolve service hataları burada çözümlenmelidir.

// AddScoped: istek (request) boyunca objenin referansını (genelde interface veya abstract class) kullandığımız yerde obje (somut class'tan oluşturulacak) bir kere oluşturulur ve yanıt (response) dönene kadar bu obje hayatta kalır.
// AddSingleton: web uygulaması başladığında objenin referansnı (genelde interface veya abstract class) kullandığımız yerde obje (somut class'tan oluşturulacak) bir kere oluşturulur ve uygulama çalıştığı (IIS üzerinden uygulama durdurulmadığı veya yeniden başlatılmadığı) sürece bu obje hayatta kalır.
// AddTransient: istek (request) bağımsız ihtiyaç olan objenin referansını (genelde interface veya abstract class) kullandığımız her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullanılır.

#region 1) IoC Container üzerinden servisler dışındaki tüm bağımlılıkların yönetimi
// Bunun için öncellikle ETicaretContext tipi AddDbContext ile belirtilmeli, daha sonra da her entity için bir repository base ile repository oluşturulup aşağıdaki şekilde bağımlılıkları yönetilmelidir.
// Bu işlem doğru ancak zahmetli olduğu için repository ve ETicaretContext bağımlılıklarını burada yönetmek yerine service'lerde repository'leri default constructor üzerinden new'leyeceğiz, dolayısıyla repository'lerdeki DbContext de new'lenecek. Böylelikle sadece service bağımlılıklarını aşağıdaki şekilde tanımlamamız yeterli olacaktır.
builder.Services.AddDbContext<ETicaretContext>(); // ETicaretContext tipindeki parametre kullanılan her constructor injection'da ETicaretContext tipinde bir obje new'ler.

builder.Services.AddScoped<KategoriRepositoryBase, KategoriRepository>();
#endregion

builder.Services.AddScoped<IKategoriService, KategoriService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error"); // controller/action
    app.UseExceptionHandler("/Home/Hata"); // hatalar için bizim oluşturduğumuz aksiyon

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS olarak yönlendirir

app.UseStaticFiles(); // ~/wwwroot klasörü altındaki css, js, html ve diğer dosyaların kullanılmasını sağlar

app.UseRouting(); // aşağıdaki default olarak belirlenen ve ekstra eklenebilecek route kullanımlarını sağlar

app.UseAuthorization(); // sen işlem için yetkili misin?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
