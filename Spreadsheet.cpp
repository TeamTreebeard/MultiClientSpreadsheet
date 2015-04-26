/*
Filename: Spreadsheet.h
Authors: Kevin Faustino, Christyn Phillippi, Brady Mathews, Brandon Hilton
Last Modified: 4/25/2015

C++ File containing the definitions for the Spreadsheet class. Spreadsheet is used to keep track of cells and contents. It gives functionality
to remove a cell from the spreadsheet and performs checks to make sure no circular dependecies can occur when adding a new cell or 
changing the contents of a cell
 */


#include <iostream>
#include <locale>
#include <boost/algorithm/string.hpp>
#include <stdlib.h> 
#include <fstream>
#include "Spreadsheet.h"
#include "CircularException.h"

using namespace std;

//default constructor
Spreadsheet::Spreadsheet()
{

}

//constructor for spreadsheet which takes the filename given by client
Spreadsheet::Spreadsheet(string filename)
{
	ss_name = filename;

}

//default destructor
Spreadsheet::~Spreadsheet()
{

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
}

//operator= overload for Spreadsheet required by implementing copy constructor
const Spreadsheet& Spreadsheet::operator= (const Spreadsheet & rhs)
{
	this->ss_name = rhs.ss_name;
	this->sheet = rhs.sheet;
	this->undoList = rhs.undoList;
	this->userList = rhs.userList;
	this->graph = rhs.graph;
	return *this;
}

//undos the previous accepted change to the spreadsheet
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

//returns the bool value true if there is something on the undoList that can be undone.
bool Spreadsheet::canUndo()
{
	return !undoList.empty();
}

//returns a vector of the client sockets connected to the spreadsheet
vector<int> Spreadsheet::getSocketList()
{
	vector<int> userSockets;
	for(int i = 0; i<userList.size(); i++)
	{
		userSockets.push_back(userList[i].getSocket());
	}
	return userSockets;
}

//returns a string the name of the spreadsheet
string Spreadsheet::getName()
{
  return ss_name;
}

//adds a user to the spreadsheet
void Spreadsheet::addUser(user newUser)
{
   userList.push_back(newUser);
}

//returns a bool value true if the spreadsheet contains the given user
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

//returns a string of the contents of a given cell
string Spreadsheet::GetCellContents(string name)
{
  for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
    {
      if(it->first == name)
	  return sheet[name];
    }	
  return "";
}

//returns a vector of all of the non-empty cells in the spreadsheet
vector<string> Spreadsheet::GetNamesOfAllNonemptyCells()
{
  vector<string> returnVector;
  for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
    {
      returnVector.push_back(it->first);
    }
  return returnVector;
}

//returns the map with the cell name and contents for the spreadsheet
map<string,string> Spreadsheet::getSheet()
{
	return sheet;
}

//returns a bool true if the contents were set successfully. Returns false if a circulardependency was found.
bool Spreadsheet::SetContentsOfCell (string name, string content, bool isUndo)
{
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

//checks if a circular exception would occur by adding the new cell contents
void Spreadsheet::CircularCheck(string name)
{
	vector<string> cell_list;
	GetAllDependents(name, cell_list);
	
	for(int i = 0; i < cell_list.size(); i++)
	{
		if(cell_list[i] == name)
		{
			throw CircularException();
		}
	}
	
	return;
}

//adds cells to the depend_list
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
			cell_list.push_back(depend_list[i]);
			GetAllDependents(depend_list[i], cell_list);	
		}
	}
	return;
}

//Saves the spreadsheet and stores it into the file system with the server
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

//opens the spreadsheet and returns a map of all the cell names and contents. 
//If no spreadsheet with that name exists, creates a new one.
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

//returns a vector of all the variables in a content string
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
