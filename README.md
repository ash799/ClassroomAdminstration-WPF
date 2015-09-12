
# Person
- int pId
- string Name 姓名
- bool ApplyRent() 申请租用

### Administrator : Person 管理员 
- void ApproveRent() 审批

# User 用户 : Person 
- string Department 专业
- RentTable Schedule 课程表
- bool IsMale 性别

---

# Rent 教室的使用
- int rId 
- int cId 所在教室
- bool Approved 审核状态  
- RentTime Time 占用的时间
- int pId 教室申请人
- string Info 活动概述
- List< int (pId) > Students 所有相关人员 //参加的同学

---

# RentTable 课程列表
- List< Rent >

---

### Building 教学楼
- int bId 
- string Name
- List< int (cId) > Classrooms 教学楼里所有的教室

### Classroom 教室 
- int cId (ClassroomId)
- RentTable RentTable 教室使用情况
- int bId 所在的教学楼

---

# 数据库托管
- public static Person Login(int pId, string tPassword)
- public static Rent GetRent(int rId)
- public static RentTable GetPersonRentTable(int pId)
- public static RentTable GetClassroomRentTable(int cId)
- public static RentTable GetDateRentTable(DateTime date)
- public static string GetName(int pId)
- public static List<int> GetRentPIdList(int rId)
- public static RentTable GetUnapprovedRentTable()
- public static bool SetRent(Rent r)
- public static bool ApproveRent(Rent r)

### MySQL数据库内容

create table person
(
pId 			int not null primary key,
name 			varchar(10) not null,
password 		varchar(32),
department		varchar(20),
sex				char(1)
);

create table rent
(
rId				int(9) not null auto_increment primary key,
cId				int(6) not null,
approved		bool not null,
pId				int,
info			varchar(280),
startDate		date,
endDate			date,
cycDays			int,
startClass		int,
endClass		int
);

create table takePartIn
(
pId				int,
rId				int
);


### 存于客户端的静态数据
- Building.GetBuilding()
- Building.GetClassroom()