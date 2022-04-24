using AppCore.Business.Models.JsonWebToken;
using AppCore.Business.Utils.JsonWebToken;
using AppCore.Business.Utils.JsonWebToken.Bases;
using AppCore.DataAccess.Configs;
using AppCore.MvcWebUI.Utils;
using AppCore.MvcWebUI.Utils.Bases;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebApi",
        Description = "A Web API for E-Ticaret Core",
        //TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Çaðýl Alsaç",
            Email = string.Empty
            //Url = new Uri("https://www.cagilalsac.com")
        },
        License = new OpenApiLicense
        {
            Name = "Free License"
            //Url = new Uri("https://example.com/license")
        }
    });

    // Swagger üzerinden Authorization yapabilmek için eklendi.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});
#endregion

#region CORS (Cross Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder
            .AllowAnyOrigin() // Her kaynaktan gelen isteklere yanýt ver, kaynaklara örnek https://sahibinden.com, https://hepsiburada.com, vb. Mesela origin için baþka methodlar ile sadece sahibinden.com üzerinden gelen isteklere yanýt verilmesi ayarlanabilir.
            .AllowAnyHeader() // Ýsteklerin (request) gövdesi (body) dýþýnda baþlýk (head) içerisinde gönderilen ekstra veriler, örneðin Authorize, Content-Type, vb. Mesela header'lar için baþka methodlar ile sadece Content-Type header'ýna izin verilebilir. 
            .AllowAnyMethod() // Method'lar: get, post, put, delete, vb. Ýsteklerdeki bütün method'lara izin ver. Mesela method'lar için builder üzerinden baþka methodlar ile sadece get header'ýna yanýt verilmesi saðlanabilir.
    );
});
#endregion

#region JWT (Json Web Token)
var appSettingsUtil = new AppSettingsUtil(builder.Configuration);

//var jwtOptions = appSettingsUtil.Bind<JwtOptions>("JwtOptions");
var jwtOptions = appSettingsUtil.Bind<JwtOptions>(nameof(JwtOptions));

var jwtUtil = new JwtUtil(appSettingsUtil);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = jwtUtil.CreateSecurityKey(jwtOptions.SecurityKey)
        };
    });
#endregion

// Tüm veritabaný projelerinde kullanýlmak üzere AppCore altýnda ConnectionConfig class'ýný ve içerisine ConnectionString özelliðini oluþturuyoruz
// ve appsettings.json altýndaki ConnectionStrings üzerinden set ediyoruz: 
ConnectionConfig.ConnectionString = builder.Configuration.GetConnectionString("ETicaretContext");

#region IoC Container : Inversion of Control Container (Baðýmlýlýklarýn Yönetimi) 
// Alternatif olarak Autofac ve Ninject gibi kütüphaneler de kullanýlabilir.

// Unable to resolve service hatalarý burada çözümlenmelidir.

// AddScoped: istek (request) boyunca objenin referansýný (genelde interface veya abstract class) kullandýðýmýz yerde obje (somut class'tan oluþturulacak) bir kere oluþturulur ve yanýt (response) dönene kadar bu obje hayatta kalýr.
// AddSingleton: web uygulamasý baþladýðýnda objenin referansný (genelde interface veya abstract class) kullandýðýmýz yerde obje (somut class'tan oluþturulacak) bir kere oluþturulur ve uygulama çalýþtýðý (IIS üzerinden uygulama durdurulmadýðý veya yeniden baþlatýlmadýðý) sürece bu obje hayatta kalýr.
// AddTransient: istek (request) baðýmsýz ihtiyaç olan objenin referansýný (genelde interface veya abstract class) kullandýðýmýz her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullanýlýr.

builder.Services.AddScoped<IUrunService, UrunService>();
builder.Services.AddScoped<IKategoriService, KategoriService>();

builder.Services.AddScoped<IHesapService, HesapService>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();

builder.Services.AddSingleton<JwtUtilBase, JwtUtil>();
builder.Services.AddSingleton<AppSettingsUtilBase, AppSettingsUtil>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    #region Swagger
    //app.UseSwagger();
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });

    //app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
    #endregion
}

app.UseHttpsRedirection();

#region CORS (Cross Origin Resource Sharing)
app.UseCors();
#endregion

#region JWT (Json Web Token)
app.UseAuthentication();
#endregion

app.UseAuthorization();

app.MapControllers();

app.Run();
