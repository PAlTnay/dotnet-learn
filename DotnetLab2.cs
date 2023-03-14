using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace Lab2;
public class Lab2 : UserCustom.Lab
{
    public void Entry()
    {

    }
}

public class Cinema
{
    public MovieTicketsContext ticketsContext { get; }
    public Cinema()
    {
        ticketsContext = new MovieTicketsContext();
        ticketsContext.Movies.Add(new Movie { MovieName = "mvi1" });
        ticketsContext.Movies.Add(new Movie { MovieName = "mvi2" });
        ticketsContext.Movies.Add(new Movie { MovieName = "mvi3" });
        ticketsContext.Movies.Add(new Movie { MovieName = "mvi4" });
        ticketsContext.SaveChanges();
        ticketsContext.Halls.Add(new Hall
        {
            CurrentMovie = (from movie in ticketsContext.Movies orderby movie select movie).LastOrDefault(),
            SeatsCount = 100,
            FreeSeatsCount = 100
        });
        ticketsContext.SaveChanges();
    }

    public void RemoveMovie(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if (movie is null) return;

        ticketsContext.MovieTickets.RemoveRange(from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie select ticket);

        foreach (var hall in ticketsContext.Halls)
        {
            if (hall.CurrentMovie == movie)
            {
                hall.CurrentMovie = Movie.EmptyMovie;
                hall.FreeSeatsCount = hall.SeatsCount;
            }
        }

        ticketsContext.Movies.Remove(movie);
        ticketsContext.SaveChanges();
    }

    public void AddMovie(string _MovieName)
    {
        if ((from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).Count() > 0) return;

        ticketsContext.Movies.Add(new Movie { MovieName = _MovieName });
        ticketsContext.SaveChanges();
    }

    public void AddTicket(string _MovieName, int HallNumber)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if (movie is null) return;

        Hall? choosenHall = (from hall in ticketsContext.Halls where hall.HallNumber == HallNumber select hall).FirstOrDefault();
        if (choosenHall is null) return;

        if (choosenHall.CurrentMovie != movie) return;

        --choosenHall.FreeSeatsCount;

        ticketsContext.MovieTickets.Add(
            new MovieTicket
            {
                MovieInfo = movie,
                HallInfo = choosenHall
            }
        );
        ticketsContext.SaveChanges();
    }
    public void AddTicket(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if (movie is null) return;

        Hall? choosenHall = (from hall in ticketsContext.Halls where hall.CurrentMovie == movie select hall).FirstOrDefault();
        if (choosenHall is null) return;

        --choosenHall.FreeSeatsCount;

        ticketsContext.MovieTickets.Add(
            new MovieTicket
            {
                MovieInfo = movie,
                HallInfo = choosenHall
            }
        );
        ticketsContext.SaveChanges();
    }

    public void UpdateHallMovieSession(int HallNumber, string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if (movie is null) return;

        Hall? choosenHall = (from hall in ticketsContext.Halls where hall.HallNumber == HallNumber select hall).FirstOrDefault();
        if (choosenHall is null) return;

        choosenHall.CurrentMovie = movie;
        choosenHall.FreeSeatsCount = choosenHall.SeatsCount - (from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie && ticket.HallInfo == choosenHall select ticket).Count();
        ticketsContext.MovieTickets.RemoveRange(from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie select ticket);
        ticketsContext.SaveChanges();
    }
}



public class MovieTicketsContext : DbContext
{

    public DbSet<MovieTicket> MovieTickets { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public Hall? Hall { get => (from hall in Halls select hall).FirstOrDefault(); }

    public string DbPath { get; }

    public MovieTicketsContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "MovieTickets.db");
        Console.WriteLine(DbPath);
        Database.EnsureDeleted();
        Database.EnsureCreated();
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(movie => movie.Tickets).WithOne(ticket => ticket.MovieInfo).IsRequired();
        modelBuilder.Entity<MovieTicket>().HasOne(ticket => ticket.MovieInfo).WithMany(movie => movie.Tickets).IsRequired();
        modelBuilder.Entity<Hall>().HasKey(hall => hall.HallNumber);
        modelBuilder.Entity<Hall>().HasOne(hall => hall.CurrentMovie).WithMany().IsRequired();
        modelBuilder.Entity<MovieTicket>().HasOne(ticket => ticket.HallInfo).WithMany().IsRequired();
        modelBuilder.Entity<MovieTicket>().HasKey(ticket => ticket.Id);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}

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

