﻿namespace ProductService.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public long SellerId { get; set; }
        public int MaxQuantity { get; set; }
        public byte[] PhotoUrl { get; set; }
    }
}
