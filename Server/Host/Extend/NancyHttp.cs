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
                // throw new Exception("不能获取名称为" + name + "的请求头");
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
                throw new Exception("不能获取名称为" + name + "的值");
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
        /// 按属性路径设置对象中某个属性的值，如果路径中的对象为空则创建
        /// 说明：
        /// 1、列表对象只支持List&lt;T&gt;类型。
        /// 2、动态构建的对象（为空时）需要有无参的公共构造函数，否则请在父级对象构造时一并构建。
        /// 3、设置对象值时将自动进行类型转换，如果无法自动转换，将使用类型默认值（不会抛出异常）。
        /// 
        /// 路径格式：   [PropertyName|Index]
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
                //处理数组索引
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
                        throw new Exception("索引错误");
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