using PaymentGateway.Api.BLL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("BankClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BankSimulator:ApiUri"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add Scoped service for manager
builder.Services.AddScoped<IPaymentGatewayManager, PaymentGatewayManager>();

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
