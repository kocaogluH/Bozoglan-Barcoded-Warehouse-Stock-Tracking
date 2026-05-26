[Setup]
AppName=Poseidon Depo ve Stok Yönetimi
AppVersion=1.0.7
DefaultDirName={pf}\PoseidonYazilim
DefaultGroupName=Poseidon Yazilim
OutputDir=.\Installer
OutputBaseFilename=Poseidon_Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Derlenen dosyaları kopyalamak için (Release klasörü üzerinden)
Source: "bin\Release\Barcoded Warehouse Stock Tracking.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "Resources\poseidon_logo.png"; DestDir: "{app}\Resources"; Flags: ignoreversion

[Icons]
Name: "{group}\Poseidon Depo ve Stok"; Filename: "{app}\Barcoded Warehouse Stock Tracking.exe"
Name: "{commondesktop}\Poseidon Depo ve Stok"; Filename: "{app}\Barcoded Warehouse Stock Tracking.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\Barcoded Warehouse Stock Tracking.exe"; Description: "{cm:LaunchProgram,Poseidon Depo ve Stok}"; Flags: nowait postinstall skipifsilent
