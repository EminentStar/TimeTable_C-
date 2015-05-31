using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel; //참조추가
using TimeTable.DB;
using TimeTable.Models;
using TimeTable.Services;
using TimeTable.Useful_Functions;

namespace TimeTable
{
    public class TimeTable
    {
        private static TimeTable timeTableObj = new TimeTable();
        EnrollmentDBService enrollmentDBService = EnrollmentDBService.GetInstance();
        HomeService homeService = HomeService.GetInstance();
        JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();
        
        private TimeTable()
        {
        }

        public static TimeTable GetInstance()
        {
            return timeTableObj;
        }
        
        public string[,] GetEntireTimeTable(LoggedInUser paramLoggedInUser)
        {
            int userID = paramLoggedInUser.User.User_id;
            Dictionary<string, string> lecTimeDic = enrollmentDBService.FetchLectureDic(paramLoggedInUser.Con, userID);
            string[,] timeTable = new string[21, 6];

            InitializeTimeTable(timeTable);

            SetTimeTable(timeTable, lecTimeDic);

            return timeTable;
        }

        public string[,] ShowTimeTable(LoggedInUser paramLoggedInUser)
        {
            String[,] timeTable = new String[21, 6];
            int rowCount = timeTable.GetLength(0);
            int columnCount = timeTable.GetLength(1);
            int userID = paramLoggedInUser.User.User_id;
            int i, j;
            Dictionary<string, string> lecTimeDic = null;

            homeService.Clear();
            //get the information of the registered lectures
            lecTimeDic = enrollmentDBService.FetchLectureDic(paramLoggedInUser.Con, userID);

            InitializeTimeTable(timeTable);

            SetTimeTable(timeTable, lecTimeDic);

            Console.WriteLine("{1} ({0})의 수강신청 시간표", paramLoggedInUser.User.User_id, paramLoggedInUser.User.User_name);
            //Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────");
            for (i = 0; i < rowCount; i++)
            {
                Console.Write(timeTable[i, 0] + "│");
                for (j = 1; j < columnCount; j++)
                {
                    if (timeTable[i, j] == null)
                    {
                        Console.Write("".PadLeft(25, ' '));
                    }
                    else
                    {
                        Console.Write(jkAppExceptions.KoreanPadLeft(timeTable[i, j], 25, ' '));
                    }
                    Console.Write("│");

                }
                //Console.WriteLine("\n----------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("\n──────────────────────────────────────────────────────────────────────────");
            }

            Console.Write("아무키나 입력");
            Console.ReadLine();

            return timeTable;
        }

        public void InitializeTimeTable(String[,] paramTimeTable)
        {
            int rowCount = paramTimeTable.GetLength(0);
            int columnCount = paramTimeTable.GetLength(1);
            int i;
            DateTime date1 = DateTime.ParseExact("09:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            DateTime date2;

            paramTimeTable[0, 1] = "월";
            paramTimeTable[0, 2] = "화";
            paramTimeTable[0, 3] = "수";
            paramTimeTable[0, 4] = "목";
            paramTimeTable[0, 5] = "금";

            paramTimeTable[0, 0] = "           ";

            for (i = 1; i < rowCount; i++)
            {
                date2 = date1.AddMinutes(30);
                paramTimeTable[i, 0] = date1.ToString("HH:mm") + "-" + date2.ToString("HH:mm");
                date1 = date2;
            }
        }

        public void SetTimeTable(string[,] paramArr, Dictionary<string, string> paramDic)
        {
            string paramString1, paramString2, paramString3, paramString4;
            int rowCount = paramArr.GetLength(0);
            int columnCount = paramArr.GetLength(1);
            int listLength = paramDic.Count();
            int addNum = 0;
            int i = 0, j = 0, k = 0;
            bool isTwoDay1 = false;
            DateTime date1, date2;
            System.TimeSpan diff;
            int diffCount = 0;



            foreach (KeyValuePair<string, string> kv in paramDic)
            {
                diffCount = 0;
                addNum = 0;

                paramString1 = kv.Value.Substring(0, 1);
                paramString2 = kv.Value.Substring(1, 1);

                if (!(paramString2.Equals("0") || paramString2.Equals("1"))) // if tempString2 means day; mind you, two days exist
                {
                    isTwoDay1 = true;
                }
                else //one day exists
                {
                    addNum += 1;
                }

                paramString3 = kv.Value.Substring(2 - addNum, 5);
                paramString4 = kv.Value.Substring(8 - addNum, 5);

                date1 = DateTime.ParseExact(paramString3, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                date2 = DateTime.ParseExact(paramString4, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                diff = date2.Subtract(date1);

                diffCount += Convert.ToInt32(diff.ToString().Substring(1, 1)) * 2;
                diffCount += (diff.ToString().Substring(3, 1).Equals("3")) ? 1 : 0;


                for (i = 0; i < columnCount; i++)
                {
                    if (paramString1.Equals(paramArr[0, i]))
                    {
                        for (j = 0;
                            j < rowCount; j++)
                        {
                            if (paramArr[j, 0].Substring(0, 5).Equals(paramString3))
                            {
                                for (k = 0; k < diffCount; k++)
                                {
                                    paramArr[j + k, i] = kv.Key;
                                }
                                break;
                            }
                        }
                    }
                }

                if (isTwoDay1)
                {
                    for (i = 0; i < columnCount; i++)
                    {
                        if (paramString2.Equals(paramArr[0, i]))
                        {
                            for (j = 0; j < rowCount; j++)
                            {
                                if (paramArr[j, 0].Substring(0, 5).Equals(paramString3))
                                {
                                    for (k = 0; k < diffCount; k++)
                                    {
                                        paramArr[j + k, i] = kv.Key;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

        public void MakeTableFile(User param_has_signed_in_user, string[,] paramTimeTable)
        {
            int i = 0, j;
            int rowCount = paramTimeTable.GetLength(0);
            int columnCount = paramTimeTable.GetLength(1);
            string fileName = null;
            try
            {
                homeService.Clear();
                //First we have to initialize the Excel application Object.
                Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                object misValue = System.Reflection.Missing.Value;


                //Before creating new Excel Workbook, you should check whether Excel is installed in your system.
                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                //Then create new Workbook
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                //Excel.Range exRange;
                xlWorkSheet.Columns[1].ColumnWidth = 10;
                while (i < 5)
                {
                    xlWorkSheet.Columns[i + 2].ColumnWidth = 23;
                    i++;
                }
                xlWorkSheet.Columns[7].ColumnWidth = 47;

                xlWorkSheet.Cells[1, 7] = param_has_signed_in_user.User_id + " " + param_has_signed_in_user.User_name + "의 2015학년도 1학기 수강신청 내역";

                for (i = 0; i < rowCount; i++)
                {
                    for (j = 0; j < columnCount; j++)
                    {
                        xlWorkSheet.Cells[i + 1, j + 1] = paramTimeTable[i, j];
                    }
                }

                //In the above code we write the data in the Sheet1, If you want to write data in sheet 2 then you should code like this..
                /*  xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
                xlWorkSheet.Cells[1, 1] = "Sheet 2 content";    */

                Console.Write("저장할 파일명 입력 : ");
                fileName = Console.ReadLine();

                xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + fileName);

                xlApp.Workbooks.Close();
                xlApp.Quit();
            }
            catch (SystemException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
