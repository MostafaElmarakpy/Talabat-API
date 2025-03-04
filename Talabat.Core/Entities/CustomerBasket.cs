﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket(string id)
        {
            this.Id = id;
            Items = new List<BasketItem>();
        }

        public string Id { get; set; }
        public IReadOnlyList<BasketItem> Items { get; private set; }

        public string PaymentIntentId { get; set; }

        public string ClientSecret { get; set; }

        public int? DeliveryMethodId { get; set; }

        public decimal ShippingPrice { get; set; }

    }
}
