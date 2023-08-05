﻿using OrderService.Models;

namespace OrderService.DTOs
{
    public class OrderAllDTO
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public double TotalPrice { get; set; }
        public bool Confirmed { get; set; }
        public bool Approved { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}