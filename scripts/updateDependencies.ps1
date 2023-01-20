param(
    [Parameter(Mandatory = $true)][String]$organization,
    [Parameter(Mandatory = $true)][String]$gitHubToken,
    [Parameter(Mandatory = $true)][String]$gitHubUser)

$ErrorActionPreference = "Stop"
$SleepTimeInSecondsToRunChecks = 10
$maxRetryCount = 10
$sleepBetweenRetries = 60;

Function Test-CommandExists {
    Param ($command)
    $oldPreference = $ErrorActionPreference
    $ErrorActionPreference = "Stop"
    try {
        if (Get-Command $command) {
            return $true
        }
    }
    Catch {
        return $false
    }
    Finally {
        $ErrorActionPreference = $oldPreference
    }
}

function IsPrCreatedByNanodu {
    Param ($login, $title)

    if ($login -eq $gitHubUser -And $prs.title -match 'Update (\d*) NuGet dependencies') {
        return $true
    }

    return $false
}

function SetPrMergeSqush {
    param ($prNumber)
    Write-Host "Auto merging"$prNumber
    $url = "https://api.github.com/repos/$organization/$repoName/pulls/$prNumber/merge"
    $data = @{        
        merge_method = "squash"
    };
    $json = $data | ConvertTo-Json;
    Invoke-RestMethod -Method PUT -Uri $url -ContentType "application/json" -Headers @{"Authorization" = $tokenHeader } -Body $json;
}

function DeleteBranch {
    param ($prBranchName)
    Write-Host 'Remove Branch'$prBranchName ;
    $url = "https://api.github.com/repos/$organization/$repoName/git/refs/heads/$prBranchName"
    Invoke-RestMethod -Method DELETE -Uri $url -ContentType "application/json" -Headers @{"Authorization" = $tokenHeader };
}

function CanSafeDeleteBranch {
    param ($prNumber)
    param ($retry = 0)

    Write-Host 'Check if branch is safe to delete. Retry'$prBranchName ;
    $url = "https://api.github.com/repos/$organization/$repoName/pulls/$prNumber"
    $response = Invoke-WebRequest -Uri $url -Headers @{"Authorization" = $tokenHeader }
    $pr = $response | ConvertFrom-Json

    if ($pr.state -eq 'closed'){
        return
    }

    if ($retry -eq $maxRetryCount){
        Write-Warning 'Unable to delete branch from PR '$prNumber
        return
    }

    Write-Host 'Waiting for' $sleepBetweenRetries 'before checking if delete is safe.'
    Start-Sleep $sleepBetweenRetries
    $newRetry = $retry + 1;
    CanSafeDeleteBranch $prNumber $newRetry 
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

If (!(Test-CommandExists "nanodu")) {
    Write-Host "nanodu command not exists. Trying to install."
    dotnet nuget add source https://pkgs.dev.azure.com/nanoframework/feed/_packaging/sandbox/nuget/v3/index.json --name nano
    dotnet tool install nanodu -g
    Write-Host "nanodu installed."
    dotnet nuget remove source nano
}
$request = "https://api.github.com/orgs/$organization/repos?per_page=100"
Write-Host "Executing request "$request
$tokenHeader = "Bearer $gitHubToken"
$response = Invoke-WebRequest -Uri $request -Headers @{"Authorization" = $tokenHeader }
$repositories = $response | ConvertFrom-Json

foreach ($repository in $repositories) {
    if ($repository.full_name -notlike "*Mako-IoT.Device.Services.Messaging*") {
        continue;
    }

    if ($repository.full_name -notlike "*Mako-IoT.Device*") {
        continue;
    }

    $env:NF_Library = $repository.name

    Write-Host "Trying to update"$repository.full_name

    if (Test-Path -Path $repository.name) {
        Remove-Item -Recurse -Force $repository.name
    }

    $Expr = 'nanodu --git-hub-user $gitHubUser --use-git-token-for-clone true --repo-owner $organization --repos-to-update $repository.name'
    Invoke-Expression $Expr
}

Write-Host "Sleeping for" $SleepTimeInSecondsToRunChecks " to allow GitHub to initialize checks"
Start-Sleep $SleepTimeInSecondsToRunChecks

# Close any PR if created by nanodu
foreach ($repository in $repositories) {
    $repoName = $repository.name
    $requestPr = "https://api.github.com/repos/$organization/$repoName/pulls"
    Write-Host "Executing request "$requestPr
    $tokenHeader = "Bearer $gitHubToken"
    $response = Invoke-WebRequest -Uri $requestPr -Headers @{"Authorization" = $tokenHeader }
    $prs = $response | ConvertFrom-Json

    foreach ($pr in $prs) {
        if (!(IsPrCreatedByNanodu $prs.user.login $prs.title)) {
            continue
        }
    
        SetPrMergeSqush $pr.number  
        
        if (CanSafeDeleteBranch $pr.number) {
            DeleteBranch $prs.head.ref
        }
    }
}

Write-Host "Done"
exit 0