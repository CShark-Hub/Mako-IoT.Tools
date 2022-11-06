#.\syncForks.ps1 -organization "CShark-Hub" -baseRepository "Mako-IoT.Base" -targetBranch "main" -gitHubToken "token"

param(
[Parameter(Mandatory=$true)][String]$organization,
[Parameter(Mandatory=$true)][String]$baseRepository,
[Parameter(Mandatory=$true)][String]$targetBranch,
[Parameter(Mandatory=$true)][String]$gitHubToken)

$ErrorActionPreference = "Stop"

Write-Host "Running script with parameters: "
Write-Host "Organization: $organization"
Write-Host "Base fork repository: $baseRepository"
Write-Host "Target branch: $targetBranch"
Write-Host "Github token lenght: "$gitHubToken.Length
Write-Debug "Github token: $gitHubToken"
Write-Host ""
Write-Host ""

#If not installed
#Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
#irm get.scoop.sh | iex
#scoop bucket add github-gh https://github.com/cli/scoop-gh.git
#scoop install gh

$response = Invoke-WebRequest -Uri "https://api.github.com/repos/$organization/$baseRepository/forks?per_page=100" -Headers @{"Authorization"="Bearer $token"}
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