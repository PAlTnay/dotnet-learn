using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

class Lab2 : UserCustom.Lab{
    public void Entry(){
        // using (var db = new SomeContext()) {

        //     Console.WriteLine($"Database path: {db.DbPath}.");

        //     Console.WriteLine("Inserting a new info");
        //     db.Add(new SomeInfo { SomeValue = 6 });
        //     db.SaveChanges();

        //     // Read
        //     Console.WriteLine("Querying for a someInfo");
            
        //     var someInfo = (from i in db.Info orderby i.Id select i).First();

        //     // Update
        //     Console.WriteLine("Updating the someInfo");
        //     someInfo.SomeValue = 100;
        //     db.SaveChanges();

        //     // Delete
        //     Console.WriteLine("Delete the someInfo");
        //     db.Remove(someInfo);
        //     db.SaveChanges();
        // }
    }
}






public class MovieTicketsContext :DbContext{

    public DbSet<MovieTicket> MovieTickets { get; set; }

    
    public string DbPath { get; }

    public MovieTicketsContext(){
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "MovieTickets.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options){
        options.UseSqlite($"Data Source={DbPath}");
    }

}

public class MovieTicket
{
    public int Id { get; set; }

    public string MovieName { get; set; }
    public int HallNumber { get; set; }
    public int SeatPlaceNumber { get; set; }
}