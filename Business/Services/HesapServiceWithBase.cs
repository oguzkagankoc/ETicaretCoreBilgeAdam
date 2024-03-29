﻿using AppCore.Business.Models.Results;
using Business.Enums;
using Business.Models;

namespace Business.Services
{
    public interface IHesapService
    {
        Result<KullaniciModel> Giris(KullaniciGirisModel model);
        Result Kayit(KullaniciKayitModel model);
    }

    public class HesapService : IHesapService
    {
        // readonly değişkenler aşağıdaki gibi new'lenebileceği gibi constructor'da da new'lenebilir.
        //private readonly RepositoryBase<Kullanici, ETicaretContext> _repository = new Repository<Kullanici, ETicaretContext>();
        //private readonly RepositoryBase<Kullanici, ETicaretContext> _repository;

        //public HesapService()
        //{
        //    _repository = new Repository<Kullanici, ETicaretContext>();
        //}

        // hesap servisi kullanıcı işlemleri için direkt kullanıcı repository'sini kullanmak yerine kullanıcı CRUD işlemlerini yapan kullanıcı servisini kullanmalıdır
        private readonly IKullaniciService _kullaniciService;

        public HesapService(IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        public Result<KullaniciModel> Giris(KullaniciGirisModel model)
        {
            KullaniciModel kullanici = _kullaniciService.Query().SingleOrDefault(k => k.KullaniciAdi == model.KullaniciAdi && k.Sifre == model.Sifre && k.AktifMi);
            if (kullanici == null)
                return new ErrorResult<KullaniciModel>("Geçersiz kullanıcı adı ve şifre!");
            return new SuccessResult<KullaniciModel>(kullanici);
        }

        public Result Kayit(KullaniciKayitModel model)
        {
            var kullanici = new KullaniciModel()
            {
                AktifMi = true, // yeni kaydolan kullanıcılar aktif olarak kaydedilmektedir

                // rol id'nin direkt integer olarak atanması doğru değildir çünkü veritabanında rol tablosundaki veriler dolayısıyla id'ler değişebilir
                //model.RolId = 2; 
                RolId = (int)Rol.Kullanıcı, // yeni kaydolan kullanıcılar Kullanıcı Rolü'nde kaydedilmektedir

                KullaniciAdi = model.KullaniciAdi,
                Sifre = model.Sifre,
                KullaniciDetayi = new KullaniciDetayiModel()
                {
                    Adres = model.KullaniciDetayi.Adres.Trim(),
                    Cinsiyet = model.KullaniciDetayi.Cinsiyet,
                    Eposta = model.KullaniciDetayi.Eposta.Trim(),
                    SehirId = model.KullaniciDetayi.SehirId,
                    UlkeId = model.KullaniciDetayi.UlkeId
                }
            };
            return _kullaniciService.Add(kullanici);
        }
    }
}
