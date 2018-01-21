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
            DataContext db = new DataContext();
            List<ClientApp> apps = db.ClientApp.ToList();
            
        }
    }
}