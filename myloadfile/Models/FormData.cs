using System.Text;

namespace myloadfile.Models;

// Processing multipart data received from form with file/s
public class FormData
{
    public string ContentDisposition { get; }
    public string FileName { get; }
    public string Name { get; }
    public byte[] Content { get; set; }

    public FormData(string partData)
    {
        // Search for Content-Disposition
        int start = partData.IndexOf("Content-Disposition: ");
        int end = partData.IndexOf("\r\n", start);
        ContentDisposition = partData.Substring(start, end - start);

        // Extract the form field name and file name
        int nameStart = ContentDisposition.IndexOf("name=\"") + 6;
        int nameEnd = ContentDisposition.IndexOf("\"", nameStart);
        Name = ContentDisposition.Substring(nameStart, nameEnd - nameStart);

        int fileNameStart = ContentDisposition.IndexOf("filename=\"") + 10;
        int fileNameEnd = ContentDisposition.IndexOf("\"", fileNameStart);
        FileName = ContentDisposition.Substring(fileNameStart, fileNameEnd - fileNameStart);

        // Extract the body of the part (taking into account binary data)
        start = partData.IndexOf("\r\n\r\n") + 4; // Skip the headlines
        Content = new byte[partData.Length - start];
        Buffer.BlockCopy(Encoding.GetEncoding("windows-1251").GetBytes(partData.Substring(start)), 0, Content, 0, Content.Length);
    }
}