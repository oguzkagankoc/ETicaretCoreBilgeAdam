using AppCore.Business.Services.Bases;
using Business.Models;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace Business.Services.Bases
{
    // Program.cs'de IoC Container'da kullanabilmek için oluşturulmalı
    public interface IKategoriService : IService<KategoriModel, Kategori, ETicaretContext>
    {

    }
}
