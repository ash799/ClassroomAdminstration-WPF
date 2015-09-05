using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    class Administrator : Person
    {
        public bool ApproveRent(Rent r)
        {
            r.GetApproved();
            return DatabaseLinker.ApproveRent(r);
        }

        public Administrator(int id, string name)
        {
            this.pid = id;
            this.name = name;
        }
    }
}
