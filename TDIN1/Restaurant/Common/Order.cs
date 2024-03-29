﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class Order
    {
        public Guid Id { get; set; }
        public int TableId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
        public enum State { NOT_PROCESSED, PROCESSING, FINISHED, DELIVERED, CLOSED }
        public State StateProperty { get; set; }

        public Order(int tableId, Product product, int qty, State state)
        {
            Id = Guid.NewGuid();
            this.TableId = tableId;
            this.Product = product;
            this.Quantity = qty;
            this.TotalPrice = this.Product.Price * this.Quantity;
            this.StateProperty = state;
        }

        public Order(int tableId, Product product, int qty)
        {
            Id = Guid.NewGuid();
            this.TableId = tableId;
            this.Product = product;
            this.Quantity = qty;
            this.TotalPrice = this.Product.Price * this.Quantity;
            this.StateProperty = State.NOT_PROCESSED;
        }

    }


}