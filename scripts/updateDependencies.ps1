param(
[Parameter(Mandatory=$true)][String]$organization,
[Parameter(Mandatory=$true)][String]$gitHubToken,
[Parameter(Mandatory=$true)][String]$gitHubUser)

$ErrorActionPreference = "Stop"

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
Write-Host "GitHub user: $gitHubUser"
Write-Host "Github token lenght: "$gitHubToken.Length
Write-Debug "Github token: $gitHubToken"
Write-Host ""
Write-Host ""

$env:GIT_REDIRECT_STDERR = '2>&1'
$env:GITHUB_TOKEN = $gitHubToken

If (!(Test-CommandExists "nanodu"))
{
    Write-Host "nanodu command not exists. Trying to install."
    dotnet tool install nanodu -g --add-source https://pkgs.dev.azure.com/nanoframework/feed/_packaging/sandbox/nuget/v3/index.json
    Write-Host "nanodu installed."
}
$request = "https://api.github.com/orgs/$organization/repos?per_page=100"
Write-Host "Executing request "$request
$tokenHeader = "Bearer $gitHubToken"
$response = Invoke-WebRequest -Uri $request -Headers @{"Authorization"=$tokenHeader}
$repositories = $response | ConvertFrom-Json

foreach ($repository in $repositories)
{
    if ($repository.full_name -notlike "*Mako-IoT.Device*")
    {
        continue;
    }

    Write-Host "Trying to update"$repository.full_name

    if (Test-Path -Path $repository.name)
    {
        Remove-Item -Recurse -Force $repository.name
    }

    $Expr  = 'nanodu --git-hub-user $gitHubUser --use-git-token-for-clone true --repos-to-update $repository.name'
    Invoke-Expression $Expr

    #TODO: Error handling
    #if(-not $Success){
    #    Write-Error "Error when trying to update repository"
    #}
    
    #break;
}

Write-Host "Done"
