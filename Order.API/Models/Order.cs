﻿namespace Order.API.Models;


public class Order
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItem> OrderItems { get; set; } = default!;

}
