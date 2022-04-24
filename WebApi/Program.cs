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
            Name = "�a��l Alsa�",
            Email = string.Empty
            //Url = new Uri("https://www.cagilalsac.com")
        },
        License = new OpenApiLicense
        {
            Name = "Free License"
            //Url = new Uri("https://example.com/license")
        }
    });

    // Swagger �zerinden Authorization yapabilmek i�in eklendi.
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
            .AllowAnyOrigin() // Her kaynaktan gelen isteklere yan�t ver, kaynaklara �rnek https://sahibinden.com, https://hepsiburada.com, vb. Mesela origin i�in ba�ka methodlar ile sadece sahibinden.com �zerinden gelen isteklere yan�t verilmesi ayarlanabilir.
            .AllowAnyHeader() // �steklerin (request) g�vdesi (body) d���nda ba�l�k (head) i�erisinde g�nderilen ekstra veriler, �rne�in Authorize, Content-Type, vb. Mesela header'lar i�in ba�ka methodlar ile sadece Content-Type header'�na izin verilebilir. 
            .AllowAnyMethod() // Method'lar: get, post, put, delete, vb. �steklerdeki b�t�n method'lara izin ver. Mesela method'lar i�in builder �zerinden ba�ka methodlar ile sadece get header'�na yan�t verilmesi sa�lanabilir.
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

// T�m veritaban� projelerinde kullan�lmak �zere AppCore alt�nda ConnectionConfig class'�n� ve i�erisine ConnectionString �zelli�ini olu�turuyoruz
// ve appsettings.json alt�ndaki ConnectionStrings �zerinden set ediyoruz: 
ConnectionConfig.ConnectionString = builder.Configuration.GetConnectionString("ETicaretContext");

#region IoC Container : Inversion of Control Container (Ba��ml�l�klar�n Y�netimi) 
// Alternatif olarak Autofac ve Ninject gibi k�t�phaneler de kullan�labilir.

// Unable to resolve service hatalar� burada ��z�mlenmelidir.

// AddScoped: istek (request) boyunca objenin referans�n� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak) bir kere olu�turulur ve yan�t (response) d�nene kadar bu obje hayatta kal�r.
// AddSingleton: web uygulamas� ba�lad���nda objenin referansn� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak) bir kere olu�turulur ve uygulama �al��t��� (IIS �zerinden uygulama durdurulmad��� veya yeniden ba�lat�lmad���) s�rece bu obje hayatta kal�r.
// AddTransient: istek (request) ba��ms�z ihtiya� olan objenin referans�n� (genelde interface veya abstract class) kulland���m�z her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullan�l�r.

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
