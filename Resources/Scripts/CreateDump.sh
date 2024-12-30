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

# Capture .NET Core dump
CaptureDotNetCoreDump() {
  local RESULT=$(createdump -f $2 --full $1)
  echo "$RESULT" | grep "^\[createdump\] Dump successfully written in " &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Capture .NET Core dump operation failed."
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
PARSED_ARGUMENTS=$(getopt -q --alternative --options n:,o:,s: --longoptions name:,output:,symbols: -- "$@")
eval set -- "$PARSED_ARGUMENTS"
while : ; do
  case "$1" in
    -n | --name) FLAG_TARGET_NAME="$2"; shift 2 ;;
    -o | --output) FLAG_OUTPUT_FILE="$2"; shift 2 ;;
    -s | --symbols) FLAG_SYMBOLS_FOLDER="$2"; shift 2 ;;
    --) shift; break ;;
  esac
done

# Script body
if [ ! -z $FLAG_TARGET_NAME ]; then
  TARGET_PID=$(GetTargetProcessId $FLAG_TARGET_NAME)
  [[ -z $TARGET_PID ]] && DisplayErrorAndStop "Target .NET Core process name '$FLAG_TARGET_NAME' is not found.";
  OUTPUT_FILE=$(CaptureDotNetCoreDump $TARGET_PID $FLAG_OUTPUT_FILE)
  echo ".NET Core dump created (PID = $TARGET_PID, File = '$OUTPUT_FILE')."
  DownloadSymbols $OUTPUT_FILE $FLAG_SYMBOLS_FOLDER
else
  DisplayErrorAndStop "Target process name (--name) is not specified."
fi