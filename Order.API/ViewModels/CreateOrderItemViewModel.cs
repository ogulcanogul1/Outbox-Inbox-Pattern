﻿namespace Order.API.ViewModels
{
    public class CreateOrderItemViewModel
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
