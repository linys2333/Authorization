using System;

namespace Common
{
    public class AppServiceAttribute : Attribute
    {
        public string Name { get; }

        public AppServiceAttribute(string name)
        {
            Name = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : Attribute { }

    /// <summary>
    /// 标识哪些方法无需做身份验证，比如登录，但绝大部分都是要验证的
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class PassAuthAttribute : Attribute { }
}
