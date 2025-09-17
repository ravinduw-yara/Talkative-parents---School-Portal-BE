using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonUtility
{
    public class WriteLogFile
    {
        public static void LogErrorMessage(Exception e)
        {

            //Create a file
            DateTime dt = DateTime.Today;
            string monthYear = (dt.Month).ToString() + (dt.Year).ToString();
            string fileName = @"D:\LogFiles\ErrorLog" + monthYear + ".txt";
            FileInfo fi = new FileInfo(fileName);

            try
            {
                FileStream objFilestream = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                // Check if file already exists. If yes, Append the Data.     
                if (!fi.Exists)
                {
                    // Create a new file     
                    using (FileStream fs = fi.Create());
                }

                Log(e.ToString(), objStreamWriter);

                objStreamWriter.Close();
                objFilestream.Close();
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex);
            }
        }

        private static void Log(String logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
            // Update the underlying file.
            w.Flush();
        }

    }
}
