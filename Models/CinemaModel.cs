
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CinemaSpace.Models;
public class MovieTicket
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("MovieInfo")]
    public string MovieName { get; set; } = "Empty";
    public Movie MovieInfo { get; set; } = Movie.EmptyMovie;
    [ForeignKey("HallInfo")]
    public int HallNumber { get; set; } = 0;
    public Hall HallInfo { get; set; } = Hall.EmptyHall;
    public int SeatNumber {get; set; } = 0;
}

public class Movie
{
    public static Movie EmptyMovie = new Movie();

    [Key]
    public string MovieName { get; set; } = "Empty";
}

public class Hall
{
    public static Hall EmptyHall = new Hall();
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HallNumber { get; set; } = 0;
    [ForeignKey("CurrentMovie")]
    public string MovieName { get; set; } = "";
    public Movie CurrentMovie = Movie.EmptyMovie;
    public int SeatsCount { get; set; } = 100;
    public int FreeSeatsCount { get; set; } = 100;
}

