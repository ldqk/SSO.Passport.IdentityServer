using System;
using System.Collections.Generic;
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
            Menu menu = db.Menu.FirstOrDefault(m => m.Name.Equals("用户管理"));
            Console.WriteLine("ok");
            Console.ReadKey();
        }
    }
}
