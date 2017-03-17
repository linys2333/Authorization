using System;
using System.Collections.Generic;
using AnyExtend;
using Common;
using Common.Interfaces;
using Domain.User;

namespace Service.User
{
    [AppService("User")]
    public interface IUserAppService : IAppService
    {
        [HttpPost, PassAuth]
        void Login(UserDto dto);

        [HttpPost]
        void Logout();

        [HttpPost, PassAuth]
        string GetSecret(UserDto dto);
    }

    class UserAppService : IUserAppService
    {
        public static Dictionary<string, string> dicSecret;
        private readonly UserMng _userMng;

        public UserAppService(UserMng userMng)
        {
            _userMng = userMng;
        }

        public void Login(UserDto dto)
        {
            var user = _userMng.Load(dto.UserName);

            // 测试用（数据库密码存储采用 md5(pwd)，pwd是注册的原始密码）
            user.Pwd = CryptoExt.GetMd5Hash("", null, EncodeType.Hex);
            
            // 校验密码（前端密码加密格式为 md5(md5(pwd).salt)）
            string secrt = CryptoExt.GetMd5Hash(user.Pwd, dicSecret[dto.UserName], EncodeType.Hex);
            if (!secrt.EqualsNoCase(dto.Password))
            {
                dicSecret.Remove(dto.UserName);
                throw new ValidateException("用户名或密码错误！");
            }

            dicSecret.Remove(dto.UserName);
        }

        public void Logout()
        {
        }

        /// <summary>
        /// 创建随机字符串，用于前端密码加密
        /// </summary>
        public string GetSecret(UserDto dto)
        {
            if (dicSecret == null)
            {
                dicSecret = new Dictionary<string, string>();
            }

            string ctx = DateTime.Now.ToString();
            string result = EncodingExt.ToBase64Url(ctx);

            dicSecret.SafeAdd(dto.UserName, result);

            return result;
        }
    }
}
