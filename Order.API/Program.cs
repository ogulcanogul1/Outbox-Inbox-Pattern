using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Contexts;
using Order.API.Models;
using Order.API.ViewModels;
using Shared.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddMassTransit(configuration =>
{
    configuration.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderViewModel orderViewModel, OrderDbContext orderDbContext, IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Order order = new()
    {
        BuyerId = orderViewModel.BuyerId,
        OrderItems = orderViewModel.OrderItems.Select(x => new Order.API.Models.OrderItem
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId
        }).ToList(),
        CreatedDate = DateTime.UtcNow,
        TotalPrice = orderViewModel.OrderItems.Sum(x => x.Price * x.Count)
    };

    await orderDbContext.AddAsync(order);

    OrderCreatedEvent orderCreatedEvent = new()
    {
        OrderId = order.Id,
        OrderItems = order.OrderItems.Select(x => new Shared.Messages.OrderItemMessage
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId
        }).ToList(),
        BuyerId = order.BuyerId,
        TotalPrice = order.TotalPrice
    };

    #region Outbox Design Pattern olmadan
    //await publishEndpoint.Publish(orderCreatedEvent);
    #endregion

    #region Outbox Design Pattern
    OrderOutbox orderOutbox = new()
    {
        OccuredOn = DateTime.UtcNow,
        ProcessedDate = null,
        Type = orderCreatedEvent.GetType().Name,
        Payload = JsonSerializer.Serialize(orderCreatedEvent),
        IdempotentToken = Guid.NewGuid()
    };

    await orderDbContext.AddAsync(orderOutbox);

    await orderDbContext.SaveChangesAsync();
    #endregion


});

app.Run();
