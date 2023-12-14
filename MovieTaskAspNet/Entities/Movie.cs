namespace MovieTaskAspNet.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string? MovieName { get; set; }
        public string? About { get; set; }
        public string? ImagePath { get; set; }
        public double? Rating { get; set; }
    }
}
