using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace Acutis
{
    public partial class Acutis : ServiceBase
    {

        string processedTargetFolder = @"C:\ACData\ACD";
       

        public Acutis()
        {
            InitializeComponent();

            if (!System.Diagnostics.EventLog.SourceExists("Acutis"))
            {
                System.Diagnostics.EventLog.CreateEventSource("Acutis", "AcutisLog");
            }
            eventLog1.Source = "Acutis";
            eventLog1.Log = "AcutisLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Acutis Startup.");
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Acutis Stopped.");
        }

        void RunProcessFile(string fileName)
        {
           string OriginalFilesFolder = @"C:\ACData"; // appSet.getValue("DataFolderPath");
           string fullFileName = null;

            fullFileName = Path.Combine(OriginalFilesFolder, fileName);
            // Create processed folder within the original data folder
            try
            {
                string fp = Path.Combine(OriginalFilesFolder, "Temp");  // create new processed file folder off the original folder tree
                if (Directory.Exists(fp))
                {
                    Directory.Delete(fp, true);
                    Directory.CreateDirectory(fp);
                }
                else
                {
                    Directory.CreateDirectory(fp);
                }
            }
            catch (Exception ex)
            {
                
            }

            try   // clean up folders
            {
                if (Directory.Exists(processedTargetFolder))    // if processed folder exists delete it and all file within it.
                {
                    DirectoryInfo directory = new DirectoryInfo(processedTargetFolder);
                    foreach (FileInfo file in directory.GetFiles()) file.Delete();
                    foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
                }
            }
            catch (Exception ex)
            {
               
            }
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            ProcessFile(OriginalFilesFolder, processedTargetFolder, Path.GetFileName(fullFileName));
          
        }

        private void ProcessFile(string OriginalFileFullPath, string ProcessedFolderPath, string fileName)
        {
            // OriginalFullPath example :       C:\AKVAData\Data\20151106\AP7000_BGA.csv
            // ProcessedFolderPath example :    C:\AKVAData\Data\20151106\Processed
            // filename example :               AP7000_BGA.csv

            try
            {
                //string test = "2015-11-05 15:20:17.072;20.9";

                //string[] spl = test.Split(';');
                //string[] dt = spl[0].Split('.');        // dt[0] contains "2015-11-05 15:20:17"

                //fileName = "p_" + fileName;

                int processedLineCounter = 0;
                int originalLineCounter = 0;
                string line = String.Empty;
                string previousLine = String.Empty;
                string fileToProcessFullPath = Path.Combine(OriginalFileFullPath, fileName);

                //lineCount = File.ReadLines(fileToProcessFullPath).Count(); // gets the line count in file

                StreamReader file = new StreamReader(fileToProcessFullPath);
                StreamWriter processedfile = new StreamWriter(Path.Combine(ProcessedFolderPath, fileName));
                while ((line = file.ReadLine()) != null)
                {
                    string[] spl = line.Split(';');         // spl[0] = "2015-11-05 15:20:17.072"   spl[1] = "20.9"
                    string[] dt = spl[0].Split('.');        // dt[0] contains "2015-11-05 15:20:17"   dt[1] = "072"

                    if (dt[0] != previousLine)
                    {
                        processedfile.WriteLine(dt[0] + ";" + spl[1]);
                        processedLineCounter++;
                    }

                    previousLine = dt[0];
                    originalLineCounter++;
                }

                processedfile.Close();
                processedfile.Dispose();
                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
               
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string lastLine = File.ReadLines(targetFilePath).Last(); // gets the last line from file.

            string[] dt2 = lastLine.Split(';');
            DateTime lastLineDT = Convert.ToDateTime(dt2[0]);
           
        }
    }
}
