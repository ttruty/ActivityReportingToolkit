
using System;
using System.IO;
using System.Reflection;

namespace EpochCompressingTool
{
    public static class VersionInfo
    {
        public static string ApplicationName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static int ApplicationVersion
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return version.Major * 10 + version.Minor;
            }
        }

        public static string ApplicationVersionString
        {
            get
            {
                return ((float)VersionInfo.ApplicationVersion / 10f).ToString("#.0");
            }
        }

        public static string ApplicationTitle
        {
            get
            {
                return VersionInfo.ApplicationName + " " + VersionInfo.ApplicationVersionString;
            }
        }

        public static string ApplicationBuild
        {
            get
            {
                return VersionInfo.RetrieveLinkerTimestamp().ToString("yyyy-MM-dd");
            }
        }

        public static string ApplicationCopyright
        {
            get
            {
                object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (customAttributes.Length == 0 || customAttributes.Length == 0)
                    return "";
                return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
            }
        }

        public static string DecodeApplicationTitle(string appTitle, out int appVer)
        {
            string[] strArray = appTitle.Split(' ');
            appVer = 0;
            if (strArray.Length < 1)
                return strArray[0];
            appVer = int.Parse(strArray[strArray.Length - 1].Replace(".", ""));
            return string.Join(" ", strArray, 0, strArray.Length - 1);
        }

        public static int TestApplicationTitleVersion(string appTitle)
        {
            int appVer = 0;
            if (string.Compare(VersionInfo.DecodeApplicationTitle(appTitle, out appVer), VersionInfo.ApplicationName, true) != 0 || appVer > VersionInfo.ApplicationVersion)
                return -1;
            return appVer != VersionInfo.ApplicationVersion ? 1 : 0;
        }

        private static DateTime RetrieveLinkerTimestamp()
        {
            string location = Assembly.GetCallingAssembly().Location;
            byte[] buffer = new byte[2048];
            Stream stream = (Stream)null;
            try
            {
                stream = (Stream)new FileStream(location, FileMode.Open, FileAccess.Read);
                stream.Read(buffer, 0, 2048);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            int int32_1 = BitConverter.ToInt32(buffer, 60);
            int int32_2 = BitConverter.ToInt32(buffer, int32_1 + 8);
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0);
            time = time.AddSeconds((double)int32_2);
            time = time.AddHours((double)TimeZone.CurrentTimeZone.GetUtcOffset(time).Hours);
            return time;
        }
    }
}
