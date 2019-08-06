
namespace EpochCompressingTool
{
    public class CalibrationInformation
    {
        private short m_XGain;
        private short m_XOffset;
        private short m_YGain;
        private short m_YOffset;
        private short m_ZGain;
        private short m_ZOffset;
        private short m_Volts;
        private short m_Lux;
        private short m_ClockOffset;
        private string m_DeviceElectronicID;

        public short Xgain
        {
            set
            {
                this.m_XGain = value;
            }
            get
            {
                return this.m_XGain;
            }
        }

        public short Xoffset
        {
            set
            {
                this.m_XOffset = value;
            }
            get
            {
                return this.m_XOffset;
            }
        }

        public short Ygain
        {
            set
            {
                this.m_YGain = value;
            }
            get
            {
                return this.m_YGain;
            }
        }

        public short Yoffset
        {
            set
            {
                this.m_YOffset = value;
            }
            get
            {
                return this.m_YOffset;
            }
        }

        public short Zgain
        {
            set
            {
                this.m_ZGain = value;
            }
            get
            {
                return this.m_ZGain;
            }
        }

        public short Zoffset
        {
            set
            {
                this.m_ZOffset = value;
            }
            get
            {
                return this.m_ZOffset;
            }
        }

        public short Volts
        {
            set
            {
                this.m_Volts = value;
            }
            get
            {
                return this.m_Volts;
            }
        }

        public short Lux
        {
            set
            {
                this.m_Lux = value;
            }
            get
            {
                return this.m_Lux;
            }
        }

        public short ClockOffset
        {
            set
            {
                this.m_ClockOffset = value;
            }
            get
            {
                return this.m_ClockOffset;
            }
        }

        public string DeviceElectronicID
        {
            set
            {
                this.m_DeviceElectronicID = value;
            }
            get
            {
                return this.m_DeviceElectronicID;
            }
        }
    }
}
