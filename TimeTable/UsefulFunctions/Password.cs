using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.Models;

namespace TimeTable.Useful_Functions
{
    public class Password
    {
        public string HidePassword()
        {
            ConsoleKeyInfo key; /*this structure describes the console key pressed, 
                                 * including the character represented by the console key 
                                 * and the state of the SHIFT, ALT,and CTRL modifier keys  */
            string pass = "";
            do
            {
                key = Console.ReadKey(true); /*obtaining the next character or function key 
                pressed by the user. The pressed key is optionally displayed in the console window.  */

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            
            Console.WriteLine();

            return pass;
        }

        public Boolean CheckPassword(User param_has_signed_in_user)
        {
            string check_password01, check_password02;
            Boolean check_same_passwd = false ;
            Boolean check_correct_passwd = false ;

            Console.Write("     비밀번호 입력 : ");
            check_password01 = HidePassword();
            Console.Write("     비밀번호 확인 : ");
            check_password02 = HidePassword();
            check_same_passwd = check_password01.Equals(check_password02);

            if (check_same_passwd)
            {
                check_correct_passwd = param_has_signed_in_user.User_passwd.Equals( check_password01 );
                if (check_correct_passwd)
                {
                    Console.WriteLine("비밀번호가 일치합니다.");
                }
                else
                {
                    Console.WriteLine("비밀번호가 일치하지 않습니다.");
                }
            }
            else
            {
                Console.WriteLine("비밀번호를 올바르게 입력해주세요.");
            }
            return check_correct_passwd ;
        }
    }
}
