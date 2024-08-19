using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.BLL;
using PaymentGateway.Api.Database;
using PaymentGateway.Api.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core to use an in-memory database
builder.Services.AddDbContext<Context>(x => {
    x.EnableSensitiveDataLogging();
    x.UseInMemoryDatabase("InMemoryDb");
});

// Register HttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("BankClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BankSimulator:ApiUri"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add Scoped service for manager
builder.Services.AddScoped<IPaymentGatewayManager, PaymentGatewayManager>();

// Add Scoped service for data access layer
builder.Services.AddScoped<IPaymentGatewayDal, PaymentGatewayDal>();

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

app.Run();
