using AnyExtend;

namespace Common
{
    /// <summary>
    /// Token������Ϣ���洢��App.config��
    /// </summary>
    public class TokenConfig : Config
    {
        public static string BaseSalt => Get("Token.BaseSalt");
        public static string Alg => Get("Token.Alg");
        public static string Type => Get("Token.Type");
        public static int EffectSecond => TypeExt.ConvertType<int>(Get("Token.EffectSecond"));
    }
}