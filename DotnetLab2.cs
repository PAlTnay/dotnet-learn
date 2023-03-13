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
        Cinema cinema = new Cinema();
        while(true)
        {
            Console.Write("Current movie: {0}\nSeats count: {1}\nFree seats count: {2}\n",
                cinema.ticketsContext.Hall.CurrentMovie.MovieName, cinema.ticketsContext.Hall.SeatsCount, cinema.ticketsContext.Hall.FreeSeatsCount);
            Console.Write("1: Add ticket \n2: Choose movie\n3: Add movie\nq: exit\n");
            string? line = Console.ReadLine();
            if (line == "q") break;
            switch (line) {
                case "1":
                    Console.WriteLine("Choose movie:");
                    foreach(var movie in cinema.ticketsContext.Movies)
                    {
                        Console.WriteLine(movie.MovieName);
                    }
                    cinema.AddTicket(Console.ReadLine());
                    break;
                case "2":
                    Console.WriteLine("Choose movie:");
                    foreach (var movie in cinema.ticketsContext.Movies)
                    {
                        Console.WriteLine(movie.MovieName);
                    }
                    cinema.UpdateHallMovieSession(Console.ReadLine());
                    break;
                case "3":
                    Console.WriteLine("Print movie:");
                    cinema.AddMovie(Console.ReadLine());
                    break;
                default: break;
            }
        }
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

        ticketsContext.Hall.CurrentMovie = Movie.EmptyMovie;


        ticketsContext.Movies.Remove(movie);
        ticketsContext.SaveChanges();
    }

    public void AddMovie(string _MovieName)
    {
        if ((from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).Count() > 0) return;

        ticketsContext.Movies.Add(new Movie { MovieName = _MovieName });
        ticketsContext.SaveChanges();
    }

    public void AddTicket(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if (movie is null) return;

        if (ticketsContext.Hall.CurrentMovie != movie) return;


        --ticketsContext.Hall.FreeSeatsCount;

        ticketsContext.MovieTickets.Add(
            new MovieTicket
            {
                MovieInfo = movie
            }
        );
        ticketsContext.SaveChanges();
    }

    public void UpdateHallMovieSession(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if (movie is null) return;

        ticketsContext.Hall.CurrentMovie = movie;
        ticketsContext.Hall.FreeSeatsCount = ticketsContext.Hall.SeatsCount - (from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie select ticket).Count();
        ticketsContext.MovieTickets.RemoveRange(from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie select ticket);
        ticketsContext.SaveChanges();
    }
}



public class MovieTicketsContext : DbContext
{

    public DbSet<MovieTicket> MovieTickets { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public Hall Hall { get => (from hall in Halls select hall).FirstOrDefault(); }

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
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HallNumber { get; set; }
    public Movie CurrentMovie = Movie.EmptyMovie;
    public int SeatsCount { get; set; }
    public int FreeSeatsCount { get; set; }
}

