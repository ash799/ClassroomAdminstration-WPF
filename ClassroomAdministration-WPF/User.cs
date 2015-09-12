using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    class User : Person //, IRentTableControl
    {

        protected string department; // 专业
        protected char sex;

        public string Department { get { return department;  } }
        public bool IsMale { get { return sex != 'f'; } }

        public User(int id, string nm, char sx = 'm', string dpt = "???")
        {
            this.pid = id;
            this.name = nm;
            this.sex = sx;
            this.department = dpt;
        }

    }
}
