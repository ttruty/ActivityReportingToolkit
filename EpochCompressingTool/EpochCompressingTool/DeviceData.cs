

using System;

namespace EpochCompressingTool
{
    public class DeviceData
    {
        private DeviceSettingsInfo m_ObjDeviceInfo;
        //private SubjectInfo m_ObjSubjectInfo;
        //private TrialInfo m_ObjTrialInfo;
        private RecordedData m_ObjRecordedData;
        private CalibrationInformation m_ObjCalibInfo;
        private DeviceStatusInfo m_ObjDeviceStatusInfo;

        public DeviceData()
        {
            this.m_ObjDeviceInfo = new DeviceSettingsInfo();
            //this.m_ObjSubjectInfo = new SubjectInfo();
            //this.m_ObjTrialInfo = new TrialInfo();
            this.m_ObjRecordedData = new RecordedData();
            this.m_ObjCalibInfo = new CalibrationInformation();
            this.m_ObjDeviceStatusInfo = new DeviceStatusInfo();
        }

        public float CalibrateXAxis(float xAxis)
        {
            // Console.WriteLine("Raw xAxis: " + xAxis);
            if ((int)this.m_ObjCalibInfo.Xgain == 0)
                return 0.0f;
            //Console.WriteLine("x: " + (xAxis * 100f - (float)this.m_ObjCalibInfo.Xoffset) / (float)this.m_ObjCalibInfo.Xgain);
            // Console.WriteLine("xoffset: " + this.m_ObjCalibInfo.Xoffset);
            //Console.WriteLine("xGain " + this.m_ObjCalibInfo.Xgain);

            return this.m_ObjDeviceInfo.ScaleAccValue((xAxis * 100f - (float)this.m_ObjCalibInfo.Xoffset) / (float)this.m_ObjCalibInfo.Xgain);
        }

        public float CalibrateYAxis(float yAxis)
        {
            //Console.WriteLine("Raw yAxis: " + yAxis);
            if ((int)this.m_ObjCalibInfo.Ygain == 0)
                return 0.0f;
            //Console.WriteLine("y: " + ((yAxis * 100f - (float)this.m_ObjCalibInfo.Yoffset) / (float)this.m_ObjCalibInfo.Ygain));
            //Console.WriteLine("yoffset: " + this.m_ObjCalibInfo.Yoffset);
            //Console.WriteLine("yGain: " + this.m_ObjCalibInfo.Ygain);
            return this.m_ObjDeviceInfo.ScaleAccValue((yAxis * 100f - (float)this.m_ObjCalibInfo.Yoffset) / (float)this.m_ObjCalibInfo.Ygain);
        }

        public float CalibrateZAxis(float zAxis)
        {
            //Console.WriteLine("Raw zAxis: " + zAxis);
            if ((int)this.m_ObjCalibInfo.Zgain == 0)
                return 0.0f;
            //Console.WriteLine("z: " + ((zAxis * 100f - (float)this.m_ObjCalibInfo.Zoffset) / (float)this.m_ObjCalibInfo.Zgain));
            //Console.WriteLine("zoffset: " + this.m_ObjCalibInfo.Zoffset);
            //Console.WriteLine("zGain: " + this.m_ObjCalibInfo.Zgain);
            return this.m_ObjDeviceInfo.ScaleAccValue((zAxis * 100f - (float)this.m_ObjCalibInfo.Zoffset) / (float)this.m_ObjCalibInfo.Zgain);
        }

        public float DeviceAccZAxisTempCompensated(float compValue)
        {
            return this.m_ObjDeviceInfo.TempCompensateAxisValue(this.m_ObjDeviceStatusInfo.AccZaxis, this.m_ObjDeviceStatusInfo.Temperature, compValue);
        }

        public float DeviceAccXAxisNormalised
        {
            get
            {
                return this.m_ObjDeviceInfo.NormaliseAxisValue(this.m_ObjDeviceStatusInfo.AccXaxis);
            }
        }

        public float DeviceAccYAxisNormalised
        {
            get
            {
                return this.m_ObjDeviceInfo.NormaliseAxisValue(this.m_ObjDeviceStatusInfo.AccYaxis);
            }
        }

        public float DeviceAccZAxisNormalised
        {
            get
            {
                return this.m_ObjDeviceInfo.NormaliseAxisValue(this.m_ObjDeviceStatusInfo.AccZaxis);
            }
        }

        public float RecordedAccXAxisNormalised(int index)
        {
            return this.m_ObjDeviceInfo.NormaliseAxisValue(this.m_ObjRecordedData.ArrXaxis[index]);
        }

        public float RecordedAccYAxisNormalised(int index)
        {
            return this.m_ObjDeviceInfo.NormaliseAxisValue(this.m_ObjRecordedData.ArrYaxis[index]);
        }

        public float RecordedAccZAxisNormalised(int index)
        {
            return this.m_ObjDeviceInfo.NormaliseAxisValue(this.m_ObjRecordedData.ArrZaxis[index]);
        }

        public DeviceSettingsInfo ObjDeviceInfo
        {
            set
            {
                this.m_ObjDeviceInfo = value;
            }
            get
            {
                return this.m_ObjDeviceInfo;
            }
        }

        //public SubjectInfo ObjSubjectInfo
        //{
        //  set
        //  {
        //    this.m_ObjSubjectInfo = value;
        //  }
        //  get
        //  {
        //    return this.m_ObjSubjectInfo;
        //  }
        //}

        //public TrialInfo ObjTrialInfo
        //{
        //  set
        //  {
        //    this.m_ObjTrialInfo = value;
        //  }
        //  get
        //  {
        //    return this.m_ObjTrialInfo;
        //  }
        //}

        public RecordedData ObjRecordedData
        {
            set
            {
                this.m_ObjRecordedData = value;
            }
            get
            {
                return this.m_ObjRecordedData;
            }
        }

        public CalibrationInformation ObjCalibInfo
        {
            set
            {
                this.m_ObjCalibInfo = value;
            }
            get
            {
                return this.m_ObjCalibInfo;
            }
        }

        public DeviceStatusInfo ObjDeviceStatusInfo
        {
            set
            {
                this.m_ObjDeviceStatusInfo = value;
            }
            get
            {
                return this.m_ObjDeviceStatusInfo;
            }
        }
    }
}
