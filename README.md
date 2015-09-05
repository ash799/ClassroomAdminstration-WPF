此为WPF版,原WinForm项目是 https://github.com/sugar10w/ClassroomAdministration.git 

# Person
- int pId
- string Name 姓名
- bool ApplyRent() 申请租用

### Administrator : Person 管理员 
- void ApproveRent() 审批

# *User 用户* : Person //, IRentTableControl
- string Department 专业
- RentTable Schedule 课程表
- bool IsMale 性别

以下两个类暂时架空:
### //Student : User 学生
### //Teacher : User 老师
到目前为止,Student和Teacher的区别只体现在UI上。老师默认申请Course,也可以申请Activity;学生只能申请Activity。

---

# Rent 教室的使用
- int rId 
- int cId 所在教室
- bool Approved 审核状态  
- RentTime Time 占用的时间
- int pId 教室申请人
- string Info 活动概述
- List< int (pId) > Students 所有相关人员 //参加的同学

以下两个类暂时架空:
### //Activity : Rent 活动
### //Course : Rent 课程
到目前为止,Activity和Course的区别只体现在Time时间上。Activity是一次性的,Course是循环的。

---
以下一个接口暂时架空:
# //IRentTableControl
- bool AddRentTable();
- bool DeleteRentTable();

# RentTable 课程表
- List< Rent >

---

### Building 教学楼
- int bId 
- string Name
- List< int (cId) > Classrooms 教学楼里所有的教室

### Classroom 教室 //: IRentControl
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

### 数据库内容

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