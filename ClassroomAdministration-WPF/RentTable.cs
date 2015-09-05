using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    class RentTable
    {
        public const int maxClass = 15;
        public  List<Rent> Rents;

        public RentTable(List<Rent> list)
        {
            Rents = list;          
        }

        public List<Rent> GetFromDateClass(DateTime date, int c)
        {
            List<Rent> list = new List<Rent>();
            foreach (Rent r in Rents)
            {
                if (r.Time.Include(date, c)) list.Add(r);
            }
            return list;
        }
        
        //public List<Rent> GetTableFromDate(DateTime date)
        //{
        //    List<Rent> table = new List<Rent>();
        //    List<Rent> unTable = new List<Rent>();

        //    for (int i = 0; i < maxClass; ++i) table.Add(null);

        //    for (int i = 1; i < maxClass; ++i)
        //    {
        //        List<Rent> list = GetFromDateClass(date, i);
        //        if (list.Count == 0) continue;
        //        table[i] = list[0];
                
        //        list.RemoveAt(0);
        //        unTable.AddRange(list);
        //    }

        //    table.AddRange(unTable.Distinct().ToList());

        //    return table;
        //}
        
        public void MoveRentToFirst(int rId)
        {
            Rent r = GetRent(rId);
            if (r == null) return;

            if (Rents.Contains(r)) Rents.Remove(r);
            Rents.Insert(0, r);
        }
        public Rent GetRent(int rId)
        {
            foreach (Rent r in Rents)
                if (r.rId == rId) return r;
            return null;
        }

        public List<Rent> GetClassroom(int cId)
        {
            List<Rent> list = new List<Rent>();
            foreach (Rent r in Rents)
                if (r.cId == cId) list.Add(r);
            return list;
        }
        public int CntRentsInBuilding(int bId)
        {
            int cnt = 0;
            foreach (Rent r in Rents)
                if (bId == Classroom.CId2BId(r.cId)) ++cnt;
            return cnt;

        }

        public string Display()
        {
            string s="";
            foreach (Rent r in Rents)
                if (null != r) s += r.Display() + "\r\n"; else s += "null \r\n";
            return s;
        }

        
    }
}
