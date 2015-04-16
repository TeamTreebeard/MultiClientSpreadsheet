#ifndef DEFINE_USER_H
#define DEFINE_USER_H

#include <string>

class user
{
 public: 
  user(std::string username, int socketIn);
  int getSocket();

 private:
  int socket;
  std::string name;
  
};

#endif

