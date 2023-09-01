using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllers();
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
        if(_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();  //this will automatically apply all the pending migration in the database
        }


    }
}