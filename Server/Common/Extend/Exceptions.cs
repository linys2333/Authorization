using System;

namespace Common
{
    public class BusinessException : Exception
    {
        public BusinessException(string msg) : base(msg) { }
    }

    public class ValidateException : BusinessException
    {
        public ValidateException(string msg) : base(msg) { }
    }
}
