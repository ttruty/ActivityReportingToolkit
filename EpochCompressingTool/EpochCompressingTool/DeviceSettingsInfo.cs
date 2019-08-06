

using System;
using System.Collections.Generic;
using System.Linq;

namespace EpochCompressingTool
{
    public class DeviceSettingsInfo
    {
        private static float[] GENEA_ACC_RANGE_LOOKUP = new float[4]
        {
      16f,
      32f,
      16f,
      16f
        };
        private static float[] GENEACTIV_FREQ_RANGE = new float[11]
        {
      10f,
      20f,
      25f,
      30f,
      40f,
      50f,
      60f,
      66.7f,
      75f,
      85.7f,
      100f
        };
        private static float[] GENEACTION_FREQ_RANGE = new float[2]
        {
      100f,
      1000f
        };
        private static float[] GENEASLEEP_FREQ_RANGE = new float[11]
        {
      10f,
      20f,
      25f,
      30f,
      40f,
      50f,
      60f,
      66.7f,
      75f,
      85.7f,
      100f
        };
        private static float[] GENEAIR_FREQ_RANGE = new float[4]
        {
      10f,
      50f,
      100f,
      500f
        };
        public static float[] GENEA_FREQ_RANGE = new float[13]
        {
      10f,
      20f,
      25f,
      30f,
      40f,
      50f,
      60f,
      66.7f,
      75f,
      85.7f,
      100f,
      500f,
      1000f
        };
        private string m_DeviceUniqueSerialCode;
        private string m_AccRange;
        private string m_AccResolution;
        private string m_DeviceType;
        private string m_DeviceModel;
        private string m_FirmwareVersion;
        private PageTime m_CalibrationDate;
        private ushort m_MemorySize;
        private string m_BatterySizeAndType;
        private int m_AvailableRecordingTime;
        private int m_AvailableStandbyTime;
        private float m_MeasurementFreq;
        private int m_MeasurementPeriod;
        private string m_AccUnits;
        private string m_LightmeterRange;
        private string m_LightmeterResolution;
        private string m_LightmeterUnits;
        private string m_TemperatureRange;
        private string m_TemperatureResolution;
        private string m_TemperatureUnits;
        private string m_StartMode;
        private PageTime m_StartTime;
        private PageTime m_ConfigTime;
        private string m_TimeZone;
        private string m_ConfigOperatorID;
        private bool m_TempSensorPresent;
        private bool m_LightSensorPresent;
        private bool m_ButtonPresent;
        private DateTime m_SetConfigTime;
        private GENEADeviceType m_geneaDeviceType;
        private float m_deviceAccRange;

        public DeviceSettingsInfo()
        {
            this.StartTime = new PageTime();
            this.CalibrationDate = new PageTime();
            this.ConfigTime = new PageTime();
        }

        public float ScaleAccValue(float accValue)
        {
            return (float)((double)accValue * (double)this.m_deviceAccRange / 16.0);
        }

        public float TempCompensateAxisValue(float axisValue, float tempValue, float compValue)
        {
            return axisValue + (30f - tempValue) * compValue;
        }

        public float NormaliseAxisValue(float axisValue)
        {
            float num = (float)(0.5 + (double)axisValue / (double)this.m_deviceAccRange);
            if ((double)num < 0.0)
                return 0.0f;
            if ((double)num > 1.0)
                return 1f;
            return num;
        }

        public bool TestMeasurementFreqValid(float freq)
        {
            return ((IEnumerable<float>)this.DeviceMeasurementFrequencies).Contains<float>(freq);
        }

        public float DenormaliseAxisValue(float normValue)
        {
            return (normValue - 0.5f) * this.m_deviceAccRange;
        }

        private void UpdateDeviceType(string devTypeID)
        {
            this.m_DeviceType = devTypeID;
            try
            {
                this.m_geneaDeviceType = (GENEADeviceType)Enum.Parse(typeof(GENEADeviceType), devTypeID.Trim(), true);
            }
            catch (Exception ex)
            {
                this.m_geneaDeviceType = GENEADeviceType.GENEActiv;
            }
            this.m_deviceAccRange = DeviceSettingsInfo.GENEA_ACC_RANGE_LOOKUP[(int)this.m_geneaDeviceType];
        }

        public string DeviceUniqueSerialCode
        {
            set
            {
                this.m_DeviceUniqueSerialCode = value;
            }
            get
            {
                return this.m_DeviceUniqueSerialCode;
            }
        }

        public string AccRange
        {
            set
            {
                this.m_AccRange = value;
            }
            get
            {
                return this.m_AccRange;
            }
        }

        public PageTime CalibrationDate
        {
            set
            {
                this.m_CalibrationDate = value;
            }
            get
            {
                return this.m_CalibrationDate;
            }
        }

        public string accResolution
        {
            set
            {
                this.m_AccResolution = value;
            }
            get
            {
                return this.m_AccResolution;
            }
        }

        public string DeviceType
        {
            set
            {
                this.UpdateDeviceType(value);
            }
            get
            {
                return this.m_DeviceType;
            }
        }

        public GENEADeviceType GENEADeviceType
        {
            get
            {
                return this.m_geneaDeviceType;
            }
        }

        public float DeviceAccRange
        {
            get
            {
                return this.m_deviceAccRange;
            }
        }

        public float[] DeviceMeasurementFrequencies
        {
            get
            {
                switch (this.m_geneaDeviceType)
                {
                    case GENEADeviceType.GENEAction:
                        return DeviceSettingsInfo.GENEACTION_FREQ_RANGE;
                    case GENEADeviceType.GENEAsleep:
                        return DeviceSettingsInfo.GENEASLEEP_FREQ_RANGE;
                    case GENEADeviceType.GENEAir:
                        return DeviceSettingsInfo.GENEAIR_FREQ_RANGE;
                    default:
                        return DeviceSettingsInfo.GENEACTIV_FREQ_RANGE;
                }
            }
        }

        public string DeviceModel
        {
            set
            {
                this.m_DeviceModel = value;
            }
            get
            {
                return this.m_DeviceModel;
            }
        }

        public string FirmwareVersion
        {
            set
            {
                this.m_FirmwareVersion = value;
            }
            get
            {
                return this.m_FirmwareVersion;
            }
        }

        public ushort MemorySize
        {
            set
            {
                this.m_MemorySize = value;
            }
            get
            {
                return this.m_MemorySize;
            }
        }

        public string BatterySizeAndType
        {
            set
            {
                this.m_BatterySizeAndType = value;
            }
            get
            {
                return this.m_BatterySizeAndType;
            }
        }

        public int AvailableRecordingTime
        {
            set
            {
                this.m_AvailableRecordingTime = value;
            }
            get
            {
                return this.m_AvailableRecordingTime;
            }
        }

        public int AvailableStandbyTime
        {
            set
            {
                this.m_AvailableStandbyTime = value;
            }
            get
            {
                return this.m_AvailableStandbyTime;
            }
        }

        public float MeasurementFreq
        {
            set
            {
                this.m_MeasurementFreq = value;
            }
            get
            {
                return this.m_MeasurementFreq;
            }
        }

        public int MeasurementPeriod
        {
            set
            {
                this.m_MeasurementPeriod = value;
            }
            get
            {
                return this.m_MeasurementPeriod;
            }
        }

        public string AccUnits
        {
            set
            {
                this.m_AccUnits = value;
            }
            get
            {
                return this.m_AccUnits;
            }
        }

        public string LightmeterRange
        {
            set
            {
                this.m_LightmeterRange = value;
            }
            get
            {
                return this.m_LightmeterRange;
            }
        }

        public string LightmeterResolution
        {
            set
            {
                this.m_LightmeterResolution = value;
            }
            get
            {
                return this.m_LightmeterResolution;
            }
        }

        public string LightmeterUnits
        {
            set
            {
                this.m_LightmeterUnits = value;
            }
            get
            {
                return this.m_LightmeterUnits;
            }
        }

        public string TemperatureRange
        {
            set
            {
                this.m_TemperatureRange = value;
            }
            get
            {
                return this.m_TemperatureRange;
            }
        }

        public string TemperatureResolution
        {
            set
            {
                this.m_TemperatureResolution = value;
            }
            get
            {
                return this.m_TemperatureResolution;
            }
        }

        public string TemperatureUnits
        {
            set
            {
                this.m_TemperatureUnits = value;
            }
            get
            {
                return this.m_TemperatureUnits;
            }
        }

        public string StartMode
        {
            set
            {
                this.m_StartMode = value;
            }
            get
            {
                return this.m_StartMode;
            }
        }

        public string TimeZone
        {
            set
            {
                this.m_TimeZone = value;
            }
            get
            {
                return this.m_TimeZone;
            }
        }

        public string ConfigOperatorID
        {
            set
            {
                this.m_ConfigOperatorID = value;
            }
            get
            {
                return this.m_ConfigOperatorID;
            }
        }

        public bool TempSensorPresent
        {
            set
            {
                this.m_TempSensorPresent = value;
            }
            get
            {
                return this.m_TempSensorPresent;
            }
        }

        public bool LightSensorPresent
        {
            set
            {
                this.m_LightSensorPresent = value;
            }
            get
            {
                return this.m_LightSensorPresent;
            }
        }

        public bool ButtonPresent
        {
            set
            {
                this.m_ButtonPresent = value;
            }
            get
            {
                return this.m_ButtonPresent;
            }
        }

        public PageTime StartTime
        {
            set
            {
                this.m_StartTime = value;
            }
            get
            {
                return this.m_StartTime;
            }
        }

        public PageTime ConfigTime
        {
            set
            {
                this.m_ConfigTime = value;
            }
            get
            {
                return this.m_ConfigTime;
            }
        }

        public DateTime SetConfigTime
        {
            set
            {
                this.m_SetConfigTime = value;
            }
            get
            {
                return this.m_SetConfigTime;
            }
        }

        public static string StartModeString(byte startMode)
        {
            switch (startMode)
            {
                case 0:
                    return "Not configured";
                case 1:
                    return "On button press";
                case 2:
                    return "On button press - button active";
                case 3:
                    return "Immediately on disconnect";
                case 4:
                    return "At future time";
                default:
                    return "Not configured";
            }
        }

        //public static string StartModeString(byte startMode, FutureDateType futureDateType, DateTime futureTime)
        //{
        //    string str = DeviceSettingsInfo.StartModeString(startMode);
        //    if ((int)startMode == 4)
        //        str = str + (futureDateType == FutureDateType.Tomorrow ? " - Tomorrow" : " - Today") + " at " + futureTime.ToString("HH:mm:ss");
        //    return str;
        //}

        //public static DateTime CalcFutureTime(FutureDateType futureDateType, DateTime futureTime)
        //{
        //    DateTime dateTime = futureDateType == FutureDateType.Tomorrow ? DateTime.Today.AddDays(1.0) : DateTime.Today;
        //    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, futureTime.Hour, futureTime.Minute, futureTime.Second, 0, DateTimeKind.Utc);
        //}
    }
}