﻿using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using Microsoft.Ajax.Utilities;

namespace CmsShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Deklaracja listy PageViewModel
            List<PageViewModel> pagesList;

            //Inicjalizacja listy
            using (Db db = new Db())
            {
                pagesList = db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new PageViewModel(x))
                    .ToList();
            }

            // zwracamy strony do widoku
            return View(pagesList);
        }

        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }
        //POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageViewModel model)
        {
            //Sprawdzanie model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                string slug;

                //Inicjaclizacja PageDTO
                PageDTO dto= new PageDTO();

                
                //jeżeli slug jest pusty to przypisujemy tytuł
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //zapobiegamy dodaniu takiej samej nazwy strony
                if (db.Pages.Any(x => x.Title == model.Title ) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten tytuł lub adres już istnieje");
                    return View(model);
                }

                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;

                //zapis dto w bazie danych
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "Dodałeś nową stronę";

            return RedirectToAction("AddPage");
        }

        //GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //deklaracja page View Model
            PageViewModel model;


            using (Db db = new Db())
            {
                //pobieramy strone z bazy o danym id
                PageDTO dto = db.Pages.Find(id);

                //sprawdzamy czy ta strona istnieje
                if (dto == null)
                {
                    return Content("Strona nie istnieje");
                }
                
                model = new PageViewModel(dto);

            }

            return View(model);
        }
        
        //POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //pobranie Id strony
                int id = model.Id;

                string slug = "home";

                //pobranie stron do edycji
                PageDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //sprawdzamy czy strona i adres są unikalne
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title)||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona lub adres już istnieje");
                }

                //modyfikacje DTO
                dto.Title = model.Title;
                dto.Slug = model.Slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                
                //zapis do bazy
                db.SaveChanges();

            }

            //ustawienie komunikatu
            TempData["SM"] = "Edytowałeś stronę";

            //Redirect
            return RedirectToAction("EditPage");
        }

        //GET: Admin/Pages/Details
        [HttpGet]
        public ActionResult Details (int id)
        {
            PageViewModel model;

            using (Db db = new Db())
            {
                //pobranie strony do wyświetlenia
                PageDTO dto = db.Pages.Find(id);

                //sprawdzenie czy strona o takim id istnieje
                if(dto == null)
                {
                    return Content("Strona o podanym id nie istnieje");
                }

                //inicjalizacja
                model = new PageViewModel(dto);
                
            }

            return View(model);
        }


        //GET: Admin/Pages/Details
        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (Db db = new Db())
            {
                //pobranie strony do usunięcia
                PageDTO dto = db.Pages.Find(id);

                //usuwanie strony z bazy
                db.Pages.Remove(dto);

                //zapis
                db.SaveChanges();
            }

            //przekierowanie
            return RedirectToAction("Index");
        }

        //POST: Admin/Pages/ReorderPages
        [HttpPost]
        public ActionResult ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                PageDTO dto;

                // sortowanie stron, zapis na bazie
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }

            }

            return View();
        }

        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Deklaracja SidebarViewModel
            SidebarViewModel model;

            using (Db db = new Db())
            {
                //pobieramy sidebarDto
                SidebarDTO dto = db.Sidebar.Find(1);
                
                //Inicjalizacja modelu
                model = new SidebarViewModel(dto);
            }

            return View(model);
        }

        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarViewModel model)
        {
            using (Db db = new Db())
            {
                //pobieramy Sidebar DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //modyfikacja sidebar
                dto.Body = model.Body;

                //zapis na bazie
                db.SaveChanges();
            }
            
            //komunikat o modyfikacji
            TempData["SM"] = "Pasek boczny został zmodyfikowany";

            return RedirectToAction("EditSidebar");
        }
    }

}