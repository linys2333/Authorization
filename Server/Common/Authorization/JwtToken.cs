using System;
using AnyExtend;

namespace Common
{
    /// <summary>
    /// Tokenͷ��
    /// </summary>
    [Serializable]
    public class JwtHeader
    {
        /// <summary>
        /// �����㷨
        /// </summary>
        public string Alg { get; set; }

        /// <summary>
        /// Token����
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Token�غ�
    /// </summary>
    [Serializable]
    public class JwtPayload
    {
        /// <summary>
        /// ʱ���
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ����IP��ַ
        /// </summary>
        public string IP { get; set; }
    }

    [Serializable]
    public class JwtToken : IToken
    {
        #region ����˴洢����Ϣ���Կͻ�������

        /// <summary>
        /// ������
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ��Чʱ�䣨�룩
        /// </summary>
        public int EffectSecond { get; set; }

        #endregion

        #region �ṩ���ͻ��˵���Ϣ

        public JwtHeader Header { get; set; }
        public JwtPayload Payload { get; set; }

        #endregion
    }

    public class JwtTokenOpr : ITokenOpr<JwtToken>
    {
        public string ComputeHash(string header, string payload, string salt)
        {
            return CryptoExt.GetHmacHash(TokenConfig.Alg, $"{header}.{payload}", salt, EncodeType.Encode);
        }

        public JwtToken Create(object payload)
        {
            return new JwtToken
            {
                Header = new JwtHeader
                {
                    Alg = TokenConfig.Alg,
                    Type = TokenConfig.Type
                },
                Payload = payload.DeepClone<JwtPayload>(),
                Salt = TokenConfig.BaseSalt + new Random().Next()
            };
        }

        public bool Check(string tokenStr, string salt)
        {
            // 1����ʽ�Ƿ�Ϸ�
            string[] parts = tokenStr.Split(".");
            if (parts.Length != 3)
            {
                return false;
            }

            // 2��ǩ���Ƿ��Ӧ
            string signature = EncodingExt.FromBase64Url(parts[2], null);
            if (!ComputeHash(parts[0], parts[1], salt).EqualsNoCase(signature))
            {
                return false;
            }

            // 3��ͷ���Ƿ�Ϸ�
            var header = Decode(tokenStr).Header;
            if (!header.Alg.EqualsNoCase(TokenConfig.Alg) || !header.Type.EqualsNoCase(TokenConfig.Type))
            {
                return false;
            }

            return true;
        }

        public bool Check(JwtToken token)
        {
            var payload = token.Payload;

            // 1���Ƿ����
            double second = (DateTime.Now - payload.Timestamp).TotalSeconds;
            if (second < 0 || second > token.EffectSecond || second > TokenConfig.EffectSecond)
            {
                return false;
            }

            return true;
        }

        public string Encode(JwtToken token)
        {
            // ���ڣ��ṩ���ͻ��˵ģ�������Ϣ����
            string header = EncodingExt.ToBase64Url(SerializeExt.ToJson(token.Header));
            string payload = EncodingExt.ToBase64Url(SerializeExt.ToJson(token.Payload));

            // ����ǩ��
            string signature = EncodingExt.ToBase64Url(ComputeHash(header, payload, token.Salt));
            
            return $"{header}.{payload}.{signature}";
        }

        public JwtToken Decode(string tokenStr)
        {
            string[] parts = tokenStr.Split(".");

            var header = SerializeExt.JsonTo<JwtHeader>(EncodingExt.FromBase64Url(parts[0], null));
            var payload = SerializeExt.JsonTo<JwtPayload>(EncodingExt.FromBase64Url(parts[1], null));

            return new JwtToken
            {
                Header = header,
                Payload = payload
            };
        }
    }
}