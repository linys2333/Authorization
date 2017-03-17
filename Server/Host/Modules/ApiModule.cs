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
        /// һ���������Ӧ����
        /// </summary>
        private Response HandleResponse(Response response)
        {
            // �����401�����˳���¼����
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
        /// ��¼�������Ӧ����
        /// </summary>
        private Response HandleLogin(Response response)
        {
            CreateToken();
            return HandleResponse(response);
        }

        /// <summary>
        /// �˳���¼�������Ӧ����
        /// </summary>
        private Response HandleLogout(Response response)
        {
            string token = _httpHeaders["Authorization"];
            _auth.Delete(token);
            
            response.Headers.Remove("Authorization");
            return response;
        }

        /// <summary>
        /// ��������
        /// </summary>
        private Response Handle(string serviceName, string methodName, string httpType)
        {
            // ��������ͷ
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
                // У��
                CheckRequest(methodInfo, httpType);

                // ��У��ͨ���������Token����ֹ�ظ�����
                UpdateToken();

                var res = InvokeMethod(methodInfo, service);

#if DEBUG
                _log.Debug(">>" + SerializeExt.ToJson(res, false));
#endif

                return Response.AsJson(ApiResponse.SuccessResponse(res));
            }
            catch (BusinessException ex)
            {
                _log.Info(ex, "ҵ���쳣");
                return Response.AsJson(ApiResponse.FailResponse(ex.Message));
            }
            catch (AuthException ex)
            {
                _log.Warning(ex, "�����֤�쳣");
                return Response.AsJson(ApiResponse.FailResponse(ex.Message), HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "�������쳣");
                return Response.AsJson(ApiResponse.FailResponse("ϵͳ�쳣"), HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// У������
        /// </summary>
        private void CheckRequest(MethodInfo method, string httpType)
        {
            // 1��У����������
            switch (httpType)
            {
                case "GET":
                    if (method.GetCustomAttribute<HttpPostAttribute>() != null)
                    {
                        throw new Exception("������֧��GET����");
                    }
                    break;
                case "POST":
                    if (method.GetCustomAttribute<HttpPostAttribute>() == null)
                    {
                        throw new Exception("������֧��POST����");
                    }
                    break;
            }

            // 2�������֤
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
        /// ����Token��У��ͨ������ã�
        /// </summary>
        private void UpdateToken()
        {
            string token = _httpHeaders["Authorization"];

            // ÿ�����󶼻����token����ֹ(α��)�ظ�����
            if (!token.IsEmpty())
            {
                _httpHeaders["Authorization"] = _auth.Update(token);
            }
        }

        /// <summary>
        /// ����Token
        /// </summary>
        private void CreateToken()
        {
            // �ṩ���ͻ��˵���Ϣ
            var payload = new JwtPayload
            {
                Timestamp = DateTime.Now,
                IP = Request.UserHostAddress
            };

            _httpHeaders.SafeAdd("Authorization", _auth.Create(payload));
        }

        /// <summary>
        /// ִ�з���
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