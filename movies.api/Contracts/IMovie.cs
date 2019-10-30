namespace movies.api.Interfaces
{
    public interface IMovie
    {
        string Title { get; set; }
        string Year { get; set; }
        string ImdbId { get; set; }
        string Poster { get; set; }
    }
}