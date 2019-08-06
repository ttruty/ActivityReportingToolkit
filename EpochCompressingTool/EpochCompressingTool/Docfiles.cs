using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace EpochCompressingTool
{
    class Docfiles
    {
        private ConnectionManager m_ConnectionManagerObject = ConnectionManager.ConnectionManagerInstance;
        private List<string> m_ConnectedDeviceIDs = new List<string>();
        private DeviceCapabilities m_DeviceCapabilitiesObject = new DeviceCapabilities();
        private RecordedData m_AvGData = new RecordedData();
        private GeneralUtility m_GeneralUtility = new GeneralUtility();

        public static String docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private String savePath = docsPath + "\\GENEActiv\\Reports";
        private String gaDataPath = docsPath + "\\GENEActiv\\Data";
        private  String excelPath = docsPath + "\\GeneActivAUTO.xltm";

        public void CovertDir()
        {
            Directory.CreateDirectory(savePath);
            DirSearch(gaDataPath);
        }

        private void DirSearch(string sDir)
        {
            Stopwatch stopWatch = new Stopwatch();
            int totalCount = Directory.EnumerateFiles(sDir, "*.bin*", SearchOption.AllDirectories).Count();

            try
            {
                int i = 1;
                foreach (string file in Directory.EnumerateFiles(sDir, "*.bin*", SearchOption.AllDirectories))
                {
                    stopWatch.Start();
                    string binFilename = Path.GetFileName(file);
                    Console.WriteLine("Compressing " + binFilename);
                    ConvertData(file);
                    drawTextProgressBar(i, totalCount);
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    Console.WriteLine("Conversion time:  " + elapsedTime);

                    i++;
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        void ConvertData(string inputPath)
        {
            string lower = Path.GetExtension(inputPath).ToLower();
            string input = Path.ChangeExtension(inputPath, null);
            string outputPath = input + "_EPOCHCONVERTED.csv";
            string outputFilename = Path.GetFileName(outputPath);
            outputPath = Path.Combine(savePath, outputFilename);
            //Console.WriteLine(outputPath);
            //Console.ReadKey();

            if (!File.Exists(outputPath))
            {
                uint recoveredDataBlocks = 0;
                float result = 0.0f;
                int int32 = Convert.ToInt32(60);
                eReturn eReturn = !(lower == ".bin") ? new EpochConverter().SampleCSVFile(inputPath, outputPath, int32) : new DataConverter().CompressBinToCSV(inputPath, outputPath, int32, result, out recoveredDataBlocks);
                //Console.WriteLine(eReturn);

                switch (eReturn)
                {
                    case eReturn.SUCCESS:
                        Console.WriteLine("<< EPOCH COMPRESSION COMPLETE >>");
                        break;
                    case eReturn.CORRUPTED_BIN_FILE:
                        Console.WriteLine("Epoch conversion incomplete/failed. Input .csv/.bin file was incomplete or corrupt, the output file may also be incomplete");
                        break;
                    case eReturn.FILE_IO_EXCEPTION:
                        Console.WriteLine("Epoch conversion failed. Error in opening .csv/.bin file");
                        break;
                    case eReturn.INVALID_CSV_FILE:
                        Console.WriteLine("Epoch conversion failed. Invalid .csv/.bin file");
                        break;
                    case eReturn.OUT_OF_MEMORY:
                        Console.WriteLine("There is not enough space on the disk.");
                        break;
                    default:
                        Console.WriteLine("Epoch conversion failed");
                        break;
                }
            }
            else
            {
                Console.WriteLine(outputPath + " <--<-- CSV COMPRESSED FILE ALREADY EXISTS -->-->");
            }
        }

        void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}
