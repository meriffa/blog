//-----------------------------------------------------------------------------
// Intrinsics functions
//-----------------------------------------------------------------------------

#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>

#define __BeginBenchmark(x) x = clock()
#define __EndBenchmark(x, y) y = ((double)(clock() - x)) / CLOCKS_PER_SEC

const int REGION_SIZE = 1000003;

/*
 * Generates random array
 */
int __attribute__((noinline)) * GenerateRegionInt(int length)
{
    int range = rand();
    int *result = malloc(length * sizeof(int));
    for (int i = 0; i < length; i++)
        result[i] = rand() % (2 * range + 1) - range;
    return result;
}

/*
 * Generates random array
 */
char __attribute__((noinline)) * GenerateRegionByte(int length)
{
    char *result = malloc(length * sizeof(char));
    for (int i = 0; i < length; i++)
        result[i] = rand() % 256;
    return result;
}

/*
 * Returns array copy
 */
char __attribute__((noinline)) * CopyRegionByte(char *region, int length)
{
    char *result = malloc(length * sizeof(char));
    for (int i = 0; i < length; i++)
        result[i] = region[i];
    return result;
}

/*
 * Return array minimum (loop)
 */
int __attribute__((noinline)) MinLoop(int *region, int length)
{
    int result = region[0];
    for (int i = 1; i < length; i++)
        if (result > region[i])
            result = region[i];
    return result;
}

/*
 * Return array minimum (AVX2)
 */
extern int MinAvx2_Interop(int *region, int length);

/*
 * Return array maximum (loop)
 */
int __attribute__((noinline)) MaxLoop(int *region, int length)
{
    int result = region[0];
    for (int i = 1; i < length; i++)
        if (result < region[i])
            result = region[i];
    return result;
}

/*
 * Return array maximum (AVX2)
 */
extern int MaxAvx2_Interop(int *region, int length);

/*
 * Return array sum (loop)
 */
int __attribute__((noinline)) SumLoop(int *region, int length)
{
    int result = 0;
    for (int i = 0; i < length; i++)
        result += region[i];
    return result;
}

/*
 * Return array sum (AVX2)
 */
extern int SumAvx2_Interop(int *region, int length);

/*
 * Return item count in array (loop)
 */
int __attribute__((noinline)) CountLoop(int *region, int length, int item)
{
    int result = 0;
    for (int i = 0; i < length; i++)
        if (region[i] == item)
            result++;
    return result;
}

/*
 * Return item count in array (AVX2)
 */
extern int CountAvx2_Interop(int *region, int length, int item);

/*
 * Compare arrays (loop)
 */
bool __attribute__((noinline)) CompareLoop(char *region1, char *region2, int length)
{
    for (int i = 0; i < length; i++)
        if (region1[i] != region2[i])
            return false;
    return true;
}

/*
 * Compare arrays (loop)
 */
extern bool CompareAvx2_Interop(char *region1, char *region2, int length);

/*
 * Execute functions
 */
void ExecuteIntrinsics()
{
    clock_t start;
    double minLoopDuration, minInteropDuration, maxLoopDuration, maxInteropDuration, sumLoopDuration, sumInteropDuration, countLoopDuration, countInteropDuration, matchLoopDuration, matchInteropDuration;
    srand(time(NULL));
    int length = REGION_SIZE;
    int *region = GenerateRegionInt(length);
    char *region1 = GenerateRegionByte(length);
    char *region2 = CopyRegionByte(region1, length);
    __BeginBenchmark(start);
    int minLoop = MinLoop(region, length);
    __EndBenchmark(start, minLoopDuration);
    __BeginBenchmark(start);
    int minInterop = MinAvx2_Interop(region, length);
    __EndBenchmark(start, minInteropDuration);
    __BeginBenchmark(start);
    int maxLoop = MaxLoop(region, length);
    __EndBenchmark(start, maxLoopDuration);
    __BeginBenchmark(start);
    int maxInterop = MaxAvx2_Interop(region, length);
    __EndBenchmark(start, maxInteropDuration);
    __BeginBenchmark(start);
    int sumLoop = SumLoop(region, length);
    __EndBenchmark(start, sumLoopDuration);
    __BeginBenchmark(start);
    int sumInterop = SumAvx2_Interop(region, length);
    __EndBenchmark(start, sumInteropDuration);
    __BeginBenchmark(start);
    int countLoop = CountLoop(region, length, region[length - 1]);
    __EndBenchmark(start, countLoopDuration);
    __BeginBenchmark(start);
    int countInterop = CountAvx2_Interop(region, length, region[length - 1]);
    __EndBenchmark(start, countInteropDuration);
    __BeginBenchmark(start);
    bool matchLoop = CompareLoop(region1, region2, length);
    __EndBenchmark(start, matchLoopDuration);
    __BeginBenchmark(start);
    bool matchInterop = CompareAvx2_Interop(region1, region2, length);
    __EndBenchmark(start, matchInteropDuration);
    printf("Min     = %11d (Loop),         Duration = %f\n", minLoop, minLoopDuration);
    printf("Min     = %11d (AVX2 Interop), Duration = %f\n", minInterop, minInteropDuration);
    printf("Max     = %11d (Loop),         Duration = %f\n", maxLoop, maxLoopDuration);
    printf("Max     = %11d (AVX2 Interop), Duration = %f\n", maxInterop, maxInteropDuration);
    printf("Sum     = %11d (Loop),         Duration = %f\n", sumLoop, sumLoopDuration);
    printf("Sum     = %11d (AVX2 Interop), Duration = %f\n", sumInterop, sumInteropDuration);
    printf("Count   = %11d (Loop),         Duration = %f\n", countLoop, countLoopDuration);
    printf("Count   = %11d (AVX2 Interop), Duration = %f\n", countInterop, countInteropDuration);
    printf("Match   = %11s (Loop),         Duration = %f\n", matchLoop ? "True" : "False", matchLoopDuration);
    printf("Match   = %11s (AVX2 Interop), Duration = %f\n", matchInterop ? "True" : "False", matchInteropDuration);
    free(region);
    free(region1);
    free(region2);
}