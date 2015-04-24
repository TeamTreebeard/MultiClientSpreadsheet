#include<iostream>
#include<regex>

using namespace std;

int main()
{
	cout<<"i am doing stuff"<<endl;
	if(regex_match("A5", regex("[A-Z][1-9][0-9]{0,1}"))))
	{
		cout<<"true"<<endl;
	}
	else
	{
		cout<<"false"<<endl;
	}
	
	return 0;
}