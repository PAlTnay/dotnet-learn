using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

class Lab2 : UserCustom.Lab{
    public void Entry(){
        Cinema cinema = new Cinema();
    }
}

public class Cinema
{
    private MovieTicketsContext ticketsContext;

    public Cinema()
    {
        ticketsContext = new MovieTicketsContext();
    }

    public void RemoveMovieSessions(string _MovieName)
    {
        Movie? movie = (from movieInfo in ticketsContext.Movies where movieInfo.MovieName == _MovieName select movieInfo).FirstOrDefault();
        if(movie is null) return;

        ticketsContext.MovieTickets.RemoveRange(from ticket in ticketsContext.MovieTickets where ticket.MovieInfo == movie select ticket);
        foreach(Hall curhHall in (from hall in ticketsContext.Halls where hall.CurrentMovie == movie select hall))
        {
            curhHall.CurrentMovie = null;
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

        (from hall in ticketsContext.Halls where hall.CurrentMovie == movie select hall).ToList().ForEach(hall=>hall.CurrentMovie = movie);
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
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(movie => movie.Tickets).WithOne(ticket => ticket.MovieInfo).IsRequired();
        modelBuilder.Entity<MovieTicket>().HasOne(ticket => ticket.MovieInfo).WithMany(movie => movie.Tickets).IsRequired();
        modelBuilder.Entity<MovieTicket>().HasOne(ticket => ticket.MovieHall).WithMany().IsRequired();
        modelBuilder.Entity<Hall>().HasOne(hall => hall.CurrentMovie).WithOne().IsRequired();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

}

[Table("MovieTicket")]
public class MovieTicket
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Movie MovieInfo { get; set; }
    public Hall MovieHall { get; set; }
}

[Table("MovieInfo")]
public class Movie
{
    [Key]
    public string MovieName { get; set; }
    public List<MovieTicket> Tickets {get; set; }
}

[Table("Hall")]
public class Hall
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HallNumber;
    public Movie? CurrentMovie;
    public int SeatsCount;
    public int FreeSeatsCount;
}


