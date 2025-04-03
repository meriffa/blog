#!/bin/bash

RED='\033[0;31m'
NC='\033[0m'
SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Install NASM
InstallNASM() {
  sudo apt-get install nasm -y -qq
  [ $? != 0 ] && DisplayErrorAndStop "NASM install failed.";
  nasm -v | grep -i "NASM version " &> /dev/null;
  [ $? != 0 ] && DisplayErrorAndStop "NASM verification failed.";
  echo "NASM installed.";
}

# Compile assembly sources
CompileSources() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.Asm 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Assembly source code folder not found.";
  if [ ! -d ./bin ]; then
    mkdir ./bin
    [ $? != 0 ] && DisplayErrorAndStop "Assembly output folder creation failed.";
  fi
  CompileSourceFolder .
  popd 1> /dev/null
}

# Compile assembly source folder
CompileSourceFolder() {
  shopt -s nullglob dotglob
  for PathName in "$1"/*; do
    if [ -d "$PathName" ]; then
      CompileSourceFolder "$PathName"
    else
      case "$PathName" in *.asm|*.S)
        local FileName=$(basename "$PathName")
        local Name="${FileName%.*}"
        nasm -f elf64 "$PathName" -i "./inc" -o "./bin/$Name.o"
        [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PathName' compile failed.";
        ld -o "./bin/$Name" "./bin/$Name.o"
        [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PathName' link failed.";
        rm "./bin/$Name.o"
        [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PathName' cleanup failed.";
        if [ "$COMPILE_RELEASE" == "1" ]; then
          strip -s "./bin/$Name"
          [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PathName' strip failed.";
        fi
        local FullName=$(realpath "./bin/$Name")
        local FileSize=$(wc -c < "./bin/$Name")
        echo "'$FullName' compiled (Size = $FileSize)."
      esac
    fi
  done
}

# Clean assembly output
CleanSources() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.Asm 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Assembly source code folder not found.";
  if [ -d ./bin ]; then
    rm -rf ./bin
    [ $? != 0 ] && DisplayErrorAndStop "Assembly output folder cleanup failed.";
  fi
  popd 1> /dev/null
  echo "Assembly output folder cleared.";
}

# Display application details
DisplayApplicationDetails() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.Asm/bin 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Assembly output code folder not found.";
  shopt -s nullglob dotglob
  for PathName in ./*; do
    echo "--- Headers & Symbols ---"
    objdump -x "$PathName"
    echo "--- Section Headers ---"
    readelf -S "$PathName"
    echo "---"
  done
  popd 1> /dev/null
}

# Debug commands
DebugCommands() {
  # Compile
  gcc ABI.c -g -o ABI
  gcc ABI.c -g -o ABI -mno-red-zone
  gcc ABI.c -g -o ABI -O3
  # LLDB
  target create ABI                                                                             # Create Target
  target create ByteZoo.Blog.Asm                                                                # Create Target
  breakpoint set --basename call_target                                                         # Set Breakpoint
  breakpoint set --file ABI.c --line 18                                                         # Set Breakpoint
  breakpoint set --basename _start                                                              # Set Breakpoint
  breakpoint set -a [Address]                                                                   # Set Breakpoint
  process launch --stop-at-entry                                                                # Create Process
  c                                                                                             # Resume Execution
  ni                                                                                            # Instruction single step (over calls)
  si                                                                                            # Instruction single step (into calls)
  disassemble -b                                                                                # Display Assembly
  f                                                                                             # Display C Source
  image dump symtab ABI                                                                         # Display Symbols
  # Shell
  nm --debug-syms ABI                                                                           # Display Symbols
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified.";
elif [ -n $1 ]; then
  OPERATION=$( tr '[:upper:]' '[:lower:]' <<<"$1" );
fi

# Get operation parameters
shift;
PARSED_ARGUMENTS=$(getopt -q --alternative --options r --longoptions release -- "$@");
eval set -- "$PARSED_ARGUMENTS";
while : ; do
  case "$1" in
    -r | --release) COMPILE_RELEASE="1"; shift 1 ;;
    --) shift; break ;;
  esac
done

# Execute operation
case $OPERATION in
  install)
    InstallNASM ;;
  compile)
    CompileSources ;;
  clean)
    CleanSources ;;
  details)
    DisplayApplicationDetails ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac