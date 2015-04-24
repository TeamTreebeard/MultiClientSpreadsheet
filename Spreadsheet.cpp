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
	cout<<"SS DEFAULT CONSTRUCTOR"<<endl;
}

Spreadsheet::Spreadsheet(string filename)
{
	ss_name = filename;
	cout<<"SS CONSTRUCTOR"<<endl;
}

Spreadsheet::~Spreadsheet()
{
	cout<<"SS DESTRUCTOR"<<endl;
}

/*
* Copy constructor
*/
Spreadsheet::Spreadsheet (const Spreadsheet & other)
{
	this->ss_name = other.ss_name;
	this->sheet = other.sheet;
	this->undoList = other.undoList;
	this->userList = other.userList;
	this->graph = other.graph;
	cout<<"SS COPY CONSTRUCTOR"<<endl;
}

const Spreadsheet& Spreadsheet::operator= (const Spreadsheet & rhs)
{
	this->ss_name = rhs.ss_name;
	this->sheet = rhs.sheet;
	this->undoList = rhs.undoList;
	this->userList = rhs.userList;
	this->graph = rhs.graph;
	cout<<"SS OP="<<endl;
	return *this;
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

bool Spreadsheet::canUndo()
{
	return !undoList.empty();
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

//removes a user from the spreadsheet
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

bool Spreadsheet::SetContentsOfCell (string name, string content, bool isUndo)
{
  cout<<"Setting contents of Cell"<<endl;
  string copy ="";
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
			 cout << "CircularCheck(" << name << ")" << endl;
			 CircularCheck(name);
			 cell newChange;
			 newChange.name = name;
			 newChange.content = copy;
			 undoList.push(newChange);
			 return false;
		}
		  catch(CircularException e)
		{
		  SetContentsOfCell(name, copy, true);
		  return true;
		}
    }
	else
	{
		vector<string> blankVector;			
		graph.ReplaceDependents(name, blankVector);
	
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
	}
}

void Spreadsheet::CircularCheck(string name)
{
	vector<string> cell_list;
	GetAllDependents(name, cell_list);
	
	for(int i = 0; i < cell_list.size(); i++)
	{
		cout << cell_list[i] << " : Cell List" << endl;
		if(cell_list[i] == name)
		{
			cout << "CircularException Here" << endl;
			throw CircularException();
		}
	}
	
	return;
}

void Spreadsheet::GetAllDependents(string name, vector<string>& cell_list)
{
	vector<string> depend_list;
	depend_list = graph.GetDependents(name);
	bool exists;
	for(int i = 0; i < depend_list.size(); i++)
	{
		exists = false;
		for(int j = 0; j < cell_list.size(); j++)
		{
			if(cell_list[j] == depend_list[i])
			{
				exists = true;
			}
		}
		if(!exists)
		{
			cout << depend_list[i] << " : Depend List" << endl;
			cell_list.push_back(depend_list[i]);
			GetAllDependents(depend_list[i], cell_list);	
		}
	}
	return;
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
      SetContentsOfCell(name, contents, true);
    }
  stream.close();
  return sheet;
	
}

vector<string> Spreadsheet::getVariables(string content)
{
  vector<string> strs;
  vector<string> myReturn;
  boost::split(strs, content, boost::is_any_of("-|+|/|*|="));
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
