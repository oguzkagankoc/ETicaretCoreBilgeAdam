using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework;
using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services
{
    public interface IMagazaService : IService<MagazaModel, Magaza, ETicaretContext>
    {

    }

    public class MagazaService : IMagazaService
    {
        public RepoBase<Magaza, ETicaretContext> Repo { get; set; } = new Repo<Magaza, ETicaretContext>();

        public IQueryable<MagazaModel> Query()
        {
            return Repo.Query().OrderBy(m => m.Adi).Select(m => new MagazaModel()
            {
                Id = m.Id,
                Adi = m.Adi,
                SanalMi = m.SanalMi,
                SanalMiDisplay = m.SanalMi ? "Evet" : "Hayır"
            });
        }

        public Result Add(MagazaModel model)
        {
            if (Repo.Query().Any(m => m.Adi.ToUpper() == model.Adi.ToUpper().Trim()))
                return new ErrorResult("Girdiğiniz mağaza adına sahip kayıt bulunmaktadır!");
            Magaza entity = new Magaza()
            {
                Adi = model.Adi.Trim(),
                SanalMi = model.SanalMi
            };
            Repo.Add(entity);
            return new SuccessResult();
        }

        public Result Update(MagazaModel model)
        {
            if (Repo.Query().Any(m => m.Adi.ToUpper() == model.Adi.ToUpper().Trim() && m.Id != model.Id))
                return new ErrorResult("Girdiğiniz mağaza adına sahip kayıt bulunmaktadır!");
            Magaza entity = Repo.Query(m => m.Id == model.Id).SingleOrDefault();
            entity.Adi = model.Adi.Trim();
            entity.SanalMi = model.SanalMi;
            Repo.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            // önce ürünle ilişkili olan ürün mağaza kayıtları mağaza id'ye göre getirilir ve silinir.
            using (RepoBase<UrunMagaza, ETicaretContext> urunMagazaRepo = new Repo<UrunMagaza, ETicaretContext>())
            {
                // 1. yöntem:
                //List<UrunMagaza> urunMagazaEntities = urunMagazaRepo.Query(um => um.MagazaId == id).ToList();
                //foreach (UrunMagaza urunMagazaEntity in urunMagazaEntities)
                //{
                //    urunMagazaRepo.Delete(urunMagazaEntity, false); // birden çok ürün mağaza kaydı silinebileceği için save parametresini false gönderip veritabanında silme sorgusunu çalıştırmıyoruz, sadece ürün mağaza kaydını silinecek olarak işaretliyoruz.
                //}
                //urunMagazaRepo.Save(); // yukarıda silinecek ürün mağaza kayıtlarını işaretledikten sonra tek seferde veritabanında silme sorgularını çalıştırıyoruz (unit of work).
                // 2. yöntem:
                urunMagazaRepo.Delete(um => um.MagazaId == id);
            }
            Repo.Delete(m => m.Id == id); // son olarak mağazamızı siliyoruz.
            return new SuccessResult();
        }

        public void Dispose()
        {
            Repo.Dispose();
        }
    }
}
