namespace BookStore.Web.ViewModels
{
    public class RatingDistributionViewModel
    {
        public int BookId { get; set; }

        public int OneStar { get; set; }

        public int TwoStars { get; set; }

        public int ThreeStars { get; set; }

        public int FourStars { get; set; }

        public int FiveStars { get; set; }

        public int TotalRatings => OneStar + TwoStars + ThreeStars + FourStars + FiveStars;

        public double AverageRating
        {
            get
            {
                if (TotalRatings == 0) return 0;

                var total = (OneStar * 1) + (TwoStars * 2) + (ThreeStars * 3) +
                           (FourStars * 4) + (FiveStars * 5);

                return Math.Round((double)total / TotalRatings, 1);
            }
        }

        public int OneStarPercentage => TotalRatings > 0 ? (int)((OneStar / (double)TotalRatings) * 100) : 0;
        public int TwoStarsPercentage => TotalRatings > 0 ? (int)((TwoStars / (double)TotalRatings) * 100) : 0;
        public int ThreeStarsPercentage => TotalRatings > 0 ? (int)((ThreeStars / (double)TotalRatings) * 100) : 0;
        public int FourStarsPercentage => TotalRatings > 0 ? (int)((FourStars / (double)TotalRatings) * 100) : 0;
        public int FiveStarsPercentage => TotalRatings > 0 ? (int)((FiveStars / (double)TotalRatings) * 100) : 0;
    }
}