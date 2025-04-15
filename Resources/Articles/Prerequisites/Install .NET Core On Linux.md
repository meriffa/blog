# Install .NET Core On Linux

This article describes the steps to install [.NET Core](https://dotnet.microsoft.com/) on [Debian 12 ("Bookworm")](https://www.debian.org/) Linux.

There are two methods - Package Manager (APT) and Scripted Install (dotnet-install.sh). The recommended approach is to use the Package Manager method first and Scripted Install only as a fallback option. The Package Manager deployment is available in most cases (physical machines, virtual machines, cloud infrastructure, on-premise infrastructure, x86, x64, Arm32, Arm64). This method provides more streamlined future updates (via apt-get upgrade). There are few exceptions where Scripted Install is the only option available (Raspberry Pi OS, DietPi).

Each deployment method contains three .NET Core installation options - .NET Core Runtime, ASP.NET Core Runtime and .NET Core SDK. The .NET Core Runtime is used for backend processing applications (Class Library, Console App). The ASP.NET Core Runtime is used for web UI and web services applications (Web App, REST API, MVC, Razor, Blazor) and contains the .NET Core Runtime as well. .NET Core SDK is used for .NET development and contains both .NET Core Runtime and ASP.NET Core Runtime.

## Package Manager (APT) Install

* Install software prerequisites:

```sudo apt-get install -y -qq wget```

* Update the list of trusted keys and package repository:

```
wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get -qq update
```

* Install .NET Core Runtime:

```sudo apt-get install -y -qq dotnet-runtime-9.0```

* Install ASP.NET Core Runtime:

```sudo apt-get install -y -qq aspnetcore-runtime-9.0```

* Install .NET Core SDK:

```sudo apt-get install -y -qq dotnet-sdk-9.0```

## Scripted Install (dotnet-install.sh)

* Install software prerequisites:

```sudo apt-get install curl libicu-dev -y -qq```

* Install .NET Core Runtime:

```curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --runtime dotnet --install-dir /opt/dotnet --no-path```

* Install ASP.NET Core Runtime:

```curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --runtime aspnetcore --install-dir /opt/dotnet --no-path```

* Install .NET Core SDK:

```curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 9.0 --install-dir /opt/dotnet --no-path```

* Complete .NET Core installation:

```
sudo ln -s /opt/dotnet/dotnet /usr/bin/dotnet
echo "export DOTNET_ROOT=/opt/dotnet" >> ~/.bashrc
source ~/.bashrc
```

## Verify Install (Optional)

To verify the .NET Core installation use the following:

```
dotnet --list-runtimes
dotnet --list-sdks
```

* If you installed .NET Core Runtime you should see:

```
Microsoft.NETCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
```

* If you installed ASP.NET Core Runtime you should see:

```
Microsoft.NETCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
Microsoft.AspNetCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
```

* If you installed .NET Core SDK you should see:

```
Microsoft.NETCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
Microsoft.AspNetCore.App 9.0.0 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
9.0.101 [/usr/share/dotnet/sdk]
```

## .NET CLI Telemetry Opt Out (Optional)

To opt out of .NET CLI telemetry use the following:

```
echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1" >> ~/.bashrc
source ~/.bashrc
```

## References

* [Article Script](/Resources/Scripts/Install%20.NET%20Core%20On%20Linux.sh)

<!--- Category: .NET Prerequisites, Tags: .NET, .NET Core, Install, Linux --->