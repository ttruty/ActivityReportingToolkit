
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EpochCompressingTool
{
    internal sealed class DeviceManagement
    {
        internal const int DBT_DEVICEARRIVAL = 32768;
        internal const int DBT_DEVICEREMOVECOMPLETE = 32772;
        internal const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        internal const int DBT_DEVTYP_HANDLE = 6;
        internal const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        internal const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        internal const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        internal const int WM_DEVICECHANGE = 537;
        internal const int DIGCF_PRESENT = 2;
        internal const int DIGCF_DEVICEINTERFACE = 16;

        internal eReturn EnumerateDevices(Guid geneaGuid, List<string> devicePathName)
        {
            int RequiredSize = 0;
            IntPtr num = IntPtr.Zero;
            IntPtr DeviceInfoSet = new IntPtr();
            bool flag1 = false;
            DeviceManagement.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new DeviceManagement.SP_DEVICE_INTERFACE_DATA();
            bool flag2 = false;
            eReturn eReturn1 = eReturn.FAILURE;
            eReturn eReturn2;
            try
            {
                DeviceInfoSet = DeviceManagement.SetupDiGetClassDevs(ref geneaGuid, IntPtr.Zero, IntPtr.Zero, 18);
                eReturn2 = eReturn.SUCCESS;
            }
            catch
            {
                eReturn2 = eReturn.FAILURE;
            }
            if (eReturn2 == eReturn.SUCCESS)
            {
                eReturn1 = eReturn.FAILURE;
                int MemberIndex = 0;
                try
                {
                    DeviceInterfaceData.cbSize = Marshal.SizeOf<DeviceManagement.SP_DEVICE_INTERFACE_DATA>(DeviceInterfaceData);
                    eReturn2 = eReturn.SUCCESS;
                }
                catch
                {
                    eReturn2 = eReturn.FAILURE;
                }
                if (eReturn2 == eReturn.SUCCESS)
                {
                    eReturn1 = eReturn.FAILURE;
                    do
                    {
                        try
                        {
                            flag2 = DeviceManagement.SetupDiEnumDeviceInterfaces(DeviceInfoSet, IntPtr.Zero, ref geneaGuid, MemberIndex, ref DeviceInterfaceData);
                            eReturn2 = eReturn.SUCCESS;
                        }
                        catch
                        {
                            eReturn2 = eReturn.FAILURE;
                        }
                        if (eReturn2 == eReturn.SUCCESS)
                        {
                            eReturn2 = eReturn.FAILURE;
                            if (!flag2)
                            {
                                flag1 = true;
                            }
                            else
                            {
                                try
                                {
                                    flag2 = DeviceManagement.SetupDiGetDeviceInterfaceDetail(DeviceInfoSet, ref DeviceInterfaceData, IntPtr.Zero, 0, ref RequiredSize, IntPtr.Zero);
                                    num = Marshal.AllocHGlobal(RequiredSize);
                                    Marshal.WriteInt32(num, IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8);
                                    flag2 = DeviceManagement.SetupDiGetDeviceInterfaceDetail(DeviceInfoSet, ref DeviceInterfaceData, num, RequiredSize, ref RequiredSize, IntPtr.Zero);
                                    eReturn2 = eReturn.SUCCESS;
                                }
                                catch
                                {
                                    eReturn2 = eReturn.FAILURE;
                                }
                                if (eReturn2 == eReturn.SUCCESS)
                                {
                                    eReturn2 = eReturn.FAILURE;
                                    if (flag2)
                                    {
                                        IntPtr ptr = new IntPtr(num.ToInt32() + 4);
                                        devicePathName.Add(Marshal.PtrToStringAuto(ptr));
                                    }
                                }
                            }
                        }
                        ++MemberIndex;
                    }
                    while (!flag1);
                }
                if (num != IntPtr.Zero)
                    Marshal.FreeHGlobal(num);
                if (DeviceInfoSet != IntPtr.Zero)
                {
                    try
                    {
                        DeviceManagement.SetupDiDestroyDeviceInfoList(DeviceInfoSet);
                        eReturn2 = eReturn.SUCCESS;
                    }
                    catch
                    {
                        eReturn2 = eReturn.FAILURE;
                    }
                }
                if (flag1)
                    eReturn2 = eReturn.SUCCESS;
            }
            return eReturn2;
        }

        internal eReturn RegisterDevices(IntPtr formHandle, Guid classGuid, ref IntPtr deviceNotificationHandle)
        {
            DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE structure = new DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE();
            IntPtr num = IntPtr.Zero;
            int cb = 0;
            eReturn eReturn1 = eReturn.FAILURE;
            eReturn eReturn2;
            try
            {
                cb = Marshal.SizeOf<DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE>(structure);
                eReturn2 = eReturn.SUCCESS;
            }
            catch
            {
                eReturn2 = eReturn.FAILURE;
            }
            if (eReturn2 == eReturn.SUCCESS)
            {
                eReturn1 = eReturn.FAILURE;
                structure.dbcc_size = cb;
                structure.dbcc_devicetype = 5;
                structure.dbcc_reserved = 0;
                structure.dbcc_classguid = classGuid;
                num = Marshal.AllocHGlobal(cb);
                try
                {
                    Marshal.StructureToPtr<DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE>(structure, num, true);
                    eReturn2 = eReturn.SUCCESS;
                }
                catch
                {
                    eReturn2 = eReturn.FAILURE;
                }
                if (eReturn2 == eReturn.SUCCESS)
                {
                    eReturn1 = eReturn.FAILURE;
                    deviceNotificationHandle = DeviceManagement.RegisterDeviceNotification(formHandle, num, 0);
                    try
                    {
                        Marshal.PtrToStructure<DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE>(num, structure);
                        eReturn2 = eReturn.SUCCESS;
                    }
                    catch
                    {
                        eReturn2 = eReturn.FAILURE;
                    }
                    if (eReturn2 == eReturn.SUCCESS)
                    {
                        eReturn1 = eReturn.FAILURE;
                        try
                        {
                            eReturn2 = deviceNotificationHandle.ToInt32() != IntPtr.Zero.ToInt32() ? eReturn.SUCCESS : eReturn.FAILURE;
                        }
                        catch
                        {
                            eReturn2 = eReturn.FAILURE;
                        }
                    }
                }
            }
            if (num != IntPtr.Zero)
                Marshal.FreeHGlobal(num);
            return eReturn2;
        }

        internal eReturn UnRegisterDevices(IntPtr deviceNotificationHandle)
        {
            eReturn eReturn = eReturn.FAILURE;
            try
            {
                if (DeviceManagement.UnregisterDeviceNotification(deviceNotificationHandle))
                    eReturn = eReturn.SUCCESS;
            }
            catch
            {
                eReturn = eReturn.FAILURE;
            }
            return eReturn;
        }

        internal eReturn GetDevicePathName(Message m, ref string devicePathName)
        {
            eReturn eReturn1 = eReturn.FAILURE;
            int length = 0;
            string str = string.Empty;
            DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE_1 structure1 = new DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE_1();
            DeviceManagement.DEV_BROADCAST_HDR structure2 = new DeviceManagement.DEV_BROADCAST_HDR();
            eReturn eReturn2;
            try
            {
                Marshal.PtrToStructure<DeviceManagement.DEV_BROADCAST_HDR>(m.LParam, structure2);
                eReturn2 = eReturn.SUCCESS;
            }
            catch
            {
                eReturn2 = eReturn.FAILURE;
            }
            if (eReturn2 == eReturn.SUCCESS)
            {
                eReturn2 = eReturn.FAILURE;
                if (structure2.dbch_devicetype == 5)
                {
                    try
                    {
                        length = Convert.ToInt32((structure2.dbch_size - 32) / 2);
                        eReturn2 = eReturn.SUCCESS;
                    }
                    catch
                    {
                        eReturn2 = eReturn.FAILURE;
                    }
                    if (eReturn2 == eReturn.SUCCESS)
                    {
                        eReturn1 = eReturn.FAILURE;
                        structure1.dbcc_name = new char[length + 1];
                        try
                        {
                            Marshal.PtrToStructure<DeviceManagement.DEV_BROADCAST_DEVICEINTERFACE_1>(m.LParam, structure1);
                            eReturn2 = eReturn.SUCCESS;
                        }
                        catch
                        {
                            eReturn2 = eReturn.FAILURE;
                        }
                        if (eReturn2 == eReturn.SUCCESS)
                        {
                            eReturn1 = eReturn.FAILURE;
                            try
                            {
                                str = new string(structure1.dbcc_name, 0, length);
                                eReturn2 = eReturn.SUCCESS;
                            }
                            catch
                            {
                                eReturn2 = eReturn.FAILURE;
                            }
                        }
                    }
                }
            }
            if (eReturn2 == eReturn.SUCCESS)
                devicePathName = str;
            return eReturn2;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern int SetupDiCreateDeviceInfoList(ref Guid ClassGuid, int hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref DeviceManagement.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref DeviceManagement.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterDeviceNotification(IntPtr Handle);

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_DEVICEINTERFACE
        {
            internal int dbcc_size;
            internal int dbcc_devicetype;
            internal int dbcc_reserved;
            internal Guid dbcc_classguid;
            internal short dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_DEVICEINTERFACE_1
        {
            internal int dbcc_size;
            internal int dbcc_devicetype;
            internal int dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
            internal byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            internal char[] dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal int dbch_size;
            internal int dbch_devicetype;
            internal int dbch_reserved;
        }

        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal int cbSize;
            internal Guid InterfaceClassGuid;
            internal int Flags;
            internal IntPtr Reserved;
        }
    }
}
