using Assessment.Models;
using ExcelDataReader;
using IronXL;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Assessment.Util
{
    //DATA MANAGER CLASS
    public class DataManager
    {
        //FORMAT FILE METHOD
        public static string FormatFile(string file)
        {
            //VARIABLES TO STORE THE DATA
            List<DataItem> data = new List<DataItem>();
            List<HeaderItem> headings = new List<HeaderItem>();

            string path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\data"}" + "\\" + file;

            //CODE ATTRIBUTION : https://www.youtube.com/watch?v=ibMfnX9_N4g
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //OPEN THE FILE AND BEGIN READING
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        //IF COUNT == 0, THEN GRAB THE HEADINGS
                        if (count == 0)
                        {
                            //ADD THE HEADINGS TO A LIST
                            for (int i = 0; i < reader.FieldCount; i += 2)
                            {
                                headings.Add(new HeaderItem() { Heading = reader.GetValue(i).ToString() });
                            }
                            count++;
                        }
                        else
                        {
                            //ELSE
                            if (reader.GetValue(0) != null)
                            {
                                //ADD DATA TO DATA LIST
                                data.Add(new DataItem()
                                {
                                    Name = reader.GetValue(0).ToString(),
                                    Surname = reader.GetValue(2).ToString(),
                                    PhoneNumber = reader.GetValue(4).ToString()
                                });
                                count++;
                            }
                        }
                    }
                }
            }

            //CREATE A NEW EXCEL DOCUMENT
            WorkBook workBook = new WorkBook();
            WorkSheet workSheet = workBook.DefaultWorkSheet;

            //VARIABLE FOR COLOUMNS
            string[] letters = { "A", "B", "C" };

            //FOR EACH NEEDED HEADING - ADD TO DOCUMENT
            for (int i = 1; i < headings.Count + 1; i++)
            {
                workSheet[letters[i - 1].ToString() + "1"].Value = headings[i - 1].Heading;
            }

            //FOR EACH NEEDED PIECE OF DATA - ADD TO DOCUMENT
            for (int i = 2; i < data.Count + 2; i++)
            {
                workSheet["A" + i].Value = data[i - 2].Name;
                workSheet["B" + i].Value = data[i - 2].Surname;
                workSheet["C" + i].Value = data[i - 2].PhoneNumber;
            }
            //SAVE THE DOCUMENT IN PROJECT FILES
            workBook.SaveAs(path);
            //RETURN THE PATH OF THE SAVED FILE
            return path;
        }
    }
}
