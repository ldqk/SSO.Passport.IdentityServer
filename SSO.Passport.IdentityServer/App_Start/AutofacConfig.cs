using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Common;
using Masuit.Tools.NoSQL;
using Microsoft.AspNet.SignalR;
using Models.Application;
using SSO.Passport.IdentityServer.Hubs;

namespace SSO.Passport.IdentityServer
{
    /// <summary>
    /// autofac配置类
    /// </summary>
    public class AutofacConfig
    {
        public static IContainer Container { get; set; }
        /// <summary>
        /// 负责调用autofac实现依赖注入，负责创建MVC控制器类的对象(调用控制器的有参构造函数)，接管DefaultControllerFactory的工作
        /// </summary>
        public static void RegisterMVC()
        {
            //1.0 实例化autofac的创建容器
            var builder = new ContainerBuilder();

            //2.0 告诉autofac将来要创建的控制器类存放在哪个程序集
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            // 注册SignalR 集线器.
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterFilterProvider();

            //3.0 告诉autofac注册所有的Bll，创建类的实例，以该类的接口实现实例存储
            builder.RegisterType<DataContext>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(Assemblies.RepositoryAssembly).AsSelf().AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).InstancePerDependency();
            builder.RegisterAssemblyTypes(Assemblies.ServiceAssembly).AsSelf().AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).InstancePerDependency();
            builder.RegisterType<RedisHelper>().InstancePerRequest();
            builder.RegisterType<MyHub>().ExternallyOwned();

            //4.0 创建一个autofac的容器
            Container = builder.Build();

            //5.0 将当前容器交给MVC底层，保证容器不被销毁，控制器由autofac来创建
            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(Container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(Container);
        }

        //public static void RegistSignalR()
        //{
        //    var builder = new ContainerBuilder();

        //    // 注册SignalR 集线器.
        //    builder.RegisterHubs(Assembly.GetExecutingAssembly());
        //    // ...手动单个注册.
        //    builder.RegisterType<MyHub>().ExternallyOwned();
        //    //...注册其他类
        //    builder.RegisterType<DataContext>().InstancePerLifetimeScope();
        //    builder.RegisterAssemblyTypes(Assemblies.RepositoryAssembly).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).InstancePerDependency();
        //    builder.RegisterAssemblyTypes(Assemblies.ServiceAssembly).AsSelf().AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).InstancePerDependency();

        //    // 将依赖处理器设置成Autofac.
        //    var container = builder.Build();
        //    GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);
        //}
    }
}