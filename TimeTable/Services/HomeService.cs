using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.Models;
using System.Threading;
using TimeTable.Useful_Functions;
using System.Text.RegularExpressions;
using TimeTable.DB;
using System.Data.SqlServerCe;

namespace TimeTable.Services
{
    public class HomeService// 스태틱 ㄴㄴ 비효율. Mind you, Don't use 'static' keyword which is inefficient
    {
        private static HomeService homeService = new HomeService();

        private HomeService()
        {
        }

        public static HomeService GetInstance()
        {
            return homeService;
        }

        JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();
        UsersDBService usersDBService = UsersDBService.GetInstance();


        public void LoginEntry(SqlCeConnection con)
        {
            MainService mainService = MainService.GetInstance();
            while (true)
            {
                User admin_check = null;

                admin_check = StartProgram(con);
                /* 
                 * 
                 * 
                 ***************Sign-In COMPLETE or NOT************************
                 * 
                 * 
                 */

                if (admin_check.User_id == 0) //Exit.
                {
                    break;
                }
                if (admin_check == null)//the id a person typed doesn't exist
                {
                    continue;
                }

                if (admin_check.User_admin == 0)
                {// not an admin user
                    //go to login as an user
                    mainService.UserMenus(new LoggedInUser(admin_check, con));
                    //has_signed_in_user.SetCheck_signed_in(0);
                    Console.WriteLine("안전히 로그아웃되었습니다. ^^ 감사합니다.");
                    usersDBService.ChangeSession(con, admin_check.User_id, 0);
                    Thread.Sleep(500);
                }
                else
                {
                    //admin user sign-in
                    //mainService.AdminMenus(admin_check, con); //before
                    mainService.AdminMenus(new LoggedInUser(admin_check, con));
                    usersDBService.ChangeSession(con, admin_check.User_id, 0);
                }
            }
        }

        public User StartProgram(SqlCeConnection con)
        {
            SignInIntro();
            return Ask_SignUpOrIn(con);
        }

        public User Ask_SignUpOrIn(SqlCeConnection con) //The program asks you whether you wanna sign in or on
        {
            int ask_num = 0;
            User admin_check = null;

            //ASCII code of 1 is 49 and 2's ASCII code is 50
            while (true)
            {
                admin_check = new User();
                Clear();

                HomePage();

                ask_num = jkAppExceptions.GetDigit();

                if (ask_num == 1)
                {
                    Clear();
                    admin_check = SignIn(con);

                    if (admin_check.User_id == 0)
                        continue;
                    else
                        break;

                }
                else if (ask_num == 2)
                {
                    Clear();
                    SignUp(con);
                    continue;
                }
                else if (ask_num == 3)
                {
                    //Exit the program
                    Clear();
                    admin_check.User_id = 0;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return admin_check;
        }

        public User SignIn(SqlCeConnection con) //sign-in
        {
            Password passwdC = new Password();
            User signInForm = new User();
            User userForCheck = null;

            while (true)
            {
                Console.Clear();
                SignInIntro();

                Console.WriteLine("                                 ━━━━");
                Console.WriteLine("                                │로그인│");
                Console.WriteLine("                                 ━━━━");

                Console.Write("학번/아이디 : ");
                signInForm.User_id = jkAppExceptions.GetNumber();

                Console.Write("PASSWORD : ");
                signInForm.User_passwd = passwdC.HidePassword(); //set the password form

                Console.WriteLine();

                if (usersDBService.SearchID(con, signInForm.User_id) == 0)
                {
                    //the id which a person typed doesn't exist.
                    Console.WriteLine("존재하지 않는 학번/아이디 입니다. 학번/아이디를 확인하여주십시오. ");
                    Console.Write("재입력은 1, 뒤로가시려면 아무키나 입력 :");
                    if (Console.ReadLine().Equals("1"))
                        continue;
                    else
                    {
                        userForCheck = new User();
                        break;
                    }
                }
                else if (usersDBService.SearchID(con, signInForm.User_id) == 1)
                {
                    //the id exists in DB
                }
                else
                {
                    continue;
                }

                if (usersDBService.CheckPasswd(signInForm, con) == 1)
                {
                    Console.WriteLine("로그인 하셨습니다.");

                    usersDBService.ChangeSession(con, signInForm.User_id, 1); //kinda session
                    userForCheck = usersDBService.FetchID(con, signInForm.User_id);

                    Thread.Sleep(500);
                    break;
                }
                else
                {
                    // you typed incorrect password. 
                    Console.WriteLine("비밀번호가 올바르지 않습니다. 비밀번호를 확인하여 주십시오. ");

                    Console.Write("재입력은 1, 뒤로가시려면 아무키나 입력 :");
                    if (Console.ReadLine().Equals("1"))
                        continue;
                    else
                    {
                        userForCheck = new User();
                        break;
                    }

                }
            }
            return userForCheck;
        }

        public void SignUp(SqlCeConnection con)
        {
            User signUpForm = new User();
            Password passwdC = new Password();
            string temp_str;
            int temp_int, totalNum;
            Boolean reg1, reg3, reg4, reg5;

            //Regular expressions so that formats could be checked 
            Regex regex1 = new Regex("[가-힣]{2,18}");
            Regex regex3 = new Regex("[a-zA-Z0-9]{8,16}");
            Regex regex4 = new Regex(@"\d{3}-\d{3,4}-\d{4}");
            Regex regex5 = new Regex(@"[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*\.[a-zA-Z]{2,3}");

            //do{
            Console.WriteLine("                                 ━━━━━");
            Console.WriteLine("                                │대학 입학│");
            Console.WriteLine("                                 ━━━━━\n");

            //set your name
            Console.Write("성명(2-18자 이내의 한글) : ");
            temp_str = Console.ReadLine();
            reg1 = regex1.IsMatch(temp_str);
            signUpForm.User_name = temp_str;

            //set your id
            while (true)
            {
                while (true)
                {
                    Console.Write("학번 (6-8자 이내의 숫자) : ");
                    temp_int = jkAppExceptions.GetNumber();
                    totalNum = Convert.ToInt32(Math.Floor(Math.Log10(temp_int) + 1));
                    if (!(totalNum >= 6 && totalNum <= 8))
                    {
                        Console.WriteLine("다시 입력 : ");
                        continue;
                    }
                    break;
                }

                signUpForm.User_id = temp_int;
                if (usersDBService.SearchID(con, temp_int) == 1)
                {
                    //the id you've typed already exists
                    Console.WriteLine("중복 아이디 입니다.");
                    Thread.Sleep(400);
                    continue;
                }
                else
                {
                    //the id you've typed is available 
                    break;
                }
            }

            //set your password
            Console.Write("비밀번호 (8-16자 이내의 대소문자 영문 혹은 숫자) : ");
            temp_str = passwdC.HidePassword();
            reg3 = regex3.IsMatch(temp_str);
            signUpForm.User_passwd = temp_str; //set the password form

            //set your phone number
            Console.Write("휴대폰 번호 (형식 => 010-4242-3843 ) : ");
            temp_str = Console.ReadLine();
            reg4 = regex4.IsMatch(temp_str);
            signUpForm.User_phone = temp_str ;

            //set your e-mail address
            Console.Write("E-MAIL (형식 => junk3843@naver.com) : ");
            temp_str = Console.ReadLine();
            reg5 = regex5.IsMatch(temp_str);
            signUpForm.User_email = temp_str;

            signUpForm.User_admin = 0;
            signUpForm.Course_count = 0;
            signUpForm.Favorite_count = 0;

            Console.WriteLine();
            Console.WriteLine();

            if (reg1 && reg3 && reg4 && reg5)
            {
                //store new user information into User_Database
                usersDBService.InsertID(signUpForm, con);
                Console.WriteLine(" 세종대학교 자유전공학부 입학을 축하합니다. , 2초 후 로그인 화면으로 넘어갑니다.");
                Thread.Sleep(2000);
            }
            else //Fail to sign up
            {
                if (!reg1)
                {
                    Console.Write("[이름]");
                }
                if (!reg3)
                {
                    Console.Write("[비밀번호]");
                }
                if (!reg4)
                {
                    Console.Write("[휴대폰번호]");
                }
                if (!reg5)
                {
                    Console.Write("[이메일 주소]");
                }
                Console.WriteLine();
                Console.WriteLine(" \n양식에 맞게 입력하셔야 합니다. 초기화면으로 돌아갑니다.");
                Thread.Sleep(2000);
            }
        }

        public void HomePage()
        {
            Console.WriteLine("\t\t\t\t\t\t\t초기 화면 \n");
            Console.WriteLine("\t\t\t\t\t\t\t\"1.로그인\" 또는 \"2.세종대 입학하기\"\n");
            Console.WriteLine("\t\t\t\t\t\t\t*학생은 학번, 관리자는 관리자아이디로 로그인이 가능합니다.\n");
            Console.WriteLine("\t\t\t\t\t\t\t1 : Sign in");
            Console.WriteLine("\t\t\t\t\t\t\t2 : Entrance into Sejong University");
            Console.WriteLine("\t\t\t\t\t\t\t3 : Exit \n");
            Console.WriteLine();
            Console.Write("(숫자만 입력)=>");
        }

        public void SignInIntro() //Window GUI
        {
            Console.WriteLine("                                               세종대학교 학사정보시스템에 오신것을 환영합니다.                                         │_│ㅁ│x│");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public void Clear()
        {
            Console.Clear();
            SignInIntro();
        }

    }
}
