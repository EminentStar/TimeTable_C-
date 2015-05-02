using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTable.Models
{
    /* WARNING : You gotta declare the class, 
     * which is included List<> parameters, as public.
     * if not, you cannot use that data format as a parameter. 
     * (Inconsistent accessibility)
     */
    public class User
    {
        private int user_id;
        private string user_name;
        private string user_passwd;
        private string user_phone;
        private string user_email;
        private int user_admin;
        private int check_signed_in;
        private int course_count;
        private int favorite_count;

        public User()
        {
        }

        public User(int param_user_id, string param_user_passwd, int param_user_admin)
        {
            this.user_id = param_user_id;
            this.user_passwd = param_user_passwd;
            this.user_admin = param_user_admin;
        }
        public User(int param_user_id, string param_user_name,
                    string param_user_passwd, int param_user_admin)
        {
            this.user_id = param_user_id;
            this.user_name = param_user_name;
            this.user_passwd = param_user_passwd;
            this.user_admin = param_user_admin;
        }
        public User(int param_user_id, string param_user_name,
                    string param_user_passwd, string param_user_phone,
                    string param_user_email, int param_user_admin)
        {
            this.user_id = param_user_id;
            this.user_name = param_user_name;
            this.user_passwd = param_user_passwd;
            this.user_phone = param_user_phone;
            this.user_email = param_user_email;
            this.user_admin = param_user_admin;
        }
        public User(User param_user)
        {
            this.user_id = param_user.GetUser_id();
            this.user_name = param_user.GetUser_name();
            this.user_passwd = param_user.GetUser_passwd();
            this.user_phone = param_user.GetUser_phone();
            this.user_email = param_user.GetUser_email();
            this.user_admin = param_user.GetUser_admin();
            this.check_signed_in = param_user.GetCheck_signed_in();
        }

        public int GetUser_id()
        {
            return user_id;
        }
        public string GetUser_name()
        {
            return user_name;
        }
        public string GetUser_passwd()
        {
            return user_passwd;
        }
        public string GetUser_phone()
        {
            return user_phone;
        }
        public string GetUser_email()
        {
            return user_email;
        }
        public int GetUser_admin()
        {
            return user_admin;
        }
        public int GetCheck_signed_in()
        {
            return check_signed_in;
        }
        public int GetCourse_count()
        {
            return course_count;
        }
        public int GetFavorite_count()
        {
            return favorite_count;
        }

        public void SetUser_id(int param_id)
        {
            this.user_id = param_id;
        }

        public void SetUser_name(string param_name)
        {
            this.user_name = param_name;
        }
        public void SetUser_passwd(string param_passwd)
        {
            this.user_passwd = param_passwd;
        }
        public void SetUser_phone(string param_phone)
        {
            this.user_phone = param_phone;
        }
        public void SetUser_email(string param_email)
        {
            this.user_email = param_email;
        }
        public void SetUser_admin(int param_admin)
        {
            this.user_admin = param_admin;
        }

        public void SetCheck_signed_in(int param_check_signed_in)
        {
            this.check_signed_in = param_check_signed_in;
        }
        public void SetCourse_count(int param_course_count)
        {
            this.course_count = param_course_count;
        }
        public void SetFavorite_count(int param_favorite_count)
        {
            this.favorite_count = param_favorite_count;
        }

    }
}
