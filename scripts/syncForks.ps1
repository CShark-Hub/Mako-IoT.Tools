param(
[Parameter(Mandatory=$true)][String]$organization,
[Parameter(Mandatory=$true)][String]$baseRepository,
[Parameter(Mandatory=$true)][String]$targetBranch,
[Parameter(Mandatory=$true)][String]$gitHubToken)

$ErrorActionPreference = "Stop"
$apiVersion = "2022-11-28"

Function Test-CommandExists
{
 Param ($command)
 $oldPreference = $ErrorActionPreference
 $ErrorActionPreference = "Stop"
 try 
 {
    if(Get-Command $command)
    {
        return $true
    }
 }
 Catch 
 {
    return $false
 }
 Finally 
 {
    $ErrorActionPreference=$oldPreference
 }
}

Write-Host "Running script with parameters: "
Write-Host "Organization: $organization"
Write-Host "Base fork repository: $baseRepository"
Write-Host "Target branch: $targetBranch"
Write-Host "Github token lenght: "$gitHubToken.Length
Write-Debug "Github token: $gitHubToken"
Write-Host ""
Write-Host ""

#if GitHub CLI is not installed
If (!(Test-CommandExists "gh"))
{
    Write-Information "GH command not exists. Trying to install."
    #if scoop is not installed
    If (!(Test-CommandExists "scoop"))
    {
        Write-Information "Scoop command not exists. Trying to install."
        Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
        irm get.scoop.sh | iex
        Write-Information "Scoop installed."
    }

    scoop bucket add github-gh https://github.com/cli/scoop-gh.git
    scoop install gh
    Write-Information "GH installed."
}

$request = "https://api.github.com/repos/$organization/$baseRepository/forks?per_page=100"
Write-Host "Executing request "$request
$tokenHeader = "Bearer $gitHubToken"
$response = Invoke-WebRequest -Uri $request -Headers @{"Authorization"=$tokenHeader} -Headers @{"X-GitHub-Api-Version"=$apiVersion}
$repositories = $response | ConvertFrom-Json

$syncError = $false
foreach ($repository in $repositories)
{
    Write-Host "Trying to sync"$repository.full_name
    try
    {
        #To get started with GitHub CLI, please run:  gh auth login
        #Alternatively, populate the GH_TOKEN environment variable with a GitHub API authentication token.
        gh repo sync $repository.full_name -b $targetBranch
    }
    catch
    {
        $syncError = $true
        Write-Warning "Unable to sync" $repository.full_name
        Write-Warning $Error[0]
    }
}

if ($syncError -eq $true)
{
    Write-Error "Unable to sync forks. Check warning messages."
}

Write-Host "Done"