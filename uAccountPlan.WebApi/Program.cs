using Microsoft.EntityFrameworkCore;
using uAccountPlan.Application.Interfaces;
using uAccountPlan.Application.Services;
using uAccountPlan.Domain.Interfaces;
using uAccountPlan.Infrastructure;
using uAccountPlan.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAccountPlanRepository, AccountPlanRepository>();
builder.Services.AddScoped<IAccountPlanService, AccountPlanService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();