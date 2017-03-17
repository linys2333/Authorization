namespace Common
{
    /// <summary>
    /// Token构成
    /// </summary>
    public interface IToken
    {
    }

    /// <summary>
    /// Token操作
    /// </summary>
    public interface ITokenOpr<T> where T : IToken
    {
        /// <summary>
        /// 创建Token
        /// </summary>
        T Create(object token);

        /// <summary>
        /// 校验Token是否合法（用于校验客户端信息）
        /// </summary>
        /// <param name="tokenStr">加密后的字符串</param>
        bool Check(string tokenStr, string salt);

        /// <summary>
        /// 校验Token是否合法（用于校验服务端信息）
        /// </summary>
        /// <param name="token">解码后的Token对象</param>
        bool Check(T token);

        /// <summary>
        /// 编码并加密Token
        /// </summary>
        /// <param name="token">待加密的Token对象</param>
        string Encode(T token);

        /// <summary>
        /// 解码Token
        /// </summary>
        /// <param name="tokenStr">加密后的字符串</param>
        T Decode(string tokenStr);
    }

    /// <summary>
    /// 身份验证
    /// </summary>
    public interface IAuth
    {
        /// <summary>
        /// 创建Token
        /// </summary>
        string Create(object token);

        /// <summary>
        /// 校验Token是否合法
        /// </summary>
        /// <param name="tokenStr">加密后的字符串</param>
        bool Check(string tokenStr);

        /// <summary>
        /// 更新Token部分信息
        /// </summary>
        /// <param name="tokenStr">加密后的字符串</param>
        string Update(string tokenStr);

        /// <summary>
        /// 销毁指定Token
        /// </summary>
        /// <param name="tokenStr">加密后的字符串</param>
        void Delete(string tokenStr);
    }
}