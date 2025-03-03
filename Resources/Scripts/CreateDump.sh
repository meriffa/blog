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

# Get parameters
FLAG_OUTPUT_FILE="./CoreDump_Full.%p"
FLAG_SYMBOLS_FOLDER=~/Symbols
PARSED_ARGUMENTS=$(getopt -q --alternative --options n:,o:,s,f:,c --longoptions name:,output:,symbols,folder:,gc -- "$@")
eval set -- "$PARSED_ARGUMENTS"
while : ; do
  case "$1" in
    -n | --name) FLAG_TARGET_NAME="$2"; shift 2 ;;
    -o | --output) FLAG_OUTPUT_FILE="$2"; shift 2 ;;
    -s | --symbols) FLAG_SYMBOLS_DOWNLOAD="TRUE"; shift ;;
    -f | --folder) FLAG_SYMBOLS_FOLDER="$2"; shift 2 ;;
    -c | --gc) FLAG_TRIGGER_GC="TRUE"; shift ;;
    --) shift; break ;;
  esac
done

# Script body
[ -z $FLAG_TARGET_NAME ] && DisplayErrorAndStop "Target process name (-n|--name) is not specified."
TARGET_PID=$(GetTargetProcessId $FLAG_TARGET_NAME)
[ -z $TARGET_PID ] && DisplayErrorAndStop "Target .NET Core process name '$FLAG_TARGET_NAME' is not found."

[ ! -z $FLAG_TRIGGER_GC ] && TriggerGCCollect $TARGET_PID

OUTPUT_FILE=$(CaptureDotNetCoreDump $TARGET_PID $FLAG_OUTPUT_FILE)
[ -z $OUTPUT_FILE ] && DisplayErrorAndStop "Capture .NET Core dump operation failed."
echo ".NET Core dump created (PID = $TARGET_PID, File = '$OUTPUT_FILE')."
[ ! -z $FLAG_SYMBOLS_DOWNLOAD ] && DownloadSymbols $OUTPUT_FILE $FLAG_SYMBOLS_FOLDER