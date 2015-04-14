#include <iostream>
#include "Spreadsheet.h"
#include "CircularException.h"

using namespace std;

Spreadsheet::Spreadsheet()
{
}

struct cell{
  string name, content;
};

void Spreadsheet::sendAll(string info)
{
    for(int i = 0; i < userList.size(); i++)
      {
	
      }
}

void Spreadsheet::undo();
{
  cell lastChange = undoList.pop()
  string change = lastChange.name;
  change += lastChange.content;
  sendAll(change);

}

void Spreadsheet::adduser(user newUser);
{
  userList.push_back(newUser);
}

string Spreadsheet::GetCellValue(string name)
{
  
}

string Spreadsheet::GetCellContents(string name)
{
	
}

vector<string> Spreadsheet::GetNamesOfAllNonemptyCells()
{
	
}

vector<string> Spreadsheet::SetContentsOfCell (string name, string content, boolean isUndo)
{
  if(content == null || content == "")
    {
    }
  // else

  if(isUndo == false)
    {
      cell newChange;
      newChange.name = name;
      newChange.content = content;
      undoList.push(newChange);
    }

 
	
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
	vector<string> my_return;
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
	//reverse the order of the vector here
	for(int i = 0; i < changed.size(); i++)
	{
		my_return.push_back(changed[i]);
	}
	return my_return;
}

void Spreadsheet::Visit(string start, string name, vector<string>& visited, vector<string>& changed)
{
	visited.push_back(name);
	vector<string> dependents = GetDirectDependents(name);
	
	for(int i = 0; i < dependents.size(); i++)
	{
		if(dependents[i] == start)
			throw CircularException();
		else
			Visit(start, dependents[i], visited, changed);
	}
	//needs to be push_front
	changed.push_back(name);
}
