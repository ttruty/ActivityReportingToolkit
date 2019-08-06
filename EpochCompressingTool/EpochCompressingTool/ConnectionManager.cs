
using GeneaPCDriver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EpochCompressingTool
{
    public sealed class ConnectionManager
    {
        private string GENEA_GUID_STRING = "{DBDC0016-1F93-49c2-9799-FCCD08A244B8}";
        private DataManager m_DataManager = DataManager.dataManager;
        private IntPtr m_GeneaFormHandle = IntPtr.Zero;
        public List<string> threadList = new List<string>();
        private object m_threadLock = new object();
        public List<string> m_DevicePathList;
        private DeviceManagement m_ObjDeviceManager;
        private uint m_NoConnectedDevices;
        private Hashtable m_DeviceIdentity;
        private Hashtable m_DevicePathID;
        private ParseAndPackData m_ParseAndPackDataObj;
        //private string m_ConnectedDevice;
        private static ConnectionManager m_ConnectionManagerInstance;
        private const ushort m_NoOfPageDataPoints = 300;
        private const ushort m_PageHeaderLength = 22;
        private const ushort m_NoOfRecordeDataBytes = 1800;
        private const ushort m_NoOfBytesPerPage = 1822;
        private const ushort m_LengthOfDataPoints = 6;
        private const ushort m_LengthOfdeviceStatus = 18;
        //private string m_BinFilePath;
        //private bool m_CreateCSV;
        public int m_extractErrorCount;
        //private bool m_configInProgress;
        //private bool m_extractAbort;

        //public event ConnectionManager.ExtractUpdateDelegate ExtractUpdate;

        public ConnectionManager()
        {
            this.m_ObjDeviceManager = new DeviceManagement();
            this.m_DeviceIdentity   = new Hashtable();
            this.m_ParseAndPackDataObj = new ParseAndPackData();
            this.m_DevicePathList = new List<string>();
            this.m_DevicePathID = new Hashtable();
        }

        public void SetGeneaFormHandle(IntPtr formHandle)
        {
            this.m_GeneaFormHandle = formHandle;
        }

        public static ConnectionManager ConnectionManagerInstance
        {
            get
            {
                if (ConnectionManager.m_ConnectionManagerInstance == null)
                    ConnectionManager.m_ConnectionManagerInstance = new ConnectionManager();
                return ConnectionManager.m_ConnectionManagerInstance;
            }
        }

        public bool IsDeviceOperationActive(string deviceID)
        {
            return this.threadList.Contains(deviceID);
        }

        public bool IsDeviceOperationActive()
        {
            return this.threadList.Count > 0;
        }

        public eReturn ListConnectedDevices(List<string> connectedDeviceIDs)
        {
            eReturn eReturn1 = eReturn.FAILURE;
            //IntPtr num = new IntPtr();
            IntPtr zero = IntPtr.Zero;
            Guid guid = new Guid(this.GENEA_GUID_STRING);
            eReturn1 = this.m_ObjDeviceManager.EnumerateDevices(guid, this.m_DevicePathList);
            this.m_NoConnectedDevices = Convert.ToUInt32(this.m_DevicePathList.Count);
            if ((int)this.m_NoConnectedDevices == 0)
                eReturn1 = eReturn.SUCCESS;
            eReturn eReturn2 = this.m_ObjDeviceManager.RegisterDevices(this.m_GeneaFormHandle, guid, ref zero);
            if (eReturn2 == eReturn.SUCCESS)
            {
                for (int deviceIdIndex = 0; (long)deviceIdIndex < (long)this.m_NoConnectedDevices; ++deviceIdIndex)
                {
                    switch (this.InitializeDevice(deviceIdIndex))
                    {
                        case eReturn.SUCCESS:
                            connectedDeviceIDs.Add(this.m_DevicePathID[(object)this.m_DevicePathList[deviceIdIndex]].ToString());
                            eReturn2 = eReturn.SUCCESS;
                            break;
                        case eReturn.ERROR_DEVICE_COMMUNICATION:
                            eReturn2 = eReturn.ERROR_DEVICE_COMMUNICATION;
                            break;
                        case eReturn.ERROR_FIRMWARE:
                            eReturn2 = eReturn.ERROR_FIRMWARE;
                            break;
                        case eReturn.DEVICE_ID_ALREADY_PRESENT:
                            eReturn2 = eReturn.DEVICE_ID_ALREADY_PRESENT;
                            break;
                        default:
                            eReturn2 = eReturn.FAILURE;
                            break;
                    }
                }
            }
            return eReturn2;
        }

        private eReturn InitializeDevice(int deviceIdIndex)
        {
            byte[] deviceInfoBuffer = new byte[452];
            byte[] calibInfoBuffer = new byte[28];
            byte[] deviceStatusData = new byte[18];
            byte[] configInfoBuffer = new byte[45];
            byte[] memoryStatusBuffer = new byte[3];
            eReturn eReturn = eReturn.FAILURE;
            GENEAG3WinUSBPCDriver geneaWinUSBPCDriverObject = new GENEAG3WinUSBPCDriver();
            if (geneaWinUSBPCDriverObject.InitialiseDevice(this.m_DevicePathList[deviceIdIndex]))
            {
                switch (geneaWinUSBPCDriverObject.GetDeviceInfo(ref deviceInfoBuffer))
                {
                    case GeneaPCDriver.eReturn.SUCCESS:
                        DeviceData deviceData = new DeviceData();
                        eReturn = this.m_ParseAndPackDataObj.ParseAndPackGetID(deviceInfoBuffer, deviceData);
                        if (eReturn == eReturn.SUCCESS)
                        {
                            string uniqueSerialCode = deviceData.ObjDeviceInfo.DeviceUniqueSerialCode;
                            if (!this.m_DeviceIdentity.Contains((object)uniqueSerialCode))
                            {
                                this.m_DeviceIdentity.Add((object)uniqueSerialCode, (object)geneaWinUSBPCDriverObject);
                                this.m_DevicePathID.Add((object)this.m_DevicePathList[deviceIdIndex], (object)uniqueSerialCode);
                                //eReturn = this.GetDeviceCapabilities(geneaWinUSBPCDriverObject, deviceData);
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    switch (geneaWinUSBPCDriverObject.GetCalibInfo(ref calibInfoBuffer))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = this.m_ParseAndPackDataObj.ParseAndPackGetCalibInfo(calibInfoBuffer, deviceData.ObjCalibInfo);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    DeviceCapabilities deviceCapabilities = new DeviceCapabilities();
                                    switch (geneaWinUSBPCDriverObject.GetDeviceStatus(ref deviceStatusData))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = deviceCapabilities.ParseAndPackGetStatus(deviceData, deviceStatusData);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    switch (geneaWinUSBPCDriverObject.GetConfiguration(ref configInfoBuffer))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            //eReturn = this.m_ParseAndPackDataObj.ParseAndPackGetConfigInfo(deviceData, configInfoBuffer);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    switch (geneaWinUSBPCDriverObject.GetMemoryUsed(ref memoryStatusBuffer))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = this.m_ParseAndPackDataObj.ParseAndPackMemoryInfo(memoryStatusBuffer, deviceData.ObjDeviceStatusInfo);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    eReturn = this.m_DataManager.AddDeviceObject(uniqueSerialCode, deviceData);
                                    break;
                                }
                                break;
                            }
                            eReturn = eReturn.DEVICE_ID_ALREADY_PRESENT;
                            break;
                        }
                        break;
                    case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                        eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                        break;
                    case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                        eReturn = eReturn.ERROR_FIRMWARE;
                        break;
                    default:
                        eReturn = eReturn.FAILURE;
                        break;
                }
            }
            return eReturn;
        }

        public eReturn OnNewDeviceConnected(Message m, ref string deviceID)
        {
            string devicePathName = (string)null;
            eReturn eReturn;
            if (this.m_ObjDeviceManager.GetDevicePathName(m, ref devicePathName) == eReturn.SUCCESS)
            {
                eReturn = eReturn.FAILURE;
                uint deviceListIndex = 0;
                if (!this.IsDevicePresent(devicePathName, ref deviceListIndex))
                {
                    this.m_DevicePathList.Add(devicePathName);
                    eReturn = this.InitializeDevice(this.m_DevicePathList.IndexOf(devicePathName));
                    switch (eReturn)
                    {
                        case eReturn.SUCCESS:
                            this.m_NoConnectedDevices = Convert.ToUInt32(this.m_DevicePathList.Count);
                            deviceID = this.m_DevicePathID[(object)devicePathName].ToString();
                            break;
                        case eReturn.DEVICE_ID_ALREADY_PRESENT:
                            deviceID = (string)null;
                            eReturn = eReturn.DEVICE_ID_ALREADY_PRESENT;
                            break;
                        default:
                            //GeneaPCSoftwareForm.m_StatusMessageLabelToolStrip.Text = "Unable to communicate with the newly connected GENEActiv device";
                            deviceID = (string)null;
                            eReturn = eReturn.FAILURE;
                            break;
                    }
                }
            }
            else
                eReturn = eReturn.FAILURE;
            return eReturn;
        }

        public void OnDeviceDisconnected(Message m, ref string deviceID)
        {
            uint deviceListIndex = 0;
            string devicePathName = (string)null;
            if (this.m_ObjDeviceManager.GetDevicePathName(m, ref devicePathName) != eReturn.SUCCESS)
                return;
            if (!this.IsDevicePresent(devicePathName, ref deviceListIndex))
                return;
            string devicePath = this.m_DevicePathList[Convert.ToInt32(deviceListIndex)];
            if (devicePath != null && this.m_DevicePathID.Contains((object)devicePath))
                deviceID = this.m_DevicePathID[(object)devicePath].ToString();
            if (deviceID != null && this.m_DeviceIdentity.Contains((object)deviceID))
                ((GENEAG3WinUSBPCDriver)this.m_DeviceIdentity[(object)deviceID]).Uninitialise();
            if (devicePath != null && this.m_DevicePathList.Contains(devicePath))
                this.m_DevicePathList.Remove(devicePath);
            this.m_NoConnectedDevices = Convert.ToUInt32(this.m_DevicePathList.Count);
            if (devicePath != null && this.m_DevicePathID.Contains((object)devicePath))
                this.m_DevicePathID.Remove((object)devicePath);
            if (deviceID != null && this.m_DeviceIdentity.Contains((object)deviceID))
                this.m_DeviceIdentity.Remove((object)deviceID);
            if (deviceID == null)
                return;
            int num = (int)this.m_DataManager.RemoveDeviceObject(deviceID);
        }

        private bool IsDevicePresent(string detectedDevicePathName, ref uint deviceListIndex)
        {
            for (int index = 0; index < this.m_DevicePathList.Count; ++index)
            {
                if (string.Compare(detectedDevicePathName, this.m_DevicePathList[index], true) == 0)
                {
                    deviceListIndex = Convert.ToUInt32(index);
                    return true;
                }
            }
            return false;
        }

        public eReturn GetSensorOutputData(string deviceIdIndex, DeviceData deviceObject)
        {
            DeviceCapabilities deviceCapabilities = new DeviceCapabilities();
            byte[] deviceStatusData = new byte[18];
            eReturn eReturn;
            switch (((GENEAG3WinUSBPCDriver)this.m_DeviceIdentity[(object)deviceIdIndex]).GetDeviceStatus(ref deviceStatusData))
            {
                case GeneaPCDriver.eReturn.SUCCESS:
                    eReturn = deviceCapabilities.ParseAndPackGetStatus(deviceObject, deviceStatusData);
                    break;
                case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                    eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                    break;
                case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                    eReturn = eReturn.ERROR_FIRMWARE;
                    break;
                default:
                    eReturn = eReturn.FAILURE;
                    break;
            }
            return eReturn;
        }

        private string ByteArrayToString(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", "");
        }

        public eReturn UpdateDevice(string deviceID)
        {
            byte[] deviceInfoBuffer = new byte[452];
            byte[] calibInfoBuffer = new byte[28];
            byte[] deviceStatusData = new byte[18];
            byte[] configInfoBuffer = new byte[45];
            byte[] memoryStatusBuffer = new byte[3];
            eReturn eReturn;
            if (this.m_DeviceIdentity.Contains((object)deviceID))
            {

                GENEAG3WinUSBPCDriver geneaWinUSBPCDriverObject = (GENEAG3WinUSBPCDriver)this.m_DeviceIdentity[(object)deviceID];
                if (this.m_DataManager.deviceIDTable.Contains((object)deviceID))
                {

                    DeviceData deviceData = (DeviceData)this.m_DataManager.deviceIDTable[(object)deviceID];
                    switch (geneaWinUSBPCDriverObject.GetDeviceInfo(ref deviceInfoBuffer))
                    {
                        case GeneaPCDriver.eReturn.SUCCESS:
                            eReturn = this.m_ParseAndPackDataObj.ParseAndPackGetID(deviceInfoBuffer, deviceData);
                            if (eReturn == eReturn.SUCCESS)
                            {
                                string uniqueSerialCode = deviceData.ObjDeviceInfo.DeviceUniqueSerialCode;
                                //eReturn = this.GetDeviceCapabilities(geneaWinUSBPCDriverObject, deviceData);
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    switch (geneaWinUSBPCDriverObject.GetCalibInfo(ref calibInfoBuffer))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = this.m_ParseAndPackDataObj.ParseAndPackGetCalibInfo(calibInfoBuffer, deviceData.ObjCalibInfo);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    DeviceCapabilities deviceCapabilities = new DeviceCapabilities();
                                    switch (geneaWinUSBPCDriverObject.GetDeviceStatus(ref deviceStatusData))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = deviceCapabilities.ParseAndPackGetStatus(deviceData, deviceStatusData);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    switch (geneaWinUSBPCDriverObject.GetConfiguration(ref configInfoBuffer))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = this.m_ParseAndPackDataObj.ParseAndPackGetConfigInfo(deviceData, configInfoBuffer);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    switch (geneaWinUSBPCDriverObject.GetMemoryUsed(ref memoryStatusBuffer))
                                    {
                                        case GeneaPCDriver.eReturn.SUCCESS:
                                            eReturn = this.m_ParseAndPackDataObj.ParseAndPackMemoryInfo(memoryStatusBuffer, deviceData.ObjDeviceStatusInfo);
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                                            break;
                                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                                            eReturn = eReturn.ERROR_FIRMWARE;
                                            break;
                                        default:
                                            eReturn = eReturn.FAILURE;
                                            break;
                                    }
                                }
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    eReturn = this.m_DataManager.AddDeviceObject(uniqueSerialCode, deviceData);
                                    break;
                                }
                                break;
                            }
                            break;
                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                            break;
                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                            eReturn = eReturn.ERROR_FIRMWARE;
                            break;
                        default:
                            eReturn = eReturn.FAILURE;
                            break;
                    }
                }
                else
                    eReturn = eReturn.FAILURE;
            }
            else
                eReturn = eReturn.FAILURE;
            return eReturn;
        }

        public eReturn GetDeviceRTC(string deviceID, ref DateTime deviceClock)
        {
            eReturn eReturn;
            if (this.m_DeviceIdentity.Contains((object)deviceID))
            {
                GENEAG3WinUSBPCDriver g3WinUsbpcDriver = (GENEAG3WinUSBPCDriver)this.m_DeviceIdentity[(object)deviceID];
                if (this.m_DataManager.deviceIDTable.Contains((object)deviceID))
                {
                    DeviceData deviceData = (DeviceData)this.m_DataManager.deviceIDTable[(object)deviceID];
                    byte[] syncTimeBuffer = new byte[9];
                    switch (g3WinUsbpcDriver.SyncTime(ref syncTimeBuffer))
                    {
                        case GeneaPCDriver.eReturn.SUCCESS:
                            eReturn = this.m_ParseAndPackDataObj.ParseGetTime(syncTimeBuffer, ref deviceClock);
                            break;
                        case GeneaPCDriver.eReturn.ERROR_DEVICE_COMMUNICATION:
                            eReturn = eReturn.ERROR_DEVICE_COMMUNICATION;
                            break;
                        case GeneaPCDriver.eReturn.ERROR_FIRMWARE:
                            eReturn = eReturn.ERROR_FIRMWARE;
                            break;
                        default:
                            eReturn = eReturn.FAILURE;
                            break;
                    }
                }
                else
                    eReturn = eReturn.FAILURE;
            }
            else
                eReturn = eReturn.FAILURE;
            return eReturn;
        }

        public delegate void ExtractUpdateDelegate(string statusString);
    }
}
