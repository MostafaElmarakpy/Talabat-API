using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderWithItemsAndDeliveryMethodSpec : BaseSpecification<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpec(string buyerEmail) :
            base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod);//Eager Loading
            Includes.Add(O => O.OrderItems);//Eager Loading

            AddOrderByDescending(O => O.OrderDate);


            // Apply Pagination();

        }
        public OrderWithItemsAndDeliveryMethodSpec(int orderId, string buyerEmail) :
    base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)
        {
            Includes.Add(O => O.DeliveryMethod);//Eager Loading
            Includes.Add(O => O.OrderItems);

        }

    }
}
