using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;

namespace EpochCompressingTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            Docfiles DocsWork = new Docfiles();
            
            DateTime startTime = DateTime.Now;
            Console.WriteLine("<< COMPRESSING TO 60 SECOND EPOCHS >>");
            Console.WriteLine("<< Started at " + startTime.ToString() + " >>");
            Console.WriteLine();

            DocsWork.CovertDir();

            Console.WriteLine("<< ALL FILES IN DATA COMPRESSED >>");
            Console.WriteLine("<< Press ANY KEY to Exit >>");
            Console.ReadKey();
        }
    }
}