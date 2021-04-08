using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.IO;
using System.Text;

namespace Chailease.SolarEnergy.Web.Commons
{
    public class PdfHalpers
    {
        /// <summary>
        /// 將Html文字 輸出到PDF檔裡
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        public byte[] ToPdf(string Html)
        {
            if (string.IsNullOrEmpty(Html))
                return null;

            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
            //Html = "<p>" + Html + "</p>";
            try
            {
                using (MemoryStream outputStream = new MemoryStream())//要把PDF寫到哪個串流
                {
                    byte[] data = Encoding.UTF8.GetBytes(Html);//字串轉成byte[]
                    using (MemoryStream msInput = new MemoryStream(data))
                    {
                        using (Document doc = new Document())//要寫PDF的文件，建構子沒填的話預設直式A4
                        {
                            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
                            //指定文件預設開檔時的縮放為100%
                            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                            //開啟Document文件 
                            doc.Open();
                            //使用XMLWorkerHelper把Html parse到PDF檔裡
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
                            //將pdfDest設定的資料寫到PDF檔
                            PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
                            writer.SetOpenAction(action);
                        }
                    }
                    //回傳PDF檔案 
                    return outputStream.ToArray();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Html Transform Pdf Fail!!!", e);
            }
        }

        public class UnicodeFontFactory : FontFactoryImp
        {
            private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");//arial unicode MS是完整的unicode字型。
            private static readonly string KaiuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KAIU.TTF");//標楷體

            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
            {
                //可用Arial或標楷體，自己選一個
                BaseFont baseFont = BaseFont.CreateFont(KaiuPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return new Font(baseFont, size, style, color);
            }
        }

          
    }
}