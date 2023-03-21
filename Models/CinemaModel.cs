
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CinemaSpace.Models;
public class MovieTicket
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Movie MovieInfo { get; set; } = Movie.EmptyMovie;
    public Hall HallInfo { get; set; } = Hall.EmptyHall;
}

public class Movie
{
    public static Movie EmptyMovie = new Movie();

    [Key]
    public string MovieName { get; set; } = "";
    public List<MovieTicket> Tickets { get; set; } = new();
}

public class Hall
{
    public static Hall EmptyHall = new Hall();
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HallNumber { get; set; } = 0;
    public Movie CurrentMovie = Movie.EmptyMovie;
    public int SeatsCount { get; set; } = 100;
    public int FreeSeatsCount { get; set; } = 100;
}

