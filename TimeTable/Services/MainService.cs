using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeTable.DB;
using TimeTable.Models;
//using ManagementOfBooks.Views;
using TimeTable.Useful_Functions;

namespace TimeTable.Services
{
    public class MainService
    {
        //MainService controlls the entire flow.

        private static MainService mainService = new MainService();

        private MainService()
        {
        }

        public static MainService GetInstance()
        {
            return mainService;
        }

        JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();
        HomeService homeService = HomeService.GetInstance();
        UsersDBService usersDBService = UsersDBService.GetInstance();
        EnrollmentDBService enrollmentDBService = EnrollmentDBService.GetInstance();

        public void StudentMenuIntro()
        {
            Console.WriteLine("                                 ━━━━━━");
            Console.WriteLine("                                │학생 메뉴│");
            Console.WriteLine("                                 ━━━━━━\n");

            Console.WriteLine("1. 강의 출력  \n");
            Console.WriteLine("2. 관심 과목 담기 \n");
            Console.WriteLine("3. 수강 신청  \n");
            Console.WriteLine("4. 수강 신청표  \n");
            Console.WriteLine("5. 시간표 저장  \n");
            Console.WriteLine("6. 로그 아웃  \n");
            Console.WriteLine("7. 자퇴 하기  \n");
            Console.Write("메뉴 선택 : ");
        }

        public void AdminMenuIntro()
        {
            Console.WriteLine("                                                            관리자님 반갑습니다. ");

            Console.WriteLine("                                 ━━━━━━━");
            Console.WriteLine("                                │관리자 메뉴│");
            Console.WriteLine("                                 ━━━━━━━\n");

            Console.WriteLine("1. 학생 목록 ");
            Console.WriteLine("2. 강제 퇴학 ");
            Console.WriteLine("3. 로그아웃  ");

            Console.Write("\n\n메뉴 선택 : ");
        }

        public void UserMenus(SqlCeConnection con, User param_has_signed_in_user)
        {
            int choice_Menu = 0;
            Lecture lecture = new Lecture();
            //HomeService homeService = HomeService.GetInstance();
            string[,] timeTable = null;

            while (!(choice_Menu == 6 || choice_Menu == 7))
            {
                homeService.Clear();

                Console.WriteLine("                                                         " + param_has_signed_in_user.GetUser_name() + " 학생 반갑습니다. ");
                StudentMenuIntro();

                choice_Menu = jkAppExceptions.GetDigit();

                switch (choice_Menu)
                {
                    case 1:     //Go to the page that we can search lectures that belong to 'Computer Science' and 'Digital Contents' major
                        lecture.SearchLectures("");
                        break;
                    case 2:     //Go to the page you can manage the list of your favorite lectures such as registering or removing.
                        lecture.ManageFavoriteLectures(param_has_signed_in_user, con);
                        break;
                    case 3:     //Go to the page which you can manage the list of your real lectures
                        lecture.ManageLectures(param_has_signed_in_user, con);
                        break;
                    case 4:     //go to the Time Table page (you need to follow the format regarding time, day
                        lecture.ShowTimeTable(con, param_has_signed_in_user);
                        break;
                    case 5:     //go to the page you can make your file or add worksheet concerning your time table
                        timeTable = lecture.GetEntireTimeTable(con, param_has_signed_in_user);
                        lecture.MakeTableFile(param_has_signed_in_user, timeTable);
                        break;
                    case 6:     //Let you sign out, guys
                        param_has_signed_in_user.SetCheck_signed_in(0);
                        break;
                    case 7:     //leave University
                        LeaveUniversity(con, param_has_signed_in_user);
                        break;
                    default:
                        continue;
                }
            }
        }

        public void AdminMenus(SqlCeConnection con, User param_has_signed_in_user)
        {

            int choice_Menu = 0;

            while (choice_Menu != 3)
            {
                homeService.Clear();
                AdminMenuIntro();

                choice_Menu = jkAppExceptions.GetDigit();

                switch (choice_Menu)
                {
                    case 1:     //Go to the page admin can see the list of students
                        SeeListOfStudents(con);
                        break;
                    case 2:     //Go to the page admin can delete students
                        RemoveUser(con);
                        break;
                    case 3:     //logout
                        param_has_signed_in_user.SetCheck_signed_in(0);
                        break;
                    default:
                        continue;
                }
            }
        }

        public void SeeListOfStudents(SqlCeConnection con)
        {
            homeService.Clear();
            //Show the entire List of students
            usersDBService.SelectID(con);
            Console.Write("아무 키나 입력 : ");
            Console.ReadLine();
            //Go back to the previous page
        }

        public void LeaveUniversity(SqlCeConnection con, User param_has_signed_in_user)
        {
            string ans = null;
            int userID = param_has_signed_in_user.GetUser_id();
            EnrollmentDBService enrollmentDBService = EnrollmentDBService.GetInstance();
            UsersDBService usersDBService = UsersDBService.GetInstance();

            while (true)
            {
                //re-ask whether you're sure to remove your id
                Console.Write("확실히 학교를 그만두고 싶습니까? (y/n) : ");
                ans = Console.ReadLine();
                if (ans.Equals("y"))
                {
                    //remove all of the lectures you've registered and the favorite lectures
                    enrollmentDBService.DeleteAllLectures(con, userID);
                    enrollmentDBService.DeleteAllFavoriteLectures(con, userID);
                    //remove the ID record from Users DB
                    usersDBService.DeleteUser(con, userID);
                    Console.WriteLine("학교를 자퇴하였습니다.");
                }
                else if (ans.Equals("n"))
                {
                    Console.WriteLine("로그아웃을 하고 마음의 정리를 다시합시다.");
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    continue;
                }
                Thread.Sleep(2000);
                break;
            }
        }

        public void RemoveUser(SqlCeConnection con)
        {
            int remove_id;

            while (true)
            {
                homeService.Clear();

                usersDBService.SelectID(con);

                Console.Write("삭제할 회원 아이디 : ");
                remove_id = jkAppExceptions.GetNumber();
                if (usersDBService.SearchID(con, remove_id) != 0)
                {
                    //remove all of the lectures you've registered and the favorite lectures
                    enrollmentDBService.DeleteAllLectures(con, remove_id);
                    enrollmentDBService.DeleteAllFavoriteLectures(con, remove_id);
                    //remove the ID record from Users DB
                    usersDBService.DeleteUser(con, remove_id);
                    Console.WriteLine("{0} 을/를 퇴학시켰습니다.", remove_id);

                }
                else
                {
                    Console.WriteLine("잘못된 학번입니다.");
                }
                Thread.Sleep(1000);
                if (jkAppExceptions.AskReinputOrGoBack())
                    continue;
                else
                    break;


            }
        }
    }
}
