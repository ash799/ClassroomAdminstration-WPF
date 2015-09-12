using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    public class Person : RentTableOwner
    {
        protected int pid;
//        protected string name;

        public int pId { get { return pid; } }
        public string Name { get { return name; } }


        public override void GetMyRentTable()
        {
            RentTable = DatabaseLinker.GetPersonRentTable(pid);
        }
        public void DeleteFromMyRentTable(int rId)
        {
            if (DatabaseLinker.DeleteTakepartin(pId, rId))
                RentTable = DatabaseLinker.GetPersonRentTable(pid);
        }
        public void AddToMyRentTable(int rId)
        {
            if (DatabaseLinker.AddTakepartin(pId, rId))
                RentTable = DatabaseLinker.GetPersonRentTable(pid);
        }
    }
}
