using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Display name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2-50 characters")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [StringLength(50)]
        [Display(Name = "Favorite Genre")]
        public string? FavoriteGenre { get; set; }

        [Range(1900, 2026, ErrorMessage = "Please enter a valid birth year")]
        [Display(Name = "Birth Year")]
        public int? BirthYear { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Current Profile Image")]
        public string? CurrentProfileImageUrl { get; set; }
    }
}