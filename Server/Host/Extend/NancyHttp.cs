using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AnyExtend;
using Nancy;

namespace Host
{
    public static class NancyHttp
    {
        public static string GetHeader(this Request requset, string name, int index = 0)
        {
            var headers = requset.GetHeaders(name);
            return -1 < index && index < headers.Count() ? headers.ToList()[index] : "";
        }

        public static IEnumerable<string> GetHeaders(this Request requset, string name)
        {
            var value = requset.Headers[name];
            if (value == null)
            {
                // throw new Exception("���ܻ�ȡ����Ϊ" + name + "������ͷ");
                return new List<string>();
            }
            return value;
        }

        public static void SetHeader(this Response reponse, string key, string value)
        {
            reponse.Headers.SafeAdd(key, value);
        }

        public static object GetValue(this Request requset, string name, Type type)
        {
            if (type != typeof(string) && type.IsClass)
            {
                return requset.GetObject(name, type);
            }

            string value = requset.Form[name];
            if (value == null)
            {
                value = requset.Query[name];
            }
            if (value == null)
            {
                throw new Exception("���ܻ�ȡ����Ϊ" + name + "��ֵ");
            }

            if (type == typeof(string)
                || type == typeof(char)
                || type == typeof(bool)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(Guid)
                || type == typeof(DateTime))
            {
                return TypeExt.ConvertType(type, value);
            }

            return SerializeExt.JsonTo(value, type);
        }

        public static object GetObject(this Request requset, string name, Type type)
        {
            var result = Activator.CreateInstance(type);
            foreach (string key in requset.Form)
            {
                if (key.Length < name.Length) continue;
                if (!key.Substring(0, name.Length).Equals(name, StringComparison.CurrentCultureIgnoreCase)) continue;

                var value = requset.Form[key];
                if (value.HasValue)
                {
                    SetValue(result, key.Substring(name.Length), value.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// ������·�����ö�����ĳ�����Ե�ֵ�����·���еĶ���Ϊ���򴴽�
        /// ˵����
        /// 1���б����ֻ֧��List&lt;T&gt;���͡�
        /// 2����̬�����Ķ���Ϊ��ʱ����Ҫ���޲εĹ������캯�����������ڸ���������ʱһ��������
        /// 3�����ö���ֵʱ���Զ���������ת��������޷��Զ�ת������ʹ������Ĭ��ֵ�������׳��쳣����
        /// 
        /// ·����ʽ��   [PropertyName|Index]
        /// eg:        [CostList][1][Name]
        /// 
        /// </summary>
        /// <param propertyPath="obj"></param>
        /// <param propertyPath="propertyPath"></param>
        /// <param propertyPath="value"></param>
        public static void SetValue(object obj, string propertyPath, object value)
        {
            var match = Regex.Match(propertyPath, @"^\[(\w+)\]");
            if (!match.Success) return;

            var type = obj.GetType();
            var pname = match.Groups[1].Value;

            if (Regex.IsMatch(pname, @"^\d+$"))
            {
                //������������
                var index = int.Parse(pname);

                if (type.IsGenericType
                    && Array.Exists(type.GetInterfaces(), t => t.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    var genericArgs = type.GetGenericArguments();
                    var itemType = genericArgs[0];
                    var listobj = obj as IList;

                    object item;
                    if (index == listobj.Count)
                    {
                        item = Activator.CreateInstance(itemType);
                        listobj.Add(item);
                    }
                    else if (index < listobj.Count)
                    {
                        item = listobj[index];
                    }
                    else
                    {
                        throw new Exception("��������");
                    }

                    SetValue(item, propertyPath.Substring(match.Length), value);
                }
            }

            var prop = type
                .GetProperty(pname,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Public);
            if (prop == null) return;
            var ptype = prop.PropertyType;

            if (ptype.IsClass && ptype != typeof(string))
            {
                var pvalue = prop.GetValue(obj);
                if (pvalue == null)
                {
                    pvalue = Activator.CreateInstance(ptype);
                    prop.SetValue(obj, pvalue);
                }
                SetValue(pvalue, propertyPath.Substring(match.Length), value);
            }
            else
            {
                prop.SetValue(obj, TypeExt.ConvertType(ptype, value));
            }
        }
    }
}