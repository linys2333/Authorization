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

            // ����˶�����Ϣ
            token.CreateDate = DateTime.Now;
            token.EffectSecond = TokenConfig.EffectSecond;

            TokenList.SafeAdd(tokenStr, token);

            return tokenStr;
        }

        public bool Check(string tokenStr)
        {
            // 1���Ƿ�洢�˸�token
            if (!TokenList.ContainsKey(tokenStr))
            {
                return false;
            }
            var tServer = Get(tokenStr);

            // 2��У�鴫��token�Ƿ�Ϸ�
            if (!_opr.Check(tokenStr, tServer.Salt))
            {
                return false;
            }
            var tUser = _opr.Decode(tokenStr);

            // 3��У��ͻ��˺ͷ������Ϣ�Գ�
            if (tUser.Payload.Timestamp != tServer.Payload.Timestamp)
            {
                return false;
            }

            // 4��У��������Ϣ
            if (!_opr.Check(tServer))
            {
                return false;
            }

            return true;
        }

        public string Update(string tokenStr)
        {
            JwtToken token = Get(tokenStr);

            // ����ʱ���
            token.Payload.Timestamp = DateTime.Now;

            // ���±���
            string tokenStrNew = _opr.Encode(token);

            Delete(tokenStr);
            TokenList.SafeAdd(tokenStrNew, token);

            return tokenStrNew;
        }

        public void Delete(string tokenStr) => TokenList.Remove(tokenStr);
    }
}