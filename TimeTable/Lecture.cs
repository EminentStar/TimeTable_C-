using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel; //참조추가
using System.Runtime.InteropServices;
using TimeTable.Services;
using TimeTable.Useful_Functions;
using TimeTable.Models;
using System.Data.SqlServerCe;
using TimeTable.DB;
using System.Threading; //참조추가

//the range of lectures belonged to major Computer Science or Digital Contents is From 1846 to 2003

namespace TimeTable
{
    public class Lecture
    {
        HomeService homeService = HomeService.GetInstance();
        EnrollmentDBService enrollmentDBService = EnrollmentDBService.GetInstance();
        UsersDBService usersDBService = UsersDBService.GetInstance();
        JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();

        const int START_NO = 1846;
        const int FINISH_NO = 2003;

        const int LECTURE_NAME = 5; //교과목명
        const int COURSE_CODE = 3; //학수번호
        const int PROFESSOR_NAME = 10; //교수명

        public void ShowListOfLectures(Array paramData, string searchStr, int searchType)
        {
            int rowCount = paramData.GetLength(0); //get the first dimension size
            int columnCount = paramData.GetLength(1); //get the second dimension size
            int i = 1;

            Console.WriteLine("───────────────────────────────────────────────────────────────────────");
            Console.WriteLine("│ NO │학수번호│분반│        교과목명         │이수구분│학점/이론/실습│학년│    주관학과    │교수명│요일 및 강의시간│   강의실    │");
            Console.WriteLine("───────────────────────────────────────────────────────────────────────");
            for (i = 2; i <= rowCount; i++)
            {
                if (((paramData.GetValue(i, 2).Equals("컴퓨터공학과") || paramData.GetValue(i, 2).Equals("디지털콘텐츠학과"))) && paramData.GetValue(i, searchType).ToString().Contains(searchStr))
                {
                    //더러운 코드 죄송합니다........... For handling null data........
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 1) != null) ? paramData.GetValue(i, 1).ToString().PadLeft(4, ' ') : "".PadLeft(4, ' '));
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 3) != null) ? paramData.GetValue(i, 3).ToString().PadLeft(8, ' ') : "".PadLeft(8, ' '));
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 4) != null) ? paramData.GetValue(i, 4).ToString().PadLeft(4, ' ') : "".PadLeft(4, ' '));
                    Console.Write("│");
                    Console.Write(jkAppExceptions.BytesPadLeft(paramData.GetValue(i, 5).ToString(), 25, ' '));
                    Console.Write("│");
                    Console.Write(jkAppExceptions.KoreanPadLeft(paramData.GetValue(i, 6).ToString(), 8, ' '));
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 7) != null) ? paramData.GetValue(i, 7).ToString().PadLeft(14, ' ') : "".PadLeft(14, ' '));
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 8) != null) ? paramData.GetValue(i, 8).ToString().PadLeft(4, ' ') : "".PadLeft(4, ' '));
                    Console.Write("│");
                    Console.Write(jkAppExceptions.KoreanPadLeft(paramData.GetValue(i, 9).ToString(), 16, ' '));
                    Console.Write("│");
                    Console.Write(jkAppExceptions.KoreanPadLeft(paramData.GetValue(i, 10).ToString(), 6, ' '));
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 11) != null) ? jkAppExceptions.BytesPadLeft(paramData.GetValue(i, 11).ToString(), 16, ' ') : "".PadLeft(16, ' '));
                    Console.Write("│");
                    Console.Write((paramData.GetValue(i, 12) != null) ? jkAppExceptions.BytesPadLeft(paramData.GetValue(i, 12).ToString(), 13, ' ') : "".PadLeft(13, ' '));
                    Console.Write("│");
                    Console.WriteLine("\n───────────────────────────────────────────────────────────────────────");
                }
            }
        }

        public void SearchLectures(string paramStr)
        {
            try
            {
                homeService.Clear();

                Array data = ReturnExcelDataArray();

                ShowListOfLectures(data, paramStr, LECTURE_NAME); //print the entire list of the lectures

                Console.Write("아무키나 입력");
                Console.ReadLine();
            }
            catch (SystemException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Array ReturnExcelDataArray()
        {

            //  Excel Application 객체 생성
            Excel.Application ExcelApp = new Excel.Application();

            // Workbook 객체 생성 및 파일 오픈
            Excel.Workbook workbook = ExcelApp.Workbooks.Open(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\TimeTable.xls");

            //sheets에 읽어온 엑셀값을 넣기
            Excel.Sheets sheets = workbook.Sheets;

            // 특정 sheet의 값 가져오기
            Excel.Worksheet worksheet = sheets["강의시간표"] as Excel.Worksheet;

            // 범위 설정
            Excel.Range cellRange = worksheet.get_Range("A1", "M2675") as Excel.Range;

            // 설정한 범위만큼 데이터 담기
            Array data = cellRange.Cells.Value2;

            ExcelApp.Workbooks.Close();
            ExcelApp.Quit();

            return data;
        }

        public void ManageFavoriteLectures(LoggedInUser paramLoggedInUser)
        {
            int choice_Menu = 0;


            while (!(choice_Menu == 3))
            {
                homeService.Clear();

                Console.WriteLine("관심과목 등록 현황\n");

                enrollmentDBService.SelectFavoriteLecture(paramLoggedInUser.Con, paramLoggedInUser.User.User_id);

                Console.WriteLine("                                 1. 관심과목 담기 ");
                Console.WriteLine("                                 2. 관심과목 삭제  ");
                Console.WriteLine("                                 3. 뒤로 가기  ");

                Console.Write("\n\n                                  메뉴 선택 (한자리 숫자만 입력가능) : ");

                choice_Menu = jkAppExceptions.GetDigit();

                switch (choice_Menu)
                {
                    case 1:     //Go to the page that we can search lectures that belong to 'Computer Science' and 'Digital Contents' major
                        RegisterFavoriteLecture(paramLoggedInUser);
                        break;
                    case 2:     //Go to the page you can manage the list of your favorite lectures such as registering or removing.
                        RemoveFavoriteLecture(paramLoggedInUser);
                        break;
                    case 3:    //Go back to the previous page
                        break;
                    default:
                        continue;
                }
            }
        }

        public void SearchLectureWithCondition(int choice, Array data, string tempStr)
        {
            while (!(choice == 1 || choice == 2 || choice == 3))
            {
                homeService.Clear();

                Console.WriteLine("-검색 조건- \n");
                Console.WriteLine("1. 교과목 명");
                Console.WriteLine("2. 학수번호");
                Console.WriteLine("3. 교수명\n");
                Console.Write("선택 : ");

                choice = jkAppExceptions.GetDigit();

                homeService.Clear();

                switch (choice)
                {
                    case 1: //교과목 명으로 검색1
                        Console.Write("교과목 명 : ");
                        tempStr = Console.ReadLine();
                        ShowListOfLectures(data, tempStr, LECTURE_NAME); //print the entire list of 3he lectures
                        break;
                    case 2: //학수번호로 검색
                        Console.Write("학수번호 : ");
                        tempStr = Console.ReadLine();
                        ShowListOfLectures(data, tempStr, COURSE_CODE); //print the entire list of 3he lectures
                        break;
                    case 3: //교수명으로 검색
                        Console.Write("교수명 : ");
                        tempStr = Console.ReadLine();
                        ShowListOfLectures(data, tempStr, PROFESSOR_NAME); //print the entire list of 3he lectures
                        break;
                    default:
                        Console.WriteLine("다시입력");
                        break;
                }
            }
        }

        public void RegisterFavoriteLecture(LoggedInUser paramLoggedInUser)
        {
            const int MAX_F_GRADE_SUM = 25;

            int userID = paramLoggedInUser.User.User_id;
            int tempNum, rowCount, columnCount, grade = 0, totalGrade = 0, i, j;
            int choice = 0;
            string tempStr = null;
            Enrollment enrollment = null;
            Array data = null;

            data = ReturnExcelDataArray();

            rowCount = data.GetLength(0);   //get the first dimension size
            columnCount = data.GetLength(1);    //get the second dimension size

            SearchLectureWithCondition(choice, data, tempStr);

            while (true)
            {
                try
                {
                    Console.Write("담을 관심과목 NO (숫자만 입력가능) : ");
                    tempNum = jkAppExceptions.GetNumber();
                    if (NotInRange(tempNum))
                    {
                        Console.WriteLine("벗어난 NO 입니다.");
                        if (jkAppExceptions.AskReinputOrGoBack())
                            continue;
                        else
                            break;
                    }
                    tempStr = tempNum.ToString();

                    for (i = 2; i < rowCount; i++)
                    {
                        if (data.GetValue(i, 1).Equals(tempStr))
                        {
                            for (j = 1; j < columnCount; j++)
                            {
                                if (data.GetValue(i, j) == null)
                                    data.SetValue("", i, j);
                            }
                            enrollment = new Enrollment(Convert.ToInt32(data.GetValue(i, 1)), paramLoggedInUser.User.User_id,
                                                        data.GetValue(i, 2).ToString(), data.GetValue(i, 3).ToString(),
                                                        data.GetValue(i, 4).ToString(), data.GetValue(i, 5).ToString(),
                                                        data.GetValue(i, 6).ToString(), data.GetValue(i, 7).ToString(),
                                                        Convert.ToInt32(data.GetValue(i, 8)), data.GetValue(i, 9).ToString(),
                                                        data.GetValue(i, 10).ToString(), data.GetValue(i, 11).ToString(),
                                                        data.GetValue(i, 12).ToString());

                            grade = Convert.ToInt32(enrollment.Grade.Substring(0, 1));
                            totalGrade = usersDBService.GetFavoriteCount(paramLoggedInUser.Con, userID);

                            if ((totalGrade + grade) > MAX_F_GRADE_SUM)
                            //If the sum of the grade that you would get after you register the course is over 18, you can't deserve it. 
                            {
                                Console.WriteLine("관심과목 신청 학점이 25학점을 초과할 수 없습니다. ");
                                Thread.Sleep(1500);
                                break;
                            }

                            usersDBService.IncreaseFavoriteCount(paramLoggedInUser.Con, paramLoggedInUser.User.User_id, grade);

                            enrollmentDBService.InsertFavoriteLecture(paramLoggedInUser.Con, enrollment);

                            break;
                        }
                    }
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e.Message);
                }
                break;
            }
        }

        public void RemoveFavoriteLecture(LoggedInUser paramLoggedInUser)
        {
            int tempC_index = 0;
            int tempGrade = 0;
            int userID = paramLoggedInUser.User.User_id;

            //1. print the list of the favorite Lectures
            while (true)
            {
                homeService.Clear();
                Console.WriteLine("\n");
                if (enrollmentDBService.SelectFavoriteLecture(paramLoggedInUser.Con, userID) == 0)
                {
                    Thread.Sleep(1500);
                    break;
                }

                //2. Insert the c_index of the lecture
                Console.Write("삭제할 관심과목(숫자만 입력가능) : ");
                tempC_index = jkAppExceptions.GetNumber();

                //3. remove the lecture
                tempGrade = enrollmentDBService.GetGrade(paramLoggedInUser.Con, userID, tempC_index);
                if (tempGrade == 0)
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }
                enrollmentDBService.DeleteFavoriteLecture(paramLoggedInUser.Con, userID, tempC_index);
                //4. reduce the total number of favorite lecture of the student
                usersDBService.DecreaseFavoriteCount(paramLoggedInUser.Con, userID, tempGrade);
                break;
            }
        }

        public void ManageLectures(LoggedInUser paramLoggedInUser)
        {
            int choice_Menu = 0;


            while (!(choice_Menu == 4))
            {
                homeService.Clear();

                Console.WriteLine("현재 수강신청 현황\n");
                Console.WriteLine("현재 총 수강신청 학점 : {0}", usersDBService.GetCourseCount(paramLoggedInUser.Con, paramLoggedInUser.User.User_id));
                enrollmentDBService.SelectLecture(paramLoggedInUser.Con, paramLoggedInUser.User.User_id);

                Console.WriteLine("                                 1. 관심과목중에서 신청 ");
                Console.WriteLine("                                 2. 과목명으로 검색하여 신청 ");
                Console.WriteLine("                                 3. 수강 삭제");
                Console.WriteLine("                                 4. 뒤로 가기  ");
                Console.Write("\n\n                                  메뉴 선택 (한자리 숫자만 입력가능) : ");

                choice_Menu = jkAppExceptions.GetDigit();

                switch (choice_Menu)
                {
                    case 1:     //Go to the page that you can register lectures among the favorite lecture
                        RegisterLectureByFavorite(paramLoggedInUser);
                        break;
                    case 2:     //Go to the page you can register lectures by searching the entire list 
                        RegisterLectureBySearching(paramLoggedInUser);
                        break;
                    case 3:
                        RemoveLecture(paramLoggedInUser);
                        break;
                    case 4:
                        break;
                    default:
                        continue;
                }
            }
        }

        public void RegisterLectureByFavorite(LoggedInUser paramLoggedInUser)
        {
            int tempInt = 0, grade = 0, totalGrade = 0, userID = paramLoggedInUser.User.User_id;
            const int MAX_GRADE_SUM = 18;

            Enrollment enrollment = null;

            while (true)
            {
                homeService.Clear();
                Console.WriteLine("\n");
                if (enrollmentDBService.SelectFavoriteLecture(paramLoggedInUser.Con, userID) == 0)
                {
                    Thread.Sleep(1500);
                    break;
                }

                //input a number for registering a lecture among the favorite list 
                Console.Write("신청할 강의NO (숫자만 입력가능) : ");
                tempInt = jkAppExceptions.GetNumber();

                //check whether the lecture exists in FavoriteEnrollment DB
                //copy an Enrollment object from FavoriteEnrollment DB
                enrollment = enrollmentDBService.FetchFavoriteEnrollment(paramLoggedInUser.Con, userID, tempInt);

                if (enrollment == null)
                {
                    Console.Write("잘못된 입력입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;

                }

                //check the lecture has already been registered by the student
                if (enrollmentDBService.CheckLectureRegistered(paramLoggedInUser.Con, userID, enrollment.Course_code) != 0)
                {
                    Console.Write("이미 수강신청한 과목입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }

                //*********CHECK_SAME_TIME********************************************************
                if (!IsNotSameTime(paramLoggedInUser.Con, userID, enrollment.Course_time))
                {
                    Console.WriteLine("시간표 중복입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }

                grade = Convert.ToInt32(enrollment.Grade.Substring(0, 1));
                totalGrade = usersDBService.GetCourseCount(paramLoggedInUser.Con, userID);

                if ((totalGrade + grade) > MAX_GRADE_SUM)
                //If the sum of the grade that you would get after you register the course is over 18, you can't deserve it. 
                {
                    Console.WriteLine("수강신청 학점이 18학점을 초과할 수 없습니다. ");
                    Thread.Sleep(1500);
                    break;
                }

                //delete the lecture from FavoriteEnrollment DB
                enrollmentDBService.DeleteFavoriteLecture(paramLoggedInUser.Con, userID, tempInt);

                //subtract the sum of the lecture you've gotten in FavoriteEnrollment

                usersDBService.DecreaseFavoriteCount(paramLoggedInUser.Con, userID, grade);

                //input the lecture into Enrollment DB
                enrollmentDBService.InsertLecture(paramLoggedInUser.Con, enrollment);

                //increase the sum of the lecture you've registered
                usersDBService.IncreaseCourseCount(paramLoggedInUser.Con, userID, grade);
                break;
            }
        }

        public void RegisterLectureBySearching(LoggedInUser paramLoggedInUser)
        {
            int tempInt = 0, grade = 0, totalGrade = 0, rowCount, columnCount, i, j;
            int userID = paramLoggedInUser.User.User_id;
            int choice = 0;
            const int MAX_GRADE_SUM = 18;
            string tempStr = null;
            Enrollment enrollment = null;
            Array data = null;

            data = ReturnExcelDataArray();

            rowCount = data.GetLength(0);   //get the first dimension size
            columnCount = data.GetLength(1);    //get the second dimension size

            SearchLectureWithCondition(choice, data, tempStr);

            //Print the entire list of the favorite lecture
            while (true)
            {

                //input a number for registering a lecture among the favorite list 
                Console.Write("신청할 강의NO (숫자만 입력가능) : ");
                tempInt = jkAppExceptions.GetNumber();
                if (NotInRange(tempInt))
                {
                    Console.WriteLine("벗어난 NO 입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }
                tempStr = tempInt.ToString();
                //check whether the input NO exists in the data array
                for (i = 2; i < rowCount; i++)
                {
                    if (data.GetValue(i, 1).Equals(tempStr))
                    {
                        for (j = 1; j < columnCount; j++)
                        {
                            if (data.GetValue(i, j) == null)
                                data.SetValue("", i, j);
                        }

                        enrollment = new Enrollment(Convert.ToInt32(data.GetValue(i, 1)), paramLoggedInUser.User.User_id,
                                                    data.GetValue(i, 2).ToString(), data.GetValue(i, 3).ToString(),
                                                    data.GetValue(i, 4).ToString(), data.GetValue(i, 5).ToString(),
                                                    data.GetValue(i, 6).ToString(), data.GetValue(i, 7).ToString(),
                                                    Convert.ToInt32(data.GetValue(i, 8)), data.GetValue(i, 9).ToString(),
                                                    data.GetValue(i, 10).ToString(), data.GetValue(i, 11).ToString(),
                                                    data.GetValue(i, 12).ToString());

                        break;
                    }
                }

                if (enrollment == null)
                {
                    Console.Write("잘못된 입력입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }

                //check the lecture has already been registered by the student
                if (enrollmentDBService.CheckLectureRegistered(paramLoggedInUser.Con, userID, enrollment.Course_code) != 0)
                {
                    Console.Write("이미 수강신청한 과목입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }

                //*********CHECK_SAME_TIME********************************************************
                if (!IsNotSameTime(paramLoggedInUser.Con, userID, enrollment.Course_time))
                {
                    Console.WriteLine("시간표 중복입니다.");
                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }

                grade = Convert.ToInt32(enrollment.Grade.Substring(0, 1));
                totalGrade = usersDBService.GetCourseCount(paramLoggedInUser.Con, userID);

                if ((totalGrade + grade) > MAX_GRADE_SUM)
                //If the sum of the grade that you would get after you register the course is over 18, you can't deserve it. 
                {
                    Console.WriteLine("수강신청 학점이 18학점을 초과할 수 없습니다. ");
                    Thread.Sleep(1500);
                    break;
                }
                //input the lecture into Enrollment DB
                enrollmentDBService.InsertLecture(paramLoggedInUser.Con, enrollment);

                //increase the sum of the lecture you've registered
                usersDBService.IncreaseCourseCount(paramLoggedInUser.Con, userID, grade);


                break;
            }
        }

        public bool IsNotSameTime(SqlCeConnection con, int paramUserID, string paramCourseTime)
        {
            //**Warning*** The code of this method might be extremely dirty and too messy to understand context for you

            //Example: 
            //Registerd Lecture1             월수13:00-14:30
            //Lecutre2 being registered      월수13:00-14:00
            /*
             * 
             ***** L1 = Lecture1 , L2 = Lecture2 , S = StartTime, F = FinishTime
             *
             * At least one day is overlapped with the registered lectures
             *  =>
             * L2S <  L1S AND L2F <= L1S  (O)
             * L2S <  L1S AND L2F >  L1S  (X)
             * L2S >  L1S AND L2S >= L1F  (O)
             * L2S >  L1S AND L2S <  L1F  (X)
             * L2S =  L1S                 (X)
             * 
             */

            bool isNotSameTime = true;
            bool isTwoDay1 = false, isTwoDay2 = false;
            int i = 0, j = 0, diffCount = 0;
            string paramStringDay1, paramStringDay2, paramString3, paramString4;
            string DBStringDay1, DBStringDay2, DBString3, DBString4;
            DateTime registeredTime1, registeredTime2, willBeRegisteredTime1, willBeRegisteredTime2;

            //get course_time data from Enrollment DB (let you make the set of the data as list<>)
            List<string> courseTimeList = enrollmentDBService.GetCourse_timeList(con, paramUserID);

            //Separate paramCourseTime string
            paramStringDay1 = paramCourseTime.Substring(0, 1);
            paramStringDay2 = paramCourseTime.Substring(1, 1);

            if (!(paramStringDay2.Equals("0") || paramStringDay2.Equals("1"))) // if tempString2 means day; mind you, two days exist
            {
                isTwoDay1 = true;
            }
            else //one day exists
            {
                i += 1;
            }
            paramString3 = paramCourseTime.Substring(2 - i, 5);
            paramString4 = paramCourseTime.Substring(8 - i, 5);

            willBeRegisteredTime1 = DateTime.ParseExact(paramString3, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            willBeRegisteredTime2 = DateTime.ParseExact(paramString4, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            //check the same day
            foreach (string element in courseTimeList)
            {
                j = 0;
                diffCount = 0;
                DBStringDay1 = element.Substring(0, 1);
                DBStringDay2 = element.Substring(1, 1);

                if (!(DBStringDay2.Equals("0") || DBStringDay2.Equals("1"))) // if tempString2 means day; mind you, two days exist
                {
                    isTwoDay2 = true;
                }
                else //one day exists
                {
                    j += 1;
                }
                DBString3 = element.Substring(2 - j, 5);
                DBString4 = element.Substring(8 - j, 5);
                registeredTime1 = DateTime.ParseExact(DBString3, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                registeredTime2 = DateTime.ParseExact(DBString4, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                diffCount += (!DBStringDay1.Equals(paramStringDay1)) ? 1 : 0;

                if (isTwoDay1) //the lecture will be twice a week
                {
                    if (isTwoDay2)// the registered lecture will be twice a week
                    {

                        diffCount += (!DBStringDay2.Equals(paramStringDay2)) ? 1 : 0;

                        //both lectures are in different days
                        if (diffCount == 2)
                            continue;
                    }
                    else// the registered lecture will be once a week
                    {
                        diffCount += (!DBStringDay1.Equals(paramStringDay2)) ? 1 : 0;
                        //different days
                        if (diffCount == 2)
                            continue;
                    }
                }
                else // isTwoDay1 == false ;the lecture will be once a week
                {
                    if (isTwoDay2)// the registered lecture will be twice a week
                    {
                        diffCount += (!DBStringDay2.Equals(paramStringDay1)) ? 1 : 0;

                        //both lectures are in different days
                        if (diffCount == 2)
                            continue;
                    }
                    else// the registered lecture will be once a week
                    {
                        //different days
                        if (diffCount == 1)
                            continue;
                    }
                }
                //check time overlapping

                //if (((DateTime.Compare(willBeRegisteredTime1, registeredTime1) < 0) && (DateTime.Compare(willBeRegisteredTime2, registeredTime1) <= 0)))
                //{
                //    //you can register this course
                //    isNotSameTime = true;
                //    break;
                //}
                //else if (((DateTime.Compare(willBeRegisteredTime1, registeredTime1) > 0) && (DateTime.Compare(willBeRegisteredTime1, registeredTime2) >= 0)))
                //{
                //    //you can register this course
                //    isNotSameTime = true;
                //    break;
                //}
                if (IsNotOverlapped(willBeRegisteredTime1, willBeRegisteredTime2, registeredTime1, registeredTime2))
                {
                    isNotSameTime = true;
                }
                else
                {
                    //the time is overlapped
                    isNotSameTime = false;
                    break;
                }
            }
            return isNotSameTime;
        }

        public Boolean IsNotOverlapped(DateTime willBeRegisteredTime1, DateTime willBeRegisteredTime2, DateTime registeredTime1, DateTime registeredTime2)
        {
            Boolean isNotSameTime = false;

            if (((DateTime.Compare(willBeRegisteredTime1, registeredTime1) < 0) && (DateTime.Compare(willBeRegisteredTime2, registeredTime1) <= 0)))
            {
                //you can register this course
                isNotSameTime = true;

            }
            else if (((DateTime.Compare(willBeRegisteredTime1, registeredTime1) > 0) && (DateTime.Compare(willBeRegisteredTime1, registeredTime2) >= 0)))
            {
                //you can register this course
                isNotSameTime = true;
            }

            return isNotSameTime;
        }

        public void RemoveLecture(LoggedInUser paramLoggedInUser)
        {
            int userID = paramLoggedInUser.User.User_id;
            int tempNo = 0, grade = 0;
            Enrollment tempEnrollment = null;

            while (true)
            {
                //Print the present registered lectures
                homeService.Clear();
                Console.WriteLine("현재 수강신청 현황\n");
                if (enrollmentDBService.SelectLecture(paramLoggedInUser.Con, userID) == 0)
                {
                    Thread.Sleep(1000);
                    break;
                }

                //input the c_index of the lecture you wanna remove
                Console.Write("삭제할 과목 NO (숫자만 입력가능) : ");
                tempNo = jkAppExceptions.GetNumber();

                //check whether the number you input is in the enrollment DB or not
                tempEnrollment = enrollmentDBService.FetchEnrollment(paramLoggedInUser.Con, userID, tempNo);

                if (tempEnrollment == null)
                {
                    Console.WriteLine("잘못된 입력입니다.");

                    if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
                }
                else
                {
                    //delete the lecture's record in Enrollment DB
                    enrollmentDBService.DeleteLecture(paramLoggedInUser.Con, userID, tempNo);
                    //subtract the sum of the grade you've registered
                    grade = Convert.ToInt32(tempEnrollment.Grade.Substring(0, 1));
                    usersDBService.DecreaseCourseCount(paramLoggedInUser.Con, userID, grade);

                    Console.WriteLine("수강삭제에 성공하였습니다.");
                }
                Thread.Sleep(2000);
                //back to previous page
                break;
            }
        }

        private Boolean NotInRange(int paramNum)
        {
            return paramNum < START_NO || paramNum > FINISH_NO;
        }
    }
}