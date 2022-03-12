using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;
using System.Globalization;

namespace Business.Services
{
    public interface IUrunService : IService<UrunModel, Urun, ETicaretContext>
    {

    }

    public class UrunService : IUrunService
    {
        public RepositoryBase<Urun, ETicaretContext> Repository { get; set; } = new Repository<Urun, ETicaretContext>();

        public IQueryable<UrunModel> Query()
        {
            return Repository.EntityQuery("Kategori").OrderBy(u => u.Kategori.Adi).ThenBy(u => u.Adi).Select(u => new UrunModel()
            {
                Id = u.Id,
                Adi = u.Adi,
                Aciklamasi = u.Aciklamasi,
                BirimFiyati = u.BirimFiyati,
                SonKullanmaTarihi = u.SonKullanmaTarihi,
                StokMiktari = u.StokMiktari,
                KategoriId = u.KategoriId,

                // eğer ürün model üzerinden bir kategorinin adı dışında diğer özellikleri (Id, Aciklamasi, vb.) de kullanılmak isteniyorsa bu şekilde modelde referans tanımlanabilir ve bu referans new'lenerek set edilebilir.
                // ürün model üzerinden KategoriDisplay kullanımı (genelde view'de): urunModel.KategoriDisplay.Id, urunModel.KategoriDisplay.Adi, urunModel.KategoriDisplay.Aciklamasi
                //KategoriDisplay = new KategoriModel()
                //{
                //    Id = u.Kategori.Id,
                //    Adi = u.Kategori.Adi,
                //    Aciklamasi = u.Kategori.Aciklamasi
                //},
                KategoriAdiDisplay = u.Kategori.Adi,

                // https://docs.microsoft.com/tr-tr/dotnet/standard/base-types/standard-numeric-format-strings
                //BirimFiyatiDisplay = u.BirimFiyati.ToString("C2", new CultureInfo("tr-TR"))
                // İngilizce bölgesel ayar için: en-US, sadece tarih ve ondalık veri tipleri için CultureInfo kullanılmalı,
                // ~/Program.cs içersinde tüm uygulama için tek seferde AppCore üzerinden tanımlanıp kullanılabilir.
                BirimFiyatiDisplay = u.BirimFiyati.ToString("C2"), // Para birimi formatı

                //SonKullanmaTarihiDisplay = u.SonKullanmaTarihi.HasValue ? u.SonKullanmaTarihi.Value.ToString("dd.MM.yyyy") : "" // Türkçe tarih formatı
                //SonKullanmaTarihiDisplay = u.SonKullanmaTarihi.HasValue ? u.SonKullanmaTarihi.Value.ToShortDateString() : "" // Kısa tarih formatı
                SonKullanmaTarihiDisplay = u.SonKullanmaTarihi.HasValue ? u.SonKullanmaTarihi.Value.ToString("yyyy-MM-dd") : "" // SQL tarih formatı
            });
        }

        public Result Add(UrunModel model)
        {
            if (Repository.EntityQuery().Any(u => u.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz ürün adına sahip kayıt bulunmaktadır!");
            Urun entity = new Urun()
            {
                Adi = model.Adi.Trim(),
                Aciklamasi = model.Aciklamasi?.Trim(),
                BirimFiyati = model.BirimFiyati,
                StokMiktari = model.StokMiktari,
                SonKullanmaTarihi = model.SonKullanmaTarihi,
                KategoriId = model.KategoriId
            };
            Repository.Add(entity);
            return new SuccessResult("Ürün başarıyla eklendi.");
        }

        public Result Update(UrunModel model)
        {
            throw new NotImplementedException();
        }

        public Result Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
