using Nancy;
using System;
using System.Collections.Generic;
using System.Reflection;
using AnyExtend;
using Common;
using Common.Interfaces;
using Nancy.Responses;

namespace Host
{
    public class ApiModule : NancyModule
    {
        private static IAuth _auth = Ioc.Resolve<IAuth>();
        private static ILog _log = Ioc.Resolve<ILog>();
        private Dictionary<string, string> _httpHeaders;

        static ApiModule()
        {
        }

        public ApiModule() : base("/api")
        {
            Post("User/Login", pms =>
            {
                string serviceName = "User";
                string methodName = "Login";
                var response = Handle(serviceName, methodName, "Post");
                return HandleLogin(response);
            });
            
            Post("User/Logout", pms =>
            {
                string serviceName = "User";
                string methodName = "Logout";
                var response = Handle(serviceName, methodName, "Post");
                return HandleLogout(response);
            });

            Get("/{service}/{method}", pms =>
            {
                string serviceName = pms.service;
                string methodName = pms.method;
                var response = Handle(serviceName, methodName, "GET");
                return HandleResponse(response);
            });

            Post("/{service}/{method}", pms =>
            {
                string serviceName = pms.service;
                string methodName = pms.method;
                var response = Handle(serviceName, methodName, "POST");
                return HandleResponse(response);
            });

        }

        /// <summary>
        /// 一般请求的响应处理
        /// </summary>
        private Response HandleResponse(Response response)
        {
            // 如果是401，则按退出登录处理
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return HandleLogout(response);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string token = _httpHeaders["Authorization"];
                if (!token.IsEmpty())
                {
                    response.SetHeader("Authorization", token);
                }
            }

            return response;
        }

        /// <summary>
        /// 登录请求的响应处理
        /// </summary>
        private Response HandleLogin(Response response)
        {
            CreateToken();
            return HandleResponse(response);
        }

        /// <summary>
        /// 退出登录请求的响应处理
        /// </summary>
        private Response HandleLogout(Response response)
        {
            string token = _httpHeaders["Authorization"];
            _auth.Delete(token);
            
            response.Headers.Remove("Authorization");
            return response;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        private Response Handle(string serviceName, string methodName, string httpType)
        {
            // 缓存请求头
            _httpHeaders = new Dictionary<string, string>
            {
                { "Authorization", Request.GetHeader("Authorization") ?? "" }
            };

#if DEBUG
            _log.Debug(Request.Url);
#endif

            Type interfaceType;
            var service = Ioc.ResolveAppService(serviceName, out interfaceType);
            if (service == null)
            {
                return "Invalid Service";
            }

            var methodInfo = interfaceType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            
            try
            {
                // 校验
                CheckRequest(methodInfo, httpType);

                // 若校验通过，则更新Token，防止重复请求
                UpdateToken();

                var res = InvokeMethod(methodInfo, service);

#if DEBUG
                _log.Debug(">>" + SerializeExt.ToJson(res, false));
#endif

                return Response.AsJson(ApiResponse.SuccessResponse(res));
            }
            catch (BusinessException ex)
            {
                _log.Info(ex, "业务异常");
                return Response.AsJson(ApiResponse.FailResponse(ex.Message));
            }
            catch (AuthException ex)
            {
                _log.Warning(ex, "身份验证异常");
                return Response.AsJson(ApiResponse.FailResponse(ex.Message), HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "请求发生异常");
                return Response.AsJson(ApiResponse.FailResponse("系统异常"), HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// 校验请求
        /// </summary>
        private void CheckRequest(MethodInfo method, string httpType)
        {
            // 1、校验请求类型
            switch (httpType)
            {
                case "GET":
                    if (method.GetCustomAttribute<HttpPostAttribute>() != null)
                    {
                        throw new Exception("方法不支持GET请求");
                    }
                    break;
                case "POST":
                    if (method.GetCustomAttribute<HttpPostAttribute>() == null)
                    {
                        throw new Exception("方法不支持POST请求");
                    }
                    break;
            }

            // 2、身份验证
            string token = _httpHeaders["Authorization"];
            if (!token.IsEmpty() || method.GetCustomAttribute<PassAuthAttribute>() == null)
            {
                if (token.IsEmpty() || !_auth.Check(token))
                {
                    throw new AuthException();
                }
            }
        }

        /// <summary>
        /// 更新Token（校验通过后调用）
        /// </summary>
        private void UpdateToken()
        {
            string token = _httpHeaders["Authorization"];

            // 每次请求都会更新token，防止(伪造)重复请求
            if (!token.IsEmpty())
            {
                _httpHeaders["Authorization"] = _auth.Update(token);
            }
        }

        /// <summary>
        /// 创建Token
        /// </summary>
        private void CreateToken()
        {
            // 提供给客户端的信息
            var payload = new JwtPayload
            {
                Timestamp = DateTime.Now,
                IP = Request.UserHostAddress
            };

            _httpHeaders.SafeAdd("Authorization", _auth.Create(payload));
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        private object InvokeMethod(MethodInfo method, object serviceObj)
        {
            var paramsList = method.GetParameters();
            var argList = new object[paramsList.Length];

            for (var i = 0; i < paramsList.Length; i++)
            {
                var pm = paramsList[i];
                var name = pm.Name;
                var type = pm.ParameterType;

                argList[i] = Request.GetValue(name, type);
            }

            try
            {
                return method.Invoke(serviceObj, argList);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}