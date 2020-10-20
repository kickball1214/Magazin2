using Data.Data.Connection;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var all = db.Products.ToList();
        }
    }
}
