using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;  
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Service;
using Mango.Services.OrderAPI.Utility;
using Mango.Services.OrderAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //send the connection string of your database or define it in appsettings and give the path
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
//finally we will use automapper using DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

//This will add httpclient for the product with the base address 
builder.Services.AddHttpClient("Product", u => u.BaseAddress =
new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "enter the bearer authorization string as follows : `Bearer Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
        {
            Reference=new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id=JwtBearerDefaults.AuthenticationScheme
            }
        },new string[]{}
        }
    });
});

builder.AddAppAuthentication();

//now add the authorization here
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//retrieve the value of secret key and assign that to the API key
//another method to do it is GetValue in GetSection
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();  

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();   //called it here before app starts everytime
app.Run();

void ApplyMigration()
{
    //I want appdbcontext service here and check if there are any pending migrations
    using (var scope = app.Services.CreateScope())
    {
        //this will basically get all the services here but we only want appdbcontext here so
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //on that appdbcontext we will check if pending migration count > 0
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();  //this will automatically apply all the pending migration in the database
        }
    }
}