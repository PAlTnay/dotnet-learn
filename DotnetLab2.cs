using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

class Lab2 : UserCustom.Lab{
    public void Entry(){
        using (var db = new SomeContext()) {

            Console.WriteLine($"Database path: {db.DbPath}.");

            Console.WriteLine("Inserting a new info");
            db.Add(new SomeInfo { SomeValue = 6 });
            db.SaveChanges();

            // Read
            Console.WriteLine("Querying for a someInfo");
            
            var someInfo = (from i in db.Info orderby i.Id select i).First();

            // Update
            Console.WriteLine("Updating the someInfo");
            someInfo.SomeValue = 100;
            db.SaveChanges();

            // Delete
            Console.WriteLine("Delete the someInfo");
            db.Remove(someInfo);
            db.SaveChanges();
        }
    }
}



public class SomeContext :DbContext{

    public DbSet<SomeInfo> Info { get; set; }

    
    public string DbPath { get; }

    public SomeContext(){
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Some.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options){
        options.UseSqlite($"Data Source={DbPath}");
    }

}

public class SomeInfo
{
    //[System.ComponentModel.DataAnnotations.Key]
    public int Id { get; set; }
    public int SomeValue { get; set; }
}