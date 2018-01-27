using System;
using System.Linq;
using Models.Application;
using Models.Entity;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DataContext db = new DataContext();
            Permission p = db.Permission.LastOrDefault();
            IQueryable<Control> controls = db.Control.Where(c => c.Permission.Any(x=>x.Id==p.Id));
            Console.ReadKey();
        }
    }
}