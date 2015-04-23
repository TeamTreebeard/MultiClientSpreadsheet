#include "DependencyGraph.h"
#include <iostream>
using namespace std;

Node::Node()
{
	
}
Node::~Node()
{
	
}

Node::Node (const Node & other)
{
	this->dependents = other.dependents;
	this->dependees = other.dependees;
}

const Node& Node::operator= (const Node & rhs)
{
	this->dependents = rhs.dependents;
	this->dependees = rhs.dependees;
	return *this;
}

void Node::AddDependents(string s)
{
	bool exists = false;
	for(int i = 0; i < dependents.size(); i++)
	{
		if (dependents[i] == s)
		{
			exists = true;
		}
	}
	if(!exists)
	{
		dependents.push_back(s);
	}
}
void Node::AddDependees(string s)
{
	bool exists = false;
	for(int i = 0; i < dependees.size(); i++)
	{
		if (dependees[i] == s)
		{
			exists = true;
		}
	}
	if(!exists)
	{
		dependees.push_back(s);
	}
}
vector<string> Node::GetDependents()
{
	return dependents;
}
vector<string> Node::GetDependees()
{
	return dependees;
}
void Node::RemoveDependents(string s)
{
	for(int i = 0; i < dependents.size(); i++)
	{
		if(dependents[i] == s)
		{
			dependents.erase(dependents.begin()+i);
			break;
		}
	}
}
void Node::RemoveDependees(string s)
{
	for(int i = 0; i < dependees.size(); i++)
	{
		if(dependees[i] == s)
		{
			dependees.erase(dependees.begin()+i);
			break;
		}
	}
}
bool Node::HasDependents()
{
	if(dependents.size() > 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}
bool Node::HasDependees()
{
	if(dependees.size() > 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}

DependencyGraph::DependencyGraph()
{
	Size = 0;
	cout<<"DG CONSTRUCTOR"<<endl;
}
DependencyGraph::~DependencyGraph()
{
	
}

DependencyGraph::DependencyGraph (const DependencyGraph & other)
{
	this->Size = other.Size;
	this->node = other.node;
	this->lookup = other.lookup;
	this->pivot = other.pivot;
	cout<<"DG COPY CONSTRUCTOR"<<endl;
}

const DependencyGraph& DependencyGraph::operator= (const DependencyGraph & rhs)
{
	this->Size = rhs.Size;
	this->node = rhs.node;
	this->lookup = rhs.lookup;
	this->pivot = rhs.pivot;
	cout<<"DG OP= CONSTRUCTOR"<<endl;
	return *this;
}
int DependencyGraph::get_Size()
{
	return Size;
}
bool DependencyGraph::HasDependents(string s)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		return node->HasDependents();
	}
	else
	{
		return false;
	}
}
bool DependencyGraph::HasDependees(string s)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		return node->HasDependees();
	}
	else
	{
		return false;
	}
}
vector<string> DependencyGraph::GetDependents(string s)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		pivot = node->GetDependents();
		return pivot;
	}
	else
	{
		vector<string> blank_vector;
		return blank_vector;
	}
}
vector<string> DependencyGraph::GetDependees(string s)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		pivot = node->GetDependees();
		return pivot;
	}
	else
	{
		vector<string> blank_vector;
		return blank_vector;
	}
}
void DependencyGraph::AddDependency(string s, string t)
{
	//Check to see if cell s exists
	if(ContainsKey(s))
	{
		node = &lookup[s];
		pivot = node->GetDependents();
		for(int i = 0; i < pivot.size(); i++)
		{
			if(pivot[i] == t)
			{
				//if the pairing exists already, don't add it again
				//this avoids increasing the size falsely
				return;
			}
		}
	}
	//If the cell exists but the pairing does not exist, add it
	node = new Node();
	node->AddDependents(t);
	lookup[s] = *node;
	node = &lookup[s];
	node->AddDependents(t);
	
	//Check to see if the cell t exists and add the pairing
	if(!ContainsKey(t))
	{
		node = new Node();
		node->AddDependees(s);
		lookup[t] = *node;
	}
	else
	{
		node = &lookup[t];
		node->AddDependees(s);
	}
	Size++;
}
void DependencyGraph::RemoveDependency(string s, string t)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		pivot = node->GetDependents();
		bool found = false;
		for(int i = 0; i < pivot.size(); i++)
		{
			if(pivot[i] == t)
			{
				found = true;
				break;
			}
		}	
		
		if(found)
		{
			node->RemoveDependents(t);
			node = &lookup[t];
			node->RemoveDependees(s);
			Size--;
		}
	}
}
void DependencyGraph::ReplaceDependents(string s, vector<string> newDependents)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		pivot = node->GetDependents();
		vector<string> copy = pivot;
		for(int i = 0; i < copy.size(); i++)
		{
			RemoveDependency(s, copy[i]);
		}
		for(int i = 0; i < newDependents.size(); i++)
		{
			AddDependency(s, newDependents[i]);
		}
	}
}
void DependencyGraph::ReplaceDependees(string s, vector<string> newDependees)
{
	if(ContainsKey(s))
	{
		node = &lookup[s];
		pivot = node->GetDependees();
		vector<string> copy = pivot;
		for(int i = 0; i < copy.size(); i++)
		{
			RemoveDependency(s, copy[i]);
		}
		for(int i = 0; i < newDependees.size(); i++)
		{
			AddDependency(s, newDependees[i]);
		}
	}	
}

bool DependencyGraph::ContainsKey(string s)
{
	for(map<string,Node>::iterator it = lookup.begin(); it != lookup.end(); it++)
	{
		if((it->first) == s)
		{
			return true;
		}
	}
	return false;
}
