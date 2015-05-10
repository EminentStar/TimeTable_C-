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
            this.user_id = param_user.User_id;
            this.user_name = param_user.User_name;
            this.user_passwd = param_user.User_passwd;
            this.user_phone = param_user.User_phone;
            this.user_email = param_user.User_email;
            this.user_admin = param_user.User_admin;
            this.check_signed_in = param_user.Check_signed_in;
        }

        public int User_id
        {
            get { return user_id; }
            set { user_id = value; }
        }
        public string User_name
        {
            get { return user_name; }
            set { user_name = value; }
        }
        public string User_passwd
        {
            get { return user_passwd; }
            set { user_passwd = value; }
        }
        public string User_phone
        {
            get { return user_phone; }
            set { user_phone = value; }
        }
        public string User_email
        {
            get { return user_email; }
            set { user_email = value; }
        }
        public int User_admin
        {
            get { return user_admin; }
            set { user_admin = value; }
        }
        public int Check_signed_in
        {
            get { return check_signed_in; }
            set { check_signed_in = value; }
        }
        public int Course_count
        {
            get { return course_count; }
            set { course_count = value; }
        }
        public int Favorite_count
        {
            get { return favorite_count; }
            set { favorite_count = value; }
        }
    }
}
