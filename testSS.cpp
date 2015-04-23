#include <iostream>
#include "Spreadsheet.h"
#include "CircularException.h"

using namespace std;

int main()
{
string filename = "testingSS.txt";
Spreadsheet SS("hola");
SS.Open(filename);
cout<<"sheet size after open should be 5 == "<<SS.sheetSize()<<endl;
SS.Save();
cout<<"sheet size after open should be 5 == "<<SS.sheetSize()<<endl;
SS.Open(filename);
cout<<"sheet size after open should be 5 == "<<SS.sheetSize()<<endl;
SS.SetContentsOfCell("C1", "5", false);
SS.SetContentsOfCell("C2", "4", false);
SS.SetContentsOfCell("C3", "3", false);
SS.SetContentsOfCell("C4", "2", false);
SS.SetContentsOfCell("C5", "1", false);
cout<<"sheet size before save should be 10 "<<SS.sheetSize()<<endl;
SS.Save();
cout<<"sheet size "<<SS.sheetSize()<<endl;

return 0;
}