# Install Visual Studio Code On Linux

This article describes the steps to install [Visual Studio Code](https://code.visualstudio.com/) on [Debian 12 ("Bookworm")](https://www.debian.org/) Linux.

## Package Manager (APT) Install

* Install software prerequisites:

```sudo apt-get install -y -qq wget gpg apt-transport-https```

* Update package repository references:

```
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor > packages.microsoft.gpg
sudo install -D -o root -g root -m 644 packages.microsoft.gpg /etc/apt/keyrings/packages.microsoft.gpg
echo "deb [arch=amd64,arm64,armhf signed-by=/etc/apt/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main" | sudo tee /etc/apt/sources.list.d/vscode.list > /dev/null
rm -f packages.microsoft.gpg
sudo apt-get -qq update
```

* Install Visual Studio Code:

```sudo apt-get install -y -qq code```

## Verify Install (Optional)

To verify the Visual Studio Code installation use the following:

```
code --version
```

If the Visual Studio Code installation completed successfully you should see output similar to the following:

```
1.95.3
...
x64
```

## Extensions Install (Optional)

To install Visual Studio Extension with specific Extension ID use the following:

```
code --install-extension <ExtensionID>
```

Here is an example of how to install several Visual Studio Extensions - C# (ms-dotnettools.csharp), Remote - SSH (ms-vscode-remote.remote-ssh), Bookmarks (alefragnani.Bookmarks) and Code Spell Checker (streetsidesoftware.code-spell-checker):

```
local EXTENSIONS=("ms-dotnettools.csharp" "ms-vscode-remote.remote-ssh" "alefragnani.Bookmarks" "streetsidesoftware.code-spell-checker")
for EXTENSION in "${EXTENSIONS[@]}"; do
  code --install-extension $EXTENSION
done
```

> [!NOTE]
> The Extension ID can be obtained using the 'Copy Extension ID' option in the context menu (right-click) of specific extension in Visual Studio Code.

## References

* [Article Script](/Resources/Scripts/Install%20Visual%20Studio%20Code%20On%20Linux.sh)

<!--- Category: .NET Prerequisites, Tags: .NET, .NET Core, Visual Studio Code, Linux --->