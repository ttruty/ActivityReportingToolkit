
using System.Collections.Generic;

namespace EpochCompressingTool
{
    public class RecordedData
    {
        private List<float> m_ArrXaxis;
        private List<float> m_ArrYaxis;
        private List<float> m_ArrZaxis;
        private List<ushort> m_ArrLightMeter;
        private List<string> m_ArrButtonStatus;
        private List<float> m_ArrTemperature;
        private List<string> m_TimeStamp;

        public RecordedData()
        {
            this.m_ArrXaxis = new List<float>();
            this.m_ArrYaxis = new List<float>();
            this.m_ArrZaxis = new List<float>();
            this.m_ArrLightMeter = new List<ushort>();
            this.m_ArrButtonStatus = new List<string>();
            this.m_ArrTemperature = new List<float>();
            this.m_TimeStamp = new List<string>();
        }

        public List<float> ArrXaxis
        {
            set
            {
                this.m_ArrXaxis = value;
            }
            get
            {
                return this.m_ArrXaxis;
            }
        }

        public List<float> ArrYaxis
        {
            set
            {
                this.m_ArrYaxis = value;
            }
            get
            {
                return this.m_ArrYaxis;
            }
        }

        public List<float> ArrZaxis
        {
            set
            {
                this.m_ArrZaxis = value;
            }
            get
            {
                return this.m_ArrZaxis;
            }
        }

        public List<ushort> ArrLightMeter
        {
            set
            {
                this.m_ArrLightMeter = value;
            }
            get
            {
                return this.m_ArrLightMeter;
            }
        }

        public List<string> ArrButtonStatus
        {
            set
            {
                this.m_ArrButtonStatus = value;
            }
            get
            {
                return this.m_ArrButtonStatus;
            }
        }

        public List<float> ArrTemperature
        {
            set
            {
                this.m_ArrTemperature = value;
            }
            get
            {
                return this.m_ArrTemperature;
            }
        }

        public List<string> TimeStamp
        {
            set
            {
                this.m_TimeStamp = value;
            }
            get
            {
                return this.m_TimeStamp;
            }
        }
    }
}
