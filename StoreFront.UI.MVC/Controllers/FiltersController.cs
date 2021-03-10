using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;//Access to EF and connection
using System.Data.Entity;//Empty controller needed reference to entity
using PagedList;//MVC paging
using PagedList.Mvc;//MVC paging


namespace StoreFront.UI.MVC.Controllers
{
    public class FiltersController : Controller
    {
        //connection to Data Structure
        private StoreFrontEntities ctx = new StoreFrontEntities();

        // GET: Filters
        public ActionResult Index()
        {
            return View();
        }//end Index()

        public ActionResult SeedsMVCPaging(string searchString, int page =1)
        {

            int pageSize = 9;

            var seeds = ctx.Seeds.OrderBy(s => s.CommonName).ToList();
            #region Search Logic
            if(!string.IsNullOrEmpty(searchString))
            {
                seeds = seeds.Where(s => s.CommonName.ToLower().Contains(searchString.ToLower())).ToList();

            }

            ViewBag.SearchString = searchString;
            #endregion
            return View(seeds.ToPagedList(page, pageSize));
        }//end ActionResult
    }//end class
}//end namespace