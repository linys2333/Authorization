using System;
using AnyExtend;

namespace Common
{
    /// <summary>
    /// Token头部
    /// </summary>
    [Serializable]
    public class JwtHeader
    {
        /// <summary>
        /// 加密算法
        /// </summary>
        public string Alg { get; set; }

        /// <summary>
        /// Token类型
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Token载荷
    /// </summary>
    [Serializable]
    public class JwtPayload
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 请求方IP地址
        /// </summary>
        public string IP { get; set; }
    }

    [Serializable]
    public class JwtToken : IToken
    {
        #region 服务端存储的信息，对客户端隐藏

        /// <summary>
        /// 加密盐
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 有效时间（秒）
        /// </summary>
        public int EffectSecond { get; set; }

        #endregion

        #region 提供给客户端的信息

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
            // 1、格式是否合法
            string[] parts = tokenStr.Split(".");
            if (parts.Length != 3)
            {
                return false;
            }

            // 2、签名是否对应
            string signature = EncodingExt.FromBase64Url(parts[2], null);
            if (!ComputeHash(parts[0], parts[1], salt).EqualsNoCase(signature))
            {
                return false;
            }

            // 3、头部是否合法
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

            // 1、是否过期
            double second = (DateTime.Now - payload.Timestamp).TotalSeconds;
            if (second < 0 || second > token.EffectSecond || second > TokenConfig.EffectSecond)
            {
                return false;
            }

            return true;
        }

        public string Encode(JwtToken token)
        {
            // 基于（提供给客户端的）基础信息编码
            string header = EncodingExt.ToBase64Url(SerializeExt.ToJson(token.Header));
            string payload = EncodingExt.ToBase64Url(SerializeExt.ToJson(token.Payload));

            // 生成签名
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