using System.Text;
using myloadfile.Models;

namespace myloadfile.Services;

public static class FormDataParser
{
    // Extract Content-Type Boundaries
    public static string GetBoundary(string contentType)
    {
        var elements = contentType.Split(';');
        foreach (var element in elements)
        {
            if (element.Trim().StartsWith("boundary="))
                return element.Split('=')[1].Trim('"');
        }
        return string.Empty;
    }

    // Parse multipart/form-data
    public static List<FormData> ParseMultipartFormData(byte[] data, string boundary)
    {
        var parts = new List<FormData>();
        string boundaryString = "--" + boundary;
        string endBoundaryString = "--" + boundary + "--";
        string rawData = Encoding.GetEncoding("windows-1251").GetString(data); // Converting data to string to find boundaries
        int start = rawData.IndexOf(boundaryString);
        while (start != -1)
        {
            int end = rawData.IndexOf(boundaryString, start + boundaryString.Length);
            if (end == -1)
                break;
            string partData = rawData.Substring(start, end - start);
            var part = new FormData(partData);
            parts.Add(part);
            start = end;
        }
        return parts;
    }
}