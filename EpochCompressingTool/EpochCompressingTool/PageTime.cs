
using System;

namespace EpochCompressingTool
{
    public class PageTime
    {
        private ushort m_Year;
        private ushort m_Month;
        private ushort m_Date;
        private ushort m_Hour;
        private ushort m_Minute;
        private ushort m_Second;
        private double m_Millisecond;

        public PageTime()
        {
            this.m_Year = (ushort)0;
            this.m_Month = (ushort)0;
            this.m_Date = (ushort)0;
            this.m_Hour = (ushort)0;
            this.m_Minute = (ushort)0;
            this.m_Second = (ushort)0;
            this.m_Millisecond = 0.0;
        }

        public PageTime(ushort y, ushort m, ushort d, ushort hh, ushort mm, ushort ss, ushort ms)
        {
            this.m_Year = y;
            this.m_Month = m;
            this.m_Date = d;
            this.m_Hour = hh;
            this.m_Minute = mm;
            this.m_Second = ss;
            this.m_Millisecond = (double)ms;
        }

        public PageTime(long ticks)
        {
            this.Ticks = ticks;
        }

        public PageTime(DateTime dt)
        {
            this.Ticks = dt.Ticks;
        }

        public PageTime(string[] timeStr)
        {
            if (timeStr.Length < 7)
                throw new ArgumentException("timeStr array must contain at least 7 entries");
            this.m_Year = Convert.ToUInt16(timeStr[0]);
            this.m_Month = Convert.ToUInt16(timeStr[1]);
            this.m_Date = Convert.ToUInt16(timeStr[2]);
            this.m_Hour = Convert.ToUInt16(timeStr[3]);
            this.m_Minute = Convert.ToUInt16(timeStr[4]);
            this.m_Second = Convert.ToUInt16(timeStr[5]);
            this.m_Millisecond = (double)Convert.ToUInt16(timeStr[6]);
        }

        public static PageTime Now
        {
            get
            {
                return new PageTime(DateTime.Now);
            }
        }

        public ushort Year
        {
            set
            {
                this.m_Year = value;
            }
            get
            {
                return this.m_Year;
            }
        }

        public ushort Month
        {
            set
            {
                this.m_Month = value;
            }
            get
            {
                return this.m_Month;
            }
        }

        public ushort Date
        {
            set
            {
                this.m_Date = value;
            }
            get
            {
                return this.m_Date;
            }
        }

        public ushort Hour
        {
            set
            {
                this.m_Hour = value;
            }
            get
            {
                return this.m_Hour;
            }
        }

        public ushort Minute
        {
            set
            {
                this.m_Minute = value;
            }
            get
            {
                return this.m_Minute;
            }
        }

        public ushort Second
        {
            set
            {
                this.m_Second = value;
            }
            get
            {
                return this.m_Second;
            }
        }

        public ushort Millisecond
        {
            set
            {
                this.m_Millisecond = (double)value;
            }
            get
            {
                return (ushort)Math.Floor(this.m_Millisecond);
            }
        }

        public DateTime ToDateTime()
        {
            return new DateTime(this.Ticks);
        }

        public long Ticks
        {
            get
            {
                return new DateTime((int)this.m_Year, (int)this.m_Month, (int)this.m_Date, (int)this.m_Hour, (int)this.m_Minute, (int)this.m_Second, DateTimeKind.Utc).Ticks + (long)(this.m_Millisecond * 10000.0);
            }
            set
            {
                DateTime dateTime = new DateTime(value);
                this.m_Year = (ushort)dateTime.Year;
                this.m_Month = (ushort)dateTime.Month;
                this.m_Date = (ushort)dateTime.Day;
                this.m_Hour = (ushort)dateTime.Hour;
                this.m_Minute = (ushort)dateTime.Minute;
                this.m_Second = (ushort)dateTime.Second;
                this.m_Millisecond = 0.0;
                this.m_Millisecond = Math.Round((double)(value - this.Ticks) / 10000.0, 4);
            }
        }

        public static PageTime operator +(PageTime left, double addSecs)
        {
            return new PageTime(left.Ticks + (long)(addSecs * 10000000.0));
        }

        public static PageTime operator -(PageTime left, double minusSecs)
        {
            return new PageTime(left.Ticks - (long)(minusSecs * 10000000.0));
        }

        public bool IsSet
        {
            get
            {
                try
                {
                    return this.ToDateTime() > new DateTime(1980, 1, 1);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public string ToString(string format)
        {
            try
            {
                return this.ToDateTime().ToString(format);
            }
            catch (Exception ex)
            {
                return "Not set";
            }
        }

        public override string ToString()
        {
            return this.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
