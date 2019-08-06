

using System;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace EpochCompressingTool
{
    public class DeviceCapabilities
    {
        private DataManager m_DataManagerObject = DataManager.dataManager;
        private ConnectionManager m_ConnectionManagerObject = ConnectionManager.ConnectionManagerInstance;
        //private int m_MaxNoOfDataTraceDataPoints = 60;
        private const ushort m_NoOfPageDataPoints = 300;
        private const ushort m_PageHeaderLength = 22;
        private const ushort m_NoOfRecordeDataBytes = 1800;
        private const ushort m_NoOfBytesPerPage = 1822;
        private const ushort m_LengthOfDataPoints = 6;
        public string m_DataTraceDateList;
        public double m_DataTraceAccXYValue;
        public double m_DataTraceAccYYValue;
        public double m_DataTraceAccZYValue;
        public double m_DataTraceLightMeterYValue;
        public double m_DataTraceButtonStatusYValue;
        public double m_DataTraceTempYValue;

        public eReturn getDeviceCapabilitiesInfo(string selectedDeviceId, out DeviceData deviceObject)
        {
            deviceObject = (DeviceData)null;
            eReturn eReturn;
            if (this.m_DataManagerObject.deviceIDTable.Contains((object)selectedDeviceId))
            {
                deviceObject = (DeviceData)this.m_DataManagerObject.deviceIDTable[(object)selectedDeviceId];
                eReturn = eReturn.SUCCESS;
            }
            else
                eReturn = eReturn.DEVICE_NOT_CONNECTED;
            return eReturn;
        }

        public eReturn GetSensorOutputData(string deviceIndex, DeviceData deviceObject)
        {
            return this.m_ConnectionManagerObject.GetSensorOutputData(deviceIndex, deviceObject);
        }


        public eReturn ParseAndPackRecodedData(byte[] buffer, DeviceData recordedData)
        {
            eReturn eReturn;
            if (buffer.Length != 6)
            {
                eReturn = eReturn.BUFFER_LENGTH_NOT_PROPER;
            }
            else
            {
                byte[] parseByte1 = new byte[2]
                {
          buffer[0],
          buffer[1]
                };
                byte[] parseByte2 = new byte[2]
                {
          buffer[1],
          buffer[2]
                };
                byte[] parseByte3 = new byte[2]
                {
          buffer[3],
          buffer[4]
                };
                byte[] parseByte4 = new byte[2]
                {
          buffer[4],
          buffer[5]
                };
                byte parseByte5 = buffer[5];
                CalibrationInformation calibrationInformation = new CalibrationInformation();
                calibrationInformation.Xgain = recordedData.ObjCalibInfo.Xgain;
                calibrationInformation.Xoffset = recordedData.ObjCalibInfo.Xoffset;
                calibrationInformation.Ygain = recordedData.ObjCalibInfo.Ygain;
                calibrationInformation.Yoffset = recordedData.ObjCalibInfo.Yoffset;
                calibrationInformation.Zgain = recordedData.ObjCalibInfo.Zgain;
                calibrationInformation.Zoffset = recordedData.ObjCalibInfo.Zoffset;
                calibrationInformation.Volts = recordedData.ObjCalibInfo.Volts;
                calibrationInformation.Lux = recordedData.ObjCalibInfo.Lux;
                float accXaxis = (float)this.ParseAccXaxis(parseByte1);
                float accYaxis = (float)this.ParseAccYaxis(parseByte2);
                float accZaxis = (float)this.ParseAccZaxis(parseByte3);
                ushort lightMeter = this.ParseLightMeter(parseByte4);
                string buttonStatus = this.ParseButtonStatus(parseByte5);
                float num1 = recordedData.CalibrateXAxis(accXaxis);
                recordedData.ObjRecordedData.ArrXaxis.Add(num1);
                float num2 = recordedData.CalibrateYAxis(accYaxis);
                recordedData.ObjRecordedData.ArrYaxis.Add(num2);
                float num3 = recordedData.CalibrateZAxis(accZaxis);
                recordedData.ObjRecordedData.ArrZaxis.Add(num3);
                ushort num4 = (int)calibrationInformation.Volts != 0 ? (ushort)((uint)lightMeter * (uint)calibrationInformation.Lux / (uint)calibrationInformation.Volts) : (ushort)0;
                recordedData.ObjRecordedData.ArrLightMeter.Add(num4);
                recordedData.ObjRecordedData.ArrButtonStatus.Add(buttonStatus);
                eReturn = eReturn.SUCCESS;
            }
            return eReturn;
        }

        public eReturn ParseAndPackGetStatus(DeviceData deviceData, byte[] buffer)
        {
            eReturn eReturn;
            if (buffer.Length != 18)
            {
                eReturn = eReturn.BUFFER_LENGTH_NOT_PROPER;
            }
            else
            {
                byte[] parseByte1 = new byte[2]
                {
          buffer[0],
          buffer[1]
                };
                byte[] parseByte2 = new byte[2]
                {
          buffer[1],
          buffer[2]
                };
                byte[] parseByte3 = new byte[2]
                {
          buffer[3],
          buffer[4]
                };
                byte[] parseByte4 = new byte[2]
                {
          buffer[4],
          buffer[5]
                };
                byte[] temperature1 = new byte[2]
                {
          buffer[5],
          buffer[6]
                };
                byte[] batteryVoltage1 = new byte[2]
                {
          buffer[7],
          buffer[8]
                };
                byte parseByte5 = (byte)((uint)buffer[8] >> 4);
                byte[] deviceStatus = new byte[2]
                {
          (byte) ((uint) buffer[8] & 15U),
          (byte) 0
                };
                byte[] pageTimeByte = new byte[9];
                for (int index = 0; index < 9; ++index)
                    pageTimeByte[index] = buffer[index + 9];
                int accXaxis = this.ParseAccXaxis(parseByte1);
                //Console.WriteLine(accXaxis);
                //Console.ReadKey();
                int accYaxis = this.ParseAccYaxis(parseByte2);
                //Console.WriteLine(accYaxis);
                //Console.ReadKey();
                int accZaxis = this.ParseAccZaxis(parseByte3);
                //Console.WriteLine(accZaxis);
                //Console.ReadKey();
                ushort lightMeter = this.ParseLightMeter(parseByte4);
                //Console.WriteLine(lightMeter);
                //Console.ReadKey();
                ushort temperature2 = this.ParseTemperature(temperature1);
                //Console.WriteLine(temperature2);
                //Console.ReadKey();          
                ushort batteryVoltage2 = this.ParseBatteryVoltage(batteryVoltage1);

                deviceData.ObjDeviceStatusInfo.AccXaxis = accXaxis;
                deviceData.ObjDeviceStatusInfo.AccYaxis = accYaxis;
                deviceData.ObjDeviceStatusInfo.AccZaxis = accZaxis;
                deviceData.ObjDeviceStatusInfo.LightMeter = (int)deviceData.ObjCalibInfo.Volts != 0 ? (float)(ushort)((uint)lightMeter * (uint)deviceData.ObjCalibInfo.Lux / (uint)deviceData.ObjCalibInfo.Volts) : 0.0f;
                deviceData.ObjDeviceStatusInfo.BatteryVoltage = batteryVoltage2;
                deviceData.ObjDeviceStatusInfo.Temperature = (float)((int)temperature2 - 181) / 3.76f;
                deviceData.ObjDeviceStatusInfo.ButtonStatus = this.ParseButtonStatus(parseByte5);
                //deviceData.ObjDeviceStatusInfo.deviceTime = this.ParsePageTime(pageTimeByte);
                deviceData.ObjDeviceStatusInfo.DeviceStatus = this.ParseDeviceStatus(deviceStatus);
                eReturn = eReturn.SUCCESS;
            }
            return eReturn;
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
                Console.WriteLine("XGAIN" + calib.Xgain);
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

        private int ParseAccXaxis(byte[] parseByte)
        {
            bool flag = false;
            byte num1 = (byte)((uint)parseByte[1] & 240U);
            if ((int)parseByte[0] >> 7 != 0)
                flag = true;
            byte num2 = (byte)((uint)num1 >> 4);
            byte num3 = (byte)((uint)parseByte[0] << 4 | (uint)num2);
            short int16;
            if (flag)
            {
                parseByte[0] = (byte)(((int)parseByte[0] & 240) >> 4);
                int16 = BitConverter.ToInt16(new byte[2]
                {
          num3,
          (byte) ((uint) parseByte[0] | 240U)
                }, 0);
            }
            else
            {
                parseByte[0] = (byte)(((int)parseByte[0] & 112) >> 4);
                int16 = BitConverter.ToInt16(new byte[2]
                {
          num3,
          parseByte[0]
                }, 0);
            }

            return (int)int16;
        }

        private int ParseAccYaxis(byte[] parseByte)
        {
            bool flag = false;
            byte num1 = (byte)((uint)parseByte[0] & 15U);
            if ((int)num1 >> 3 != 0)
                flag = true;
            short int16;
            if (flag)
            {
                byte num2 = (byte)((uint)num1 | 240U);
                int16 = BitConverter.ToInt16(new byte[2]
                {
          parseByte[1],
          num2
                }, 0);
            }
            else
            {
                byte num2 = (byte)((uint)num1 & 7U);
                int16 = BitConverter.ToInt16(new byte[2]
                {
          parseByte[1],
          num2
                }, 0);
            }
            return (int)int16;
        }

        private int ParseAccZaxis(byte[] parseByte)
        {
            bool flag = false;
            byte num1 = (byte)((uint)parseByte[1] & 240U);
            if ((int)parseByte[0] >> 7 != 0)
                flag = true;
            byte num2 = (byte)((uint)num1 >> 4);
            byte num3 = (byte)((uint)parseByte[0] << 4 | (uint)num2);
            short int16;
            if (flag)
            {
                parseByte[0] = (byte)(((int)parseByte[0] & 240) >> 4);
                int16 = BitConverter.ToInt16(new byte[2]
                {
          num3,
          (byte) ((uint) parseByte[0] | 240U)
                }, 0);
            }
            else
            {
                parseByte[0] = (byte)(((int)parseByte[0] & 112) >> 4);
                int16 = BitConverter.ToInt16(new byte[2]
                {
          num3,
          parseByte[0]
                }, 0);
            }
            return (int)int16;
        }

        private ushort ParseLightMeter(byte[] parseByte)
        {
            byte num1 = (byte)((uint)parseByte[0] & 15U);
            parseByte[1] = (byte)(((int)parseByte[1] & 252) >> 2 | ((int)num1 & 3) << 6);
            byte num2 = (byte)((uint)num1 >> 2);
            return BitConverter.ToUInt16(new byte[2]
            {
        parseByte[1],
        num2
            }, 0);
        }

        private string ParseButtonStatus(byte parseByte)
        {
            int num1 = (int)(byte)((uint)(byte)((uint)parseByte & 2U) >> 1);
            int num2 = 0;
            int num3 = 1;
            if (num1 == num3)
                ++num2;
            return num2.ToString();
        }

        private ushort ParseTemperature(byte[] temperature)
        {
            return BitConverter.ToUInt16(new byte[2]
            {
        temperature[1],
        (byte) ((uint) temperature[0] & 3U)
            }, 0);
        }

        private ushort ParseBatteryVoltage(byte[] batteryVoltage)
        {
            batteryVoltage[1] = (byte)(((int)batteryVoltage[1] & 192) >> 6);
            batteryVoltage[1] = (byte)((uint)batteryVoltage[1] | (uint)batteryVoltage[0] << 2);
            batteryVoltage[0] = (byte)(((int)batteryVoltage[0] & 192) >> 6);
            return BitConverter.ToUInt16(new byte[2]
            {
        batteryVoltage[1],
        batteryVoltage[0]
            }, 0);
        }

        private string ParseDeviceStatus(byte[] deviceStatus)
        {
            string str = (string)null;
            switch (BitConverter.ToUInt16(deviceStatus, 0))
            {
                case 0:
                    str = "Not configured";
                    break;
                case 1:
                    str = "Recording interrupted";
                    break;
                case 2:
                    str = "Recording finished";
                    break;
                case 3:
                    str = "Ready to record on button press";
                    break;
                case 4:
                    str = "Ready to record on button press (active button)";
                    break;
                case 5:
                    str = "Ready to record on disconnect";
                    break;
                case 6:
                    str = "Ready to record at <future time>";
                    break;
                case 7:
                    str = "Busy";
                    break;
                case 8:
                    str = "Recording";
                    break;
                case 9:
                    str = "Firmware uploading";
                    break;
                case 10:
                    str = "Configuration incomplete";
                    break;
            }
            return str;
        }

        private string ParseSerialCode(byte[] serialCodeByte)
        {
            string str = (string)null;
            if (serialCodeByte != null)
                str = Encoding.ASCII.GetString(serialCodeByte);
            return str;
        }

        private uint ParsePageSequenceNumber(byte[] pageNumber)
        {
            byte[] numArray = new byte[4]
            {
        pageNumber[2],
        pageNumber[1],
        pageNumber[0],
        (byte) 0
            };
            uint num = 0;
            if (pageNumber != null)
                num = BitConverter.ToUInt32(numArray, 0);
            return num;
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

        private byte[] DecimalToBcd(int decimalValue)
        {
            int num1 = 0;
            if (decimalValue >= 0 && decimalValue <= 9999)
            {
                for (int index = 0; index < 4; ++index)
                {
                    int num2 = decimalValue % 10;
                    num1 |= num2 << index * 4;
                    decimalValue /= 10;
                }
            }
            return new byte[2]
            {
        (byte) (num1 >> 8 & (int) byte.MaxValue),
        (byte) (num1 & (int) byte.MaxValue)
            };
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
    }
}
