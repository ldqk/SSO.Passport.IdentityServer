using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Models.Application;
using Models.Entity;
using Models.Enum;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataContext db = new DataContext();
            //List<ClientApp> apps = db.ClientApp.ToList();
            
        }
    }

    public class MyClass
    {
        public MyClass(string name)
        {
            Console.WriteLine(name);
        }
    }

    public class MyClass2:MyClass
    {
        public MyClass2(string name) : base(name)
        {
        }
    }
}