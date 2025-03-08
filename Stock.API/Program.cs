using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Consumers;
using Stock.API.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StockDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));    
});

builder.Services.AddMassTransit(configuration =>
{
    configuration.AddConsumer<OrderCreatedEventConsumer>();
    configuration.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint("stock-order-created-event-consumer", e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

var app = builder.Build();


app.Run();
