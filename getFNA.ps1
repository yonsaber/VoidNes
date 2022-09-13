#!/bin/bash
# Program: getFNA
# Author: Caleb Cornett (converted to powershell to work with windows)
# Usage: ./getFNA.sh
# Description: Quick and easy way to install a local copy of FNA and its native libraries.

# Checks if dotnet is installed
function checkDotnet()
{
    try { dotnet | Out-Null }
    catch [System.Management.Automation.CommandNotFoundException]
    {
        Write-Output "ERROR: Dotnet is not installed. Please install dotnet to download the t4 tool."
        exit
    }
}

function check7zip ()
{
    if ((Test-Path "C:\Program Files\7-Zip") -eq 0)
    {
        Write-Output "ERROR: 7zip is not installed, please install 7zip and try again."
        exit
    }
}

function installT4 ()
{
    if (checkDotnet) { Invoke-Expression 'dotnet tool install -g dotnet-t4' }
}

function checkGit ()
{
    try { git | Out-Null }
    catch [System.Management.Automation.CommandNotFoundException]
    {
        Write-Output "ERROR: Git is not installed. Please install git to download FNA."
        exit
    }
}

function downloadFNA()
{
    checkGit
    git -C $PSScriptRoot clone https://github.com/FNA-XNA/FNA.git --recursive

    if ($? -eq 1) { Write-Output "Finished Downloading!" }
    else { Write-Output "ERROR: Download failed, try again later?" exit}

}

function updateFNA ()
{
    checkGit
    Write-Output "Updating to the latest git version of FNA..."

    git -C "${PSScriptRoot}\FNA" pull --recurse-submodules

    if ($? -eq 1)
    {
        Write-Output "Finished updating!"
    }
    else
    {
        Write-Output "ERROR: Unable to update."
        exit
    }
}

function getLibs ()
{
    Write-Output "Downloading the latest FNAlibs..."
    Invoke-WebRequest -Uri http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2 -OutFile "${PSScriptRoot}/project_name/fnalibs.tar.bz2"
    if ($? -eq 1) { Write-Output "Finished downloading!" }
    else { Write-Output "ERROR: Unable to download successfully." exit}

    Write-Output "Decompressing fnalibs..."
    check7zip
    if ((Test-Path "${PSScriptRoot}\project_name\fnalibs") -eq 0)
    {
        & "C:\Program Files\7-Zip\7z.exe" x "${PSScriptRoot}\project_name\fnalibs.tar.bz2"
        if ($? -eq 1)
        {
            Remove-Item "${PSScriptRoot}\project_name\fnalibs.tar.bz2"
        }
        else
        {
            Write-Output "ERROR: Unable to decompress successfully."
            exit
        }
        & "C:\Program Files\7-Zip\7z.exe" x "${PSScriptRoot}\fnalibs.tar" -ofnalibs
        if ($? -eq 1)
        {
            Remove-Item "${PSScriptRoot}\fnalibs.tar"
            Write-Output "Finished decompressing!"
        }
        else
        {
            Write-Output "ERROR: Unable to decompress successfully."
            exit
        }
    }
}

if (Test-Path "${PSScriptRoot}\FNA")
{
    $shouldUpdate = Read-Host -Prompt "Update FNA (y/n)?"
}
else
{
    $shouldDownload = Read-Host -Prompt "Download FNA (y/n)?"
}

if (Test-Path "${PSScriptRoot}\fnalibs")
{
    $shouldDownloadLibs = Read-Host -Prompt "Redownload fnalibs (y/n)?"
}
else
{
    $shouldDownloadLibs = Read-Host -Prompt "Download fnalibs (y/n)?"
}

if ((Test-Path "${PSScriptRoot}\project_name") -eq 1)
{
    $newProjectName = Read-Host -Prompt "Enter the project name to use for your folder and csproj file or 'exit' to quit: "
}

if ($shouldDownload -like 'y') { downloadFNA }
elseif ($shouldUpdate -like 'y') { updateFNA }

if ($shouldDownloadLibs -like 'y') { getLibs }

installT4

# Only proceed from here if we have not yet renamed the project
if ((Test-Path "${PSScriptRoot}\project_name") -ne 1) { exit }

if ($newProjectName -eq "exit" -or $newProjectName -eq "") { exit }

$files= "project_name.sln",
        ".gitignore",
        "project_name\DllMap.cs",
        "project_name\Options.cs",
        "project_name\Program.cs",
        "project_name\App\DrawVertDeclaration.cs",
        "project_name\App\GameWrapper.cs",
        "project_name\App\ProjectGame.cs",
        "project_name\App\Interfaces\IGame.cs",
        "project_name\App\Interfaces\IGameWrapper.cs",
        "project_name\Extensions\ServiceCollectionExtensions.cs",
        "project_name\Layout\Footer.cs",
        "project_name\Renderers\GraphicsDeviceManagerWrapper.cs",
        "project_name\Renderers\ImGuiRenderer.cs",
        "project_name\Renderers\ImGuiRenderFactory.cs",
        "project_name\Renderers\Interfaces\IGraphicsDeviceManagerWrapper.cs",
        "project_name\Renderers\Interfaces\IImGuiRenderer.cs",
        "project_name\Renderers\Interfaces\IImGuiRendererFactory.cs",
        "project_name\Renderers\Interfaces\IImGuiWidget.cs",
        "project_name\Services\DesktopAppHostedService.cs",
        "project_name\Services\GameFactory.cs",
        "project_name\Services\Interfaces\IGameFactory.cs",
        "project_name\Utilities\MemoryMetrics.cs",
        "project_name\Utilities\SystemInformation.cs",
        "project_name\Widgets\ImGuiWidget.cs",
        "project_name\Widgets\WidgetFactory.cs",
        "project_name\Widgets\Interfaces\IWidgetFactory.cs",
        ".vscode/tasks.json",
        ".vscode/settings.json",
        ".vscode/launch.json",
        ".vscode/buildEffects.sh",
        ".vscode/processT4Templates.sh",
        ".vscode/buildEffects.ps1",
        ".vscode/processT4Templates.ps1"

Move-Item fnalibs "project_name\fnalibs"

foreach ($file in $files)
{
    ((Get-Content -Path $file -Raw) -replace 'project_name', $newProjectName) | Set-Content -Path $file
}

Rename-Item -Path "project_name.sln"                        -NewName "${newProjectName}.sln"
Rename-Item -Path "project_name/project_name.csproj"        -NewName "${newProjectName}.csproj"
Rename-Item -Path "project_name"                            -NewName $newProjectName

git init

"Restoring..."
Set-Location $PSScriptRoot

"Building..."
dotnet restore $newProjectName
dotnet msbuild -t:buildcontent $newProjectName
dotnet build --no-restore "${newProjectName}.sln"
