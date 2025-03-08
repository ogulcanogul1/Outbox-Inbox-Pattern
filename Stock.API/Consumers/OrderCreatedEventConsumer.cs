using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Contexts;
using Stock.API.Models;
using System.Text.Json;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(StockDbContext stockDbContext) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var result = await stockDbContext.OrderInboxes.AnyAsync(i => i.IdempotentToken == context.Message.IdempotentToken);

            if (!result)
            {
                await stockDbContext.AddAsync(new OrderInbox
                {
                    Payload = JsonSerializer.Serialize(context.Message),
                    Process = false,
                    IdempotentToken = context.Message.IdempotentToken
                });

                await stockDbContext.SaveChangesAsync();
            }

            List<OrderInbox> orderInboxes = stockDbContext.OrderInboxes.Where(x => x.Process == false).ToList();

            foreach (var orderInbox in orderInboxes)
            {
                OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload);
                Console.WriteLine($"{orderCreatedEvent.OrderId} siparişin stock işlemleri başarılı");
                orderInbox.Process = true;
                await stockDbContext.SaveChangesAsync();
            }
        }
    }
}
