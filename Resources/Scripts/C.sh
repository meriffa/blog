#!/bin/bash

RED='\033[0;31m'
NC='\033[0m'
SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Compile C sources
CompileSources() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.C 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "C source code folder not found."
  if [ ! -d ./bin ]; then
    mkdir ./bin
    [ $? != 0 ] && DisplayErrorAndStop "C output folder creation failed."
  fi
  CompileSourceFolder .
  popd 1> /dev/null
}

# Compile C source folder
CompileSourceFolder() {
  if [ "$COMPILE_RELEASE" == "1" ]; then
    gcc ./src/ABI.c -g -o ./bin/ABI -O3
  else
    gcc ./src/ABI.c -g -o ./bin/ABI
  fi
  # gcc ./src/ABI.c -g -o ./bin/ABI -mno-red-zone
  echo "'$(realpath ./bin/ABI)' compiled (Size = $(wc -c < ./bin/ABI))."
}

# Debug commands
DebugCommands() {
  target create ABI                                                                             # Create Target
  breakpoint set --basename call_target                                                         # Set Breakpoint
  breakpoint set --file ABI.c --line 18                                                         # Set Breakpoint
  process launch --stop-at-entry                                                                # Create Process
  f                                                                                             # Display C Source
  image dump symtab ABI                                                                         # Display Symbols
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified."
elif [ -n $1 ]; then
  OPERATION=$( tr '[:upper:]' '[:lower:]' <<<"$1" )
fi

# Get operation parameters
shift
PARSED_ARGUMENTS=$(getopt -q --alternative --options r --longoptions release -- "$@")
eval set -- "$PARSED_ARGUMENTS"
while : ; do
  case "$1" in
    -r | --release) COMPILE_RELEASE="1"; shift 1 ;;
    --) shift; break ;;
  esac
done

# Execute operation
case $OPERATION in
  compile)
    CompileSources ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac