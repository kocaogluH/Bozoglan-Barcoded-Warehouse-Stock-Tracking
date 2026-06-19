[Setup]
AppName=Poseidon Depo ve Stok Yönetimi
AppVersion=1.0.8.1
DefaultDirName={pf}\PoseidonYazilim
DefaultGroupName=Poseidon Yazilim
OutputDir=.\Installer
OutputBaseFilename=Poseidon_Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
SetupIconFile=app_logo.ico

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Derlenen dosyaları kopyalamak için (Release klasörü üzerinden)
Source: "bin\Release\Barcoded Warehouse Stock Tracking.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "Resources\poseidon_logo.png"; DestDir: "{app}\Resources"; Flags: ignoreversion
Source: "app_logo.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Poseidon Depo ve Stok"; Filename: "{app}\Barcoded Warehouse Stock Tracking.exe"; IconFilename: "{app}\app_logo.ico"
Name: "{commondesktop}\Poseidon Depo ve Stok"; Filename: "{app}\Barcoded Warehouse Stock Tracking.exe"; Tasks: desktopicon; IconFilename: "{app}\app_logo.ico"

[Run]
Filename: "{app}\Barcoded Warehouse Stock Tracking.exe"; Description: "{cm:LaunchProgram,Poseidon Depo ve Stok}"; Flags: nowait postinstall skipifsilent
