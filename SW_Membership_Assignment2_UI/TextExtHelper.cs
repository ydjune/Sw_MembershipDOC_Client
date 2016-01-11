using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordTextExt.Office;

namespace SW_Membership_Assignment2_UI
{
    public class TextExtHelper
    {

      

        private IWordFile wordFile = null; //워드 파일 열었을때의 파일 포인터;
        private IOfficeFile _file; //텍스트 추출 객체
        public TextExtHelper() 
        {
          
        }



        public void OpenFile(String filePath)
        {

            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = System.IO.Path.GetExtension(filePath);
            string addName = fileName + fileExtension;
            try
            {
                this._file = OfficeFileFactory.CreateOfficeFile(filePath);
                Console.WriteLine(String.Format((this._file == null ? "Failed to open \"{0}\"." : ""), filePath));
            }
            catch (Exception ex)
            {
                this.CloseFile();
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseFile()
        {
            this._file = null;
            Console.WriteLine("");
            Console.WriteLine("닫혔음!");
        }


        public void ShowSummary(Dictionary<String, String> dictionary)
        {
            if (dictionary == null)
            {
                Console.WriteLine("This file is not Microsoft Office file.");
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<String, String> pair in dictionary)
            {
                sb.AppendFormat("[{0}]={1}", pair.Key, pair.Value);
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }



        public void ShowContentsFunc(IOfficeFile file)
        {
            if (file is IWordFile)
            {
                wordFile = file as IWordFile;
            }
            else if (file is IPowerPointFile)
            {
                IPowerPointFile pptFile = file as IPowerPointFile;
            }
        }

        public IOfficeFile get_file()
        {
            return _file;
        }

        public IWordFile getwordFile()
        {
            return wordFile;
        }

    }
}
