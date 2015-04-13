#include <iostream>
#include "Spreadsheet.h"

using namespace std;

string Spreadsheet::GetCellValue(string name)
{
	
}

string Spreadsheet::GetCellContents(string name)
{
	
}

vector<string> Spreadsheet::GetNamesOfAllNonemptyCells()
{
	
}

vector<string> Spreadsheet::SetContentsOfCell (string name, string content)
{
	
}

vector<string> Spreadsheet::GetDirectDependents(string name)
{
	return graph.GetDependents(name);
}

vector<string> Spreadsheet::GetCellsToRecalculate(string name)
{
	vector<string> new_list (1,name);
	return GetCellsToRecalculate(new_list);
}

void Spreadsheet::Save(string filename)
{
	
}

void Spreadsheet::Open(string filename)
{
	
}

vector<string> Spreadsheet::GetCellsToRecalculate(vector<string> names)
{
	
}

void Spreadsheet::Visit(string start, string name, vector<string> visited, vector<string> changed)
{
	
}