#include <iostream>
#include "Spreadsheet.h"

using namespace std;

bool Spreadsheet::IsValid(string word)
{
	
}

string Spreadsheet::Normalize(string word)
{
	
}

string Spreadsheet::GetCellValue(stirng name)
{
	
}

string Spreadsheet::GetCellContents(string name)
{
	
}

vector<string> Spreadsheet::GetNamesOfAllNonemptyCells();
{
	
}

vector<string> Spreadsheet::SetContentsOfCell (string name, string content)
{
	
}

vector<string> Spreadsheet::GetDirectDependents(string name)
{
	
}

vector<string> Spreadsheet::GetCellsToRecalculate(string name)
{
	
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