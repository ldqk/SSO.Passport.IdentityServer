using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace SSO.Passport.IdentityServer
{
    /// <summary>
    /// autofac配置类
    /// </summary>
    public class AutofacConfig
    {
        /// <summary>
        /// 负责调用autofac实现依赖注入，负责创建MVC控制器类的对象(调用控制器的有参构造函数)，接管DefaultControllerFactory的工作
        /// </summary>
        public static void Register()
        {
            //1.0 实例化autofac的创建容器
            var builder = new ContainerBuilder();

            //2.0 告诉autofac将来要创建的控制器类存放在哪个程序集
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            //3.0 告诉autofac注册所有的Bll，创建类的实例，以该类的接口实现实例存储
            builder.RegisterTypes(Assembly.Load("BLL").GetTypes()).AsImplementedInterfaces();

            //4.0 创建一个autofac的容器
            IContainer container = builder.Build();

            //5.0 将当前容器交给MVC底层，保证容器不被销毁，控制器由autofac来创建
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}