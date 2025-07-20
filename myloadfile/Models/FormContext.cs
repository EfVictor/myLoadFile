namespace myloadfile.Models;

// Variables for form handling and upload paths
public class FormContext
{
    public string OS { get; set; } = "";    // Environment variable to get OS name - windows/linux
    public int ContentLength { get; set; }  // Read the size of the request body (which contains the file and other data)

    // Form variables
    public string USER { get; set; } = "";
    public string TYPELISTCART { get; set; } = "";
    public string STATUSLISTCART { get; set; } = "";
    public string ADDLISTCART { get; set; } = "";
    public string REMARKFILE { get; set; } = "";
    public string NameFileInternet { get; set; } = "";
    public string TestCONTENT { get; set; } = "";   // For debugging
    public string TestInput { get; set; } = "";      // For debugging

    // Displaying the uploaded file on the outpatient chart sheet
    public string PREOPENERLINK { get; set; } = ""; // Used when adding a new inspection sheet
    public string OPENERLINK { get; set; } = "";

    // Paths to directories for uploading received files to the server
    public string PathImageCart { get; set; } = "";
    public string PathDir { get; set; } = "";
    public string PathFile { get; set; } = "";
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string WindowsPath = "..\\imagecart\\";
    public string LinuxPath = "../imagecart/";
}