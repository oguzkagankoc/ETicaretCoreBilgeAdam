using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using Business.Models.Filters;
using Business.Models.Reports;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public interface IUrunService : IService<UrunModel, Urun, ETicaretContext>
    {
        Task<Result<List<UrunRaporModel>>> RaporGetirAsync(UrunRaporFilterModel filtre);
    }

    public class UrunService : IUrunService
    {
        public RepoBase<Urun, ETicaretContext> Repo { get; set; } // Ürün repository

        private readonly RepoBase<UrunMagaza, ETicaretContext> _urunMagazaRepo;
        private readonly RepoBase<UrunSiparis, ETicaretContext> _urunSiparisRepo;

        private readonly RepoBase<Kategori, ETicaretContext> _kategoriRepo;
        private readonly RepoBase<Magaza, ETicaretContext> _magazaRepo;

        public UrunService()
        {
            ETicaretContext eTicaretContext = new ETicaretContext();
            Repo = new Repo<Urun, ETicaretContext>(eTicaretContext);
            _urunMagazaRepo = new Repo<UrunMagaza, ETicaretContext>(eTicaretContext);
            _urunSiparisRepo = new Repo<UrunSiparis, ETicaretContext>(eTicaretContext);

            _kategoriRepo = new Repo<Kategori, ETicaretContext>(eTicaretContext);
            _magazaRepo = new Repo<Magaza, ETicaretContext>(eTicaretContext);
        }

        public IQueryable<UrunModel> Query()
        {
            return Repo.Query("Kategori", "UrunMagazalar").OrderBy(u => u.Kategori.Adi).ThenBy(u => u.Adi).Select(u => new UrunModel()
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
                //BirimFiyatiDisplay = u.BirimFiyati.ToString("C2", new CultureInfo("tr-TR")) // Türkçe para birimi formatı
                // İngilizce bölgesel ayar için: en-US, sadece tarih ve ondalık veri tipleri için CultureInfo kullanılmalı,
                // ~/Program.cs içersinde tüm uygulama için tek seferde AppCore üzerinden tanımlanıp kullanılabilir.
                BirimFiyatiDisplay = u.BirimFiyati.ToString("C2"), // Bölgesel ayara göre para birimi formatı

                //SonKullanmaTarihiDisplay = u.SonKullanmaTarihi.HasValue ? u.SonKullanmaTarihi.Value.ToString("dd.MM.yyyy") : "" // Türkçe tarih formatı
                //SonKullanmaTarihiDisplay = u.SonKullanmaTarihi.HasValue ? u.SonKullanmaTarihi.Value.ToShortDateString() : "" // Kısa tarih formatı
                SonKullanmaTarihiDisplay = u.SonKullanmaTarihi.HasValue ? u.SonKullanmaTarihi.Value.ToString("yyyy-MM-dd") : "", // SQL tarih formatı, tarihin doğru bir şekilde sıralanması için

                MagazaIdleri = u.UrunMagazalar.Select(um => um.MagazaId).ToList(),
                MagazalarDisplay = u.UrunMagazalar.Select(um => um.Magaza.Adi + " (" + (um.Magaza.SanalMi ? "Sanal Mağaza" : "Gerçek Mağaza") + ")").ToList(),

                ImajDosyaUzantisi = u.ImajDosyaUzantisi, // .jpg
                ImajDosyaYoluDisplay = string.IsNullOrWhiteSpace(u.ImajDosyaUzantisi) ? null : "/dosyalar/urunler/" + u.Id + u.ImajDosyaUzantisi // /dosyalar/urunler/1.jpg
            });
        }

        public Result Add(UrunModel model)
        {
            if (Repo.Query().Any(u => u.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz ürün adına sahip kayıt bulunmaktadır!");
            Urun entity = new Urun()
            {
                Adi = model.Adi.Trim(),
                Aciklamasi = model.Aciklamasi?.Trim(),
                BirimFiyati = model.BirimFiyati.Value,
                StokMiktari = model.StokMiktari.Value,
                SonKullanmaTarihi = model.SonKullanmaTarihi,
                KategoriId = model.KategoriId.Value,

                UrunMagazalar = model.MagazaIdleri?.Select(mId => new UrunMagaza()
                {
                    MagazaId = mId
                }).ToList(),

                ImajDosyaUzantisi = model.ImajDosyaUzantisi
            };
            Repo.Add(entity);

            model.Id = entity.Id; // Entity Framework insert işleminden sonra tabloda oluşan Id'yi entity'de referans parametresi olduğu için günceller ve biz de bu Id'yi bu methoda gelen model referans parametresi üzerinde set edip methodu çağırdığımız yerde kullanabiliriz.

            return new SuccessResult("Ürün başarıyla eklendi.");
        }

        public Result Update(UrunModel model)
        {
            if (Repo.Query().Any(u => u.Adi.ToUpper() == model.Adi.ToUpper().Trim() && u.Id != model.Id))
                return new ErrorResult("Girdiğiniz ürün adına sahip kayıt bulunmaktadır!");

            Urun entity = Repo.Query(u => u.Id == model.Id, "UrunMagazalar").SingleOrDefault();
            
            // önce eğer ürüne bağlı ilişkili ürün mağaza kayıtları varsa onları siliyoruz
            foreach (UrunMagaza urunMagazaEntity in entity.UrunMagazalar)
            {
                _urunMagazaRepo.Delete(urunMagazaEntity, false);
            }

            entity.Adi = model.Adi.Trim();
            entity.Aciklamasi = model.Aciklamasi?.Trim();
            entity.BirimFiyati = model.BirimFiyati.Value;
            entity.StokMiktari = model.StokMiktari.Value;
            entity.SonKullanmaTarihi = model.SonKullanmaTarihi;
            entity.KategoriId = model.KategoriId.Value;

            // sonra ürün model üzerinden kullanıcının seçtiği mağazalara göre ilişkili ürün mağaza entity'lerini oluşturuyoruz
            entity.UrunMagazalar = model.MagazaIdleri?.Select(mId => new UrunMagaza()
            {
                MagazaId = mId
            }).ToList();

            entity.ImajDosyaUzantisi = model.ImajDosyaUzantisi;

            Repo.Update(entity);
            return new SuccessResult("Ürün başarıyla güncellendi.");
        }

        public Result Delete(int id)
        {
            Urun urun = Repo.Query(u => u.Id == id).SingleOrDefault();

            // önce eğer ürüne bağlı ilişkili sipariş kayıtları varsa onları siliyoruz
            _urunSiparisRepo.Delete(us => us.UrunId == id, false);

            // daha sonra eğer ürüne bağlı ilişkili ürün mağaza kayıtları varsa onları siliyoruz
            _urunMagazaRepo.Delete(um => um.UrunId == id, false);

            // en son ürünü siliyoruz
            Repo.Delete(u => u.Id == id);

            return new SuccessResult("Ürün başarıyla silindi.");
        }

        public void Dispose()
        {
            Repo.Dispose();
        }

        /*
        select k.Aciklamasi KategoriAciklamasi, k.Adi KategoriAdi, k.Id KategoriId,
        u.SonKullanmaTarihi UrunSonKullanmaTarihi, u.Aciklamasi UrunAciklamasi,
        u.Adi UrunAdi, u.StokMiktari UrunStokMiktari, u.BirimFiyati UrunBirimFiyati,
        m.Adi MagazaAdi, m.Id MagazaId
        from ETicaretUrunler u 
        left outer join ETicaretKategoriler k
        on u.KategoriId = k.Id
        left outer join ETicaretUrunMagazalar um
        on u.Id = um.UrunId
        left outer join ETicaretMagazalar m
        on um.MagazaId = m.Id
        */
        public async Task<Result<List<UrunRaporModel>>> RaporGetirAsync(UrunRaporFilterModel filtre) // asenkron metodlar mutlaka async Task dönmeli ve içinde çağrılan asenkron metodla birlikte await kullanılmalı!
        {
            List<UrunRaporModel> list;

            #region Select Sorgusu
            var urunQuery = Repo.Query();
            var kategoriQuery = _kategoriRepo.Query();
            var urunMagazaQuery = _urunMagazaRepo.Query();
            var magazaQuery = _magazaRepo.Query();

            var query = from urun in urunQuery
                        join kategori in kategoriQuery
                        on urun.KategoriId equals kategori.Id into kategoriler
                        from subKategoriler in kategoriler.DefaultIfEmpty()
                        join urunMagaza in urunMagazaQuery
                        on urun.Id equals urunMagaza.UrunId into urunMagazalar
                        from subUrunMagazalar in urunMagazalar.DefaultIfEmpty()
                        join magaza in magazaQuery
                        on subUrunMagazalar.MagazaId equals magaza.Id into magazalar
                        from subMagazalar in magazalar.DefaultIfEmpty()
                        //orderby subKategoriler.Adi, urun.Adi
                        //where subKategoriler.Id == 7
                        select new UrunRaporModel()
                        {
                            KategoriAciklamasi = subKategoriler.Aciklamasi,
                            KategoriAdi = subKategoriler.Adi,
                            KategoriId = subKategoriler.Id,
                            MagazaAdi = subMagazalar.Adi,
                            MagazaId = subMagazalar.Id,
                            UrunAciklamasi = urun.Aciklamasi,
                            UrunAdi = urun.Adi,
                            UrunBirimFiyatiDisplay = urun.BirimFiyati.ToString("C2"),
                            UrunSonKullanmaTarihiDisplay = urun.SonKullanmaTarihi.HasValue ? urun.SonKullanmaTarihi.Value.ToShortDateString() : "",
                            UrunStokMiktari = urun.StokMiktari
                        };
            #endregion

            #region Filtre
            //if (filtre.KategoriId != null)
            if (filtre.KategoriId.HasValue)
                query = query.Where(q => q.KategoriId == filtre.KategoriId.Value);
            #endregion

            list = await query.ToListAsync();
            if (list.Count == 0)
                return new ErrorResult<List<UrunRaporModel>>("Kayıt bulunamadı!");
            return new SuccessResult<List<UrunRaporModel>>(list.Count + " kayıt bulundu.", list);
        }
    }
}
