using AppCore.Business.Models.Results;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı
    public class KategoriService : IKategoriService
    {
        // 2
        public RepoBase<Kategori, ETicaretContext> Repo { get; set; } = new Repo<Kategori, ETicaretContext>();

        // 1
        //public KategoriService(KategoriRepoBase repo) // Repository'nin servise enjekte edilmesi. Ancak bu projede repository'yi new'leyerek kullanağız. Dolayısıyla repository'lerdeki DbContext de new'lenecek.
        //{
        //    Repo = repo;
        //}

        // Sadece örneğin select Id, Adi, Aciklamasi from Kategoriler order by Adi sorgusunu oluşturur.
        public IQueryable<KategoriModel> Query()
        {
            // Entity ve model özellik verileri atama işlemleri AutoMapper third-party kütüphanesi üzerinden yapılabilir: https://automapper.org/
            //IQueryable<KategoriModel> query = Repo.Query().OrderBy(kategori => kategori.Adi).Select(kategori => new KategoriModel()
            IQueryable<KategoriModel> query = Repo.Query("Urunler").OrderBy(kategori => kategori.Adi).Select(kategori => new KategoriModel()
            {
                Id = kategori.Id,
                Adi = kategori.Adi,
                Aciklamasi = kategori.Aciklamasi,
                UrunSayisiDisplay = kategori.Urunler.Count
            });
            return query;
        }

        public Result Add(KategoriModel model)
        {
            #region Validasyon İşlemleri
            if (string.IsNullOrWhiteSpace(model.Adi))
                return new ErrorResult("Kategori adı boş olamaz!");
            if (model.Adi.Length > 100)
                return new ErrorResult("Kategori adı en fazla 100 karakter olmalıdır!");
            if (!string.IsNullOrWhiteSpace(model.Aciklamasi) && model.Aciklamasi.Length > 1000)
                return new ErrorResult("Kategori açıklaması en fazla 1000 karakter olmalıdır!");

            //Kategori existingEntity = Repo.EntityQuery().SingleOrDefault(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim());
            //Kategori existingEntity = Repo.EntityQuery(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim()).SingleOrDefault();
            //if (existingEntity != null)
            //    return new ErrorResult("Girdiğiniz kategori adına sahip kayıt bulunmaktadır!");
            if (Repo.Query().Any(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz kategori adına sahip kayıt bulunmaktadır!");
            #endregion

            #region Yeni Kayıt Ekleme İşlemi
            Kategori newEntity = new Kategori()
            {
                Adi = model.Adi.Trim(),

                //Aciklamasi = model.Aciklamasi != null ? model.Aciklamasi.Trim() : null, // Ternary Operator
                Aciklamasi = model.Aciklamasi?.Trim() // eğer null bir referansın bir özelliği veya methodu kullanılıyorsa o zaman mutlaka referansın (Aciklamasi) sonuna ? eklenmelidir. Eğer Aciklamasi girildiyse değerini Trim'le ve entity'de set et, girilmediyse (null ise) entity'de null olarak set et.
            };
            Repo.Add(newEntity);
            #endregion

            return new SuccessResult();
        }

        public Result Update(KategoriModel model)
        {
            // güncelleme işlemlerinde güncellenen kayıt dışında koşulunun belirtilmesi gerektiğinden mutlaka Id üzerinden koşul eklenmelidir.
            if (Repo.Query().Any(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim() && k.Id != model.Id))
                return new ErrorResult("Girdiğiniz kategori adına sahip kayıt bulunmaktadır!");
            Kategori entity = Repo.Query(k => k.Id == model.Id).SingleOrDefault();
            if (entity == null)
                return new ErrorResult("Kategori kaydı bulunamadı!");
            entity.Adi = model.Adi.Trim();
            entity.Aciklamasi = model.Aciklamasi?.Trim();
            Repo.Update(entity);
            return new SuccessResult("Kategori başarıyla güncellendi.");
        }

        public Result Delete(int id)
        {
            Kategori entity = Repo.Query(k => k.Id == id, "Urunler").SingleOrDefault();
            if (entity.Urunler != null && entity.Urunler.Count > 0) // bu id'ye sahip kategorinin ürünleri varsa
            {
                return new ErrorResult("Silinmek istenen kategoriye ait ürünler bulunmaktadır!");
            }
            Repo.Delete(k => k.Id == id); 
            return new SuccessResult("Kategori başarıyla silindi.");
        }

        public void Dispose()
        {
            Repo.Dispose();
        }
    }
}
