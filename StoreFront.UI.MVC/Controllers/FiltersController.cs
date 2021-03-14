using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;//Access to EF and connection
using System.Data.Entity;//Empty controller needed reference to entity
using PagedList;//MVC paging
using PagedList.Mvc;//MVC paging
using System.Net;
using System.Drawing;
using StoreFront.UI.MVC.Utilites;

namespace StoreFront.UI.MVC.Controllers
{
    public class FiltersController : Controller
    {
        //connection to Data Structure
        private StoreFrontEntities ctx = new StoreFrontEntities();

        #region Ajax Delete
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AjaxDelete(int id)
        {
            //get the publisher from the db
            Seed seed = ctx.Seeds.Find(id);//find only returns one value
                                           //remove the publisher from the EF
            ctx.Seeds.Remove(seed);
            //save the changes to the database
            ctx.SaveChanges();
            //Create a message to send to the user as a jason result
            var message = $"Deleted the following seed from the database: {seed.CommonName}";
            //return the jsonResult
            return Json(new
            {
                id = id,
                message = message
            });
        }

        #endregion


        // GET: Filters
        public ActionResult Index()
        {
            return View();
        }//end Index()

        public ActionResult SeedsMVCPaging(string searchString, int page = 1)
        {

            int pageSize = 9;
            
            
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


        // GET: Seeds/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Seed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seed seed = ctx.Seeds.Find(id);
            ctx.Seeds.Remove(seed);
            ctx.SaveChanges();
            return RedirectToAction("SeedsMVCPaging");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
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
                seed.ImageUrl = imgName;
                #endregion
                #region Exception TRY/CATCH
                Exception e;
                try
                {
                    ctx.Seeds.Add(seed);
                    ctx.SaveChanges();
                }
                catch (Exception exception)
                {
                    throw e = new Exception($"{exception.GetType()}\n{exception.Message}");
                }
                #endregion
                return RedirectToAction("SeedsMVCPaging");
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
                if (seedPacket != null)
                {
                    string imgName = seedPacket.FileName;

                    string ext = imgName.Substring(imgName.LastIndexOf('.'));

                    string[] goodExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    if (goodExts.Contains(ext.ToLower()) && (seedPacket.ContentLength <= 4194304))
                    {
                        imgName = Guid.NewGuid() + ext.ToLower();

                        string savePath = Server.MapPath("~/Content/img/product/");

                        Image convertedImage = Image.FromStream(seedPacket.InputStream);
                        int maxImageSize = 500;
                        int maxThumbSize = 100;

                        ImageService.ResizeImage(savePath, imgName, convertedImage, maxImageSize, maxThumbSize);

                        if (seed.ImageUrl != null && seed.ImageUrl != "NoImage.png")
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
                return RedirectToAction("SeedsMVCPaging");
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

       


    }//end class
}//end namespace