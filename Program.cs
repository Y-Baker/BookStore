using BookStore.Configuration;
using BookStore.Models;
using BookStore.UnitOfWorks;
using BookStore.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

const string corsPolicy = "AllowAll";

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BookStoreContext>(op => op.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SQL-Server")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<BookStoreContext>();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddSingleton<JWT>();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Book Store",
        Version = "v1",
        Description = "RESTFul Api For Book Store",
        TermsOfService = new Uri("https://github.com/Y-Baker"),
        Contact = new OpenApiContact()
        {
            Email = "yuossefbakier@gmail.com",
            Name = "Yousef Bakier"
        }
    });

    option.EnableAnnotations();
});


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op => 
    {
        op.SaveToken = true;
        #region secret key
        string key = "Arsenal -> North London Forever -> JOO";
        SymmetricSecurityKey? secertkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        #endregion
        op.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = secertkey,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddCors(e => e.AddPolicy(corsPolicy, p =>
{
    p.AllowAnyOrigin();
    p.AllowAnyMethod();
    p.AllowAnyHeader();
}));

WebApplication? app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
