using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqProductAPI.Data;
using RabbitMqProductAPI.RabbitMQ;
using RabbitMqProductAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddDbContext<DbContextClass>();

builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();

builder.Services.AddSingleton<IModel>(serviceProvider =>
{
    //Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
    var factory = new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:Hostname"],
        UserName = builder.Configuration["RabbitMQ:Username"],
        Password = builder.Configuration["RabbitMQ:Password"]
    };

    //Create the RabbitMQ connection using connection factory details as i mentioned above
    var connection = factory.CreateConnection();

    //Here we create channel with session and model
    var channel = connection.CreateModel();

    return channel;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
}
);

var app = builder.Build();

// Migrate latest database changes during startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<DbContextClass>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
}
);

app.UseAuthorization();

app.MapControllers();

app.Run();
