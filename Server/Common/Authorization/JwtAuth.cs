using System;
using System.Collections.Generic;
using AnyExtend;

namespace Common
{
    public class JwtAuth : IAuth
    {
        protected static IDictionary<string, JwtToken> TokenList { get; set; }
        private readonly JwtTokenOpr _opr;

        public JwtAuth(JwtTokenOpr opr)
        {
            _opr = opr;
            TokenList = new Dictionary<string, JwtToken>();
        }

        public JwtToken Get(string tokenStr) => TokenList[tokenStr];

        public string Create(object payload)
        {
            var token = _opr.Create(payload);
            var tokenStr = _opr.Encode(token);

            // 服务端额外信息
            token.CreateDate = DateTime.Now;
            token.EffectSecond = TokenConfig.EffectSecond;

            TokenList.SafeAdd(tokenStr, token);

            return tokenStr;
        }

        public bool Check(string tokenStr)
        {
            // 1、是否存储了该token
            if (!TokenList.ContainsKey(tokenStr))
            {
                return false;
            }
            var tServer = Get(tokenStr);

            // 2、校验传入token是否合法
            if (!_opr.Check(tokenStr, tServer.Salt))
            {
                return false;
            }
            var tUser = _opr.Decode(tokenStr);

            // 3、校验客户端和服务端信息对称
            if (tUser.Payload.Timestamp != tServer.Payload.Timestamp)
            {
                return false;
            }

            // 4、校验服务端信息
            if (!_opr.Check(tServer))
            {
                return false;
            }

            return true;
        }

        public string Update(string tokenStr)
        {
            JwtToken token = Get(tokenStr);

            // 更新时间戳
            token.Payload.Timestamp = DateTime.Now;

            // 重新编码
            string tokenStrNew = _opr.Encode(token);

            Delete(tokenStr);
            TokenList.SafeAdd(tokenStrNew, token);

            return tokenStrNew;
        }

        public void Delete(string tokenStr) => TokenList.Remove(tokenStr);
    }
}