#!/bin/bash

# Package Manager (APT) Install
sudo apt-get install -y -qq wget
wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get -qq update
sudo apt-get install -y -qq dotnet-runtime-9.0
sudo apt-get install -y -qq aspnetcore-runtime-9.0
sudo apt-get install -y -qq dotnet-sdk-9.0

# Scripted Install (dotnet-install.sh)
sudo apt-get install curl libicu-dev -y -qq
curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --runtime dotnet --install-dir /opt/dotnet --no-path
curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --runtime aspnetcore --install-dir /opt/dotnet --no-path
curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --install-dir /opt/dotnet --no-path
sudo ln -s /opt/dotnet/dotnet /usr/bin/dotnet
echo "export DOTNET_ROOT=/opt/dotnet" >> ~/.bashrc
source ~/.bashrc

# Verify Install (Optional)
dotnet --list-runtimes
dotnet --list-sdks

# .NET CLI Telemetry Opt Out (Optional)
echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1" >> ~/.bashrc
source ~/.bashrc