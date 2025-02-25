#!/bin/bash

RED='\033[0;31m';
NC='\033[0m';
DATABASE_NAME="ByteZoo.Blog.App.Data"
DATABASE_USER="ByteZoo.Blog.App.User"
DATABASE_PASSWORD="ByteZoo.Blog.App.Password"

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
}

# Setup Prerequisites
SetupPrerequisites() {
  ssh -i "$SSH_KEY" $VM_TARGET "sudo su - postgres -c \"psql -c 'CREATE DATABASE \\\"$DATABASE_NAME\\\";' -q\"";
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (SetupPrerequisites-1).";
  ssh -i "$SSH_KEY" $VM_TARGET "sudo su - postgres -c \"psql -c 'CREATE ROLE \\\"$DATABASE_USER\\\" WITH LOGIN SUPERUSER ENCRYPTED PASSWORD \\\$\\\$$DATABASE_PASSWORD\\\$\\\$;' -q\"";
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (SetupPrerequisites-2).";
  ssh -i "$SSH_KEY" $VM_TARGET -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install jq -y -qq" &> /dev/null;
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (SetupPrerequisites-3).";
  echo "Prerequisites setup completed.";
}

# Import JSON file
ImportJson() {
  [ -z $1 ] && DisplayErrorAndStop "No input JSON file specified."
  [ -z $2 ] && DisplayErrorAndStop "No target table specified."
  scp -q -i "$SSH_KEY" $1 $VM_TARGET:~/Import.tmp.json;
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (ImportJson-1).";
  ssh -i "$SSH_KEY" $VM_TARGET "jq -c '.[]' ~/Import.tmp.json > /tmp/Import.tmp.json.ndjson";
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (ImportJson-2).";
  ssh -i "$SSH_KEY" $VM_TARGET "sudo su - postgres -c \"psql -d \\\"$DATABASE_NAME\\\" -c '\COPY \\\"$2\\\" FROM '/tmp/Import.tmp.json.ndjson';' -q\"";
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (ImportJson-3).";
  ssh -i "$SSH_KEY" $VM_TARGET "rm ~/Import.tmp.json && rm /tmp/Import.tmp.json.ndjson";
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed (ImportJson-4).";
  echo "JSON file '$1' import completed.";
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified.";
elif [ -n $1 ]; then
  OPERATION=$( tr '[:upper:]' '[:lower:]' <<<"$1" );
fi

# Get operation parameters
shift;
PARSED_ARGUMENTS=$(getopt -q --alternative --options t:k:d:u:p:f: --longoptions target:,sshKey:,database:,user:,password:,file:,table: -- "$@");
eval set -- "$PARSED_ARGUMENTS";
while : ; do
  case "$1" in
    -t | --target) VM_TARGET="$2"; shift 2 ;;
    -k | --sshKey) SSH_KEY="$2"; shift 2 ;;
    -d | --database) DATABASE_NAME="$2"; shift 2 ;;
    -u | --user) DATABASE_USER="$2"; shift 2 ;;
    -p | --password) DATABASE_PASSWORD="$2"; shift 2 ;;
    -f | --file) SELECTED_FILE="$2"; shift 2 ;;
    --table) SELECTED_TABLE="$2"; shift 2 ;;
    --) shift; break ;;
  esac
done

[ -z $VM_TARGET ] && DisplayErrorAndStop "No target machine specified."
[ -z $SSH_KEY ] && DisplayErrorAndStop "No target machine SSH key specified."

# Execute operation
case $OPERATION in
  setup-prerequisites) SetupPrerequisites ;;
  import-json) ImportJson $SELECTED_FILE $SELECTED_TABLE ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac