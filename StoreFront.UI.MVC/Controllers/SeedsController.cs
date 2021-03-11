using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;//empty controller needed ref. to entity
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;//Access to EF and connection
using StoreFront.UI.MVC.Models;
using StoreFront.UI.MVC.Utilites;
using PagedList.Mvc;
using PagedList;


namespace StoreFront.UI.MVC.Controllers
{
    public class SeedsController : Controller
    {
        private StoreFrontEntities ctx = new StoreFrontEntities();

        #region Paging
        public ActionResult SeedsMVCPaging(string searchString, int page = 1)
        {

            int pageSize = 5;

            var seeds = ctx.Seeds.OrderBy(s => s.CommonName).ToList();
            #region Search Logic
            if (!string.IsNullOrEmpty(searchString))
            {
                seeds = seeds.Where(s => s.CommonName.ToLower().Contains(searchString.ToLower())).ToList();

            }

            ViewBag.SearchString = searchString;
            #endregion
            return View(seeds.ToPagedList(page, pageSize));
        }//end ActionResult
        #endregion

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
            Seed product = ctx.Seeds.Where(s => s.SeedID == seedID).FirstOrDefault();//which will allow a null value
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

        //#region QueryString Seeds
        ////Server Side filter - QueryString
        //public ActionResult SeedsQS(string searchFilter)
        //{
        //    //2 options
        //    //- search has not been used (initial page demand or subsequent demands)
        //    //- search has been used and return filtered results

        //    //get a list of seeds
        //    var seeds = ctx.Seeds;

        //    //branch - no filter
        //    if (string.IsNullOrEmpty(searchFilter))
        //    {
        //        //return all results
        //        return View(seeds.ToList());
        //    }
        //    //branch for Filtered Results
        //    else
        //    {
        //        //searchFilter has some value
        //        //return a list of seeds that matches common name
        //        //Method/Lambda syntax
        //        var filteredSeeds = seeds.Where(s => s.CommonName.ToLower().Contains(searchFilter.ToLower())).ToList();

        //        //keyword syntax
        //        var filteredSeedKW = (from s in seeds
        //                                 where s.CommonName.ToLower().Contains(searchFilter.ToLower())
        //                                 select s).ToList();//making this a list make sure the return type is the same as filteredSeeds.
        //        //Keyword returning a new anonymous type ...interchangeable with filteredAuthorsKW
        //        //this result would need ot be passes via viewbag into an
        //        var filteredSeedsKWAnon = from s in seeds
        //                                  where s.CommonName.ToLower().Contains(searchFilter.ToLower())
        //                                  select new
        //                                  {
        //                                      SeedName = s.CommonName
        //                                  };
                                                  


        //        return View(filteredSeeds);
        //    }//end else

        //}//end ActionResult AuthorsQS()
        //#endregion

        // GET: Seeds
        public ActionResult Index()
        {
            var seeds = ctx.Seeds.Include(s => s.FrostHardy).Include(s => s.GeneType).Include(s => s.IdealTemp).Include(s => s.LifeCycle).Include(s => s.MinFullSun).Include(s => s.PlantSpacing).Include(s => s.Product).Include(s => s.Season).Include(s => s.SeedCategory).Include(s => s.SeedDepth).Include(s => s.SproutIn).Include(s => s.UnitsPerPacket);
            return View(seeds.ToList());
        }

        // GET: Seeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seed seed = ctx.Seeds.Find(id);
            if (seed == null)
            {
                return HttpNotFound();
            }
            return View(seed);
        }

        // GET: Seeds/Create
        public ActionResult Create()
        {
            ViewBag.FrostID = new SelectList(ctx.FrostHardys, "FrostID", "FrostID");
            ViewBag.GeneID = new SelectList(ctx.GeneTypes, "GeneID", "GeneName");
            ViewBag.TempID = new SelectList(ctx.IdealTemps, "TempID", "Temp");
            ViewBag.CycleID = new SelectList(ctx.LifeCycles, "CycleID", "CycleType");
            ViewBag.SunID = new SelectList(ctx.MinFullSuns, "SunID", "SunTime");
            ViewBag.SpacingID = new SelectList(ctx.PlantSpacings, "SpacingID", "Spacing");
            ViewBag.ProductID = new SelectList(ctx.Products, "ProductID", "ProductName");
            ViewBag.SeasonID = new SelectList(ctx.Seasons, "SeasonID", "SeasonType");
            ViewBag.CategoryID = new SelectList(ctx.SeedCategories, "CategoryID", "CategoryName");
            ViewBag.DepthID = new SelectList(ctx.SeedDepths, "DepthID", "DepthID");
            ViewBag.SproutID = new SelectList(ctx.SproutIns, "SproutID", "SproutDays");
            ViewBag.UnitsID = new SelectList(ctx.UnitsPerPackets, "UnitsID", "UnitsID");
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
                        //-other ways to create unique ids (make sure your ctx field accomodates the size)
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


                ctx.Seeds.Add(seed);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FrostID = new SelectList(ctx.FrostHardys, "FrostID", "FrostID", seed.FrostID);
            ViewBag.GeneID = new SelectList(ctx.GeneTypes, "GeneID", "GeneName", seed.GeneID);
            ViewBag.TempID = new SelectList(ctx.IdealTemps, "TempID", "Temp", seed.TempID);
            ViewBag.CycleID = new SelectList(ctx.LifeCycles, "CycleID", "CycleType", seed.CycleID);
            ViewBag.SunID = new SelectList(ctx.MinFullSuns, "SunID", "SunTime", seed.SunID);
            ViewBag.SpacingID = new SelectList(ctx.PlantSpacings, "SpacingID", "Spacing", seed.SpacingID);
            ViewBag.ProductID = new SelectList(ctx.Products, "ProductID", "ProductName", seed.ProductID);
            ViewBag.SeasonID = new SelectList(ctx.Seasons, "SeasonID", "SeasonType", seed.SeasonID);
            ViewBag.CategoryID = new SelectList(ctx.SeedCategories, "CategoryID", "CategoryName", seed.CategoryID);
            ViewBag.DepthID = new SelectList(ctx.SeedDepths, "DepthID", "DepthID", seed.DepthID);
            ViewBag.SproutID = new SelectList(ctx.SproutIns, "SproutID", "SproutDays", seed.SproutID);
            ViewBag.UnitsID = new SelectList(ctx.UnitsPerPackets, "UnitsID", "UnitsID", seed.UnitsID);
            return View(seed);
        }

        // GET: Seeds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seed seed = ctx.Seeds.Find(id);
            if (seed == null)
            {
                return HttpNotFound();
            }
            ViewBag.FrostID = new SelectList(ctx.FrostHardys, "FrostID", "FrostID", seed.FrostID);
            ViewBag.GeneID = new SelectList(ctx.GeneTypes, "GeneID", "GeneName", seed.GeneID);
            ViewBag.TempID = new SelectList(ctx.IdealTemps, "TempID", "Temp", seed.TempID);
            ViewBag.CycleID = new SelectList(ctx.LifeCycles, "CycleID", "CycleType", seed.CycleID);
            ViewBag.SunID = new SelectList(ctx.MinFullSuns, "SunID", "SunTime", seed.SunID);
            ViewBag.SpacingID = new SelectList(ctx.PlantSpacings, "SpacingID", "Spacing", seed.SpacingID);
            ViewBag.ProductID = new SelectList(ctx.Products, "ProductID", "ProductName", seed.ProductID);
            ViewBag.SeasonID = new SelectList(ctx.Seasons, "SeasonID", "SeasonType", seed.SeasonID);
            ViewBag.CategoryID = new SelectList(ctx.SeedCategories, "CategoryID", "CategoryName", seed.CategoryID);
            ViewBag.DepthID = new SelectList(ctx.SeedDepths, "DepthID", "DepthID", seed.DepthID);
            ViewBag.SproutID = new SelectList(ctx.SproutIns, "SproutID", "SproutDays", seed.SproutID);
            ViewBag.UnitsID = new SelectList(ctx.UnitsPerPackets, "UnitsID", "UnitsID", seed.UnitsID);
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
                ctx.Entry(seed).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FrostID = new SelectList(ctx.FrostHardys, "FrostID", "FrostID", seed.FrostID);
            ViewBag.GeneID = new SelectList(ctx.GeneTypes, "GeneID", "GeneName", seed.GeneID);
            ViewBag.TempID = new SelectList(ctx.IdealTemps, "TempID", "Temp", seed.TempID);
            ViewBag.CycleID = new SelectList(ctx.LifeCycles, "CycleID", "CycleType", seed.CycleID);
            ViewBag.SunID = new SelectList(ctx.MinFullSuns, "SunID", "SunTime", seed.SunID);
            ViewBag.SpacingID = new SelectList(ctx.PlantSpacings, "SpacingID", "Spacing", seed.SpacingID);
            ViewBag.ProductID = new SelectList(ctx.Products, "ProductID", "ProductName", seed.ProductID);
            ViewBag.SeasonID = new SelectList(ctx.Seasons, "SeasonID", "SeasonType", seed.SeasonID);
            ViewBag.CategoryID = new SelectList(ctx.SeedCategories, "CategoryID", "CategoryName", seed.CategoryID);
            ViewBag.DepthID = new SelectList(ctx.SeedDepths, "DepthID", "DepthID", seed.DepthID);
            ViewBag.SproutID = new SelectList(ctx.SproutIns, "SproutID", "SproutDays", seed.SproutID);
            ViewBag.UnitsID = new SelectList(ctx.UnitsPerPackets, "UnitsID", "UnitsID", seed.UnitsID);
            return View(seed);
        }

        //// GET: Seeds/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Seed seed = ctx.Seeds.Find(id);
        //    if (seed == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(seed);
        //}

        //// POST: Seeds/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Seed seed = ctx.Seeds.Find(id);
        //    ctx.Seeds.Remove(seed);
        //    ctx.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        ctx.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
