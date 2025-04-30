//-----------------------------------------------------------------------------
// ByteZoo.Blog.C application
//-----------------------------------------------------------------------------

#include <stdio.h>
#include "./Includes/ABI.h"
#include "./Includes/Intrinsics.h"

/*
 * Display application title
 */
void PrintTitle()
{
    printf("******************************\n");
    printf("* ByteZoo.Blog.C Application *\n");
    printf("******************************\n\n");
}

/*
 * Application entry
 */
int main()
{
    PrintTitle();
    int result = ExecuteABI();
    ExecuteIntrinsics();
    return result;
}