using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //send the connection string of your database or define it in appsettings and give the path
});
//it basically congigures the class file and using DI we can access the apisettings from appsettings
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

//we need to define dbcontext with dotnetIdentity means we have to use db context to create all the identity related tables, so define it like this
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();  //this will be the bridge bet Appdbcontext and Identity

builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  //this always comes before authorization
app.UseAuthorization();

app.MapControllers();
ApplyMigration();

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