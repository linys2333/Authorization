using Common.Interfaces;
using System;

namespace Service.User
{
    [Serializable]
    public class UserDto : IDto
    {
        public string TenantName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
