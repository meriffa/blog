//-----------------------------------------------------------------------------
// ABI functions
//-----------------------------------------------------------------------------

#include <stdio.h>

/*
 * Call target method
 */
int CallTarget(int a, int b, int c, int d, int e, int f, int g, int h, int i)
{
    int result = 0x00000000;
    result += a;
    result += b;
    result += c;
    result += d;
    result += e;
    result += f;
    result += g;
    result += h;
    result += i;
    return result;
}

/*
 * Execute functions
 */
int ExecuteABI()
{
    int result = CallTarget(0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9);
    printf("Exit    = %11d\n", result);
    return result;
}