using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.Models;

namespace TimeTable
{
    /*
     * This class is used for binding User and SqlCeConnection parameters 
     * when both of them is used at the same time.
     */
    public class LoggedInUser
    {
        private readonly SqlCeConnection con;
        private readonly User user;

        public LoggedInUser(User paramUser, SqlCeConnection paramCon)
        {
            this.user = paramUser;
            this.con = paramCon;
        }

        public SqlCeConnection Con
        {
            get { return con; }
        }
        public User User
        {
            get { return user; }
        }
    }
}
