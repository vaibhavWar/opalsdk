# PowerShell test script for Product Description Generator
# Usage: .\test.ps1 [-BaseUrl "http://localhost:8787"]

param(
    [string]$BaseUrl = "http://localhost:8787"
)

Write-Host "Testing Product Description Generator" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Test 1: Health Check
Write-Host "Test 1: Health Check (GET /)" -ForegroundColor Cyan
Write-Host "----------------------------" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/" -Method Get
    $response | ConvertTo-Json -Depth 10
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
Write-Host ""

# Test 2: Discovery Endpoint
Write-Host "Test 2: Discovery Endpoint (GET /discovery)" -ForegroundColor Cyan
Write-Host "-------------------------------------------" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/discovery" -Method Get
    $response | ConvertTo-Json -Depth 10
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
Write-Host ""

# Test 3: Simple Product Description
Write-Host "Test 3: Simple Product Description" -ForegroundColor Cyan
Write-Host "-----------------------------------" -ForegroundColor Cyan
$body = @{
    productName = "Professional Drill Set"
    partNumber = "DRL-2024-PRO"
    attributes = @(
        "Color: Black and Orange",
        "Power: 20V Lithium-Ion",
        "Chuck Size: 1/2 inch"
    )
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/" -Method Post -Body $body -ContentType "application/json"
    if ($response.success) {
        Write-Host "✓ Success!" -ForegroundColor Green
        Write-Host "Generated description preview (first 200 chars):"
        Write-Host $response.content.Substring(0, [Math]::Min(200, $response.content.Length))
    } else {
        Write-Host "✗ Failed: $($response.error)" -ForegroundColor Red
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
Write-Host ""

# Test 4: Detailed Product Description
Write-Host "Test 4: Detailed Product Description" -ForegroundColor Cyan
Write-Host "-------------------------------------" -ForegroundColor Cyan
$body = @{
    productName = "Wireless Bluetooth Keyboard"
    partNumber = "KB-W-2024-BT"
    attributes = @(
        "Connectivity: Bluetooth 5.0",
        "Battery: Rechargeable Lithium-Ion",
        "Keys: 78-key compact layout",
        "Compatibility: Windows, Mac, iOS, Android",
        "Range: Up to 30 feet"
    )
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/" -Method Post -Body $body -ContentType "application/json"
    if ($response.success) {
        Write-Host "✓ Success!" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed" -ForegroundColor Red
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
Write-Host ""

# Test 5: Minimal Product (No Attributes)
Write-Host "Test 5: Minimal Product (No Attributes)" -ForegroundColor Cyan
Write-Host "---------------------------------------" -ForegroundColor Cyan
$body = @{
    productName = "Standard Office Stapler"
    partNumber = "STA-001"
    attributes = @()
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/" -Method Post -Body $body -ContentType "application/json"
    if ($response.success) {
        Write-Host "✓ Success!" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed" -ForegroundColor Red
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
Write-Host ""

# Test 6: Error Handling - Missing Required Field
Write-Host "Test 6: Error Handling (Missing productName)" -ForegroundColor Cyan
Write-Host "--------------------------------------------" -ForegroundColor Cyan
$body = @{
    partNumber = "TEST-001"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/" -Method Post -Body $body -ContentType "application/json"
    Write-Host "Response: $($response | ConvertTo-Json)" -ForegroundColor Yellow
} catch {
    Write-Host "✓ Expected error caught: $_" -ForegroundColor Green
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "All tests completed!" -ForegroundColor Green

