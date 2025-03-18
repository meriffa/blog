#!/bin/bash

RED='\033[0;31m'
NC='\033[0m'

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Return target process Id
GetTargetProcessId() {
  echo $(ps -ef | grep "dotnet" | grep "$1" | awk '{print $2}')
}

# Trigger GC collect (Compacting Gen2)
TriggerGCCollect() {
  dotnet-trace collect -p $1 --providers Microsoft-Windows-DotNETRuntime:0x800000:4 --duration 00:00:01 --output /tmp/dotnet.nettrace &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop ".Trigger GC collect operation failed (1)."
  rm /tmp/dotnet.nettrace
  [ $? != 0 ] && DisplayErrorAndStop ".Trigger GC collect operation failed (2)."
  echo "Trigger GC collect completed."
}

# Capture .NET Core dump
CaptureDotNetCoreDump() {
  local RESULT=$(createdump -f $2 --full $1)
  echo "$RESULT" | grep "^\[createdump\] Dump successfully written in " &> /dev/null
  echo $(echo "$RESULT" | grep -Po "^\[createdump\] Writing full dump to file \K.*$")
}

# Download symbols
DownloadSymbols() {
  ~/.dotnet/tools/dotnet-symbol -o $2 $1 &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop ".NET Core dump symbols download operation failed."
  echo ".NET Core dump symbols downloaded (Folder = '$2')."
}

# Return memory type text
GetMemoryTypeText() {
  case "${1,,}" in
    "virtual") echo "Virtual (VmSize)" ;;
    "resident") echo "Resident (VmRSS)" ;;
    "residentanonymous") echo "Resident Anonymous (RssAnon)" ;;
    "residentshared") echo "Resident Shared (RssShmem)" ;;
    "swap") echo "Swap (VmSwap)" ;;
    "used") echo "Used (VmRSS + VmSwap)" ;;
    "shared") echo "Shared (RssFile + RssShmem)" ;;
    "code") echo "Code (VmExe)" ;;
    "data") echo "Data (VmData)" ;;
    *) echo "" ;;
  esac
}

# Return memory allocation
GetMemoryAllocation() {
  case "${1,,}" in
    "virtual") [[ -f "/proc/$2/status" ]] && echo $(grep VmSize /proc/$2/status | awk '{print $2}') || echo "" ;;
    "resident") [[ -f "/proc/$2/status" ]] && echo $(grep VmRSS /proc/$2/status | awk '{print $2}') || echo "" ;;
    "residentanonymous") [[ -f "/proc/$2/status" ]] && echo $(grep RssAnon /proc/$2/status | awk '{print $2}') || echo "" ;;
    "residentshared") [[ -f "/proc/$2/status" ]] && echo $(grep RssShmem /proc/$2/status | awk '{print $2}') || echo "" ;;
    "swap") [[ -f "/proc/$2/status" ]] && echo $(grep VmSwap /proc/$2/status | awk '{print $2}') || echo "" ;;
    "used") [[ -f "/proc/$2/status" ]] && echo $(($(grep VmRSS /proc/$2/status | awk '{print $2}') + $(grep VmSwap /proc/$2/status | awk '{print $2}'))) || echo "" ;;
    "shared") [[ -f "/proc/$2/status" ]] && echo $(($(grep RssFile /proc/$2/status | awk '{print $2}') + $(grep RssShmem /proc/$2/status | awk '{print $2}'))) || echo "" ;;
    "code") [[ -f "/proc/$2/status" ]] && echo $(grep VmExe /proc/$2/status | awk '{print $2}') || echo "" ;;
    "data") [[ -f "/proc/$2/status" ]] && echo $(grep VmData /proc/$2/status | awk '{print $2}') || echo "" ;;
    *) echo "" ;;
  esac
}

# Wait for memory allocation threshold
WaitForMemoryAllocationThreshold() {
  local MEMORY_TYPE=$(GetMemoryTypeText $3)
  [[ -z $MEMORY_TYPE ]] && DisplayErrorAndStop "Invalid memory type '$3' specified (Virtual, Resident, ResidentAnonymous, ResidentShared, Swap, Used, Shared, Code, Data)."
  echo "Waiting for memory allocation to reach the specified threshold (PID = $1, $MEMORY_TYPE = $2 KB)."
  while : ; do
    local MEMORY_ALLOCATION=$(GetMemoryAllocation $3 $1)
    if [ "$MEMORY_ALLOCATION" == "" ]; then
      return -1;
    fi
    if (( $MEMORY_ALLOCATION >= $2 )); then
      echo "Memory allocation threshold reached (PID = $1, $MEMORY_TYPE = $MEMORY_ALLOCATION KB)."
      return 0;
    fi
    echo "Current memory allocation (PID = $1, $MEMORY_TYPE = $MEMORY_ALLOCATION KB)."
    sleep $FLAG_INTERVAL
  done
}

# Return CPU utilization
GetCpuUtilization() {
  [[ -f "/proc/$1/status" ]] && echo $(top -b -n 1 -p $1 | tail -1 | awk '{print $9}') || echo ""
}

# Wait for CPU utilization threshold
WaitForCpuUtilizationThreshold() {
  echo "Waiting for CPU utilization to reach the specified threshold (PID = $1, CPU = $2%)."
  while : ; do
    local CPU_UTILIZATION=$(GetCpuUtilization $1)
    if [ "$CPU_UTILIZATION" == "" ]; then
      return -1;
    fi
    if (( $(echo "$CPU_UTILIZATION >= $2" | bc -l) )); then
      echo "CPU utilization threshold reached (PID = $1, CPU = $CPU_UTILIZATION%)."
      return 0;
    fi
    echo "Current CPU utilization (PID = $1, CPU = $CPU_UTILIZATION%)."
    sleep $FLAG_INTERVAL
  done
}

# Get parameters
FLAG_OUTPUT_FILE="./CoreDump_Full.%p"
FLAG_SYMBOLS_FOLDER=~/Symbols
FLAG_MEMORY_THRESHOLD_TYPE="Used"
FLAG_INTERVAL=5
PARSED_ARGUMENTS=$(getopt -q --alternative --options n:,o:,s,f:,g,m:,c: --longoptions name:,output:,symbols,folder:,gc,memory:,memoryType:,cpu:,interval: -- "$@")
eval set -- "$PARSED_ARGUMENTS"
while : ; do
  case "$1" in
    -n | --name) FLAG_TARGET_NAME="$2"; shift 2 ;;
    -o | --output) FLAG_OUTPUT_FILE="$2"; shift 2 ;;
    -s | --symbols) FLAG_SYMBOLS_DOWNLOAD="TRUE"; shift ;;
    -f | --folder) FLAG_SYMBOLS_FOLDER="$2"; shift 2 ;;
    -g | --gc) FLAG_TRIGGER_GC="TRUE"; shift ;;
    -m | --memory) FLAG_MEMORY_THRESHOLD="$2"; shift 2 ;;
    --memoryType) FLAG_MEMORY_THRESHOLD_TYPE="$2"; shift 2 ;;
    -c | --cpu) FLAG_CPU_THRESHOLD="$2"; shift 2 ;;
    --interval) FLAG_INTERVAL="$2"; shift 2 ;;
    --) shift; break ;;
  esac
done

# Script body
[ -z $FLAG_TARGET_NAME ] && DisplayErrorAndStop "Target process name (-n|--name) is not specified."
TARGET_PID=$(GetTargetProcessId $FLAG_TARGET_NAME)
[ -z $TARGET_PID ] && DisplayErrorAndStop "Target .NET Core process name '$FLAG_TARGET_NAME' is not found."
[ ! -z $FLAG_MEMORY_THRESHOLD ] && [ ! -z $FLAG_CPU_THRESHOLD ] && DisplayErrorAndStop "Using both memory allocation and CPU utilization thresholds is not supported."
if [ ! -z $FLAG_MEMORY_THRESHOLD ]; then
  WaitForMemoryAllocationThreshold $TARGET_PID $FLAG_MEMORY_THRESHOLD $FLAG_MEMORY_THRESHOLD_TYPE
  [ $? != 0 ] && DisplayErrorAndStop "Process terminated (PID = $TARGET_PID)."
fi
if [ ! -z $FLAG_CPU_THRESHOLD ]; then
  WaitForCpuUtilizationThreshold $TARGET_PID $FLAG_CPU_THRESHOLD
  [ $? != 0 ] && DisplayErrorAndStop "Process terminated (PID = $TARGET_PID)."
fi
[ ! -z $FLAG_TRIGGER_GC ] && TriggerGCCollect $TARGET_PID
OUTPUT_FILE=$(CaptureDotNetCoreDump $TARGET_PID $FLAG_OUTPUT_FILE)
[ -z $OUTPUT_FILE ] && DisplayErrorAndStop "Capture .NET Core dump operation failed."
echo ".NET Core dump created (PID = $TARGET_PID, File = '$OUTPUT_FILE')."
[ ! -z $FLAG_SYMBOLS_DOWNLOAD ] && DownloadSymbols $OUTPUT_FILE $FLAG_SYMBOLS_FOLDER