using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreFront.DATA.EF;//used to access seeds class

namespace StoreFront.UI.MVC.Models
{
    public class CartItemViewModel
    {

        //properties
        public int Qty { get; set; }
        public Seed Product { get; set; }


        public CartItemViewModel(int qty, Seed product)
        {
            Qty = qty;
            Product = product;
        }//end FQCTOR

    }//end class
}//end namspace