using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.ViewModels
{
    public class AddBookViewModel
    {
        [Required(ErrorMessage = "Book title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1-200 characters")]
        [Display(Name = "Book Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2-100 characters")]
        [Display(Name = "Author")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10-13 characters")]
        [RegularExpression(@"^(?=(?:\D*\d){10,13}$)[\d-]+$", ErrorMessage = "Please enter a valid ISBN")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10-2000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price ($)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Cover image is required")]
        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; }

        [Range(1000, 2026, ErrorMessage = "Please enter a valid publication year")]
        [Display(Name = "Publication Year")]
        public int? PublicationYear { get; set; }

        [StringLength(100)]
        [Display(Name = "Publisher")]
        public string? Publisher { get; set; }

        [Range(1, 10000, ErrorMessage = "Page count must be between 1-10,000")]
        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        [Display(Name = "Genres")]
        public string Genres { get; set; }
    }
}