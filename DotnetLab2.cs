using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

class Lab2 : UserCustom.Lab{
    public void Entry(){

    }
}



public class SomeContext :DbContext{
    public string DbPath { get; }

    public DbSet<SomeInfo> Info { get; set; }

    public SomeContext(){
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Some.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options){
        options.UseSqlite($"Data Source={DbPath}");
    }

    public class SomeInfo
    {
        public int SomeValue { get; set; }
    }

}