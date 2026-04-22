$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$csc = Join-Path ${env:WINDIR} "Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) { $csc = Join-Path ${env:WINDIR} "Microsoft.NET\Framework\v4.0.30319\csc.exe" }

$keepass = "KeePass-2.61\KeePass.exe"
if (-not (Test-Path $keepass)) { throw "KeePass.exe not found at $keepass — adjust path in build.ps1" }

$src = @(
    Join-Path $root "FlowerPasswordPlugin\Properties\AssemblyInfo.cs"
    Join-Path $root "FlowerPasswordPlugin\FlowerPasswordUi.cs"
    Join-Path $root "FlowerPasswordPlugin\FlowerPasswordEngine.cs"
    Join-Path $root "FlowerPasswordPlugin\FlowerPasswordForm.cs"
    Join-Path $root "FlowerPasswordPlugin\FlowerPasswordPluginExt.cs"
)

$out = Join-Path $root "bin\FlowerPasswordPlugin.dll"
New-Item -ItemType Directory -Path (Split-Path $out) -Force | Out-Null

& $csc /nologo /target:library /optimize+ /debug- `
    /out:$out `
    /reference:"$keepass" `
    /reference:System.dll `
    /reference:System.Windows.Forms.dll `
    /reference:System.Drawing.dll `
    @src

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "Built: $out"
