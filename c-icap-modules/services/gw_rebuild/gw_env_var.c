#include <stdlib.h>
#include <string.h> 

/* Return value:  */
/* 1 : target set to variable_name environment value */
/* 0 : target set to specified default value         */
/* -1: invalid input arguments                       */
int set_from_environment_variable_bool(const char *variable_name, int *target, const int default_value)
{
  if (NULL == target)
    return -1;
    
  if (NULL == variable_name)
  {
    goto DEFAULT_ACTION;
  }
  
  const char *environment_value = getenv(variable_name);
  if (NULL == environment_value)
  {
    goto DEFAULT_ACTION;
  }
  
  if (strcmp(environment_value, "true") == 0)
  {
    *target = 1;
    return 1;
  }
  if (strcmp(environment_value, "false") == 0)
  {
    *target = 0;
    return 1;
  }
  
DEFAULT_ACTION:
  *target = default_value;
  return 0;
}