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
builder.Services.AddControllersWithViews(); // MVC web uygulamas�

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
    app.UseExceptionHandler("/Home/Hata"); // hatalar i�in bizim olu�turdu�umuz aksiyon

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS olarak y�nlendirir

app.UseStaticFiles(); // ~/wwwroot klas�r� alt�ndaki css, js, html ve di�er dosyalar�n kullan�lmas�n� sa�lar

app.UseRouting(); // a�a��daki default olarak belirlenen ve ekstra eklenebilecek route kullan�mlar�n� sa�lar

app.UseAuthorization(); // sen i�lem i�in yetkili misin?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
