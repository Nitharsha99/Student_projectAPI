using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Student_projectAPI.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString"), sqlServerOptions =>
        sqlServerOptions.EnableRetryOnFailure(2, TimeSpan.FromSeconds(10), null)));

//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddDefaultTokenProviders().AddDefaultUI()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));



//builder.Services.AddCors((corsoptions) =>
//{
//    corsoptions.AddPolicy("Mypolicy", (policyoptions) =>
//    {
//        policyoptions.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors("Mypolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
