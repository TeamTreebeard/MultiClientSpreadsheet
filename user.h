#ifndef DEFINE_USER_H
#define DEFINE_USER_H

#include <string>

class user
{
 public: 
  user(std::string username, int socketIn);
  int getSocket();
  //copy constructor
  user(const user & other);
	
  //operator= required to be implemented by copy constructor
  const user& operator= (const user & rhs);

 private:
  int socket;
  std::string name;
  
};

#endif

