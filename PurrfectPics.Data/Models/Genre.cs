namespace BookStore.Data.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Book> CatImages { get; set; } = new List<Book>();
    }
}