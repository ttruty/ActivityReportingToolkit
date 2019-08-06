

using System;
using System.Text;

namespace EpochCompressingTool
{
    public class ParseAndPackData
    {
        private const ushort m_SubjectInfoLength = 131;
        private const ushort m_TrialInfoLength = 180;
        private const ushort m_DeviceInfoLength = 452;
        private const ushort m_CalibInfoBufferlenght = 28;

        public eReturn ParseAndPackGetID(byte[] buffer, DeviceData deviceData)
        {
            eReturn eReturn;
            if (buffer.Length != 452)
            {
                eReturn = eReturn.BUFFER_LENGTH_NOT_PROPER;
            }
            else
            {
                byte[] bytes1 = new byte[6];
                byte[] pageTimeByte = new byte[9];
                byte[] bytes2 = new byte[20];
                byte[] bytes3 = new byte[20];
                byte[] bytes4 = new byte[20];
                byte[] bytes5 = new byte[20];
                byte[] bytes6 = new byte[20];
                byte[] bytes7 = new byte[20];
                byte[] bytes8 = new byte[20];
                byte[] bytes9 = new byte[20];
                byte[] bytes10 = new byte[20];
                byte[] bytes11 = new byte[20];
                byte[] bytes12 = new byte[20];
                byte[] bytes13 = new byte[20];
                byte[] bytes14 = new byte[20];
                for (int index = 0; index < 6; ++index)
                    bytes1[index] = buffer[index];
                for (int index = 0; index < 9; ++index)
                    pageTimeByte[index] = buffer[index + 6];
                for (int index = 0; index < 20; ++index)
                    bytes2[index] = buffer[index + 187];
                for (int index = 0; index < 20; ++index)
                    bytes3[index] = buffer[index + 207];
                for (int index = 0; index < 20; ++index)
                    bytes4[index] = buffer[index + 227];
                for (int index = 0; index < 20; ++index)
                    bytes5[index] = buffer[index + 247];
                for (int index = 0; index < 20; ++index)
                    bytes6[index] = buffer[index + 267];
                byte[] numArray = new byte[2]
                {
          buffer[287],
          (byte) 0
                };
                for (int index = 0; index < 20; ++index)
                    bytes7[index] = buffer[index + 288];
                for (int index = 0; index < 20; ++index)
                    bytes8[index] = buffer[index + 308];
                //new byte[1][0] = buffer[328];
                byte num1 = buffer[329];
                for (int index = 0; index < 20; ++index)
                    bytes9[index] = buffer[index + 330];
                for (int index = 0; index < 20; ++index)
                    bytes10[index] = buffer[index + 350];
                for (int index = 0; index < 20; ++index)
                    bytes11[index] = buffer[index + 370];
                byte num2 = buffer[390];
                for (int index = 0; index < 20; ++index)
                    bytes12[index] = buffer[index + 391];
                for (int index = 0; index < 20; ++index)
                    bytes13[index] = buffer[index + 411];
                for (int index = 0; index < 20; ++index)
                    bytes14[index] = buffer[index + 431];
                byte num3 = buffer[451];
                try
                {
                    deviceData.ObjDeviceInfo.DeviceUniqueSerialCode = Encoding.ASCII.GetString(bytes1);
                    //deviceData.ObjDeviceInfo.CalibrationDate = this.ParsePageTime(pageTimeByte);
                    deviceData.ObjDeviceInfo.AccRange = Encoding.ASCII.GetString(bytes2);
                    deviceData.ObjDeviceInfo.accResolution = Encoding.ASCII.GetString(bytes3);
                    //deviceData.ObjDeviceInfo.DeviceType = Encoding.ASCII.GetString(bytes4);
                    deviceData.ObjDeviceInfo.DeviceModel = Encoding.ASCII.GetString(bytes5);
                    deviceData.ObjDeviceInfo.FirmwareVersion = Encoding.ASCII.GetString(bytes6);
                    deviceData.ObjDeviceInfo.MemorySize = BitConverter.ToUInt16(numArray, 0);
                    deviceData.ObjDeviceInfo.BatterySizeAndType = Encoding.ASCII.GetString(bytes7);
                    deviceData.ObjDeviceInfo.AccUnits = Encoding.ASCII.GetString(bytes8);
                    deviceData.ObjDeviceInfo.LightmeterRange = Encoding.ASCII.GetString(bytes9);
                    deviceData.ObjDeviceInfo.LightmeterResolution = Encoding.ASCII.GetString(bytes10);
                    deviceData.ObjDeviceInfo.LightmeterUnits = Encoding.ASCII.GetString(bytes11);
                    deviceData.ObjDeviceInfo.TemperatureRange = Encoding.ASCII.GetString(bytes12);
                    deviceData.ObjDeviceInfo.TemperatureResolution = Encoding.ASCII.GetString(bytes13);
                    deviceData.ObjDeviceInfo.TemperatureUnits = Encoding.ASCII.GetString(bytes14);
                    eReturn = eReturn.SUCCESS;
                }
                catch
                {
                    eReturn = eReturn.FAILURE;
                }
                if (eReturn == eReturn.SUCCESS)
                {
                    deviceData.ObjDeviceInfo.TempSensorPresent = (int)Convert.ToUInt16(num2) == 1;
                    deviceData.ObjDeviceInfo.LightSensorPresent = (int)Convert.ToUInt16(num1) == 1;
                    deviceData.ObjDeviceInfo.ButtonPresent = (int)Convert.ToUInt16(num3) == 1;
                    eReturn = eReturn.SUCCESS;
                }
            }
            return eReturn;
        }

        public eReturn ParseAndPackGetConfigInfo(DeviceData deviceObject, byte[] buffer)
        {
            if (buffer.Length != 45)
                return eReturn.BUFFER_LENGTH_NOT_PROPER;
            byte[] measurementFreq = new byte[2]
            {
        buffer[0],
        (byte) 0
            };
            byte[] numArray = new byte[2] { buffer[2], buffer[1] };
            byte[] startMode = new byte[2] { buffer[3], (byte)0 };
            byte[] pageTimeByte1 = new byte[9];
            byte[] timeZone = new byte[3]
            {
        buffer[13],
        buffer[14],
        buffer[15]
            };
            byte[] bytes = new byte[20];
            byte[] pageTimeByte2 = new byte[9];
            try
            {
                Array.Copy((Array)buffer, 4, (Array)pageTimeByte1, 0, 9);
                Array.Copy((Array)buffer, 16, (Array)bytes, 0, 20);
                Array.Copy((Array)buffer, 36, (Array)pageTimeByte2, 0, 9);
                deviceObject.ObjDeviceInfo.MeasurementFreq = this.ParseMeasurementFreq(measurementFreq);
                deviceObject.ObjDeviceInfo.MeasurementPeriod = (int)BitConverter.ToInt16(numArray, 0);
                deviceObject.ObjDeviceInfo.StartMode = this.ParseStartMode(startMode);
                //deviceObject.ObjDeviceInfo.StartTime = this.ParsePageTime(pageTimeByte1);
                deviceObject.ObjDeviceInfo.TimeZone = this.ParseTimeZone(timeZone);
                // deviceObject.ObjDeviceInfo.ConfigTime = this.ParsePageTime(pageTimeByte2);
                //deviceObject.ObjDeviceInfo.ConfigOperatorID = Encoding.ASCII.GetString(bytes);
                return eReturn.SUCCESS;
            }
            catch
            {
                return eReturn.FAILURE;
            }
        }

        public eReturn ParseAndPackGetCalibInfo(byte[] buffer, CalibrationInformation calib)
        {
            if (buffer.Length != 28)
                return eReturn.BUFFER_LENGTH_NOT_PROPER;
            byte[] numArray1 = new byte[2] { buffer[1], buffer[0] };
            byte[] numArray2 = new byte[2] { buffer[3], buffer[2] };
            byte[] numArray3 = new byte[2] { buffer[5], buffer[4] };
            byte[] numArray4 = new byte[2] { buffer[7], buffer[6] };
            byte[] numArray5 = new byte[2] { buffer[9], buffer[8] };
            byte[] numArray6 = new byte[2]
            {
        buffer[11],
        buffer[10]
            };
            byte[] numArray7 = new byte[2]
            {
        buffer[13],
        buffer[12]
            };
            byte[] numArray8 = new byte[2]
            {
        buffer[15],
        buffer[14]
            };
            byte[] numArray9 = new byte[2]
            {
        buffer[17],
        buffer[16]
            };
            byte[] bytes = new byte[10];
            for (int index = 0; index < 10; ++index)
                bytes[index] = buffer[index + 18];
            try
            {
                calib.Xgain = BitConverter.ToInt16(numArray1, 0);
                calib.Xoffset = BitConverter.ToInt16(numArray2, 0);
                calib.Ygain = BitConverter.ToInt16(numArray3, 0);
                calib.Yoffset = BitConverter.ToInt16(numArray4, 0);
                calib.Zgain = BitConverter.ToInt16(numArray5, 0);
                calib.Zoffset = BitConverter.ToInt16(numArray6, 0);
                calib.Volts = BitConverter.ToInt16(numArray7, 0);
                calib.Lux = BitConverter.ToInt16(numArray8, 0);
                calib.ClockOffset = BitConverter.ToInt16(numArray9, 0);
                calib.DeviceElectronicID = Encoding.ASCII.GetString(bytes);
                return eReturn.SUCCESS;
            }
            catch
            {
                return eReturn.FAILURE;
            }
        }

        private string ParseTimeZone(byte[] timeZone)
        {
            ushort num1 = (ushort)timeZone[0];
            byte[] bcdValue1 = new byte[1] { timeZone[1] };
            byte[] bcdValue2 = new byte[1] { timeZone[2] };
            string str1 = (int)num1 != 0 ? "GMT -" : "GMT +";
            ushort num2 = this.BcdToDecimal(bcdValue1);
            string str2 = num2.ToString();
            if (str2.Length < 2)
                str1 += "0";
            string str3 = str1 + str2 + ":";
            num2 = this.BcdToDecimal(bcdValue2);
            string str4 = num2.ToString();
            if (str4.Length < 2)
                str3 += "0";
            return str3 + str4;
        }

        private string ParseStartMode(byte[] startMode)
        {
            string str;
            switch (BitConverter.ToUInt16(startMode, 0))
            {
                case 0:
                    str = "Not configured";
                    break;
                case 1:
                    str = "On button press";
                    break;
                case 2:
                    str = "On button press - button active";
                    break;
                case 3:
                    str = "Immediately on disconnect";
                    break;
                case 4:
                    str = "At future time";
                    break;
                default:
                    str = "Not configured";
                    break;
            }
            return str;
        }

        private float ParseMeasurementFreq(byte[] measurementFreq)
        {
            float num = 0.0f;
            switch (BitConverter.ToUInt16(measurementFreq, 0))
            {
                case 1:
                    num = 10f;
                    break;
                case 2:
                    num = 20f;
                    break;
                case 3:
                    num = 25f;
                    break;
                case 4:
                    num = 30f;
                    break;
                case 5:
                    num = 40f;
                    break;
                case 6:
                    num = 50f;
                    break;
                case 7:
                    num = 60f;
                    break;
                case 8:
                    num = 66.7f;
                    break;
                case 9:
                    num = 75f;
                    break;
                case 10:
                    num = 85.7f;
                    break;
                case 11:
                    num = 100f;
                    break;
                case 12:
                    num = 1000f;
                    break;
                case 13:
                    num = 500f;
                    break;
            }
            return num;
        }

        public eReturn ParseAndPackMemoryInfo(byte[] buffer, DeviceStatusInfo statusInfo)
        {
            eReturn eReturn;
            if (buffer.Length != 3)
            {
                eReturn = eReturn.BUFFER_LENGTH_NOT_PROPER;
            }
            else
            {
                byte[] numArray = new byte[4]
                {
          buffer[2],
          buffer[1],
          buffer[0],
          (byte) 0
                };
                statusInfo.NoPagesAvailable = BitConverter.ToUInt32(numArray, 0);
                eReturn = eReturn.SUCCESS;
            }
            return eReturn;
        }

        private ushort BcdToDecimal(byte[] bcdValue)
        {
            ushort num1 = 0;
            foreach (int num2 in bcdValue)
            {
                int num3 = num2 >> 4;
                int num4 = num2 & 15;
                num1 = (ushort)((int)num1 * 100 + num3 * 10 + num4);
            }
            return num1;
        }

        public eReturn ParseGetTime(byte[] dateTimeByte, ref DateTime getTime)
        {
            byte[] bcdValue1 = new byte[2]
            {
        dateTimeByte[0],
        dateTimeByte[1]
            };
            byte[] bcdValue2 = new byte[1] { dateTimeByte[2] };
            byte[] bcdValue3 = new byte[1] { dateTimeByte[3] };
            byte[] bcdValue4 = new byte[1] { dateTimeByte[4] };
            byte[] bcdValue5 = new byte[1] { dateTimeByte[5] };
            byte[] bcdValue6 = new byte[1] { dateTimeByte[6] };
            byte num = (byte)(((int)dateTimeByte[7] & 15) << 4 | (int)dateTimeByte[8] >> 4);
            byte[] bcdValue7 = new byte[2]
            {
        (byte) (((int) dateTimeByte[7] & 240) >> 4),
        num
            };
            try
            {
                getTime = new DateTime((int)this.BcdToDecimal(bcdValue1), (int)this.BcdToDecimal(bcdValue2), (int)this.BcdToDecimal(bcdValue3), (int)this.BcdToDecimal(bcdValue4), (int)this.BcdToDecimal(bcdValue5), (int)this.BcdToDecimal(bcdValue6), (int)this.BcdToDecimal(bcdValue7));
                return eReturn.SUCCESS;
            }
            catch
            {
                return eReturn.FAILURE;
            }
        }

        public eReturn PackSetTime(DateTime dateTime, ref byte[] setTime)
        {
            byte[] bcd1 = this.IntToBCD(dateTime.Year);
            byte[] bcd2 = this.IntToBCD(dateTime.Month);
            byte[] bcd3 = this.IntToBCD(dateTime.Day);
            byte[] bcd4 = this.IntToBCD(dateTime.Hour);
            byte[] bcd5 = this.IntToBCD(dateTime.Minute);
            byte[] bcd6 = this.IntToBCD(dateTime.Second);
            byte[] bcd7 = this.IntToBCD(dateTime.Millisecond);
            bcd7[0] = (byte)((uint)bcd7[0] << 4);
            bcd7[0] = (byte)((uint)bcd7[0] | (uint)bcd7[1] >> 4);
            bcd7[1] = (byte)((uint)bcd7[1] << 4);
            byte[] numArray = new byte[2] { bcd7[0], bcd7[1] };
            try
            {
                Array.Copy((Array)bcd1, 0, (Array)setTime, 0, 2);
                Array.Copy((Array)bcd2, 1, (Array)setTime, 2, 1);
                Array.Copy((Array)bcd3, 1, (Array)setTime, 3, 1);
                Array.Copy((Array)bcd4, 1, (Array)setTime, 4, 1);
                Array.Copy((Array)bcd5, 1, (Array)setTime, 5, 1);
                Array.Copy((Array)bcd6, 1, (Array)setTime, 6, 1);
                Array.Copy((Array)numArray, 0, (Array)setTime, 7, 2);
                return eReturn.SUCCESS;
            }
            catch
            {
                return eReturn.FAILURE;
            }
        }



        private byte[] IntToBCD(int input)
        {
            if (input > 9999 || input < 0)
                throw new ArgumentOutOfRangeException(nameof(input));
            int num1 = input / 1000;
            int num2 = (input -= num1 * 1000) / 100;
            int num3 = (input -= num2 * 100) / 10;
            int num4 = input -= num3 * 10;
            return new byte[2]
            {
        (byte) (num1 << 4 | num2),
        (byte) (num3 << 4 | num4)
            };
        }
    }
}
