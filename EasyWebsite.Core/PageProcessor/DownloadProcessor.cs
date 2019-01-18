using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EasyWebsite.Core.WebClient;

namespace EasyWebsite.Core.PageProcessor
{
    /// <summary>
    /// 用于下载的页面处理器，可下载小文件和http页面等
    /// </summary>
    public class DownloadProcessor : PageProcessorBase
    {
        public string StoragePath { get; set; }
        public DownloadOptions Options { get; set; }

        public DownloadProcessor(string storagePath)
        {
            StoragePath = storagePath;
            Options = new DownloadOptions();
        }

        public DownloadProcessor(string storagePath, DownloadOptions options)
        {
            StoragePath = storagePath;
            Options = options;
        }

        protected override void Handle(Response response)
        {
            if (response.ResponseType == ResponseTypeEnum.File)
            {
                using (FileStream writeStream = CreateFileStream(response.DownLoadFileName))
                {
                    if (writeStream == null)
                        return;
                    byte[] btArray = response.Content as byte[];
                    writeStream.Write(btArray,0,btArray.Length);
                    writeStream.Flush();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private FileStream CreateFileStream(string file)
        {
            int count = 1;
            string filePath = Path.GetDirectoryName(file);
            switch(Options.NameStrategy)
            {
                case SameNameFileStrategy.RenameWithNumber:
                    string filenameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileExtension = Path.GetExtension(file);
                    while (File.Exists(file))
                    {
                        file = $"{filePath}\\{filenameWithoutExtension}({count}){fileExtension}";
                        count++;
                    }
                    break;
                case SameNameFileStrategy.OverWrite:
                    break;
                case SameNameFileStrategy.Abandon:
                    return null;
            }
            return new FileStream(file, FileMode.Create);
        }
    }

    public class DownloadOptions
    {
        public string FileName { get; set; }

        public Encoding HtmlEncoding { get; set; }

        public SameNameFileStrategy NameStrategy { get; set; }

        public DownloadOptions()
        {
            FileName = null;
            HtmlEncoding = Encoding.UTF8;
            NameStrategy = SameNameFileStrategy.RenameWithNumber;
        }
    }

    public enum SameNameFileStrategy
    {
        /// <summary>
        /// 保留两份文件，并将新文件名加上后缀 
        /// eg：a.jpg => a(1).jpg 
        /// </summary>
        RenameWithNumber = 0,

        /// <summary>
        /// 新文件覆盖原本的文件
        /// </summary>
        OverWrite = 1,

        /// <summary>
        /// 不保留新文件
        /// </summary>
        Abandon = 2
    }
}
