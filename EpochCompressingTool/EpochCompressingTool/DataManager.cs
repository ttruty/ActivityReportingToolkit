

using System.Collections;
using System.Collections.Generic;

namespace EpochCompressingTool
{
    public sealed class DataManager
    {
        public Hashtable deviceIDTable;
        private static DataManager m_DataManger;

        public DataManager()
        {
            this.deviceIDTable = new Hashtable();
        }

        public static DataManager dataManager
        {
            get
            {
                if (DataManager.m_DataManger == null)
                    DataManager.m_DataManger = new DataManager();
                return DataManager.m_DataManger;
            }
        }

        public DeviceData GetDeviceObject(string deviceID)
        {
            if (this.deviceIDTable.Contains((object)deviceID))
                return (DeviceData)this.deviceIDTable[(object)deviceID];
            return (DeviceData)null;
        }

        public List<string> ConnectedDeviceIDs
        {
            get
            {
                List<string> stringList = new List<string>();
                foreach (DictionaryEntry dictionaryEntry in this.deviceIDTable)
                    stringList.Add(dictionaryEntry.Key.ToString());
                return stringList;
            }
        }

        public List<DeviceData> ConnectedDevices
        {
            get
            {
                List<DeviceData> deviceDataList = new List<DeviceData>();
                foreach (DictionaryEntry dictionaryEntry in this.deviceIDTable)
                    deviceDataList.Add((DeviceData)dictionaryEntry.Value);
                return deviceDataList;
            }
        }

        public eReturn AddDeviceObject(string deviceID, DeviceData filleddeviceObject)
        {
            eReturn eReturn = eReturn.FAILURE;
            if (!this.deviceIDTable.ContainsKey((object)deviceID))
            {
                this.deviceIDTable.Add((object)deviceID, (object)filleddeviceObject);
                eReturn = eReturn.SUCCESS;
            }
            return eReturn;
        }

        public eReturn RemoveDeviceObject(string deviceID)
        {
            eReturn eReturn = eReturn.FAILURE;
            if (this.deviceIDTable.ContainsKey((object)deviceID))
            {
                this.deviceIDTable.Remove((object)deviceID);
                eReturn = eReturn.SUCCESS;
            }
            return eReturn;
        }

        //public bool TestMeasurementFreqValid(float freq)
        //{
        //  foreach (DeviceData connectedDevice in this.ConnectedDevices)
        //  {
        //    if (!connectedDevice.ObjDeviceInfo.TestMeasurementFreqValid(freq))
        //      return false;
        //  }
        //  return true;
        //}
    }
}
