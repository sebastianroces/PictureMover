using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Outlook;
using OutlookApp = Microsoft.Office.Interop.Outlook.Application;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Http;

namespace PictureMover
{
    public partial class Form1 : Form
    {
        private static Form1 form = null;
        private BackgroundWorker _worker = null;

        public Form1()
        {

            form = this;

            InitializeComponent();
            // EmailMessage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        static class jpgFiles
        {

            public static List<string> PicturesToBeMoved = new List<string>();
        }

        


        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory, bool form)
        {

            if (form == true)
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);

                foreach (string fileName in fileEntries)
                    ProcessFile(fileName, true);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory, true); 
            }

        }

        public static int CountFilesDirectory(string targetDirectory, bool form)
        {
            int jpgCount = 0;

            if (form == true)
            {

                var var1 = targetDirectory.Replace(@"\", @"/");
                var var2 = var1.Split('/')[10];
                var var3 = var1.Replace(var2, "");

                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(var3);

                foreach (string fileName in fileEntries)
                    jpgCount += CountFilesInFile(fileName, true);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(var3);
                foreach (string subdirectory in subdirectoryEntries)
                    jpgCount += CountFilesDirectory(subdirectory, true);
            }

            return jpgCount;
        }



        // Insert logic for processing found files here.
        public static int CountFilesInFile(string path, bool TrueValue)
        {

            int JpgCount = 0;

            try
            {

                if (path.Contains(".jpg") && path.Contains("ForemanPix") || (path.Contains(".JPG") && path.Contains("ForemanPix")))
                {
                    JpgCount += 1;
                }

                Console.WriteLine(" Empty File Check '{0}'.", path);
            }
            catch { }
                return JpgCount;

            }




        // Insert logic for processing found files here.
        public static void ProcessFile(string path, bool TrueValue)
        {
            try
            {
           

            if (path.Contains(".jpg") && path.Contains("ForemanPix") || (path.Contains(".JPG") && path.Contains("ForemanPix")))
            {
                    
                var AmountOfFiles = CountFilesDirectory(path, true);

                if (AmountOfFiles > 0)
                {
                        jpgFiles.PicturesToBeMoved.Add(path);
                        Console.WriteLine("Picture Added to Move Container '{0}'.", path);
                    }
            }

            Console.WriteLine("Processed file '{0}'.", path);
                // code
            }
            catch
            { }
        }


        public static void CopyPictureFiles(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;

            _worker.DoWork += new DoWorkEventHandler((state, args) =>
            {
                do
                {

                    ProcessDirectory("//Wolfedata/DATA/Dropbox/Foreman Folders", true);

                    if (_worker.CancellationPending)
                        break;

                    foreach (var item in jpgFiles.PicturesToBeMoved)
                    {

                        CheckIfJobExists(item);

                    }

                    jpgFiles.PicturesToBeMoved.Clear();

                    Console.WriteLine("Entering Into Sleep Mode for 10 mins");
                    // have it at 10 mins now
                    Thread.Sleep(600000);

                } while (true);
            });

            _worker.RunWorkerAsync();
            button1.Enabled = false;
            button2.Enabled = true;


        }
        

        private bool CheckIfJobExists(object item)
        {
            var PathString = item.ToString().Split('\\')[1];
            var Client = item.ToString().Split('\\')[0];
            var Address = item.ToString().Split('\\')[1];
            var ModifiedJobName = item.ToString().Split('\\')[2];

            //Grabs all the jobs in the job folder
            var CollectionOfJobs = Directory.GetDirectories("//Wolfedata/Data/Wolfe House Movers, LLC/ALL JOBS");

            var ListOfPix = new List<string>();
            try
            {
                foreach (var item2 in CollectionOfJobs)
                {
                    if (item2.Contains(ModifiedJobName))
                    {
                        Console.WriteLine("Processed file '{0}'.", item2);

                        var modifiedItem2 = item2.Replace("/", "\\");

                        



                        //open item2 path
                        // To copy a folder's contents to a new location:
                        // Create a new target folder. 
                        // If the directory already exists, this method does not create a new directory.

                       // // //Wolfedata/Data/Wolfe House Movers, LLC/ALL JOBS\Foley Construction LLC
                        var address = item.ToString().Split('\\')[3];

                        var createThisPath = modifiedItem2 + "\\" + address + "\\" + "Dropbox - " + PathString;

                       // //check for empty foreman's folders.
                        
                       //// problem with paths for Dan, but not Peter.
                       if (!Directory.Exists(createThisPath))
                          {

                               // search for all the paths where item2 = address we created
                                System.IO.Directory.CreateDirectory(createThisPath);

                          }


                        // Take from existing pathway and copy all photos.
                        //      var grabphotos = GetListOfFiles(item.ToString());
                        Console.WriteLine("Copying File '{0}'.", item.ToString(), createThisPath + "\\" + Path.GetFileName(item.ToString()));
                        File.Copy(item.ToString(), createThisPath + "\\" + Path.GetFileName(item.ToString()));
                         ListOfPix.Add(item.ToString());
                        }
                        //create new folder
                        // copy item over to item2 path
                    }
                
            } catch (System.Exception ex)
            {
                 // throw new System.ArgumentException("Parameter cannot be null", "original");
                // throw ex;                
            }
                       

            //SendMovedPicturesNotification(ListOfPix, ModifiedJobName);
            return true;
        }

        public string[] GetListOfFiles(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            foreach (string fileName in fileEntries)
                ProcessFile(fileName, false);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, false);

            return fileEntries;
        }

        private void SendMovedPicturesNotification(List<string> ListOfPix, string ModifiedJobName)
        {
            OutlookApp outlookApp = new OutlookApp();
            MailItem mailItem = outlookApp.CreateItem(OlItemType.olMailItem);

            mailItem.Subject = "[AUTOMATED NOTIFICATION SYSTEM] - " ;

            foreach (var items in ListOfPix)
            {
                mailItem.HTMLBody += items + " ";
            }

            //Set a high priority to the message
            mailItem.Importance = OlImportance.olImportanceHigh;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LV1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            _worker.CancelAsync();

        }


            //    private void EmailMessage()
            //    {
            //         var smtpClient = new SmtpClient("smtp.gmail.com")
            //             {
            //               Port = 587,
            //               Credentials = new System.Net.NetworkCredential("sebastian@wolfehousemovers.com", "WINDS0fwars!!"),
            //               EnableSsl = true,
            //               };

            //smtpClient.Send(new MailAddress('sebastian@wolfehousemovers.com'), "recipient", "subject", "body");
            //    }

        }
}
