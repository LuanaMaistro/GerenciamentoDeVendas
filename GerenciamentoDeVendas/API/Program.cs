using Application.Interfaces.Services;
using Application.Services;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Recombee.ApiClient;
using Recombee.ApiClient.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Gerenciamento de Vendas API",
        Version = "v1",
        Description = "API para gerenciamento de vendas e estoque"
    });
    c.AddServer(new() { Url = "http://localhost:5001", Description = "Servidor Local" });
});

// Database (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - Infrastructure
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Recombee - cliente singleton (thread-safe, reutilizado em todas as requisições)
var recombeeRegion = builder.Configuration["Recombee:Region"]?.ToLower() switch
{
    "us-west" => Region.UsWest,
    "ap-se"   => Region.ApSe,
    _         => Region.EuWest
};
builder.Services.AddSingleton(new RecombeeClient(
    builder.Configuration["Recombee:DatabaseId"]!,
    builder.Configuration["Recombee:PrivateToken"]!,
    region: recombeeRegion
));

// Dependency Injection - Application Services
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

var app = builder.Build();

// Aplica migrations pendentes automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciamento de Vendas API v1");
    });
}

app.UseCors("AllowFrontend");

app.UseMiddleware<API.Middlewares.ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
