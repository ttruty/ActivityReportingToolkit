
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace EpochCompressingTool
{
    public class DataConverter
    {
        public string m_strUpdateInfo;
        private const string BIN_DEVICE_STATUS = "Device Status";
        private const string BIN_STATUS_RECOVERED = "Recovered";
        int pageCount;

        public event DataConverter.ExtractUpdateAndUpdate NewExtractUpdate;

        public eReturn CompressBinToCSV(string binFilePath_i, string csvFilePath_i, int nEpochPeriod_i, float tempCompVal, out uint recoveredDataBlocks)
        {
            recoveredDataBlocks = 0U;
            DeviceData deviceData = new DeviceData();
            StreamReader reader = (StreamReader)null;
            StreamWriter writer = (StreamWriter)null;
            PageTime pageTime1 = new PageTime();
            PageTime pageTime2 = new PageTime();
            PageTime StartTimeStamp_i = new PageTime();
            RecordedData recordedData_io = new RecordedData();
            StringBuilder stringBuilder_io = new StringBuilder();
            GeneralUtility generalUtility = new GeneralUtility();
            var progress = new ProgressBar();
            int progressCount = 0;

            float fSVM_io = 0.0f;
            bool flag1 = false;
            bool flag2 = true;
            if (binFilePath_i == null || csvFilePath_i == null)
                return eReturn.FAILURE;
            if (!File.Exists(binFilePath_i))
                return eReturn.FILE_DOESNOT_EXIST;
            try
            {
                reader = new StreamReader((Stream)File.Open(binFilePath_i, FileMode.Open));
                if (reader == null)
                    return eReturn.FAILURE;
                writer = new StreamWriter((Stream)File.Open(csvFilePath_i, FileMode.Create));
                if (writer == null)
                {
                    reader.Close();
                    return eReturn.FAILURE;
                }
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();
                if (writer != null)
                    writer.Close();
                return eReturn.FILE_IO_EXCEPTION;
            }
            eReturn eReturn1;
            try
            {
                eReturn eReturn2 = this.WriteCSVHeader(reader, writer, deviceData);
                if (eReturn2 == eReturn.SUCCESS)
                {
                    string str1 = (string)null;
                    DeviceCapabilities deviceCapabilities = new DeviceCapabilities();
                    CalibrationInformation calibrationInformation = new CalibrationInformation();
                    while ((str1 = reader.ReadLine()) != null)
                    {
                        string[] strArray1 = (string[])null;
                        string[] strArray2 = (string[])null;
                        string[] strArray3 = (string[])null;
                        string str2;
                        while (((str2 = reader.ReadLine()) != "Measurement Frequency" || str2 != null) && str2 != null)
                        {
                            if (str2.Contains("Temperature"))
                            {
                                strArray1 = str2.Split(':');
                            }
                            else
                            {
                                if (str2.Contains("Measurement Frequency"))
                                {
                                    strArray2 = str2.Split(':');
                                    break;
                                }
                                if (str2.Contains("Page Time"))
                                {
                                    string[] strArray4 = str2.Split(':');
                                    if (strArray4.Length == 5)
                                    {
                                        string str3 = strArray4[0] + ":";
                                        string[] strArray5 = strArray4[1].Split(' ');
                                        if (strArray5.Length == 2)
                                        {
                                            string[] strArray6 = strArray5[0].Split('-');
                                            if (strArray6.Length == 3)
                                                str3 = str3 + strArray6[0] + ":" + strArray6[1] + ":" + strArray6[2] + ":";
                                            str3 = str3 + strArray5[1] + ":";
                                        }
                                        strArray3 = (str3 + strArray4[2] + ":" + strArray4[3] + ":" + strArray4[4]).Split(':');
                                    }
                                    else
                                        break;
                                }
                                else if (str2.StartsWith("Device Status"))
                                {
                                    string[] strArray4 = str2.Split(':');
                                    if (strArray4.Length == 2 && strArray4[1] == "Recovered")
                                        ++recoveredDataBlocks;
                                }
                            }
                        }
                        if (strArray2 != null && strArray1 != null && strArray3 != null)
                        {
                            string[] strArray4 = new string[7];
                            for (int index = 0; index < 7; ++index)
                                strArray4[index] = strArray3[index + 1];
                            float floatAgnostic = generalUtility.ToFloatAgnostic(strArray1[1]);
                            double doubleAgnostic = generalUtility.ToDoubleAgnostic(strArray2[1]);
                            double num1 = 1000.0 / doubleAgnostic;
                            PageTime pageTime3 = new PageTime();
                            string hex = reader.ReadLine();
                            byte[] numArray = (byte[])null;
                            if (hex.Length % 2 == 0)
                            {
                                numArray = DataConverter.StringToByteArray(hex);
                                eReturn2 = eReturn.SUCCESS;
                            }
                            else
                                eReturn2 = eReturn.CORRUPTED_BIN_FILE;
                            if (eReturn2 == eReturn.SUCCESS)
                            {
                                int index1 = 0;
                                if (numArray.Length >= 1800)
                                {
                                    for (int index2 = 0; index2 < 300; ++index2)
                                    {
                                        byte[] buffer = new byte[6];
                                        for (int index3 = 0; index3 < 6; ++index3)
                                        {
                                            buffer[index3] = numArray[index1];
                                            ++index1;
                                        }
                                        int andPackRecodedData = (int)deviceCapabilities.ParseAndPackRecodedData(buffer, deviceData);
                                        if (index2 == 0)
                                        {
                                            pageTime1.Year = Convert.ToUInt16(strArray4[0]);
                                            pageTime1.Month = Convert.ToUInt16(strArray4[1]);
                                            pageTime1.Date = Convert.ToUInt16(strArray4[2]);
                                            pageTime1.Hour = Convert.ToUInt16(strArray4[3]);
                                            pageTime1.Minute = Convert.ToUInt16(strArray4[4]);
                                            pageTime1.Second = Convert.ToUInt16(strArray4[5]);
                                            pageTime1.Millisecond = Convert.ToUInt16(strArray4[6]);
                                        }
                                        else
                                            pageTime1 += 1.0 / doubleAgnostic;
                                        if (flag2)
                                        {
                                            StartTimeStamp_i = pageTime1;
                                            pageTime2 = pageTime1 + (double)nEpochPeriod_i;
                                            flag2 = false;
                                        }
                                        if (pageTime1.Ticks >= pageTime2.Ticks)
                                        {
                                            if (recordedData_io.ArrXaxis.Count > 0)
                                            {
                                                this.UpdateStringBuilder(StartTimeStamp_i, ref recordedData_io, ref stringBuilder_io, ref fSVM_io);
                                                flag1 = true;
                                            }
                                            pageTime2 += (double)nEpochPeriod_i;
                                            if (pageTime1.Ticks >= pageTime2.Ticks)
                                                pageTime2 = pageTime1 + (double)nEpochPeriod_i;
                                            StartTimeStamp_i = pageTime2 - (double)nEpochPeriod_i;
                                        }
                                        recordedData_io.ArrXaxis.Add(deviceData.ObjRecordedData.ArrXaxis[index2]);
                                        recordedData_io.ArrYaxis.Add(deviceData.ObjRecordedData.ArrYaxis[index2]);
                                        float num2 = deviceData.ObjDeviceInfo.TempCompensateAxisValue(deviceData.ObjRecordedData.ArrZaxis[index2], floatAgnostic, tempCompVal);
                                        recordedData_io.ArrZaxis.Add(num2);
                                        recordedData_io.ArrLightMeter.Add(deviceData.ObjRecordedData.ArrLightMeter[index2]);
                                        recordedData_io.ArrButtonStatus.Add(deviceData.ObjRecordedData.ArrButtonStatus[index2]);
                                        recordedData_io.ArrTemperature.Add(floatAgnostic);
                                        fSVM_io += Math.Abs((float)Math.Sqrt(Math.Pow((double)deviceData.ObjRecordedData.ArrXaxis[index2], 2.0) + Math.Pow((double)deviceData.ObjRecordedData.ArrYaxis[index2], 2.0) + Math.Pow((double)num2, 2.0)) - 1f);
                                    }
                                    if (flag1 && stringBuilder_io.Length > 10000)
                                    {
                                        writer.Write((object)stringBuilder_io);
                                        stringBuilder_io.Remove(0, stringBuilder_io.Length);
                                        flag1 = false;
                                    }
                                    deviceData.ObjRecordedData.ArrLightMeter.Clear();
                                    deviceData.ObjRecordedData.ArrButtonStatus.Clear();
                                    deviceData.ObjRecordedData.ArrXaxis.Clear();
                                    deviceData.ObjRecordedData.ArrYaxis.Clear();
                                    deviceData.ObjRecordedData.ArrZaxis.Clear();
                                }
                                else
                                {
                                    eReturn2 = eReturn.CORRUPTED_BIN_FILE;
                                    break;
                                }

                                progress.Report((double)progressCount / pageCount);
                                //Thread.Sleep(1);

                            }
                        }

                        else
                        {
                            eReturn2 = eReturn.CORRUPTED_BIN_FILE;
                            break;
                        }
                        progressCount++;
                    }
                    if (stringBuilder_io.Length > 0)
                    {
                        writer.Write((object)stringBuilder_io);
                        stringBuilder_io.Remove(0, stringBuilder_io.Length);
                    }
                    if (eReturn2 != eReturn.CORRUPTED_BIN_FILE)
                        writer.Close();
                }

                reader.Close();
                writer.Close();
                return eReturn2;
            }
            catch (IOException ex)
            {
                if (reader != null)
                    reader.Close();
                if (writer != null)
                    writer.Close();
                return eReturn.OUT_OF_MEMORY;
            }
            catch (OutOfMemoryException ex)
            {
                if (reader != null)
                    reader.Close();
                if (writer != null)
                    writer.Close();
                return eReturn.OUT_OF_MEMORY;
            }
            catch
            {
                if (reader != null)
                    reader.Close();
                if (writer != null)
                    writer.Close();
                eReturn1 = eReturn.INVALID_CSV_FILE;
            }
            if (eReturn1 != eReturn.SUCCESS)
                File.Delete(csvFilePath_i);
            return eReturn.INVALID_CSV_FILE;
        }

        private void UpdateStringBuilder(PageTime StartTimeStamp_i, ref RecordedData recordedData_io, ref StringBuilder stringBuilder_io, ref float fSVM_io)
        {
            if (recordedData_io.ArrXaxis.Count == 0)
                return;
            GeneralUtility generalUtility = new GeneralUtility();
            string str = StartTimeStamp_i.ToString("yyyy-MM-dd HH:mm:ss:fff");
            if (":" != CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator)
                str = str.Replace(CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator, ":");
            stringBuilder_io.Append(str);
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot((double)recordedData_io.ArrXaxis.Average(), 4));
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot((double)recordedData_io.ArrYaxis.Average(), 4));
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot((double)recordedData_io.ArrZaxis.Average(), 4));
            stringBuilder_io.Append(",");
            int num1 = 0;
            foreach (ushort num2 in recordedData_io.ArrLightMeter)
                num1 += (int)num2;
            int num3 = num1 / recordedData_io.ArrLightMeter.Count;
            stringBuilder_io.Append(num3);
            int num4 = 0;
            foreach (string arrButtonStatu in recordedData_io.ArrButtonStatus)
            {
                if ("1" == arrButtonStatu)
                    ++num4;
            }
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(num4);
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot((double)recordedData_io.ArrTemperature.Average(), 1));
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot((double)fSVM_io, 2));
            EpochConverter epochConverter = new EpochConverter();
            double standardDeviation1 = epochConverter.GetStandardDeviation(recordedData_io.ArrXaxis);
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot(standardDeviation1, 4));
            stringBuilder_io.Append(",");
            double standardDeviation2 = epochConverter.GetStandardDeviation(recordedData_io.ArrYaxis);
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot(standardDeviation2, 4));
            stringBuilder_io.Append(",");
            double standardDeviation3 = epochConverter.GetStandardDeviation(recordedData_io.ArrZaxis);
            stringBuilder_io.Append(generalUtility.ConvertCultureToDot(standardDeviation3, 4));
            stringBuilder_io.Append(",");
            stringBuilder_io.Append(recordedData_io.ArrLightMeter.Max<ushort>());
            stringBuilder_io.Append("\r\n");
            recordedData_io.ArrXaxis.Clear();
            recordedData_io.ArrYaxis.Clear();
            recordedData_io.ArrZaxis.Clear();
            recordedData_io.ArrLightMeter.Clear();
            recordedData_io.ArrButtonStatus.Clear();
            recordedData_io.ArrTemperature.Clear();
            fSVM_io = 0.0f;
        }

        public eReturn ConvertSingleBinToCsv(string binFilePath_i, string csvFilePath_i)
        {
            eReturn eReturn;
            DeviceData deviceData = new DeviceData();
            StreamReader reader = (StreamReader)null;
            StreamWriter writer = (StreamWriter)null;
            GeneralUtility generalUtility = new GeneralUtility();



            if (binFilePath_i == null || csvFilePath_i == null)
                return eReturn.FAILURE;
            if (!File.Exists(binFilePath_i))
                return eReturn.FILE_DOESNOT_EXIST;
            try
            {
                reader = new StreamReader((Stream)File.Open(binFilePath_i, FileMode.Open));
                if (reader == null)
                    return eReturn.FAILURE;
                writer = new StreamWriter((Stream)File.Open(csvFilePath_i, FileMode.Create));
                if (writer == null)
                {
                    reader.Close();
                    return eReturn.FAILURE;
                }
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();
                if (writer != null)
                    writer.Close();
                return eReturn.FILE_IO_EXCEPTION;
            }

            try
            {
                uint num1 = 0;
                string str1 = (string)null;
                uint num2 = 0;
                uint num3 = 0;
                // ISSUE: reference to a compiler-generated field
                if (this.NewExtractUpdate != null)
                {
                    string[] strArray = this.m_strUpdateInfo.Split(':');
                    str1 = strArray[0];
                    num1 = Convert.ToUInt32(strArray[1]);
                    num3 = (uint)((double)num1 * 0.006);
                }
                eReturn = this.WriteCSVHeader(reader, writer, deviceData);
                if (eReturn == eReturn.SUCCESS)
                {
                    string str2 = (string)null;
                    DeviceCapabilities deviceCapabilities = new DeviceCapabilities();
                    CalibrationInformation calibrationInformation = new CalibrationInformation();
                    while ((str2 = reader.ReadLine()) != null)
                    {
                        string[] strArray1 = (string[])null;
                        string[] strArray2 = (string[])null;
                        string[] strArray3 = (string[])null;
                        string str3;
                        while ((str3 = reader.ReadLine()) != "Measurement Frequency" || str3 != null)
                        {
                            try
                            {
                                if (str3 != null)
                                {
                                    if (str3.Contains("Temperature"))
                                        strArray1 = str3.Split(':');
                                    if (str3.Contains("Measurement Frequency"))
                                    {
                                        strArray2 = str3.Split(':');
                                        break;
                                    }
                                    if (str3.Contains("Page Time"))
                                    {
                                        string[] strArray4 = str3.Split(':');
                                        if (strArray4.Length == 5)
                                        {
                                            string str4 = strArray4[0] + ":";
                                            string[] strArray5 = strArray4[1].Split(' ');
                                            if (strArray5.Length == 2)
                                            {
                                                string[] strArray6 = strArray5[0].Split('-');
                                                if (strArray6.Length == 3)
                                                    str4 = str4 + strArray6[0] + ":" + strArray6[1] + ":" + strArray6[2] + ":";
                                                str4 = str4 + strArray5[1] + ":";
                                            }
                                            strArray3 = (str4 + strArray4[2] + ":" + strArray4[3] + ":" + strArray4[4]).Split(':');
                                        }
                                        else
                                            break;
                                    }
                                }
                                else
                                    break;
                            }
                            catch (Exception ex)
                            {
                                reader.Close();
                                writer.Close();
                                return eReturn.INVALID_CSV_FILE;
                            }
                        }
                        if (strArray2 != null && strArray1 != null && strArray3 != null)
                        {
                            // ISSUE: reference to a compiler-generated field
                            if (this.NewExtractUpdate != null)
                            {
                                ++num2;
                                int num4 = (int)((double)num2 / (double)num1 * 100.0);
                                if (num3 > 1U && (int)(num2 % 200U) == 0)
                                    --num3;
                                // ISSUE: reference to a compiler-generated field
                                this.NewExtractUpdate(str1 + ":" + num4.ToString() + ":" + num3.ToString());
                            }
                            try
                            {
                                string[] strArray4 = new string[7];
                                for (int index = 0; index < 7; ++index)
                                    strArray4[index] = strArray3[index + 1];
                                float floatAgnostic = generalUtility.ToFloatAgnostic(strArray1[1]);
                                double doubleAgnostic = generalUtility.ToDoubleAgnostic(strArray2[1]);
                                PageTime pageTime = new PageTime();
                                string hex = reader.ReadLine();
                                byte[] numArray = (byte[])null;
                                if (hex.Length % 2 == 0)
                                {
                                    numArray = DataConverter.StringToByteArray(hex);
                                    eReturn = eReturn.SUCCESS;
                                }
                                else
                                    eReturn = eReturn.CORRUPTED_BIN_FILE;
                                if (eReturn == eReturn.SUCCESS)
                                {
                                    int index1 = 0;
                                    if (numArray.Length >= 1800)
                                    {
                                        StringBuilder stringBuilder = new StringBuilder();
                                        try
                                        {
                                            for (int index2 = 0; index2 < 300; ++index2)
                                            {
                                                byte[] buffer = new byte[6];
                                                for (int index3 = 0; index3 < 6; ++index3)
                                                {
                                                    buffer[index3] = numArray[index1];
                                                    ++index1;
                                                }
                                                int andPackRecodedData = (int)deviceCapabilities.ParseAndPackRecodedData(buffer, deviceData);
                                                deviceData.ObjRecordedData.ArrTemperature.Add(floatAgnostic);
                                                if (index2 == 0)
                                                {
                                                    pageTime.Year = Convert.ToUInt16(strArray3[1]);
                                                    pageTime.Month = Convert.ToUInt16(strArray3[2]);
                                                    pageTime.Date = Convert.ToUInt16(strArray3[3]);
                                                    pageTime.Hour = Convert.ToUInt16(strArray3[4]);
                                                    pageTime.Minute = Convert.ToUInt16(strArray3[5]);
                                                    pageTime.Second = Convert.ToUInt16(strArray3[6]);
                                                    pageTime.Millisecond = Convert.ToUInt16(strArray3[7]);
                                                }
                                                else
                                                    pageTime += 1.0 / doubleAgnostic;
                                                string[] strArray5 = new string[13];
                                                int index4 = 0;
                                                ushort num4 = pageTime.Year;
                                                string str4 = num4.ToString();
                                                strArray5[index4] = str4;
                                                strArray5[1] = ":";
                                                int index5 = 2;
                                                num4 = pageTime.Month;
                                                string str5 = num4.ToString();
                                                strArray5[index5] = str5;
                                                strArray5[3] = ":";
                                                int index6 = 4;
                                                num4 = pageTime.Date;
                                                string str6 = num4.ToString();
                                                strArray5[index6] = str6;
                                                strArray5[5] = ":";
                                                int index7 = 6;
                                                num4 = pageTime.Hour;
                                                string str7 = num4.ToString();
                                                strArray5[index7] = str7;
                                                strArray5[7] = ":";
                                                int index8 = 8;
                                                num4 = pageTime.Minute;
                                                string str8 = num4.ToString();
                                                strArray5[index8] = str8;
                                                strArray5[9] = ":";
                                                int index9 = 10;
                                                num4 = pageTime.Second;
                                                string str9 = num4.ToString();
                                                strArray5[index9] = str9;
                                                strArray5[11] = ":";
                                                int index10 = 12;
                                                num4 = pageTime.Millisecond;
                                                string str10 = num4.ToString();
                                                strArray5[index10] = str10;
                                                string.Concat(strArray5);
                                                string[] strArray6 = new string[13];
                                                int index11 = 0;
                                                num4 = pageTime.Year;
                                                string str11 = num4.ToString("0000");
                                                strArray6[index11] = str11;
                                                strArray6[1] = "-";
                                                int index12 = 2;
                                                num4 = pageTime.Month;
                                                string str12 = num4.ToString("00");
                                                strArray6[index12] = str12;
                                                strArray6[3] = "-";
                                                int index13 = 4;
                                                num4 = pageTime.Date;
                                                string str13 = num4.ToString("00");
                                                strArray6[index13] = str13;
                                                strArray6[5] = " ";
                                                int index14 = 6;
                                                num4 = pageTime.Hour;
                                                string str14 = num4.ToString("00");
                                                strArray6[index14] = str14;
                                                strArray6[7] = ":";
                                                int index15 = 8;
                                                num4 = pageTime.Minute;
                                                string str15 = num4.ToString("00");
                                                strArray6[index15] = str15;
                                                strArray6[9] = ":";
                                                int index16 = 10;
                                                num4 = pageTime.Second;
                                                string str16 = num4.ToString("00");
                                                strArray6[index16] = str16;
                                                strArray6[11] = ":";
                                                int index17 = 12;
                                                num4 = pageTime.Millisecond;
                                                string str17 = num4.ToString("000");
                                                strArray6[index17] = str17;
                                                string str18 = string.Concat(strArray6);
                                                stringBuilder.Append(str18);
                                                stringBuilder.Append(",");
                                                stringBuilder.Append(generalUtility.ConvertCultureToDot((double)deviceData.ObjRecordedData.ArrXaxis[index2], 4));
                                                stringBuilder.Append(",");
                                                stringBuilder.Append(generalUtility.ConvertCultureToDot((double)deviceData.ObjRecordedData.ArrYaxis[index2], 4));
                                                stringBuilder.Append(",");
                                                stringBuilder.Append(generalUtility.ConvertCultureToDot((double)deviceData.ObjRecordedData.ArrZaxis[index2], 4));
                                                stringBuilder.Append(",");
                                                stringBuilder.Append(deviceData.ObjRecordedData.ArrLightMeter[index2]);
                                                stringBuilder.Append(",");
                                                stringBuilder.Append(deviceData.ObjRecordedData.ArrButtonStatus[index2]);
                                                stringBuilder.Append(",");
                                                stringBuilder.Append(generalUtility.ConvertCultureToDot((double)deviceData.ObjRecordedData.ArrTemperature[index2], 1));
                                                stringBuilder.Append("\r\n");
                                                eReturn = eReturn.SUCCESS;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            reader.Close();
                                            writer.Close();
                                            return eReturn.INVALID_CSV_FILE;
                                        }
                                        writer.Write((object)stringBuilder);
                                        stringBuilder.Remove(0, stringBuilder.Length);
                                        deviceData.ObjRecordedData.ArrTemperature.Clear();
                                        deviceData.ObjRecordedData.ArrLightMeter.Clear();
                                        deviceData.ObjRecordedData.ArrButtonStatus.Clear();
                                        deviceData.ObjRecordedData.ArrXaxis.Clear();
                                        deviceData.ObjRecordedData.ArrYaxis.Clear();
                                        deviceData.ObjRecordedData.ArrZaxis.Clear();

                                    }
                                    else
                                    {
                                        eReturn = eReturn.CORRUPTED_BIN_FILE;
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                reader.Close();
                                writer.Close();
                                return eReturn.INVALID_CSV_FILE;
                            }
                        }
                        else
                        {
                            eReturn = eReturn.CORRUPTED_BIN_FILE;
                            break;
                        }
                    }
                    reader.Close();
                    if (eReturn != eReturn.CORRUPTED_BIN_FILE)
                        writer.Close();
                }
                reader.Close();
                writer.Close();
            }
            catch (IOException ex)
            {
                eReturn = eReturn.OUT_OF_MEMORY;
                if (reader != null)
                    reader.Close();
                if (writer != null)
                {
                    try
                    {
                        writer.Close();
                    }
                    catch
                    {
                        eReturn = eReturn.OUT_OF_MEMORY;
                    }
                }
            }
            if (reader != null)
                reader.Close();
            return eReturn;
        }

        private string CalculateLastMeasurement(string measurementFreq, string firstMeasurement, string measurementPeriod)
        {
            string str1 = "";
            if (measurementFreq != null && measurementFreq != "" && (measurementFreq != null && measurementFreq != "") && (measurementFreq != null && measurementFreq != ""))
            {
                string[] strArray1 = firstMeasurement.Split(':');
                if (strArray1.Length != 8)
                    return "";
                string[] strArray2 = measurementPeriod.Split(':');
                if (strArray2.Length != 2)
                    return "";
                int int32 = Convert.ToInt32(strArray2[1].Split(' ')[0]);
                try
                {
                    DateTime dateTime = new DateTime(Convert.ToInt32(strArray1[1]), Convert.ToInt32(strArray1[2]), Convert.ToInt32(strArray1[3]), Convert.ToInt32(strArray1[4]), Convert.ToInt32(strArray1[5]), Convert.ToInt32(strArray1[6]), Convert.ToInt32(strArray1[7]), DateTimeKind.Unspecified).AddHours((double)int32);
                    string[] strArray3 = new string[13];
                    strArray3[0] = dateTime.Year.ToString();
                    strArray3[1] = "-";
                    strArray3[2] = dateTime.Month.ToString();
                    strArray3[3] = "-";
                    int index1 = 4;
                    int num1 = dateTime.Day;
                    string str2 = num1.ToString();
                    strArray3[index1] = str2;
                    strArray3[5] = " ";
                    int index2 = 6;
                    num1 = dateTime.Hour;
                    string str3 = num1.ToString();
                    strArray3[index2] = str3;
                    strArray3[7] = ":";
                    int index3 = 8;
                    num1 = dateTime.Minute;
                    string str4 = num1.ToString();
                    strArray3[index3] = str4;
                    strArray3[9] = ":";
                    int index4 = 10;
                    int num2 = dateTime.Second;
                    string str5 = num2.ToString();
                    strArray3[index4] = str5;
                    strArray3[11] = ":";
                    int index5 = 12;
                    num2 = dateTime.Millisecond;
                    string str6 = num2.ToString();
                    strArray3[index5] = str6;
                    str1 = string.Concat(strArray3);
                }
                catch
                {
                    str1 = "";
                }
            }
            return str1;
        }

        public static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] numArray = new byte[length / 2];
            int startIndex = 0;
            while (startIndex < length)
            {
                numArray[startIndex / 2] = Convert.ToByte(hex.Substring(startIndex, 2), 16);
                startIndex += 2;
            }
            return numArray;
        }

        public eReturn ConvertMultipleBinToCsv(List<string> binFilePaths, string csvFilePath)
        {
            eReturn eReturn = eReturn.SUCCESS;
            List<string> csvFiles = new List<string>();
            List<string> stringList = new List<string>();
            for (int index1 = 0; index1 < binFilePaths.Count; ++index1)
            {
                string str1 = csvFilePath;
                string[] strArray1 = binFilePaths[index1].Remove(binFilePaths[index1].IndexOf(".bin")).Split('\\');
                int length1 = strArray1.Length;
                string[] strArray2 = str1.Split('\\');
                int length2 = strArray2.Length;
                string str2 = (string)null;
                for (int index2 = 0; index2 < length2 - 1; ++index2)
                    str2 = str2 + strArray2[index2] + "\\";
                string str3 = str2 + strArray1[length1 - 1] + "_Intermediate.csv";
                csvFiles.Add(str3);
                eReturn = this.ConvertSingleBinToCsv(binFilePaths[index1], str3);
                switch (eReturn)
                {
                    case eReturn.CORRUPTED_BIN_FILE:
                        stringList.Add(binFilePaths[index1]);
                        csvFiles.Remove(str3);
                        try
                        {
                            File.Delete(str3);
                            eReturn = eReturn.SUCCESS;
                            break;
                        }
                        catch
                        {
                            eReturn = eReturn.FAILURE;
                            break;
                        }
                    case eReturn.FILE_IO_EXCEPTION:
                        int count1 = csvFiles.Count;
                        for (int index2 = 0; index2 < count1; ++index2)
                        {
                            if (File.Exists(csvFiles[index2]))
                            {
                                try
                                {
                                    File.Delete(csvFiles[index2]);
                                }
                                catch
                                {
                                    eReturn = eReturn.SUCCESS;
                                }
                            }
                        }
                        goto label_22;
                    case eReturn.OUT_OF_MEMORY:
                        int count2 = csvFiles.Count;
                        for (int index2 = 0; index2 < count2; ++index2)
                        {
                            if (File.Exists(csvFiles[index2]))
                            {
                                try
                                {
                                    File.Delete(csvFiles[index2]);
                                }
                                catch
                                {
                                    eReturn = eReturn.SUCCESS;
                                }
                            }
                        }
                        goto label_22;
                }
            }
            label_22:
            if (eReturn == eReturn.SUCCESS)
            {
                if (binFilePaths.Count != stringList.Count)
                {
                    if (stringList.Count != 0)
                    {
                        string str = (string)null;
                        for (int index = 0; index < stringList.Count; ++index)
                            str = str + (index + 1).ToString() + ". " + stringList[index] + "\n";
                        if (MessageBox.Show("Following bin files are corrupted \n" + str + "Do you wish to continue?", "GENEActiv PC Software", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (csvFiles.Count == 1)
                            {
                                eReturn = this.CopyFile(csvFiles, csvFilePath);
                            }
                            else
                            {
                                try
                                {
                                    eReturn = this.MergeCSVFiles(csvFiles, csvFilePath);
                                }
                                catch (IOException ex)
                                {
                                    eReturn = eReturn.OUT_OF_MEMORY;
                                }
                            }
                        }
                        else
                        {
                            int count = csvFiles.Count;
                            for (int index = 0; index < count; ++index)
                            {
                                if (File.Exists(csvFiles[index]))
                                {
                                    try
                                    {
                                        File.Delete(csvFiles[index]);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            eReturn = eReturn.ABORT_CONVERSION;
                        }
                    }
                    else
                    {
                        try
                        {
                            eReturn = this.MergeCSVFiles(csvFiles, csvFilePath);
                        }
                        catch (IOException ex)
                        {
                            eReturn = eReturn.OUT_OF_MEMORY;
                        }
                    }
                }
                else
                    eReturn = eReturn.ALL_FILES_CORRUPTED;
            }
            return eReturn;
        }

        private eReturn CopyFile(List<string> csvFiles, string csvFilePath)
        {
            try
            {
                if (File.Exists(csvFilePath))
                    File.Delete(csvFilePath);
                File.Move(csvFiles[0], csvFilePath);
                File.Delete(csvFiles[0]);
                return eReturn.SUCCESS;
            }
            catch
            {
                return eReturn.FILE_IO_EXCEPTION;
            }
        }

        private eReturn MergeCSVFiles(List<string> csvFiles, string csvFilePath)
        {
            eReturn returnStatus = eReturn.SUCCESS;
            StreamWriter streamWriter = (StreamWriter)null;
            List<StreamReader> streamReaderList = new List<StreamReader>();
            if (csvFiles != null)
            {
                if (csvFilePath != null)
                {
                    try
                    {
                        streamWriter = new StreamWriter((Stream)File.Open(csvFilePath, FileMode.Create));
                        returnStatus = streamWriter == null ? eReturn.FAILURE : eReturn.SUCCESS;
                    }
                    catch
                    {
                        returnStatus = eReturn.FILE_IO_EXCEPTION;
                    }
                }
                if (returnStatus == eReturn.SUCCESS)
                {
                    for (int index = 0; index < csvFiles.Count; ++index)
                    {
                        if (!File.Exists(csvFiles[index]))
                        {
                            eReturn eReturn = eReturn.FAILURE;
                            File.Delete(csvFiles[index]);
                            return eReturn;
                        }
                    }
                    StreamReader streamReader1 = (StreamReader)null;
                    if (csvFiles[0] != null)
                    {
                        if (File.Exists(csvFiles[0]))
                        {
                            try
                            {
                                streamReader1 = new StreamReader((Stream)File.Open(csvFiles[0], FileMode.Open));
                            }
                            catch (Exception ex)
                            {
                                return eReturn.FILE_IO_EXCEPTION;
                            }
                        }
                    }
                    for (int index = 0; index < csvFiles.Count - 1; ++index)
                    {
                        StreamReader streamReader2 = new StreamReader((Stream)File.Open(csvFiles[index + 1], FileMode.Open));
                        streamReaderList.Add(streamReader2);
                    }
                    List<float> floatList = new List<float>();
                    string[] strArray1 = (string[])null;
                    bool flag1 = false;
                    for (int index1 = 0; index1 < 100; ++index1)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        string str1 = streamReader1.ReadLine();
                        if (str1 == null)
                        {
                            flag1 = true;
                            break;
                        }
                        if (str1.Contains("Measurement Frequency(Hz)"))
                        {
                            string[] strArray2 = str1.Split(',');
                            GeneralUtility generalUtility = new GeneralUtility();
                            if ("." != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                generalUtility.ConvertCulture(strArray2[1]);
                            else
                                Convert.ToDouble(strArray2[1], (IFormatProvider)CultureInfo.InvariantCulture);
                        }
                        stringBuilder.Append(str1);
                        for (int index2 = 0; index2 < streamReaderList.Count; ++index2)
                        {
                            strArray1 = (string[])null;
                            string str2 = streamReaderList[index2].ReadLine();
                            if (str2 != "")
                            {
                                string[] strArray2 = str2.Split(',');
                                stringBuilder.Append(", , , , , , , ");
                                if (strArray2.Length == 2)
                                    stringBuilder.Append(strArray2[1]);
                                else
                                    stringBuilder.Append(" ");
                            }
                        }
                        streamWriter.WriteLine((object)stringBuilder);
                    }
                    if (!flag1)
                    {
                        long position = streamReader1.BaseStream.Position;
                        bool flag2 = false;
                        List<DateTime> dateTimeList = new List<DateTime>();
                        string str1 = streamReader1.ReadLine();
                        if (str1 == null)
                            flag1 = true;
                        if (!flag1)
                        {
                            DateTime dataTime1 = this.GetDataTime(str1.Split(',')[0], ref returnStatus);
                            if (returnStatus == eReturn.SUCCESS)
                            {
                                dateTimeList.Add(dataTime1);
                                StringBuilder stringBuilder = new StringBuilder();
                                for (int index = 0; index < csvFiles.Count - 1; ++index)
                                {
                                    string str2 = streamReaderList[index].ReadLine();
                                    if (str2 == null)
                                    {
                                        flag1 = true;
                                        break;
                                    }
                                    DateTime dataTime2 = this.GetDataTime(str2.Split(',')[0], ref returnStatus);
                                    if (returnStatus == eReturn.SUCCESS)
                                        dateTimeList.Add(dataTime2);
                                }
                                dateTimeList.Sort();
                            }
                            if (!flag1)
                            {
                                for (int index = 0; index < csvFiles.Count - 1; ++index)
                                    streamReaderList[index].Close();
                                streamReaderList.Clear();
                                for (int index = 0; index < csvFiles.Count - 1; ++index)
                                {
                                    StreamReader streamReader2 = new StreamReader((Stream)File.Open(csvFiles[index + 1], FileMode.Open));
                                    streamReaderList.Add(streamReader2);
                                }
                                for (; str1 != null; str1 = streamReader1.ReadLine())
                                {
                                    string[] strArray2 = str1.Split(',');
                                    if (strArray2.Length > 1)
                                    {
                                        returnStatus = eReturn.FAILURE;
                                        if (this.GetDataTime(strArray2[0], ref returnStatus) >= dateTimeList[dateTimeList.Count - 1])
                                            break;
                                    }
                                }
                                if (str1 != null)
                                {
                                    for (; str1 != null; str1 = streamReader1.ReadLine())
                                    {
                                        string[] strArray2 = str1.Split(',');
                                        List<string> stringList = new List<string>();
                                        for (int index = 0; index < csvFiles.Count - 1; ++index)
                                        {
                                            flag2 = false;
                                            try
                                            {
                                                string str2;
                                                while ((str2 = streamReaderList[index].ReadLine()) != null)
                                                {
                                                    if (str2.Contains(strArray2[0]))
                                                    {
                                                        flag2 = true;
                                                        break;
                                                    }
                                                }
                                                if (flag2)
                                                {
                                                    string str3 = str2.Remove(0, str2.IndexOf(','));
                                                    stringList.Add(str3);
                                                }
                                                else
                                                {
                                                    flag2 = false;
                                                    break;
                                                }
                                            }
                                            catch (IOException ex)
                                            {
                                                returnStatus = eReturn.FILE_IO_EXCEPTION;
                                                break;
                                            }
                                        }
                                        if (returnStatus != eReturn.FILE_IO_EXCEPTION)
                                        {
                                            if (flag2)
                                            {
                                                streamWriter.Write(str1);
                                                streamWriter.Write(",");
                                                for (int index = 0; index < stringList.Count; ++index)
                                                {
                                                    streamWriter.Write(stringList[index]);
                                                    streamWriter.Write(",");
                                                }
                                            }
                                            streamWriter.WriteLine("");
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    streamWriter.Close();
                    streamReader1.Close();
                }
                for (int index = 0; index < streamReaderList.Count; ++index)
                    streamReaderList[index].Close();
                if (csvFiles.Count != 0)
                {
                    int count = csvFiles.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        try
                        {
                            File.Delete(csvFiles[index]);
                        }
                        catch
                        {
                            returnStatus = eReturn.FAILURE;
                        }
                    }
                }
            }
            return returnStatus;
        }

        public eReturn PrepareCSVHeader(StreamReader reader, DeviceData objdeviceData, ref StringBuilder writeCSV, ref long lPageCount_o)
        {
            eReturn eReturn1 = eReturn.FAILURE;
            if (reader != null)
            {
                string str1 = (string)null;
                string str2 = (string)null;
                string str3 = (string)null;
                string str4 = (string)null;
                string str5 = (string)null;
                string measurementFreq = (string)null;
                string measurementPeriod = (string)null;
                string firstMeasurement = (string)null;
                string str6 = (string)null;
                string str7 = (string)null;
                string str8 = (string)null;
                string str9 = (string)null;
                string str10 = (string)null;
                string str11 = (string)null;
                string str12 = (string)null;
                string str13 = (string)null;
                string str14 = (string)null;
                string str15 = (string)null;
                string str16 = (string)null;
                string str17 = (string)null;
                string str18 = (string)null;
                string str19 = (string)null;
                string str20 = (string)null;
                string str21 = (string)null;
                string str22 = (string)null;
                string str23 = (string)null;
                string str24 = (string)null;
                string str25 = (string)null;
                string str26 = (string)null;
                string str27 = (string)null;
                string str28 = (string)null;
                string str29 = (string)null;
                string str30 = (string)null;
                string str31 = (string)null;
                string str32 = (string)null;
                string str33 = (string)null;
                string str34 = (string)null;
                string str35 = (string)null;
                string str36 = (string)null;
                string str37 = (string)null;
                string str38 = (string)null;
                string str39 = (string)null;
                string str40 = (string)null;
                string str41 = (string)null;
                int num = 0;
                string str42;
                while ((str42 = reader.ReadLine()) != "Recorded Data" && str42 != null)
                {
                    ++num;
                    if (str42 != null && str42 != "")
                    {
                        string[] strArray = str42.Split(':');
                        switch (strArray[0])
                        {
                            case "Accelerometer Range":
                                str25 = str42;
                                continue;
                            case "Accelerometer Resolution":
                                str26 = str42;
                                continue;
                            case "Accelerometer Units":
                                str27 = str42;
                                continue;
                            case "Calibration Date":
                                str5 = str42;
                                continue;
                            case "Config Notes":
                                str20 = str42;
                                continue;
                            case "Config Operator ID":
                                str19 = str42;
                                continue;
                            case "Config Time":
                                str21 = str42;
                                continue;
                            case "Date of Birth":
                                str9 = str42;
                                continue;
                            case "Device Firmware Version":
                                str4 = str42;
                                continue;
                            case "Device Location Code":
                                str6 = str42;
                                continue;
                            case "Device Model":
                                str1 = str42;
                                continue;
                            case "Device Type":
                                str2 = str42;
                                continue;
                            case "Device Unique Serial Code":
                                str3 = str42;
                                continue;
                            case "Exercise Type":
                                str18 = str42;
                                continue;
                            case "Extract Notes":
                                str24 = str42;
                                continue;
                            case "Extract Operator ID":
                                str22 = str42;
                                continue;
                            case "Extract Time":
                                str23 = str42;
                                continue;
                            case "Handedness Code":
                                str13 = str42;
                                continue;
                            case "Height":
                                str12 = str42;
                                continue;
                            case "Investigator ID":
                                str17 = str42;
                                continue;
                            case "Light Meter Range":
                                str28 = str42;
                                continue;
                            case "Light Meter Resolution":
                                str29 = str42;
                                continue;
                            case "Light Meter Units":
                                str30 = str42;
                                continue;
                            case "Lux":
                                str41 = str42;
                                continue;
                            case "Measurement Frequency":
                                measurementFreq = str42;
                                continue;
                            case "Measurement Period":
                                measurementPeriod = str42;
                                continue;
                            case "Number of Pages":
                                lPageCount_o = (long)Convert.ToInt32(strArray[1]);
                                pageCount = Convert.ToInt32(strArray[1]);
                                continue;
                            case "Sex":
                                str10 = str42;
                                continue;
                            case "Start Time":
                                firstMeasurement = str42;
                                continue;
                            case "Study Centre":
                                str15 = str42;
                                continue;
                            case "Study Code":
                                str16 = str42;
                                continue;
                            case "Subject Code":
                                str8 = str42;
                                continue;
                            case "Subject Notes":
                                str14 = str42;
                                continue;
                            case "Temperature Sensor Range":
                                str31 = str42;
                                continue;
                            case "Temperature Sensor Resolution":
                                str32 = str42;
                                continue;
                            case "Temperature Sensor Units":
                                str33 = str42;
                                continue;
                            case "Time Zone":
                                str7 = str42;
                                continue;
                            case "Volts":
                                str40 = str42;
                                continue;
                            case "Weight":
                                str11 = str42;
                                continue;
                            case "x gain":
                                str34 = str42;
                                continue;
                            case "x offset":
                                str35 = str42;
                                continue;
                            case "y gain":
                                str36 = str42;
                                continue;
                            case "y offset":
                                str37 = str42;
                                continue;
                            case "z gain":
                                str38 = str42;
                                continue;
                            case "z offset":
                                str39 = str42;
                                continue;
                            default:
                                continue;
                        }
                    }
                }
                string str43;
                while ((str43 = reader.ReadLine()) != "Recorded Data" && str43 != null)
                {
                    if (str43 != null && str43 != "")
                    {
                        if (str43.Split(':')[0] == "Measurement Frequency")
                        {
                            measurementFreq = str43;
                            if (!measurementFreq.EndsWith("Hz"))
                                measurementFreq += " Hz";
                        }
                    }
                }
                reader.BaseStream.Position = 0L;
                reader.DiscardBufferedData();
                for (int index = 0; index <= num; ++index)
                    reader.ReadLine();
                if (!string.IsNullOrEmpty(str2) && eReturn1 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str2.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjDeviceInfo.DeviceType = strArray[1];
                        eReturn1 = eReturn.SUCCESS;
                    }
                    else
                        eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                }
                eReturn eReturn2;
                if (str34 != null && eReturn1 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str34.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Xgain = Convert.ToInt16(strArray[1]);
                        eReturn2 = eReturn.SUCCESS;
                    }
                    else
                        eReturn2 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn2 = eReturn.CORRUPTED_BIN_FILE;
                eReturn eReturn3;
                if (str35 != null && eReturn2 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str35.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Xoffset = Convert.ToInt16(strArray[1]);
                        eReturn3 = eReturn.SUCCESS;
                    }
                    else
                        eReturn3 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn3 = eReturn.CORRUPTED_BIN_FILE;
                eReturn eReturn4;
                if (str36 != null && eReturn3 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str36.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Ygain = Convert.ToInt16(strArray[1]);
                        eReturn4 = eReturn.SUCCESS;
                    }
                    else
                        eReturn4 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn4 = eReturn.CORRUPTED_BIN_FILE;
                eReturn eReturn5;
                if (str37 != null && eReturn4 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str37.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Yoffset = Convert.ToInt16(strArray[1]);
                        eReturn5 = eReturn.SUCCESS;
                    }
                    else
                        eReturn5 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn5 = eReturn.CORRUPTED_BIN_FILE;
                eReturn eReturn6;
                if (str38 != null && eReturn5 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str38.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Zgain = Convert.ToInt16(strArray[1]);
                        eReturn6 = eReturn.SUCCESS;
                    }
                    else
                        eReturn6 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn6 = eReturn.CORRUPTED_BIN_FILE;
                eReturn eReturn7;
                if (str39 != null && eReturn6 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str39.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Zoffset = Convert.ToInt16(strArray[1]);
                        eReturn7 = eReturn.SUCCESS;
                    }
                    else
                        eReturn7 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn7 = eReturn.CORRUPTED_BIN_FILE;
                eReturn eReturn8;
                if (str40 != null && eReturn7 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str40.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Volts = Convert.ToInt16(strArray[1]);
                        eReturn8 = eReturn.SUCCESS;
                    }
                    else
                        eReturn8 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn8 = eReturn.CORRUPTED_BIN_FILE;
                if (str41 != null && eReturn8 != eReturn.CORRUPTED_BIN_FILE)
                {
                    string[] strArray = str41.Split(':');
                    if (strArray.Length == 2)
                    {
                        objdeviceData.ObjCalibInfo.Lux = Convert.ToInt16(strArray[1]);
                        eReturn1 = eReturn.SUCCESS;
                    }
                    else
                        eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                }
                else
                    eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                if (eReturn1 == eReturn.SUCCESS)
                {
                    if (str2 != null)
                    {
                        string[] strArray = str2.Split(':');
                        writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                    }
                    else
                        writeCSV.Append("\r\n");
                    if (str1 != null)
                    {
                        string[] strArray = str1.Split(':');
                        writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                    }
                    else
                        writeCSV.Append("\r\n");
                    if (str3 != null)
                    {
                        string[] strArray = str3.Split(':');
                        writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                    }
                    else
                        writeCSV.Append("\r\n");
                    if (str4 != null)
                    {
                        string[] strArray = str4.Split(':');
                        writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                    }
                    else
                        writeCSV.Append("\r\n");
                    if (str5 != null)
                    {
                        string[] strArray = str5.Split(':');
                        if (strArray.Length == 5)
                            writeCSV.Append(strArray[0] + "," + strArray[1] + ":" + strArray[2] + ":" + strArray[3] + ":" + strArray[4] + "\r\n");
                        else
                            eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                    }
                    else
                        writeCSV.Append("\r\n");
                    if (eReturn1 != eReturn.CORRUPTED_BIN_FILE)
                    {
                        writeCSV.Append("Application name & version ," + VersionInfo.ApplicationTitle + "\r\n");
                        for (int index = 0; index < 4; ++index)
                            writeCSV.Append("\r\n");
                        if (measurementFreq != null)
                        {
                            string[] strArray = measurementFreq.Split(':');
                            writeCSV.Append(strArray[0] + "," + strArray[1].Replace(",", ".") + "\r\n");
                        }
                        else
                            writeCSV.Append("\r\n");
                        if (firstMeasurement != null)
                        {
                            string[] strArray = firstMeasurement.Split(':');
                            if (strArray.Length == 5)
                                writeCSV.Append(strArray[0] + "," + strArray[1] + ":" + strArray[2] + ":" + strArray[3] + ":" + strArray[4] + "\r\n");
                            else
                                eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                        }
                        else
                            writeCSV.Append("\r\n");
                        if (eReturn1 != eReturn.CORRUPTED_BIN_FILE)
                        {
                            if (measurementPeriod != null)
                            {
                                string lastMeasurement = this.CalculateLastMeasurement(measurementFreq, firstMeasurement, measurementPeriod);
                                writeCSV.Append("Last measurement," + lastMeasurement + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str6 != null)
                            {
                                string[] strArray = str6.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str7 != null)
                            {
                                string[] strArray = str7.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            for (int index = 0; index < 5; ++index)
                                writeCSV.Append("\r\n");
                            if (str8 != null)
                            {
                                string[] strArray = str8.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str9 != null)
                            {
                                string[] strArray = str9.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str10 != null)
                            {
                                string[] strArray = str10.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str12 != null)
                            {
                                string[] strArray = str12.Split(':');
                                if (strArray[1].Equals("0"))
                                    writeCSV.Append(strArray[0] + ",\r\n");
                                else
                                    writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str11 != null)
                            {
                                string[] strArray = str11.Split(':');
                                if (strArray[1].Equals("0"))
                                    writeCSV.Append(strArray[0] + ",\r\n");
                                else
                                    writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str13 != null)
                            {
                                string[] strArray = str13.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str14 != null)
                            {
                                string[] strArray = str14.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            for (int index = 0; index < 3; ++index)
                                writeCSV.Append("\r\n");
                            if (str15 != null)
                            {
                                string[] strArray = str15.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str16 != null)
                            {
                                string[] strArray = str16.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str17 != null)
                            {
                                string[] strArray = str17.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str18 != null)
                            {
                                string[] strArray = str18.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str19 != null)
                            {
                                string[] strArray = str19.Split(':');
                                writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (str21 != null)
                            {
                                string[] strArray = str21.Split(':');
                                if (strArray.Length == 5)
                                    writeCSV.Append(strArray[0] + "," + strArray[1] + ":" + strArray[2] + ":" + strArray[3] + ":" + strArray[4] + ":\r\n");
                                else
                                    eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                            }
                            else
                                writeCSV.Append("\r\n");
                            if (eReturn1 != eReturn.CORRUPTED_BIN_FILE)
                            {
                                if (str20 != null)
                                {
                                    string[] strArray = str20.Split(':');
                                    writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                                }
                                else
                                    writeCSV.Append("\r\n");
                                if (str22 != null)
                                {
                                    string[] strArray = str22.Split(':');
                                    writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                                }
                                else
                                    writeCSV.Append("\r\n");
                                if (str23 != null)
                                {
                                    string[] strArray = str23.Split(':');
                                    if (strArray.Length == 5)
                                        writeCSV.Append(strArray[0] + "," + strArray[1] + ":" + strArray[2] + ":" + strArray[3] + ":" + strArray[4] + "\r\n");
                                    else
                                        eReturn1 = eReturn.CORRUPTED_BIN_FILE;
                                }
                                else
                                    writeCSV.Append("\r\n");
                                if (eReturn1 != eReturn.CORRUPTED_BIN_FILE)
                                {
                                    if (str24 != null)
                                    {
                                        string[] strArray = str24.Split(":".ToCharArray(), 2);
                                        writeCSV.Append(strArray[0] + "," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    for (int index = 0; index < 10; ++index)
                                        writeCSV.Append("\r\n");
                                    if (str25 != null)
                                    {
                                        writeCSV.Append("Sensor type,MEMS accelerometer x-axis\r\n");
                                        string[] strArray = str25.Split(':');
                                        writeCSV.Append("Range," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str26 != null)
                                    {
                                        string[] strArray = str26.Split(':');
                                        writeCSV.Append("Resolution," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str27 != null)
                                    {
                                        string[] strArray = str27.Split(':');
                                        writeCSV.Append("Units," + strArray[1] + "\r\n");
                                        writeCSV.Append("Additional information\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str25 != null)
                                    {
                                        writeCSV.Append("Sensor type,MEMS accelerometer y-axis\r\n");
                                        string[] strArray = str25.Split(':');
                                        writeCSV.Append("Range," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str26 != null)
                                    {
                                        string[] strArray = str26.Split(':');
                                        writeCSV.Append("Resolution," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str27 != null)
                                    {
                                        string[] strArray = str27.Split(':');
                                        writeCSV.Append("Units," + strArray[1] + "\r\n");
                                        writeCSV.Append("Additional information\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str25 != null)
                                    {
                                        writeCSV.Append("Sensor type,MEMS accelerometer z-axis \r\n");
                                        string[] strArray = str25.Split(':');
                                        writeCSV.Append("Range," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str26 != null)
                                    {
                                        string[] strArray = str26.Split(':');
                                        writeCSV.Append("Resolution," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str27 != null)
                                    {
                                        string[] strArray = str27.Split(':');
                                        writeCSV.Append("Units," + strArray[1] + "\r\n");
                                        writeCSV.Append("Additional information\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str28 != null)
                                    {
                                        writeCSV.Append("Sensor type,Lux Photodiode 400nm - 1100nm \r\n");
                                        string[] strArray = str28.Split(':');
                                        writeCSV.Append("Range," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str29 != null)
                                    {
                                        string[] strArray = str29.Split(':');
                                        writeCSV.Append("Resolution," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str30 != null)
                                    {
                                        string[] strArray = str30.Split(':');
                                        writeCSV.Append("Units," + strArray[1] + "\r\n");
                                        writeCSV.Append("Additional information\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    writeCSV.Append("Sensor type,User button event marker\r\n");
                                    writeCSV.Append("Range,1 or 0\r\n");
                                    writeCSV.Append("Resolution\r\n");
                                    writeCSV.Append("Units\r\n");
                                    writeCSV.Append("Additional information,1=pressed\r\n");
                                    if (str31 != null)
                                    {
                                        writeCSV.Append("Sensor type,Linear active thermistor\r\n");
                                        string[] strArray = str31.Split(':');
                                        writeCSV.Append("Range," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str32 != null)
                                    {
                                        string[] strArray = str32.Split(':');
                                        writeCSV.Append("Resolution," + strArray[1] + "\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    if (str33 != null)
                                    {
                                        string[] strArray = str33.Split(':');
                                        writeCSV.Append("Units," + strArray[1] + "\r\n");
                                        writeCSV.Append("Additional information\r\n");
                                    }
                                    else
                                        writeCSV.Append("\r\n");
                                    for (int index = 0; index < 20; ++index)
                                        writeCSV.Append("\r\n");
                                    eReturn1 = eReturn.SUCCESS;
                                }
                            }
                        }
                    }
                }
            }
            return eReturn1;
        }

        private eReturn WriteCSVHeader(StreamReader reader, StreamWriter writer, DeviceData objdeviceData)
        {
            StringBuilder writeCSV = new StringBuilder();
            long lPageCount_o = 0;
            eReturn eReturn = this.PrepareCSVHeader(reader, objdeviceData, ref writeCSV, ref lPageCount_o);
            if (eReturn == eReturn.SUCCESS)
                writer.Write((object)writeCSV);
            return eReturn;
        }

        private DateTime GetDataTime(string inputString, ref eReturn returnStatus)
        {
            returnStatus = eReturn.FAILURE;
            DateTime dateTime1 = new DateTime();
            if (inputString != null)
            {
                string[] strArray1 = inputString.Split(' ');
                if (strArray1.Length == 2)
                {
                    string[] strArray2 = strArray1[0].Split('-');
                    string[] strArray3 = strArray1[1].Split(':');
                    if (strArray2.Length == 3 && strArray3.Length == 4)
                    {
                        DateTime dateTime2 = new DateTime(Convert.ToInt32(strArray2[0]), Convert.ToInt32(strArray2[1]), Convert.ToInt32(strArray2[2]), Convert.ToInt32(strArray3[0]), Convert.ToInt32(strArray3[1]), Convert.ToInt32(strArray3[2]), Convert.ToInt32(strArray3[3]));
                        returnStatus = eReturn.SUCCESS;
                        return dateTime2;
                    }
                }
            }
            else
                returnStatus = eReturn.FAILURE;
            return dateTime1;
        }

        public delegate void ExtractUpdateAndUpdate(string statusString);
    }
}
