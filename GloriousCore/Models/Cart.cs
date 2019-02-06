using GloriousCore.Models.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace GloriousCore.Models
{
    public class Cart
    {
        List<CartLine> cart = new List<CartLine>();
        CartLine cartLine = new CartLine();


        public void Add(ProductDBO product, int quantity)
        {
            CartLine line = cart
                .Where(e => e.Product.Id == product.Id)
                .FirstOrDefault();

            if (line == null)
            {
                cart.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void Del(int id)
        {
            cart.RemoveAll(l => l.Product.Id == id);
        }

        public void Edit(ProductDBO product, int quantity)
        {
            CartLine line = cart
                .Where(e => e.Product.Id == product.Id)
                .FirstOrDefault();

            line.Quantity = quantity;
        }

        public int TotalSum()
        {
            foreach (var item in cart)
            {
                if (item.Product.Discount != 0)
                    item.Product.Price = item.Product.Discount;
            }

            return cart.Sum(c => (int)c.Product.Price * c.Quantity);
        }

        public void Clear()
        {
            cart.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return cart; }
        }

    }
    public class CartLine
    {
        public int Quantity { get; set; }
        public ProductDBO Product { get; set; }
    }
}
