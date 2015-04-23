#include <iostream>
#include <locale>
#include <boost/algorithm/string.hpp>
#include <stdlib.h> 
#include <fstream>
#include "Spreadsheet.h"
#include "CircularException.h"

using namespace std;

Spreadsheet::Spreadsheet()
{

}

Spreadsheet::Spreadsheet(string filename)
{
	ss_name = filename;
}

Spreadsheet::~Spreadsheet()
{
	
}

string Spreadsheet::undo()
{
  
  cell lastChange = undoList.top();
  undoList.pop();
  string name = lastChange.name;
  string content = lastChange.content;
  SetContentsOfCell(name, content, true);
  string change = name + " " +  content;
  return change;
}

vector<int> Spreadsheet::getSocketList()
{
	vector<int> userSockets;
	for(int i = 0; i<userList.size(); i++)
	{
		userSockets.push_back(userList[i].getSocket());
	}
	return userSockets;
}

string Spreadsheet::getName()
{
  return ss_name;
}

void Spreadsheet::addUser(user newUser)
{
   userList.push_back(newUser);
}

bool Spreadsheet::containsUser(int ID)
{
    for(vector<user>::iterator it = userList.begin(); it != userList.end(); ++it) 
    {
      if((*it).getSocket() == ID)
		return true;
    }  
    return false;
}

//fix removing from vector during iteration.
void Spreadsheet::removeUser(int socket)
{	
	for(vector<user>::iterator it = userList.begin(); it!=userList.end();)
	{
		if((*it).getSocket() == socket)
		{
				it = userList.erase(it);			
		}
		else
		{
				it++;
		}
	}
}

string Spreadsheet::GetCellContents(string name)
{
  //name = normalize(name);
  for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
    {
      if(it->first == name)
	  return sheet[name];
    }	
  return "";
}

vector<string> Spreadsheet::GetNamesOfAllNonemptyCells()
{
  vector<string> returnVector;
  for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
    {
      returnVector.push_back(it->first);
    }
  return returnVector;
}

map<string,string> Spreadsheet::getSheet()
{
	cout<<"Sheet size in getSheet "<<sheet.size()<<endl;
	return sheet;
}

int Spreadsheet::sheetSize()
{
	return sheet.size();
}

void Spreadsheet::SetContentsOfCell (string name, string content, bool isUndo)
{
 // name = normalize(name);
 // content = normalize(content);
  string copy ="";
  if(name == "")
    {
      vector<string> blankGraph;
      graph.ReplaceDependents(name, blankGraph);
    }
  if(isUndo == false)
    {
	 if(sheet.size() != 0)
	 {
		for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
		{
			if(it->first == name)
			{
			copy = it->second;
			vector<string> blankVector;
			graph.ReplaceDependents(name, blankVector);
			}
		}
	 }
	 sheet[name] = content;
      // Check to see if formula
      if(content[0] == '=')
	{
	  vector<string> variables = getVariables(content);
	  for(int i = 0; i < variables.size(); i++)
	    {
	      graph.AddDependency(name, variables[i]);
	    }
	}
      try
	{
	  GetCellsToRecalculate(name);
	  cell newChange;
	  newChange.name = name;
	  newChange.content = content;
	  undoList.push(newChange);
	}
      catch(CircularException e)
	{
	  SetContentsOfCell(name, copy, isUndo);
	  throw e;
	}
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

void Spreadsheet::Save()
{
  ofstream stream;  
  string filename = ss_name + ".txt";
  stream.open(filename.c_str());
  for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
    {

		string message = it->first + " " + it->second + "\n";
        stream << message;
    }
  stream.close();
	
}

map<string,string>& Spreadsheet::Open(string filename)
{
  string fname=filename+".txt";
  ifstream stream;
  string name, contents;
  stream.open(fname.c_str());
  while(stream >> name >> contents)
    {
	  cout<<"OPENING "<<endl;
	  cout<<name<<" Name for setcontentsofcell"<<endl;
	  cout<<contents<<endl;
      SetContentsOfCell(name, contents, false);
    }
  stream.close();
  cout<<ss_name<<" == "<<filename<<endl;
  cout<<sheet.size()<<endl;
  return sheet;
	
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
	for(int i = changed.size()-1; i >= 0; i--)
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

string Spreadsheet::normalize(string content)
{
  string change;
  locale loc;
  for(int i = 0; i < content.length(); i++)
  {
	  change += toupper(content[i],loc);
  }
  return change;
}

vector<string> Spreadsheet::getVariables(string content)
{
  vector<string> strs;
  vector<string> myReturn;
  boost::split(strs, content, boost::is_any_of("-|+|/|*"));
  for (vector<string>::iterator it = strs.begin(); it != strs.end(); ++it) 
    {
	  if((*it) != "0")
		{
		  int value = atoi((*it).c_str());
		  if(value == 0)
			myReturn.push_back((*it));
		}
    }
  return myReturn;
}
