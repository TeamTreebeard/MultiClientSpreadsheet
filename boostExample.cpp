/*
Example code of using boost regex split function to split a string at the given
regex delimiters and store in a vector
*/
#include<boost/algorithm/string.hpp>
#include<iostream>
#include<string>
#include<stdlib.h>
#include<vector>

using namespace std;

int main()
{
	string test = "5-7";
	vector<string> strs;
	boost::split(strs, test, boost::is_any_of("-"));
	for(int i = 0; i < strs.size(); i++)
	{
		cout<< strs[i] << endl;
	}
	
	return 0;
}
