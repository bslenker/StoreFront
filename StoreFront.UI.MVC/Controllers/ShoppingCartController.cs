using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;//access to EF for DB context/seed model
using StoreFront.UI.MVC.Models;//CartItemViewModel

namespace StoreFront.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        public ActionResult Index()
        {
            //create a local version of the shopping cart from the Global version
            var shoppingCart = (Dictionary<int, CartItemViewModel>)Session["cart"];

            //if value is null or count at 0, create an empty instance of the cart 
            if(shoppingCart ==null || shoppingCart.Count ==0)
            {
                shoppingCart = new Dictionary<int, CartItemViewModel>();
                //no cart items msg
                ViewBag.Message = "There are no products in your cart.";
            }//end if
            //else there is products in the cart, null the msg
            else
            {
                //msg nulled
                ViewBag.Message = null;
            }//end else

            //return shoppingCart to the view
            return View(shoppingCart);
        }//end Index()

        [HttpPost]
        public ActionResult UpdateCart(int seedID, int qty)
        {
            //if the user 0's out their qty, using the update btn, remove the product from cart
            if(qty ==0)
            {
                //removeFromCart()
                RemoveFromCart(seedID);

                return RedirectToAction("Index");
            }//end if
            //retrieve the cart from session and assign to the local
            Dictionary<int, CartItemViewModel> shoppingCart =
                (Dictionary<int, CartItemViewModel>)Session["cart"];
            //update the qty in the local
            shoppingCart[seedID].Qty = qty;
            //return the local to the session
            Session["cart"] = shoppingCart;
            //logic to display no items in cart msg if they update the last item in the cart to 0
            if(shoppingCart.Count==0)
            {
                ViewBag.Message = "There are no products in your cart.";
            }//end if
            //redirect to Index
            return RedirectToAction("Index");
        }//end UpdateCart()

        public ActionResult RemoveFromCart(int id)
        {
            //cart from session and into the local
            Dictionary<int, CartItemViewModel> shoppingCart =
                (Dictionary<int, CartItemViewModel>)Session["cart"];
            //call the remove  - local class method
            shoppingCart.Remove(id);
            //redirect back to Index Action (runs all code and repopulates the table
            return RedirectToAction("Index");
        }//end RemoveFromCart()
    }//end class
}//end namespace