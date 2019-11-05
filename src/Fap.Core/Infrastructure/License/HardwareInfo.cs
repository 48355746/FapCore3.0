using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Fap.Core.Infrastructure.License
{
    public class HardwareInfo
    {
        /// <summary>
        /// 获取操作系统描述
        /// </summary>
        /// <returns></returns>
        public string OperSystemDesc => $"{RuntimeInformation.OSDescription}";

        /// <summary>
        /// 获取以太网MAC地址
        /// </summary>
        /// <returns></returns>
        public string GetMACAddress()
        {
            string mac = string.Empty;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //显示网络适配器描述信息、名称、类型、速度、MAC 地址
                    mac += adapter.GetPhysicalAddress();
                }
            }
            return mac;
        }//end    

    }
}
