using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Product
{
    public class ProductInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public long SellerId { get; set; }
        public int MaxQuantity { get; set; }
    }
}
