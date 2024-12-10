#!/bin/bash

RED='\033[0;31m';
NC='\033[0m';
SOLUTION_FOLDER=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )/../.." &> /dev/null && pwd )
SOLUTION_NAME="$(basename $SOLUTION_FOLDER).zip"
TARGET_FOLDER=/media/sf_Downloads

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
}

# Cleanup solution
CleanupSolution() {
  folders=("bin" "obj" "build" "Publish" "TestResults" "Migrations")
  for i in "${folders[@]}"; do
    rm -rf $(find $SOLUTION_FOLDER -name "$i")
    [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CleanupSolution)."
  done
}

# Create archive
CreateArchive() {
  pushd $SOLUTION_FOLDER > /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CreateArchive-1)."
  if [ -e ../$SOLUTION_NAME ]; then
    rm ../$SOLUTION_NAME
    [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CreateArchive-2)."
  fi
  zip -r -q ../$SOLUTION_NAME . -x .git/\*
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CreateArchive-3)."
  popd > /dev/null
}

# Transfer archive
TransferArchive() {
  if [ -e $TARGET_FOLDER/$SOLUTION_NAME ]; then
    rm $TARGET_FOLDER/$SOLUTION_NAME
    [ $? != 0 ] && DisplayErrorAndStop "Operation failed (TransferArchive-1)."
  fi
  mv "$(dirname "$SOLUTION_FOLDER")"/$SOLUTION_NAME $TARGET_FOLDER
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (TransferArchive-2)."
}

CleanupSolution
CreateArchive
TransferArchive
echo "Solution archive '$SOLUTION_NAME' completed."