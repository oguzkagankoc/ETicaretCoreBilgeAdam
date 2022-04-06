1) Entity'ler oluşturulur.
2) Microsoft.EntityFrameworkCore.SqlServer ile Microsoft.EntityFrameworkCore.Tools paketleri NuGet'ten indirilir.
3) DbContext'ten türeyen Context ve içerisindeki DbSet'ler oluşturulur.
4) Context içerisindeki override edilen OnConfiguring methodunda connection string 
server=.\\SQLEXPRESS;database=BA_MoviesCore;trusted_connection=true; formatta yazılır.
5) Tools -> NuGet Package Manager -> Package Manager Console açılır ve önce add-migration v1 daha sonra 
update-database komutları çalıştırılır.
6) İstenirse ilk verileri oluşturmak için Database gibi bir controller oluşturulup içerisine Seed gibi bir action yazılarak
veritabanında ilk verilerin oluşturulması sağlanabilir.
7) Entity model dönüşümlerini gerçekleştirecek servis class'ları önce interface üzerinden methodlar tanımlanarak oluşturulur.
Tanımlanabilecek methodlar CRUD işlemlerine karşılık gelecek Query, Add, Update ve Delete methodlarıdır.
Bu aşamada entity'lere karşılık model'ler de oluşturulmalıdır.
8) appsettings.json ve istenirse appsettings.Development.json içerisine ConnectionStrings altına projenin veritabanı bağlantı bilgisi yazılır. 
Program.cs altında builder.Configuration.GetConnectionString methodu kullanılarak bağlantı bilgisi AppCore altındaki static
ConnectionConfig class'ının static ConnectionString özelliğine set edilir. Daha sonra ConnectionConfig.ConnectionString özelliği context 
class'ının OnConfiguring methodunda UseSqlServer methoduna parametre olarak gönderilir.
9) Program.cs altında IoC Container içerisinde service interface'leri için servis class'ları tanımları yapılır.
10) İlgili model için Controller oluşturulur, dependency injection için ilgili servisin interface'i tipinde parametreli 
constructor yazılır, daha sonra Index, Details, Create, Edit ve Delete aksiyonları yazılır.
11) Bu aksiyonlar sonucunda ilgili view'lar oluşturulur.
12) Bazı controller aksiyon'larını çağırabilmek için view'larda veya layout view'ında link'ler yazılır.
13) View'larda yapılan değişikliklerin proje çalışırken tarayıcıdan sayfanın yenilenmesi durumunda sayfaya yansıması için
NuGet'ten Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation paketi indirilir ve projede Properties -> launchSettings.json
dosyasına "ASPNETCORE_ENVIRONMENT" altına "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation"
tüm profiller için eklenir.
14) İstenirse launchSettings.json'daki profiles altında IIS Express development (DEV), MvcWebUI production (PROD) olarak ayarlanabilir.

Yapı:
View <-> Controller (Başlangıç noktası) <-> Service (model) <-> Repository (entity -> context -> veritabanı)

HTML - CSS - Javascript örnekleri projede wwwroot -> HTML_Javascript_CSS_Intro klasörü altındadır.

Ders içi proje geliştirme aşamaları:
1) DatabaseController
1.1) Seed Action -> İlk verileri doldurma, DbContext objesinin new'lenerek kullanılması, _Layout.cshtml'de HTML ve Tag helper'lar üzerinden link oluşturulması
2) KategorilerController -> Servislerin constructor üzerinden enjekte edilmesi, action'lardan IActionResult tipi üzerinden farklı tiplerin dönülmesi 
2.1) Index Action -> Aksiyondan Index yerine farklı isimde bir view'ın dönülmesi, View'da Model tanımı ve Razor kullanımı
2.2) Create Action -> (OlusturGetir, OlusturGonder): Sadece HTML içeren view'ın kullanılması, HTML form üzerinden input verilerinin aksiyonda parametre olarak kullanılması 
2.3) Edit Action -> ScaffoldingTemplates altındaki dosyaların ReadMe.txt ile belirtilen klasörlere kopayalanması, Http Status Code'ları, Edit view'ının scaffolding ile oluşturulması ve view'ın modele göre değiştirilmesi, Tag ile HTML helper'lar, set edilen ViewData["Title"]'ın _Layout.cshtml'de title'da kullanılması, ViewData, ViewBag ve TempData kullanımı, ModelState ile model data annotation'larının validasyonu
2.4) Delete Action -> Silinmek istenen kaydın ilişkili kayıtları var mı kontrolü, varsa kaydın silinmemesi, kayıt silinirse silindi (IsDeleted = true) olarak güncellenmesi
2.5) Details Action -> Sayfanın ihtiyacı olan ekstra özelliklerin model'de tanımlanması, servis'te set edilmesi ve view'da gösterilmesi
3) UrunlerController -> Entity Framework üzerinden scaffolding ile controller ve view'ların oluşuturlması ve bunların servislere göre düzenlenmesi
3.1) Index Action -> Web uygulamasının bölgesel ayarının sunucu bölgesel ayarından bağımsız Program.cs'de ayarlanması, third party js-css kütüphane kullanımı, bazı verilerin modeldeki Display ile biten özellik adları üzerinden formatlanarak gösterimi 
3.2) Details Action -> Partial view kullanımı
3.3) Create Action -> Verilerin kendi özellik adları ve veri tipleri üzerinden formdan alınması, aksiyonda ihtiyaç duyulan servislerin enjekte edilmesi, SelectList kullanımı
3.4) Edit Action -> Html DropDownListFor helper kullanımı
3.5) Delete Action -> ActionName filter kullanımı
4) HesaplarController -> bir servis içerisine başka bir servisin enjekte edilmesi
4.1) Giris Action -> farklı repository'lere tek bir DbContext enjekte edilmesi, inner join kullanımı, MVC SignIn işlemi, asenkron method kullanımı
4.2) Cikis Action -> MVC SignOut işlemi
4.3) YetkisizIslem Action
4.4) Kayit Action -> service interface'lerine ekstra method tanımı, view'da radio button tag helper ve AJAX kullanımı
5) Program.cs
5.1) AppCore'daki ConnectionConfig.ConnectionString özelliğinin appsettings.json'dan set edilmesi ve DbContext'ten türeyen context class'ında kullanımı
5.2) MVC projesindeki Settings klasörü altındaki AppSettings class'ı üzerinden appsettings.json'daki AppSettings section'ı içerisindeki özelliklerin okunması
6) _Layout.cshtml
6.1) AppSettings class'ındaki özelliklerin kullanımı ile icon'lar için third party js-css kütüphanesinin (FontAwesome) kullanımı
7) SepetController
7.1) Ekle Action -> Kullanıcı ID'sinin Hesaplar controller'ındaki Giris aksiyonu üzerinden claim olarak eklenmesi (type Sid olarak) ve bu claim'in kullanımı, JSON serialize - deserialize işlemleri, Session'a JSON string formatında veri atanması, herhangi bir aksiyona route değeri gönderilerek redirect yapılması, _Sonuc partial view'ında set edilen ViewBag.Sonuc veya TempData["Sonuc"] mesajının sonundaki . veya !'e göre mesajın yeşil veya kırmızı renklendirilerek gösterilmesi 
7.2) Getir Action -> Session'dan JSON string formatında veri alınması, Session'dan alınan verinin sisteme giriş yapmış kullanıcının Id'sine göre filtrelenmesi, bir kolleksiyon üzerinde GroupBy kullanımı, Razor ile view'da sayısal bir özelliğin toplamının hesaplanıp yazdırılması
7.3) Temizle Action -> Session'dan sisteme giriş yapmış kullanıcının Id'sine göre verilerin silinmesi
7.4) Sil Action -> Session'dan kullanıcı ve ürün Id'lerine göre verilerin silinmesi
8) UrunlerController -> Dosya işlemleri eklendi
8.1) Create Action -> Sunucuya fiziksel olarak dosya yükleme ve veritabanında yüklenen dosya yolunun saklanması işlemi