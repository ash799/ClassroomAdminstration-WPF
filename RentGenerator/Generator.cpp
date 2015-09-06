#include<fstream>
#include<iostream>
#include<string>
#include<ctime>
#include<cstdlib>
using namespace std;

double random(double,double);

int main()
{
	int cId[100], rId[500], ci, ri, c, r;
	string rName[500];
	ifstream infile1("ClassroomList.txt");
	ifstream infile2("RentId_Name.txt");
	ofstream outfile("Rent.txt");
	
	srand(unsigned(time(0)));
	
	if (! infile1 ) {cout<<"Fail to open.";	exit(1);}
	ci=0;
	while (! infile1.eof() )
	{
		string s;
		infile1>>cId[ci];
		cout<<"New CId: "<<cId[ci]<<endl;
		++ci;
	}
	infile1.close();
	
	if (! infile2 ) {cout<<"Fail to open.";	exit(1);}
	ri=0;
	while (! infile2.eof() )
	{
		infile2>>rId[ri];
		infile2>>rName[ri];
		cout<<"New RId: "<<rId[ri]<<rName[ri]<<endl;
		++ri;
	}
	infile2.close();
	
	c=0; r=0;
	
	int d=14;
	while (r<ri)
	{
		int SC[5]={1,3,6,8,10};
		int t = SC[int(random(0,5))];
		
		outfile<<rId[r]<<"\t"<<cId[c]<<"\t1\t0\t"<<rName[r]<<"\t2015-09-"<<d<<"\t2016-01-01\t7\t"<<t<<"\t"<<(t+1)<<endl;
		
		++r;
		++c; if (c==ci){c=0; ++d;}
	}
	outfile.close();
}

double random(double start, double end)
{
    return start+(end-start)*rand()/(RAND_MAX + 1.0);
}

//
//#include <iostream>
//#include <ctime>
//#include <cstdlib>
//using namespace std;
//
//int main()
//{
//    
//    
//    for(int icnt = 0; icnt != 10; ++icnt)
//        cout << "No." << icnt+1 << ": " << int(random(0,10))<< endl;
//    return 0;
//}

