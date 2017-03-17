using AnyExtend;

namespace Common
{
    /// <summary>
    /// Token配置信息，存储在App.config中
    /// </summary>
    public class TokenConfig : Config
    {
        public static string BaseSalt => Get("Token.BaseSalt");
        public static string Alg => Get("Token.Alg");
        public static string Type => Get("Token.Type");
        public static int EffectSecond => TypeExt.ConvertType<int>(Get("Token.EffectSecond"));
    }
}