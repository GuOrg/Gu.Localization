## To restore Packages

1. PM> `.paket/paket.bootstrapper.exe` only needed for downloading or updating paket.exe
2. PM> `.paket/paket.exe restore` restore packages.
   Alternatively:
   PM> `.paket/paket.exe auto-restore on`
   And never worry about it again.
3. PM> `.paket/paket.exe install` install packages.
3. PM> `.paket/paket.exe update` update packages.

## To create packages:
1. Build in release
2. PM> `.paket/paket.bootstrapper.exe` only needed for downloading paket.exe
3. PM> `.paket/paket.exe pack output publish symbols` symbols is optional.
4. Packages are in the publish folder.

Docs: https://fsprojects.github.io/Paket/getting-started.html