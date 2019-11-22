using Microsoft.AspNetCore.Mvc;

namespace Fap.AspNetCore.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string GetRemoteIPAddress(this Controller controller)
        {

            if (controller.HttpContext.Connection.RemoteIpAddress != null)
            {
                return controller.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            else
            {
                return "unknown";
            }

        }
        public static string GetLocalIPAddress(this Controller controller)
        {
            if (controller.HttpContext.Connection.LocalIpAddress != null)
            {
                return controller.HttpContext.Connection.LocalIpAddress.ToString() + ":" + controller.HttpContext.Connection.LocalPort;
            }
            else
            {
                return "unknown";
            }
        }
        /// <summary>
        /// 获取baseUrl
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string GetBaseUrl(this Controller controller)
        {
            return $"http://{controller.HttpContext.Request.Host.Host}:{controller.HttpContext.Request.Host.Port}";
        }
    }
}
