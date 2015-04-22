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
	sheet.clear();
	cout << "Checkpoint 0.1" << endl;
	sheet["A1"] = "5";
	cout << "Checkpoint 0.2" << endl;
}

Spreadsheet::Spreadsheet(string filename)
{
  ss_name = filename;
	sheet.clear();
	cout << "Checkpoint 0.1" << endl;
	sheet["A1"] = "5";
	cout << "Checkpoint 0.2" << endl;
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

//fix removing from vectur during iteration.
void Spreadsheet::removeUser(int socket)
{
  for(vector<user>::iterator it = userList.begin(); it != userList.end(); ++it) 
    {
      if((*it).getSocket() == socket)
	userList.erase(it);
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
	cout<<"Checkpoint 1"<<endl;
  if(isUndo == false)
    {
	 if(sheet.size() != 0)
	 {
		cout<<"Checkpoint 1.25"<<endl;
		for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
		{
			if(it->first == name)
			{
			copy = it->second;
			vector<string> blankVector;
			cout<<"Checkpoint 1.5"<<endl;
			graph.ReplaceDependents(name, blankVector);
			cout<<"Checkpoint 1.75"<<endl;
			}
		}
	 }
	 cout<<"Checkpoint 1.99"<<endl;
	 cout<<name<<endl;
	 cout<<content<<endl;
	 sheet[name] = content;
	 cout<<"Checkpoint 2"<<endl;
      // Check to see if formula
      if(content[0] == '=')
	{
	  vector<string> variables = getVariables(content);
	  for(int i = 0; i < variables.size(); i++)
	    {
	      graph.AddDependency(name, variables[i]);
	    }
	}
	cout<<"Checkpoint 3"<<endl;
      try
	{
	  GetCellsToRecalculate(name);
	  cell newChange;
	  newChange.name = name;
	  newChange.content = content;
	  undoList.push(newChange);
	  	cout<<"Checkpoint 4"<<endl;
	}
      catch(CircularException e)
	{
			cout<<"Checkpoint 5"<<endl;
	  SetContentsOfCell(name, copy, isUndo);
	  throw e;
	}
    }	
}

vector<string> Spreadsheet::GetDirectDependents(string name)
{
		cout<<"Checkpoint 6"<<endl;
	return graph.GetDependents(name);
}

vector<string> Spreadsheet::GetCellsToRecalculate(string name)
{
	vector<string> new_list (1,name);
		cout<<"Checkpoint 7"<<endl;
	return GetCellsToRecalculate(new_list);
}

void Spreadsheet::Save()
{
  ofstream stream;
  string filename = ss_name = ".txt.";
  stream.open(filename.c_str());
  for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
    {
      stream << it->first << " "<< it->second << "\n";
    }
  stream.close();
	
}

map<string,string> Spreadsheet::Open(string filename)
{
  ss_name = filename;
  ifstream stream;
  string name, contents;
  stream.open(filename.c_str());
  while(stream >> name >> contents)
    {
      SetContentsOfCell(name, contents, false);
    }
  stream.close();

  return sheet;
	
}

vector<string> Spreadsheet::GetCellsToRecalculate(vector<string> names)
{
	vector<string> changed;
	vector<string> visited;
	vector<string> my_return;
		cout<<"Checkpoint 8"<<endl;
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
		cout<<"Checkpoint 9"<<endl;
	//reverse the order of the vector here
	for(int i = changed.size()-1; i >= 0; i--)
	{
		my_return.push_back(changed[i]);
	}
		cout<<"Checkpoint 10"<<endl;
	return my_return;
}

void Spreadsheet::Visit(string start, string name, vector<string>& visited, vector<string>& changed)
{
	visited.push_back(name);
	vector<string> dependents = GetDirectDependents(name);
		cout<<"Checkpoint 11"<<endl;
	for(int i = 0; i < dependents.size(); i++)
	{
	  if(dependents[i] == start)
	    throw CircularException();
	  else
	    Visit(start, dependents[i], visited, changed);
	}
		cout<<"Checkpoint 12"<<endl;
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
		  cout<<"Checkpoint 13"<<endl;
		  int value = atoi((*it).c_str());
		  if(value == 0)
			myReturn.push_back((*it));
		}
    }
  /*for(int i = 0; i < strs.size(); i++)
  {
    if(strs[i] != "0")
      {
	int value = atoi(strs[i].c_str());
	if(value == 0)
	  myReturn.push_back(strs[i]);
      }
      }*/
  return myReturn;
}
