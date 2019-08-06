

using System;
using System.Globalization;

namespace EpochCompressingTool
{
    public class GeneralUtility
    {
        public double ConvertCulture(string Value_i)
        {
            return Convert.ToDouble(Value_i, (IFormatProvider)CultureInfo.InvariantCulture);
        }

        public string ConvertCultureToDot(double fXYZValue_i, int nPrecision_i)
        {
            return this.GetValueAccordingtoCulture(fXYZValue_i, nPrecision_i).Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.');
        }

        public string GetValueAccordingtoCulture(double fXYZValue_i, int nPrecision_i)
        {
            string str1 = Math.Round(fXYZValue_i, nPrecision_i).ToString();
            string[] strArray = str1.Split(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);
            if (2 != strArray.Length)
            {
                string str2 = str1 + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
                return str2.PadRight(nPrecision_i + str2.Length, '0');
            }
            if (strArray[1].Length < nPrecision_i)
                str1 = str1.PadRight(str1.Length + (nPrecision_i - strArray[1].Length), '0');
            else if (strArray[1].Length > nPrecision_i)
                str1 = str1.Remove(str1.Length - (strArray[1].Length - nPrecision_i));
            return str1;
        }

        public string GetValueAccordingtoCulture(string General_i)
        {
            return General_i.Replace('.', CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);
        }

        public float ToFloatAgnostic(string strVal)
        {
            return Convert.ToSingle(strVal.Replace(',', '.'), (IFormatProvider)CultureInfo.InvariantCulture);
        }

        public double ToDoubleAgnostic(string strVal)
        {
            return Convert.ToDouble(strVal.Replace(',', '.'), (IFormatProvider)CultureInfo.InvariantCulture);
        }

        public int CalculateAge(DateTime dateOfBirth)
        {
            int num = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.Month < dateOfBirth.Month || DateTime.Now.Month == dateOfBirth.Month && DateTime.Now.Day < dateOfBirth.Day)
                --num;
            return num;
        }

        public double[] ConvertHeightToFeet(int height)
        {
            double[] numArray = new double[2];
            double num1 = Math.Floor((double)height * (25.0 / 762.0));
            double num2 = Math.Floor(((double)height * (25.0 / 762.0) - num1) * 12.0);
            numArray[0] = num1;
            numArray[1] = num2;
            return numArray;
        }

        public double[] ConvertWeightToKgs(float weight)
        {
            double[] numArray = new double[2];
            double num1 = Math.Floor((double)weight * 0.157473044);
            double num2 = Math.Floor(((double)weight * 0.157473044 - num1) * 14.0);
            numArray[0] = num1;
            numArray[1] = num2;
            return numArray;
        }

        public float CalculateBMI(float weight, float height)
        {
            float num = 0.0f;
            if ((double)height != 0.0)
            {
                height /= 100f;
                num = weight / (height * height);
            }
            return num;
        }

        public static int CalculateMaximumMeasurementPeriod(float frequency)
        {
            if ((double)frequency == 10.0)
                return 1440;
            if ((double)frequency == 20.0)
                return 720;
            if ((double)frequency == 25.0)
                return 672;
            if ((double)frequency == 30.0)
                return 504;
            if ((double)frequency == 40.0)
                return 432;
            if ((double)frequency == 50.0)
                return 360;
            if ((double)frequency == 60.0)
                return 288;
            if ((double)frequency == 66.6999969482422)
                return 264;
            if ((double)frequency == 75.0)
                return 240;
            if ((double)frequency == 85.6999969482422)
                return 216;
            if ((double)frequency == 100.0)
                return 168;
            if ((double)frequency == 500.0)
                return 24;
            return (double)frequency == 1000.0 ? 12 : 0;
        }

        public static string FormatPeriod(int periodHours)
        {
            int num1 = periodHours / 24;
            int num2 = periodHours - 24 * num1;
            string str1 = "" + (object)num1 + " day";
            if (num1 != 1)
                str1 += "s";
            string str2 = str1 + " " + (object)num2 + " hour";
            if (num2 != 1)
                str2 += "s";
            return str2;
        }

        public static int TimeZoneStringToMinutes(string tz)
        {
            string[] strArray1 = tz.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray1.Length != 2 || strArray1[0] != "GMT")
                return 0;
            string[] strArray2 = strArray1[1].Trim().Split(new char[1]
            {
        ':'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray2.Length < 1)
                return 0;
            int int32 = Convert.ToInt32(strArray2[0]);
            if (int32 < -23 || int32 > 23)
                return 0;
            int num = strArray2.Length > 1 ? Convert.ToInt32(strArray2[1]) : 0;
            if (num < 0 || num > 59)
                return 0;
            if (int32 < 0)
                return int32 * 60 - num;
            return int32 * 60 + num;
        }

        public static string DateTimeZoneString(DateTime localTime)
        {
            int num = (int)Math.Round((localTime - DateTime.UtcNow).TotalMinutes);
            return string.Format("GMT {0}{1:d2}:{2:d2}", num >= 0 ? (object)"+" : (object)"", (object)(num / 60), (object)(num % 60));
        }

        public static string TimeZoneString
        {
            get
            {
                return GeneralUtility.DateTimeZoneString(DateTime.Now);
            }
        }
    }
}
