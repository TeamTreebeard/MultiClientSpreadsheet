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
	vector<string> changed;
	vector<string> visited;
	for(int i = 0; i < names.size(); i++)
	{
		bool is_visited = false;
		for(int j = 0; j < visited.size(); j++)
		{
			if(names[i] == visited[j])
				is_visited = true;
		}
		
		if(!is_visited)
			Visit(names[i], names[i], visited, changed);
	}
	return changed;
}

void Spreadsheet::Visit(string start, string name, vector<string>& visited, vector<string>& changed)
{
	visited.push_back(name);
	vector<string> dependents = GetDirectDependents(name);
	
	for(int i = 0; i < dependents.size(); i++)
	{
		if(dependents[i] == start)
		{}
			//throw exception here
		else
			Visit(start, dependents[i], visited, changed);
	}
	//needs to be push_front
	changed.push_back(name);
}