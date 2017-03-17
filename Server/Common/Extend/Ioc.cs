using Autofac;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public static class Ioc
    {
        private static IContainer _container;
        private static IDictionary<string, Type> _appServiceDic;
        
        public static void InitAndRegistCustomTypes(params Type[] customRegistTypes)
        {
            var builder = new ContainerBuilder();
            
            var assemblies = "Common,Domain,Service,EF"
                .Split(',')
                .Select(Assembly.Load)
                .ToArray();

            // 注册该程序集中的所有类
            builder.RegisterAssemblyTypes(assemblies)
                // 将类映射到继承的接口
                .AsImplementedInterfaces()
                // 将类映射到自身
                .AsSelf();
            
            if (customRegistTypes != null && customRegistTypes.Length > 0)
            {
                builder.RegisterTypes(customRegistTypes).AsImplementedInterfaces();
            }

            _container = builder.Build();

            InitAppService();
        }

        private static void InitAppService()
        {
            _appServiceDic = new Dictionary<string, Type>();
            var serviceAss = Assembly.Load("Service");
            foreach (var type in serviceAss.GetTypes())
            {
                if (type.IsInterface && type.IsAssignableTo<IAppService>())
                {
                    var appService = type.GetCustomAttribute<AppServiceAttribute>(false);
                    if (appService != null)
                    {
                        _appServiceDic.Add(appService.Name, type);
                    }
                }
            }
        }

        public static bool TryResolve<T>(out T instance)
        {
            return _container.TryResolve<T>(out instance);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public static object Resolve(string typeName)
        {
            return Resolve(Type.GetType(typeName));
        }

        public static object ResolveAppService(string serviceName, out Type interfaceType)
        {
            if (!_appServiceDic.ContainsKey(serviceName))
            {
                interfaceType = null;
                return null;
            }
            interfaceType = _appServiceDic[serviceName];
            return Resolve(interfaceType);
        }
    }
}
