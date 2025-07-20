using System.Runtime.InteropServices;
using System.Text;
using myloadfile.Models;
using myloadfile.Services;


namespace myloadfile
{
    class CGITest
    {
        static void Main()
        {
            // Setting the encoding Windows-1251
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding("windows-1251");

            FormContext context = new FormContext(); // Getting all variables

            // Getting the OS name - windows/linux
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                context.OS = "Windows";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                context.OS = "Linux";

            // Read the size of the request body (which contains the file and other data)
            string contentLengthHeader = Environment.GetEnvironmentVariable("CONTENT_LENGTH");
            context.ContentLength = int.Parse(contentLengthHeader);
           
            context.PathDir = context.DateTime.ToShortDateString();  // Path to directories for uploading received files to the server

            // If use WINDOWS, then the format of the path to the imagecart directory will be one, and if Linux, then another
            if (context.OS.Contains("Windows"))
            {
                context.PathImageCart = context.WindowsPath; // For windows
                context.PathDir = context.PathDir.Replace(".", "-");
            }
            else
            {
                context.PathImageCart = context.LinuxPath;  // For Linux
                context.PathDir = context.PathDir.Replace("/", "-");
            }

            // Checking for the existence of a directory imagecart
            if (!Directory.Exists(context.PathImageCart))
                Directory.CreateDirectory(context.PathImageCart);

            // Check for existence of current day directory to structure unloading 
            if (!Directory.Exists(context.PathImageCart + context.PathDir))
                Directory.CreateDirectory(context.PathImageCart + context.PathDir);

            try
            {
                // Read all data from standard input
                byte[] buffer = new byte[context.ContentLength];
                int bytesRead = 0;
                while (bytesRead < context.ContentLength)
                {
                    bytesRead += Console.OpenStandardInput().Read(buffer, bytesRead, context.ContentLength - bytesRead);
                }
                context.TestInput = encoding.GetString(buffer);

                // Set a boundary that will be used to separate parts of the data.
                string contentType = Environment.GetEnvironmentVariable("CONTENT_TYPE");
                string boundary = FormDataParser.GetBoundary(contentType);
               
                List<FormData> parts = FormDataParser.ParseMultipartFormData(buffer, boundary);  // Parsing multipart data

                // Processing parts of the data
                foreach (var part in parts)
                {
                    // Processing form data that contains a file in byte form
                    if (part.ContentDisposition.Contains("filename"))
                    {
                        // If use WINDOWS, then the format of the path to the downloaded file will be one, and if Linux, then another
                        if (context.OS.Contains("Windows"))//
                            context.PathFile = context.PathImageCart + context.PathDir + "\\" + part.FileName; // For windows
                        else
                            context.PathFile = context.PathImageCart + context.PathDir + "/" + part.FileName; // For Linux

                        // Checking if the file being uploaded exists on the server
                        FileInfo UploadFile = new FileInfo(context.PathFile);
                        int SerialNumber = 1;

                        // If the file already exists on the server, then add _1 to the beginning of the name
                        while (UploadFile.Exists)
                        {
                            if (context.OS.Contains("Windows"))
                                context.PathFile = context.PathImageCart + context.PathDir + "\\" + SerialNumber + "_" + part.FileName;
                            else
                                context.PathFile = context.PathImageCart + context.PathDir + "/" + SerialNumber + "_" + part.FileName;

                            UploadFile = new FileInfo(context.PathFile);
                            SerialNumber++;
                        }
                        context.TestCONTENT = encoding.GetString(part.Content);                       
                        File.WriteAllBytes(context.PathFile, part.Content);  // Uploading a file to the server

                        // Link to the uploaded file on the server. Required for the hidden variable and download link in the outpatient card sheet
                        context.NameFileInternet = "/" + context.PathFile.Replace("\\", "/");

                        // Creating a link for viewing in the outpatient card sheet (opener)
                        // If the file you are downloading is an office file, then simply create a link to it for downloading to your PC
                        if (part.FileName.ToLower().Contains(".doc") || part.FileName.ToLower().Contains(".xls"))
                            context.OPENERLINK += "Ссылка для скачивания файла: " + part.FileName + " <a href='" + context.NameFileInternet + "' contentEditable='False' >" + context.NameFileInternet + "</a><br>";
                        // If the file you are uploading is an image, and insert it into the tag img
                        else if (part.FileName.ToLower().Contains(".img") || part.FileName.ToLower().Contains(".png") || part.FileName.ToLower().Contains(".jpeg") || part.FileName.ToLower().Contains(".bmp"))
                            context.OPENERLINK += "<img height ='750px' width ='700px' src ='" + context.NameFileInternet + "'><br>";
                        // In any other case - insert it into the frame
                        else context.OPENERLINK += "Содержимое файла " + part.FileName + ":<br><iframe  height ='750px' width ='700px' src ='" + context.NameFileInternet + "'></iframe><br>";
                    }
                    else // Processing form data that contains other "text" parameters
                    {
                        if (part.Name.Contains("USER"))                         // USER
                            context.USER = encoding.GetString(part.Content);
                        if (part.Name.Contains("REMARKFILE"))                   // REMARKFILE
                            context.REMARKFILE = encoding.GetString(part.Content);
                        if (part.Name.Contains("TYPELISTCART"))                 // TYPELISTCART
                            context.TYPELISTCART = encoding.GetString(part.Content);
                        if (part.Name.Contains("STATUSLISTCART"))               // STATUSLISTCART
                            context.STATUSLISTCART = encoding.GetString(part.Content);

                        // If the inspection sheet type is not empty (and therefore the "Add as new inspection sheet" button is pressed)
                        if (context.TYPELISTCART.Replace("\r\n", "") != "")
                            context.PREOPENERLINK = HtmlResponseService.ResponsePreOpenerLink(context); // Add as a new inspection sheet (depending on the existence of buttons - with a coupon or not)
                    }
                }
                Console.Write(HtmlResponseService.ResponseSuccessHtml(context)); // Return HTML page with file upload result
            }
            catch (Exception ex)
            {
                Console.Write(HtmlResponseService.ResponseErrorHtml(ex)); // Return HTML page with error message
            }
        }
    }
}