
namespace EpochCompressingTool
{
    public class DeviceStatusInfo
    {
        private float m_AccXaxis;
        private float m_AccYaxis;
        private float m_AccZaxis;
        private float m_LightMeter;
        private float m_Temperature;
        private ushort m_BatteryVoltage;
        private string m_ButtonStatus;
        private string m_DeviceStatus;
        //public PageTime deviceTime;
        public uint m_NoPagesAvailable;
        private string m_BinFilePath;
        private bool m_SaveAsCSV;
        private string m_ExtractOperatorID;
        private string m_ExtractNotes;

        //public DeviceStatusInfo()
        //{
        //  this.deviceTime = new PageTime();
        //}

        public float AccXaxis
        {
            set
            {
                this.m_AccXaxis = value;
            }
            get
            {
                return this.m_AccXaxis;
            }
        }

        public float AccYaxis
        {
            set
            {
                this.m_AccYaxis = value;
            }
            get
            {
                return this.m_AccYaxis;
            }
        }

        public float AccZaxis
        {
            set
            {
                this.m_AccZaxis = value;
            }
            get
            {
                return this.m_AccZaxis;
            }
        }

        public float LightMeter
        {
            set
            {
                this.m_LightMeter = value;
            }
            get
            {
                return this.m_LightMeter;
            }
        }

        public float Temperature
        {
            set
            {
                this.m_Temperature = value;
            }
            get
            {
                return this.m_Temperature;
            }
        }

        public ushort BatteryVoltage
        {
            set
            {
                this.m_BatteryVoltage = value;
            }
            get
            {
                return this.m_BatteryVoltage;
            }
        }

        public string ButtonStatus
        {
            set
            {
                this.m_ButtonStatus = value;
            }
            get
            {
                return this.m_ButtonStatus;
            }
        }

        public string DeviceStatus
        {
            set
            {
                this.m_DeviceStatus = value;
            }
            get
            {
                return this.m_DeviceStatus;
            }
        }

        public uint NoPagesAvailable
        {
            set
            {
                this.m_NoPagesAvailable = value;
            }
            get
            {
                return this.m_NoPagesAvailable;
            }
        }

        public string BinFilePath
        {
            set
            {
                this.m_BinFilePath = value;
            }
            get
            {
                return this.m_BinFilePath;
            }
        }

        public bool SaveAsCSV
        {
            set
            {
                this.m_SaveAsCSV = value;
            }
            get
            {
                return this.m_SaveAsCSV;
            }
        }

        public string ExtractNotes
        {
            set
            {
                this.m_ExtractNotes = value;
            }
            get
            {
                return this.m_ExtractNotes;
            }
        }

        public string ExtractOperatorID
        {
            set
            {
                this.m_ExtractOperatorID = value;
            }
            get
            {
                return this.m_ExtractOperatorID;
            }
        }
    }
}
