using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    public class Rent
    {
        protected int rid;
        protected int cid;// 所在的教室
        protected bool approved;
        protected RentTime time; //占用的时间
        protected int pid;// 教室申请人
        protected string info;// 活动概述

        public int rId { get { return rid; } }
        public int cId { get { return cid; } }// 所在的教室
        public bool Approved { get { return approved; } }
        public RentTime Time { get { return time; } } //占用的时间
        public int pId { get { return pid; } }// 教室申请人
        public string Info { get { return info; } }// 活动概述

   //     public List<int> Students;

        public Rent(int id, string info, int classroomId, int host ,bool approved, RentTime time)
        {
            this.rid = id;
            this.info = info;
            this.cid = classroomId;
            this.approved = approved;
            this.pid = host;
            this.time = time;
        }
        public Rent() { }

        public string Display()
        {


            string s="";
            s += info + "\r\n";
            s += "申请人: " + DatabaseLinker.GetName(pId) + " 编号: R" + rid + "\r\n";

            Classroom c = Building.GetClassroom(cid);
            if (c != null) s += "教室: "+ c.Name+"  ";
            s += "上课时间: " + Time.Display();
           

            //s += " :: ";
            //foreach (int id in Students)
            //{
            //    s += DatabaseLinker.GetName(id)+" ";
            //}

            return s;
        }

        public void GetApproved() { approved = true; }
    }
}
