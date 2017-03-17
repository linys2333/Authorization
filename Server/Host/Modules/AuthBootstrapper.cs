using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Host.Modules
{
    public class AuthBootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.ApplicationStartup(container, pipelines);
            
            pipelines.BeforeRequest += ctx =>
            {
                return null;
            };

            pipelines.AfterRequest += ctx =>
            {
                // 解决跨域请求问题
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*");
                ctx.Response.WithHeader("Access-Control-Allow-Headers", "Origin,X-Requested-With,Content-Type,Accept,Authorization");
                ctx.Response.WithHeader("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
                ctx.Response.WithHeader("Access-Control-Expose-Headers", "Authorization");
            };
        }
    }
}