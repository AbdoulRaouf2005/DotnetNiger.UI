param(
    [string]$EnvFile = ".env"
)

$appsettingsPath = "wwwroot/appsettings.json"

# 1. Try environment variable
$apiBaseUrl = [System.Environment]::GetEnvironmentVariable("API_BASE_URL")

# 2. Fallback: try .env file (same directory as script)
if (-not $apiBaseUrl -and (Test-Path $EnvFile)) {
    $envContent = Get-Content $EnvFile -Raw
    $match = [regex]::Match($envContent, 'API_BASE_URL=(.*)')
    if ($match.Success) {
        $apiBaseUrl = $match.Groups[1].Value.Trim()
    }
}

# 3. Fallback: keep existing value in appsettings.json
if (-not $apiBaseUrl) {
    if (Test-Path $appsettingsPath) {
        $existing = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
        $apiBaseUrl = $existing.ApiBaseUrl
    }
}

# 4. Ultimate fallback
if (-not $apiBaseUrl) {
    $apiBaseUrl = "http://localhost:5000"
}

# Write to appsettings.json
$config = @{
    ApiBaseUrl = $apiBaseUrl
} | ConvertTo-Json

Set-Content -Path $appsettingsPath -Value $config -NoNewline
Write-Host "API_BASE_URL set to: $apiBaseUrl"
