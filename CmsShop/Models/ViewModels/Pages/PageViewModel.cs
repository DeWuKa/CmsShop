﻿using System.ComponentModel.DataAnnotations;
using CmsShop.Models.Data;

namespace CmsShop.Models.ViewModels.Pages
{
    public class PageViewModel
    {
        public PageViewModel()
        {
            
        }

        public PageViewModel(PageDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Tytuł strony")]
        public string Title { get; set; }
        [Display(Name = "Adres strony")]
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [Display(Name = "Zawartość strony")]
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name = "Pasek boczny")]
        public bool HasSidebar { get; set; }
    }
}