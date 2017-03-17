namespace Common
{
    /// <summary>
    /// Token����
    /// </summary>
    public interface IToken
    {
    }

    /// <summary>
    /// Token����
    /// </summary>
    public interface ITokenOpr<T> where T : IToken
    {
        /// <summary>
        /// ����Token
        /// </summary>
        T Create(object token);

        /// <summary>
        /// У��Token�Ƿ�Ϸ�������У��ͻ�����Ϣ��
        /// </summary>
        /// <param name="tokenStr">���ܺ���ַ���</param>
        bool Check(string tokenStr, string salt);

        /// <summary>
        /// У��Token�Ƿ�Ϸ�������У��������Ϣ��
        /// </summary>
        /// <param name="token">������Token����</param>
        bool Check(T token);

        /// <summary>
        /// ���벢����Token
        /// </summary>
        /// <param name="token">�����ܵ�Token����</param>
        string Encode(T token);

        /// <summary>
        /// ����Token
        /// </summary>
        /// <param name="tokenStr">���ܺ���ַ���</param>
        T Decode(string tokenStr);
    }

    /// <summary>
    /// �����֤
    /// </summary>
    public interface IAuth
    {
        /// <summary>
        /// ����Token
        /// </summary>
        string Create(object token);

        /// <summary>
        /// У��Token�Ƿ�Ϸ�
        /// </summary>
        /// <param name="tokenStr">���ܺ���ַ���</param>
        bool Check(string tokenStr);

        /// <summary>
        /// ����Token������Ϣ
        /// </summary>
        /// <param name="tokenStr">���ܺ���ַ���</param>
        string Update(string tokenStr);

        /// <summary>
        /// ����ָ��Token
        /// </summary>
        /// <param name="tokenStr">���ܺ���ַ���</param>
        void Delete(string tokenStr);
    }
}