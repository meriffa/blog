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
  [ $? != 0 ] && DisplayErrorAndStop "NASM install failed."
  nasm -v | grep -i "NASM version " &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "NASM verification failed."
  echo "NASM installed."
}

# Compile assembly sources
CompileSources() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.Asm 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Assembly source code folder not found."
  if [ ! -d ./bin ]; then
    mkdir ./bin
    [ $? != 0 ] && DisplayErrorAndStop "Assembly output folder creation failed."
  fi
  CompileSourceFolder .
  popd 1> /dev/null
}

# Return source output type
GetSourceOutputType() {
  case $1 in
    "ByteZoo.Blog.Asm.Application") echo "Application" ;;
    "ByteZoo.Blog.Asm.Library") echo "Library" ;;
    *) echo "Unknown" ;;
  esac
}

# Compile assembly source folder
CompileSourceFolder() {
  shopt -s nullglob dotglob
  for PATH_NAME in "$1"/*; do
    if [ -d "$PATH_NAME" ]; then
      CompileSourceFolder "$PATH_NAME"
    else
      case "$PATH_NAME" in *.asm|*.S)
        local FILE_NAME_AND_EXTENSION=$(basename "$PATH_NAME")
        local NAME_ONLY="${FILE_NAME_AND_EXTENSION%.*}"
        nasm -f elf64 "$PATH_NAME" -i "./inc" -o "./bin/$NAME_ONLY.o"
        [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PATH_NAME' compile failed."
        local OUTPUT_TYPE=$(GetSourceOutputType $NAME_ONLY)
        case $OUTPUT_TYPE in
          "Application")
            local OUTPUT_FILE_NAME="./bin/$NAME_ONLY"
            ld -o $OUTPUT_FILE_NAME "./bin/$NAME_ONLY.o"
            [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PATH_NAME' link failed." ;;
          "Library")
            local OUTPUT_FILE_NAME="./bin/$NAME_ONLY.so"
            ld -shared -o $OUTPUT_FILE_NAME "./bin/$NAME_ONLY.o"
            [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PATH_NAME' link failed." ;;
          *) DisplayErrorAndStop "Unknown assembly '$PATH_NAME' output type." ;;
        esac
        rm "./bin/$NAME_ONLY.o"
        [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PATH_NAME' cleanup failed."
        if [ "$COMPILE_RELEASE" == "1" ]; then
          strip -s $OUTPUT_FILE_NAME
          [ $? != 0 ] && DisplayErrorAndStop "Assembly '$PATH_NAME' strip failed."
        fi
        echo "'$(realpath $OUTPUT_FILE_NAME)' compiled (Type = $OUTPUT_TYPE, Size = $(wc -c < $OUTPUT_FILE_NAME))."
      esac
    fi
  done
}

# Clean assembly output
CleanSources() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.Asm 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Assembly source code folder not found."
  if [ -d ./bin ]; then
    rm -rf ./bin
    [ $? != 0 ] && DisplayErrorAndStop "Assembly output folder cleanup failed."
  fi
  popd 1> /dev/null
  echo "Assembly output folder cleared."
}

# Display application details
DisplayApplicationDetails() {
  pushd $SOLUTION_FOLDER/Sources/ByteZoo.Blog.Asm/bin 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Assembly output code folder not found."
  shopt -s nullglob dotglob
  for PATH_NAME in ./*; do
    echo "--- Export Symbols ---"
    objdump -T "$PATH_NAME"
    nm -D "$PATH_NAME"
    echo "--- Headers & Symbols ---"
    objdump -x "$PATH_NAME"
    echo "--- Section Headers ---"
    readelf -S "$PATH_NAME"
    echo "--- Debug Symbols ---"
    nm --debug-syms "$PATH_NAME"
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
  target create ByteZoo.Blog.Asm.Application                                                    # Create Target
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