
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EpochCompressingTool
{
    public class EpochConverter
    {
        private const int APP_VERSION_LINE_COUNT = 5;
        private const int MEASUREMENT_FREQUENCT_LINE_COUNT = 10;
        private const int TOTAL_LINE_COUNT = 100;
        private const int RECORD_COUNT = 7;

        public eReturn SampleCSVFile(string inputCSVFilePath_i, string outputCSVFilePath_i, int epochPeriod_i)
        {
            StreamReader streamReader = (StreamReader)null;
            StreamWriter streamWriter = (StreamWriter)null;
            GeneralUtility generalUtility = new GeneralUtility();
            try
            {
                streamReader = new StreamReader((Stream)File.Open(inputCSVFilePath_i, FileMode.Open));
                if (streamReader == null)
                    return eReturn.FILE_DOESNOT_EXIST;
                streamWriter = new StreamWriter((Stream)File.Open(outputCSVFilePath_i, FileMode.Create));
                if (streamWriter == null)
                {
                    streamReader.Close();
                    return eReturn.FILE_DOESNOT_EXIST;
                }
            }
            catch (Exception ex)
            {
                if (streamReader != null)
                    streamReader.Close();
                if (streamWriter != null)
                    streamWriter.Close();
                return eReturn.FILE_IO_EXCEPTION;
            }
            try
            {
                for (int index = 0; index < 100; ++index)
                {
                    string str = streamReader.ReadLine();
                    if (str == null)
                    {
                        if (streamReader != null)
                            streamReader.Close();
                        if (streamWriter != null)
                            streamWriter.Close();
                        return eReturn.FILE_IO_EXCEPTION;
                    }
                    if (5 == index)
                    {
                        string[] strArray = str.Split(',');
                        //if (strArray.Length != 2 || VersionInfo.TestApplicationTitleVersion(strArray[1]) < 0)
                        //{
                        //  streamReader.Close();
                        //  streamWriter.Close();
                        //  if (File.Exists(outputCSVFilePath_i))
                        //    File.Delete(outputCSVFilePath_i);
                        //  return eReturn.INVALID_CSV_FILE;
                        //}
                    }
                    if (10 == index)
                    {
                        if (str == null || "" == str)
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            if (File.Exists(outputCSVFilePath_i))
                                File.Delete(outputCSVFilePath_i);
                            return eReturn.FAILURE;
                        }
                        if (2 != str.Split(',').Length)
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            if (File.Exists(outputCSVFilePath_i))
                                File.Delete(outputCSVFilePath_i);
                            return eReturn.FAILURE;
                        }
                    }
                    streamWriter.WriteLine(str);
                }
                RecordedData recordedData = new RecordedData();
                string str1 = (string)null;
                DateTime dateTime1 = new DateTime();
                string str2;
                while ((str2 = streamReader.ReadLine()) != null)
                {
                    float num1 = 0.0f;
                    if ("" == str2)
                    {
                        streamReader.Close();
                        streamWriter.Close();
                        if (File.Exists(outputCSVFilePath_i))
                            File.Delete(outputCSVFilePath_i);
                        return eReturn.FAILURE;
                    }
                    string[] strArray1 = str2.Split(',');
                    if (7 != strArray1.Length)
                    {
                        streamReader.Close();
                        streamWriter.Close();
                        if (File.Exists(outputCSVFilePath_i))
                            File.Delete(outputCSVFilePath_i);
                        return eReturn.FAILURE;
                    }
                    for (int index = 0; index < 7; ++index)
                    {
                        if (strArray1[index] == null || "" == strArray1[index])
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            if (File.Exists(outputCSVFilePath_i))
                                File.Delete(outputCSVFilePath_i);
                            return eReturn.INVALID_CSV_FILE;
                        }
                    }
                    string[] strArray2 = strArray1[0].Split(' ');
                    string[] strArray3 = strArray2[0].Split('-');
                    string[] strArray4 = strArray2[1].Split(':');
                    DateTime dateTime2 = new DateTime(Convert.ToInt32(strArray3[0]), Convert.ToInt32(strArray3[1]), Convert.ToInt32(strArray3[2]), Convert.ToInt32(strArray4[0]), Convert.ToInt32(strArray4[1]), Convert.ToInt32(strArray4[2]), Convert.ToInt32(strArray4[3]));
                    string str3 = dateTime2.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    if (":" != CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator)
                        str3 = str3.Replace(CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator, ":");
                    DateTime dateTime3 = dateTime2.AddSeconds((double)epochPeriod_i);
                    double dXaxis_i1 = Convert.ToDouble(strArray1[1], (IFormatProvider)CultureInfo.InvariantCulture);
                    recordedData.ArrXaxis.Add((float)dXaxis_i1);
                    double dYaxis_i1 = Convert.ToDouble(strArray1[2], (IFormatProvider)CultureInfo.InvariantCulture);
                    recordedData.ArrYaxis.Add((float)dYaxis_i1);
                    double dZaxis_i1 = Convert.ToDouble(strArray1[3], (IFormatProvider)CultureInfo.InvariantCulture);
                    recordedData.ArrZaxis.Add((float)dZaxis_i1);
                    float num2 = num1 + this.GetSVM(dXaxis_i1, dYaxis_i1, dZaxis_i1);
                    recordedData.ArrLightMeter.Add(Convert.ToUInt16(strArray1[4]));
                    recordedData.ArrButtonStatus.Add(strArray1[5]);
                    recordedData.ArrTemperature.Add((float)Convert.ToDouble(strArray1[6], (IFormatProvider)CultureInfo.InvariantCulture));
                    string str4 = dateTime3.ToString("yyyy-MM-dd HH:mm:ss");
                    string str5;
                    while ((str5 = streamReader.ReadLine()) != null && !str5.Contains(str4))
                    {
                        string[] strArray5 = str5.Split(',');
                        if (7 != strArray5.Length)
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            if (File.Exists(outputCSVFilePath_i))
                                File.Delete(outputCSVFilePath_i);
                            return eReturn.INVALID_CSV_FILE;
                        }
                        double dXaxis_i2 = Convert.ToDouble(strArray5[1], (IFormatProvider)CultureInfo.InvariantCulture);
                        recordedData.ArrXaxis.Add((float)dXaxis_i2);
                        double dYaxis_i2 = Convert.ToDouble(strArray5[2], (IFormatProvider)CultureInfo.InvariantCulture);
                        recordedData.ArrYaxis.Add((float)dYaxis_i2);
                        double dZaxis_i2 = Convert.ToDouble(strArray5[3], (IFormatProvider)CultureInfo.InvariantCulture);
                        recordedData.ArrZaxis.Add((float)dZaxis_i2);
                        num2 += this.GetSVM(dXaxis_i2, dYaxis_i2, dZaxis_i2);
                        recordedData.ArrLightMeter.Add(Convert.ToUInt16(strArray5[4]));
                        recordedData.ArrButtonStatus.Add(strArray5[5]);
                        recordedData.ArrTemperature.Add((float)Convert.ToDouble(strArray5[6], (IFormatProvider)CultureInfo.InvariantCulture));
                    }
                    if (str5 != null)
                    {
                        string[] strArray5 = str5.Split(',');
                        if (7 != strArray5.Length)
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            if (File.Exists(outputCSVFilePath_i))
                                File.Delete(outputCSVFilePath_i);
                            return eReturn.INVALID_CSV_FILE;
                        }
                        double dXaxis_i2 = Convert.ToDouble(strArray5[1], (IFormatProvider)CultureInfo.InvariantCulture);
                        recordedData.ArrXaxis.Add((float)dXaxis_i2);
                        double dYaxis_i2 = Convert.ToDouble(strArray5[2], (IFormatProvider)CultureInfo.InvariantCulture);
                        recordedData.ArrYaxis.Add((float)dYaxis_i2);
                        double dZaxis_i2 = Convert.ToDouble(strArray5[3], (IFormatProvider)CultureInfo.InvariantCulture);
                        recordedData.ArrZaxis.Add((float)dZaxis_i2);
                        num2 += this.GetSVM(dXaxis_i2, dYaxis_i2, dZaxis_i2);
                        recordedData.ArrLightMeter.Add(Convert.ToUInt16(strArray5[4]));
                        recordedData.ArrButtonStatus.Add(strArray5[5]);
                        recordedData.ArrTemperature.Add((float)Convert.ToDouble(strArray5[6], (IFormatProvider)CultureInfo.InvariantCulture));
                    }
                    int num3 = 0;
                    foreach (ushort num4 in recordedData.ArrLightMeter)
                        num3 += (int)num4;
                    int num5 = num3 / recordedData.ArrLightMeter.Count;
                    int num6 = 0;
                    foreach (string arrButtonStatu in recordedData.ArrButtonStatus)
                    {
                        if ("1" == arrButtonStatu)
                            ++num6;
                    }
                    float num7 = recordedData.ArrXaxis.Average();
                    float num8 = recordedData.ArrYaxis.Average();
                    float num9 = recordedData.ArrZaxis.Average();
                    float num10 = recordedData.ArrTemperature.Average();
                    double standardDeviation1 = this.GetStandardDeviation(recordedData.ArrXaxis);
                    double standardDeviation2 = this.GetStandardDeviation(recordedData.ArrYaxis);
                    double standardDeviation3 = this.GetStandardDeviation(recordedData.ArrZaxis);
                    long num11 = (long)recordedData.ArrLightMeter.Max<ushort>();
                    streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", (object)str3, (object)generalUtility.ConvertCultureToDot((double)num7, 4), (object)generalUtility.ConvertCultureToDot((double)num8, 4), (object)generalUtility.ConvertCultureToDot((double)num9, 4), (object)num5, (object)num6.ToString(), (object)generalUtility.ConvertCultureToDot((double)num10, 1), (object)generalUtility.ConvertCultureToDot((double)num2, 2), (object)generalUtility.ConvertCultureToDot(standardDeviation1, 4), (object)generalUtility.ConvertCultureToDot(standardDeviation2, 4), (object)generalUtility.ConvertCultureToDot(standardDeviation3, 4), (object)num11.ToString());
                    str1 = (string)null;
                    recordedData.ArrButtonStatus.Clear();
                    recordedData.ArrLightMeter.Clear();
                    recordedData.ArrTemperature.Clear();
                    recordedData.ArrXaxis.Clear();
                    recordedData.ArrYaxis.Clear();
                    recordedData.ArrZaxis.Clear();
                }
                streamReader.Close();
                streamWriter.Close();
                return eReturn.SUCCESS;
            }
            catch (OutOfMemoryException ex)
            {
                streamReader.Close();
                streamWriter.Close();
                if (File.Exists(outputCSVFilePath_i))
                    File.Delete(outputCSVFilePath_i);
                return eReturn.OUT_OF_MEMORY;
            }
            catch (IndexOutOfRangeException ex)
            {
            }
            streamReader.Close();
            streamWriter.Close();
            if (File.Exists(outputCSVFilePath_i))
                File.Delete(outputCSVFilePath_i);
            return eReturn.INVALID_CSV_FILE;
        }

        private float GetSVM(double dXaxis_i, double dYaxis_i, double dZaxis_i)
        {
            return Math.Abs((float)Math.Sqrt(dXaxis_i * dXaxis_i + dYaxis_i * dYaxis_i + dZaxis_i * dZaxis_i) - 1f);
        }

        public double GetStandardDeviation(List<float> ArrAxis)
        {
            double num1 = (double)ArrAxis.Average();
            double num2 = 0.0;
            for (int index = 0; index < ArrAxis.Count; ++index)
                num2 += Math.Pow((double)ArrAxis[index] - num1, 2.0);
            return Math.Sqrt(num2 / (double)ArrAxis.Count);
        }
    }
}
