using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;



namespace Lab2;
public class Lab2 : UserCustom.Lab{
    public void Entry(){
        Cinema cinema = new Cinema();
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
        ticketsContext.Add(new Hall
        {
            CurrentMovie = (from movie in ticketsContext.Movies orderby movie select movie).LastOrDefault(),
            SeatsCount = 100,
            FreeSeatsCount = 100
        });
        ticketsContext.Add(new Hall
        {

            SeatsCount = 100,
            FreeSeatsCount = 100
        });
        ticketsContext.SaveChanges();

        ticketsContext.Add(new Hall
        {

            SeatsCount = 100,
            FreeSeatsCount = 100
        });
        ticketsContext.SaveChanges();
    }


    public void RemoveMovieSessions(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if(movie is null) return;

        ticketsContext.MovieTickets.RemoveRange(from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie select ticket);
        foreach(Hall curhHall in (from hall in ticketsContext.Halls where hall.CurrentMovie == movie select hall))
        {
            curhHall.CurrentMovie = Movie.EmptyMovie;
        }
        
        ticketsContext.Movies.Remove(movie);
        ticketsContext.SaveChanges();
    }

    public void AddTicket(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if(movie is null) return;
        
        var AvailableHalls = from hall in ticketsContext.Halls where hall.CurrentMovie == movie && hall.FreeSeatsCount < hall.SeatsCount select hall;
        if(AvailableHalls.Count() <= 0) return;
        
        Hall? AvailableHall = AvailableHalls.ElementAtOrDefault(Random.Shared.Next(AvailableHalls.Count()));
        if(AvailableHall is null) return;

        --AvailableHall.FreeSeatsCount;

        ticketsContext.MovieTickets.Add(
            new MovieTicket
            { 
                MovieInfo = movie,
                MovieHall = AvailableHall
            }
        );
        ticketsContext.SaveChanges();
    }

    public void UpdateHallMovieSession(string _MovieName, int HallNumber)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if(movie is null) return;

        (from hall in ticketsContext.Halls where hall.HallNumber == HallNumber select hall).ToList().ForEach(hall=>hall.CurrentMovie = movie);

        ticketsContext.MovieTickets.RemoveRange(from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie && ticket.MovieHall.HallNumber == HallNumber select ticket);
        ticketsContext.SaveChanges();
    }
}



public class MovieTicketsContext :DbContext{

    public DbSet<MovieTicket> MovieTickets { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Hall> Halls { get; set; }
    
    public string DbPath { get; }

    public MovieTicketsContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "MovieTickets.db");
        
        Database.EnsureDeleted();
        Database.EnsureCreated();
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(movie => movie.Tickets).WithOne(ticket => ticket.MovieInfo).IsRequired();
        modelBuilder.Entity<MovieTicket>().HasOne(ticket => ticket.MovieInfo).WithMany(movie => movie.Tickets).IsRequired();
        modelBuilder.Entity<MovieTicket>().HasOne(ticket => ticket.MovieHall).WithMany().IsRequired();
        modelBuilder.Entity<Hall>().HasKey(hall => hall.HallNumber);
        modelBuilder.Entity<Hall>().HasOne<Movie>(hall => hall.CurrentMovie).WithMany().IsRequired();
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
    public Hall MovieHall { get; set; }
}

public class Movie
{
    public static Movie EmptyMovie = new Movie();

    [Key]
    public string MovieName { get; set; } = "";
    public List<MovieTicket> Tickets { get; set; } = new ();
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

