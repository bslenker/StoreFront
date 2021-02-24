using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;
using StoreFront.UI.MVC.Models;
using StoreFront.UI.MVC.Utilites;

namespace StoreFront.UI.MVC.Controllers
{
    public class SeedsController : Controller
    {
        private StoreFrontEntities db = new StoreFrontEntities();

        #region AddToCart()
        //looking for the quantity and the seed they are wanting to add
        [HttpPost]
        public ActionResult AddToCart(int qty, int seedID)
        {                       //had to add reference
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //check cart in session (global)
            //if the cart has seeds in it, then assign its value to the local dictionary
            if(Session["cart"] != null)
            {
                shoppingCart = (Dictionary<int, CartItemViewModel>)Session["cart"];
            }//end if
            //if Global is empty
            else
            {
                //create an empty instance of the Local dictionary
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }//end else

            //here get the products object being added
            Seed product = db.Seeds.Where(s => s.SeedID == seedID).FirstOrDefault();//which will allow a null value
            //if productID (seedID) is null, return them to the seeds index
            if(product == null)
            {
                return RedirectToAction("Index");

            }//end if
            //else the productID IS valid
            else
            {
                //create the shopping cart view model object
                CartItemViewModel item = new CartItemViewModel(qty, product);

                //if the productID is represented in the shoppingcart, do an increase of quantity.. functionality
                if(shoppingCart.ContainsKey(product.SeedID))
                {
                    shoppingCart[product.SeedID].Qty += qty;
                }//end iff
                //else the product is NOT in the cart, add it there
                else
                {
                    shoppingCart.Add(product.SeedID, item);
                }//end else
                //have to update the Global (session) cart with the values from the local (dictionary)
                Session["cart"] = shoppingCart;
            }//end else
            //if the product was added => redirect to the ShoppingCart Index
            return RedirectToAction("Index", "ShoppingCart");
        }//end ActionResult


        #endregion


        // GET: Seeds
        public ActionResult Index()
        {
            var seeds = db.Seeds.Include(s => s.FrostHardy).Include(s => s.GeneType).Include(s => s.IdealTemp).Include(s => s.LifeCycle).Include(s => s.MinFullSun).Include(s => s.PlantSpacing).Include(s => s.Product).Include(s => s.Season).Include(s => s.SeedCategory).Include(s => s.SeedDepth).Include(s => s.SproutIn).Include(s => s.UnitsPerPacket);
            return View(seeds.ToList());
        }

        // GET: Seeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seed seed = db.Seeds.Find(id);
            if (seed == null)
            {
                return HttpNotFound();
            }
            return View(seed);
        }

        // GET: Seeds/Create
        public ActionResult Create()
        {
            ViewBag.FrostID = new SelectList(db.FrostHardys, "FrostID", "FrostID");
            ViewBag.GeneID = new SelectList(db.GeneTypes, "GeneID", "GeneName");
            ViewBag.TempID = new SelectList(db.IdealTemps, "TempID", "Temp");
            ViewBag.CycleID = new SelectList(db.LifeCycles, "CycleID", "CycleType");
            ViewBag.SunID = new SelectList(db.MinFullSuns, "SunID", "SunTime");
            ViewBag.SpacingID = new SelectList(db.PlantSpacings, "SpacingID", "Spacing");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "SeasonType");
            ViewBag.CategoryID = new SelectList(db.SeedCategories, "CategoryID", "CategoryName");
            ViewBag.DepthID = new SelectList(db.SeedDepths, "DepthID", "DepthID");
            ViewBag.SproutID = new SelectList(db.SproutIns, "SproutID", "SproutDays");
            ViewBag.UnitsID = new SelectList(db.UnitsPerPackets, "UnitsID", "UnitsID");
            return View();
        }

        // POST: Seeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SeedID,CommonName,ScientificName,Description,PlantingInstructions,Cost,SpacingID,UnitsID,CycleID,ProductID,SproutID,SeasonID,CategoryID,SunID,TempID,GeneID,DepthID,FrostID,ImageUrl")] Seed seed, HttpPostedFileBase seedPacket)
        {
            if (ModelState.IsValid)
            {
                #region File Upload Utility

                string imgName = "NoImage.png";

                if (seedPacket != null)
                {
                    //if NOT null
                    //retireive the image from the HPFB and assign to the img variable
                    imgName = seedPacket.FileName;
                    //declare and assign the extension
                    string ext = imgName.Substring(imgName.LastIndexOf('.'));//.ext
                    //for images, we want a good list of proper extensions to be accepted
                    //for pdfs, no collection is needed.
                    string[] goodExts = { ".jpg", ".jpeg", ".gif", ".png" };
                    //Check the ext. variable against the valid list AND make sure the file size is NOT too large
                    //(Max default ASP.NET size is ~ 4mb - expressed in bytes 4194304
                    if (goodExts.Contains(ext.ToLower()) && (seedPacket.ContentLength <= 4194304))
                    //code for the PDF check
                    //if (ext.ToLower() ==".pdf" && (bookCover.ContentLength <= 4194304))
                    {
                        //If both are good rename the file using a guid + the extension
                        //GUID = Global Unique IDentifier
                        //-other ways to create unique ids (make sure your DB field accomodates the size)
                        //substring the book title (first 10/20 characters, concatonate the currernt userID , datetimestamp)
                        //Guid works well but it is not the only option.
                        imgName = Guid.NewGuid() + ext.ToLower();
                        //alternate methodology to reneame
                        //imgName = book.BookTitle.Substring(0, 10) + User.Identity.Name + DateTime.Now;

                        //Resize the image
                        //provide all requirements  to call the ResizeImage() from the utility. SavePath, Image, MaxImageSize, MaxThumbSize
                        string savePath = Server.MapPath("~/Content/img/product/");
                        Image convertedImage = Image.FromStream(seedPacket.InputStream);//methodology to pull in that file and grabbing the input string
                        int maxImageSize = 500;
                        int maxThumbSize = 100;
                        //call the imageService.ResizeImage
                        ImageService.ResizeImage(savePath, imgName, convertedImage, maxImageSize, maxThumbSize);
                    }//end if

                    //either file size or ext are bad
                    else
                    {
                        //default to the noImage value
                        imgName = "NoImage.png";
                    }//end else
                }//end if
                //no matter what - add the imageName property of the book object to send to the DB.
                seed.ImageUrl= imgName;


                #endregion


                db.Seeds.Add(seed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FrostID = new SelectList(db.FrostHardys, "FrostID", "FrostID", seed.FrostID);
            ViewBag.GeneID = new SelectList(db.GeneTypes, "GeneID", "GeneName", seed.GeneID);
            ViewBag.TempID = new SelectList(db.IdealTemps, "TempID", "Temp", seed.TempID);
            ViewBag.CycleID = new SelectList(db.LifeCycles, "CycleID", "CycleType", seed.CycleID);
            ViewBag.SunID = new SelectList(db.MinFullSuns, "SunID", "SunTime", seed.SunID);
            ViewBag.SpacingID = new SelectList(db.PlantSpacings, "SpacingID", "Spacing", seed.SpacingID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", seed.ProductID);
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "SeasonType", seed.SeasonID);
            ViewBag.CategoryID = new SelectList(db.SeedCategories, "CategoryID", "CategoryName", seed.CategoryID);
            ViewBag.DepthID = new SelectList(db.SeedDepths, "DepthID", "DepthID", seed.DepthID);
            ViewBag.SproutID = new SelectList(db.SproutIns, "SproutID", "SproutDays", seed.SproutID);
            ViewBag.UnitsID = new SelectList(db.UnitsPerPackets, "UnitsID", "UnitsID", seed.UnitsID);
            return View(seed);
        }

        // GET: Seeds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seed seed = db.Seeds.Find(id);
            if (seed == null)
            {
                return HttpNotFound();
            }
            ViewBag.FrostID = new SelectList(db.FrostHardys, "FrostID", "FrostID", seed.FrostID);
            ViewBag.GeneID = new SelectList(db.GeneTypes, "GeneID", "GeneName", seed.GeneID);
            ViewBag.TempID = new SelectList(db.IdealTemps, "TempID", "Temp", seed.TempID);
            ViewBag.CycleID = new SelectList(db.LifeCycles, "CycleID", "CycleType", seed.CycleID);
            ViewBag.SunID = new SelectList(db.MinFullSuns, "SunID", "SunTime", seed.SunID);
            ViewBag.SpacingID = new SelectList(db.PlantSpacings, "SpacingID", "Spacing", seed.SpacingID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", seed.ProductID);
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "SeasonType", seed.SeasonID);
            ViewBag.CategoryID = new SelectList(db.SeedCategories, "CategoryID", "CategoryName", seed.CategoryID);
            ViewBag.DepthID = new SelectList(db.SeedDepths, "DepthID", "DepthID", seed.DepthID);
            ViewBag.SproutID = new SelectList(db.SproutIns, "SproutID", "SproutDays", seed.SproutID);
            ViewBag.UnitsID = new SelectList(db.UnitsPerPackets, "UnitsID", "UnitsID", seed.UnitsID);
            return View(seed);
        }

        // POST: Seeds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SeedID,CommonName,ScientificName,Description,PlantingInstructions,Cost,SpacingID,UnitsID,CycleID,ProductID,SproutID,SeasonID,CategoryID,SunID,TempID,GeneID,DepthID,FrostID,ImageUrl")] Seed seed, HttpPostedFileBase seedPacket)
        {
            if (ModelState.IsValid)
            {
                #region File Upload Utility
                if(seedPacket != null)
                {
                    string imgName = seedPacket.FileName;

                    string ext = imgName.Substring(imgName.LastIndexOf('.'));

                    string[] goodExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    if(goodExts.Contains(ext.ToLower()) && (seedPacket.ContentLength <= 4194304))
                    {
                        imgName = Guid.NewGuid() + ext.ToLower();

                        string savePath = Server.MapPath("~/Content/img/product/");

                        Image convertedImage = Image.FromStream(seedPacket.InputStream);
                        int maxImageSize = 500;
                        int maxThumbSize = 100;

                        ImageService.ResizeImage(savePath, imgName, convertedImage, maxImageSize, maxThumbSize);

                        if(seed.ImageUrl !=null &&seed.ImageUrl != "NoImage.png")
                        {
                            string path = Server.MapPath("~/Content/img/product/");
                            ImageService.Delete(path, seed.ImageUrl);
                        }
                        seed.ImageUrl = imgName;

                    }//end if
                }
                #endregion
                db.Entry(seed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FrostID = new SelectList(db.FrostHardys, "FrostID", "FrostID", seed.FrostID);
            ViewBag.GeneID = new SelectList(db.GeneTypes, "GeneID", "GeneName", seed.GeneID);
            ViewBag.TempID = new SelectList(db.IdealTemps, "TempID", "Temp", seed.TempID);
            ViewBag.CycleID = new SelectList(db.LifeCycles, "CycleID", "CycleType", seed.CycleID);
            ViewBag.SunID = new SelectList(db.MinFullSuns, "SunID", "SunTime", seed.SunID);
            ViewBag.SpacingID = new SelectList(db.PlantSpacings, "SpacingID", "Spacing", seed.SpacingID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", seed.ProductID);
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "SeasonType", seed.SeasonID);
            ViewBag.CategoryID = new SelectList(db.SeedCategories, "CategoryID", "CategoryName", seed.CategoryID);
            ViewBag.DepthID = new SelectList(db.SeedDepths, "DepthID", "DepthID", seed.DepthID);
            ViewBag.SproutID = new SelectList(db.SproutIns, "SproutID", "SproutDays", seed.SproutID);
            ViewBag.UnitsID = new SelectList(db.UnitsPerPackets, "UnitsID", "UnitsID", seed.UnitsID);
            return View(seed);
        }

        // GET: Seeds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seed seed = db.Seeds.Find(id);
            if (seed == null)
            {
                return HttpNotFound();
            }
            return View(seed);
        }

        // POST: Seeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seed seed = db.Seeds.Find(id);
            db.Seeds.Remove(seed);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
