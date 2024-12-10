#!/bin/bash

# Package Manager (APT) Install
sudo apt-get install -y -qq wget gpg apt-transport-https
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor > packages.microsoft.gpg
sudo install -D -o root -g root -m 644 packages.microsoft.gpg /etc/apt/keyrings/packages.microsoft.gpg
echo "deb [arch=amd64,arm64,armhf signed-by=/etc/apt/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main" | sudo tee /etc/apt/sources.list.d/vscode.list > /dev/null
rm -f packages.microsoft.gpg
sudo apt-get -qq update
sudo apt-get install -y -qq code

# Verify Install (Optional)
code --version

## Extensions Install (Optional)
code --install-extension <ExtensionID>
local EXTENSIONS=("ms-dotnettools.csharp" "ms-vscode-remote.remote-ssh" "alefragnani.Bookmarks" "streetsidesoftware.code-spell-checker")
for EXTENSION in "${EXTENSIONS[@]}"; do
  code --install-extension $EXTENSION
done