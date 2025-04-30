#!/bin/bash

RED='\033[0;31m'
NC='\033[0m'
SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Compile / cleanup C sources
MakeSources() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.C 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "ByteZoo.Blog.C folder not found."
  case $1 in
    "Build")
      make 1> /dev/null
      [ $? != 0 ] && DisplayErrorAndStop "ByteZoo.Blog.C compile failed."
      echo "'$(realpath ./bin/ByteZoo.Blog.C)' compiled (Size = $(wc -c < ./bin/ByteZoo.Blog.C))." ;;
    "Clean")
      make clean 1> /dev/null
      [ $? != 0 ] && DisplayErrorAndStop "ByteZoo.Blog.C cleanup failed."
      echo "ByteZoo.Blog.C cleanup completed." ;;
  esac
  popd 1> /dev/null
}

# Debug commands
DebugCommands() {
  target create ByteZoo.Blog.C                                                                  # Create Target
  breakpoint set --basename main                                                                # Set Breakpoint
  breakpoint set --file ByteZoo.Blog.C.c --line 26                                              # Set Breakpoint
  process launch --stop-at-entry                                                                # Create Process
  f                                                                                             # Display C Source
  image dump symtab ByteZoo.Blog.C                                                              # Display Symbols
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
  build)
    MakeSources "Build" ;;
  clean)
    MakeSources "Clean" ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac