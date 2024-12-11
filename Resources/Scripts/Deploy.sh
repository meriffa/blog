#!/bin/bash

RED='\033[0;31m';
NC='\033[0m';
SOLUTION_FOLDER=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )/../.." &> /dev/null && pwd )

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
}

echo "'$1' deployment started."
pushd $SOLUTION_FOLDER/Sources/$1 1> /dev/null
[ $? != 0 ] && DisplayErrorAndStop "'$1' deployment failed."
rsync -rlptz --no-implied-dirs --progress --delete --mkpath ./bin/Debug/net9.0/ $2:~/$1/
[ $? != 0 ] && DisplayErrorAndStop "'$1' deployment failed."
popd 1> /dev/null
echo "'$1' deployment completed."