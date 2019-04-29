using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.WebDownload
{
    public class WebDownloadService
    {
        public string DownloadPage(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.ReadWriteTimeout = 5000;

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return string.Empty;
            }

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;

            if (response.CharacterSet == null)
            {
                readStream = new StreamReader(receiveStream);
            }
            else
            {
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            }

            string data = readStream.ReadToEnd();

            response.Close();
            readStream.Close();

            return data;
        }
    }
}
