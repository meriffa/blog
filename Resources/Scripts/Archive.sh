#!/bin/bash

RED='\033[0;31m';
NC='\033[0m';
SOLUTION_FOLDER=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )/../.." &> /dev/null && pwd )
SOLUTION_NAME="$(basename $SOLUTION_FOLDER)"
ARCHIVE_NAME="$(basename $SOLUTION_FOLDER).tar.gz"
ARCHIVE_PATH="$(dirname $SOLUTION_FOLDER)/$ARCHIVE_NAME"
TARGET_FOLDER="/media/sf_Downloads"

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
  tar --create --gzip --file=$ARCHIVE_PATH --directory=$SOLUTION_FOLDER --exclude=.git --exclude=.git/\* .
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CreateArchive-1)."
  if [ -e $TARGET_FOLDER/$ARCHIVE_NAME ]; then
    rm $TARGET_FOLDER/$ARCHIVE_NAME
    [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CreateArchive-2)."
  fi
  mv $ARCHIVE_PATH $TARGET_FOLDER
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (CreateArchive-3)."
  echo "Solution archive '$ARCHIVE_NAME' created."
}

# Restore archive
RestoreArchive() {
  [ ! -f "$TARGET_FOLDER/$ARCHIVE_NAME" ] && DisplayErrorAndStop "Solution archive '$ARCHIVE_NAME' not found."
  find $SOLUTION_FOLDER -maxdepth 1 -type d -not -name $SOLUTION_NAME -not -name .git -exec rm -rf {} +
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (RestoreArchive-1)."
  find $SOLUTION_FOLDER -maxdepth 1 -type f -exec rm {} +
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (RestoreArchive-2)."
  tar --extract --gzip --file=$TARGET_FOLDER/$ARCHIVE_NAME --directory=$SOLUTION_FOLDER
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (RestoreArchive-3)."
  echo "Solution archive '$ARCHIVE_NAME' restored."
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified.";
elif [ -n $1 ]; then
  OPERATION=$( tr '[:upper:]' '[:lower:]' <<<"$1" );
fi

# Execute operation
case $OPERATION in
  create)
    CleanupSolution
    CreateArchive ;;
  restore)
    RestoreArchive ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac