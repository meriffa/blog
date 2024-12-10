#!/bin/bash

## Package Manager (APT) Install
# Install software prerequisites
sudo apt-get install -y -qq wget
# Update the list of trusted keys and package repository
wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get -qq update
# Install .NET Core Runtime
sudo apt-get install -y -qq dotnet-runtime-9.0
# Install ASP.NET Core Runtime
sudo apt-get install -y -qq aspnetcore-runtime-9.0
# Install .NET Core SDK
sudo apt-get install -y -qq dotnet-sdk-9.0

## Scripted Install (dotnet-install.sh)
# Install software prerequisites
sudo apt-get install curl libicu-dev -y -qq
# Install .NET Core Runtime
curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --runtime dotnet --install-dir /opt/dotnet --no-path
# Install ASP.NET Core Runtime
curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --runtime aspnetcore --install-dir /opt/dotnet --no-path
# Install .NET Core SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --install-dir /opt/dotnet --no-path
# Complete .NET Core installation
sudo ln -s /opt/dotnet/dotnet /usr/bin/dotnet
echo "export DOTNET_ROOT=/opt/dotnet" >> ~/.bashrc
source ~/.bashrc

## Verify Install (Optional)
dotnet --list-runtimes
dotnet --list-sdks

## .NET CLI Telemetry Opt Out (Optional)
echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1" >> ~/.bashrc
source ~/.bashrc