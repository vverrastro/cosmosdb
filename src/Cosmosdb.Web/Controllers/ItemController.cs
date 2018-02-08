using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cosmosdb.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cosmosdb.Web.Controllers
{
    public class ItemController : Controller
    {

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync() {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);
            return View(items);
        }

        [ActionName("Create")]
        public ActionResult CreateAsync() {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync( Item item ) {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Item>.CreateItemAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync( Item item ) {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Item>.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync( string id ) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(item);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync( string id ) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(item);
        }

    }

    internal class HttpStatusCodeResult : ActionResult
    {
        private object badRequest;

        public HttpStatusCodeResult( object badRequest ) {
            this.badRequest = badRequest;
        }
    }
}