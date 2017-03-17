using System;

namespace Common
{
    public class AuthException : Exception
    {
        public AuthException(string msg = "身份验证失败") : base(msg)
        {
        }
    }
}
