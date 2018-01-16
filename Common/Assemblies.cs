using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Common
{
    public static class Assemblies
    {
        public static Assembly BllAssembly { get; set; } = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", ConfigurationManager.AppSettings["BllPath"] ?? "BLL.dll"));
    }
}