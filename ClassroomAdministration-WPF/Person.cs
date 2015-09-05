using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    public class Person
    {
        protected int pid;
        protected string name;

        public int pId { get { return pid; } }
        public string Name { get { return name; } }

        public bool ApplyRent()
        {
            return false;
        }
    }
}
