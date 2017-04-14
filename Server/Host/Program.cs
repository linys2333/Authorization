using Common.Interfaces;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using Common;

namespace Host
{
    class Program
    {
        static void Main()
        {
            using (var app = new WebApplication())
            {
                app.Start();
                Console.ReadLine();
            }
        }
    }

    public class WebApplication : IDisposable
    {
        private readonly string _url = $"http://+:{Config.Get("ServerPort")}";
        private IDisposable _webapp;

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            AddType();

            _webapp = WebApp.Start<Startup>(_url);

            var auth = Ioc.Resolve<IAuth>();
            auth.CleanTask();

            var log = Ioc.Resolve<ILog>();
            log.Info("服务已启动：" + _url);
        }

        public void Stop()
        {
            _webapp?.Dispose();
            _webapp = null;
        }

        public void AddType()
        {
            var types = new List<Type>
            {
                typeof(ConsoleLog)
            };

            Ioc.InitAndRegistCustomTypes(types.ToArray());
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app) => app.UseNancy();
    }
}
