using AppCore.Business.Models.Results;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Bases;

namespace Business.Services
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı
    public class KategoriService : IKategoriService
    {
        // 2
        public RepositoryBase<Kategori, ETicaretContext> Repository { get; set; } = new Repository<Kategori, ETicaretContext>();

        // 1
        //public KategoriService(KategoriRepositoryBase repository) // Repository'nin servise enjekte edilmesi. Ancak bu projede repository'yi new'leyerek kullanağız. Dolayısıyla repository'lerdeki DbContext de new'lenecek.
        //{
        //    Repository = repository;
        //}

        // Sadece örneğin select Id, Adi, Aciklamasi from Kategoriler order by Adi sorgusunu oluşturur.
        public IQueryable<KategoriModel> Query()
        {
            IQueryable<KategoriModel> query = Repository.EntityQuery().OrderBy(kategori => kategori.Adi).Select(kategori => new KategoriModel()
            {
                Id = kategori.Id,
                Adi = kategori.Adi,
                Aciklamasi = kategori.Aciklamasi
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

            //Kategori existingEntity = Repository.EntityQuery().SingleOrDefault(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim());
            //Kategori existingEntity = Repository.EntityQuery(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim()).SingleOrDefault();
            //if (existingEntity != null)
            //    return new ErrorResult("Girdiğiniz kategori adına sahip kayıt bulunmaktadır!");
            if (Repository.EntityQuery().Any(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz kategori adına sahip kayıt bulunmaktadır!");
            #endregion

            #region Yeni Kayıt Ekleme İşlemi
            Kategori newEntity = new Kategori()
            {
                Adi = model.Adi.Trim(),

                //Aciklamasi = model.Aciklamasi != null ? model.Aciklamasi.Trim() : null, // Ternary Operator
                Aciklamasi = model.Aciklamasi?.Trim() // eğer null bir referansın bir özelliği veya methodu kullanılıyorsa o zaman mutlaka referansın (Aciklamasi) sonuna ? eklenmelidir. Eğer Aciklamasi girildiyse değerini Trim'le ve entity'de set et, girilmediyse (null ise) entity'de null olarak set et.
            };
            Repository.Add(newEntity);
            #endregion

            return new SuccessResult();
        }

        public Result Update(KategoriModel model)
        {
            // güncelleme işlemlerinde güncellenen kayıt dışında koşulunun belirtilmesi gerektiğinden mutlaka Id üzerinden koşul eklenmelidir.
            if (Repository.EntityQuery().Any(k => k.Adi.ToUpper() == model.Adi.ToUpper().Trim() && k.Id != model.Id))
                return new ErrorResult("Girdiğiniz kategori adına sahip kayıt bulunmaktadır!");
            Kategori entity = Repository.EntityQuery(k => k.Id == model.Id).SingleOrDefault();
            if (entity == null)
                return new ErrorResult("Kategori kaydı bulunamadı!");
            entity.Adi = model.Adi.Trim();
            entity.Aciklamasi = model.Aciklamasi?.Trim();
            Repository.Update(entity);
            return new SuccessResult("Kategori başarıyla güncellendi.");
        }

        public Result Delete(int id)
        {
            Kategori entity = Repository.EntityQuery(k => k.Id == id, "Urunler").SingleOrDefault();
            if (entity.Urunler != null && entity.Urunler.Count > 0) // bu id'ye sahip kategorinin ürünleri varsa
            {
                return new ErrorResult("Silinmek istenen kategoriye ait ürünler bulunmaktadır!");
            }
            Repository.DeleteEntity(id, true, true); // kaydı veritabanından silmeyecek, silindi olarak güncelleyecek (IsDeleted = true)
            return new SuccessResult("Kategori başarıyla silindi.");
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
