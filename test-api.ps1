# Test script for SimpleMDB Actor API
$baseUrl = "http://localhost:8081/api/v1"
$headers = @{
    "Content-Type" = "application/json"
    "Accept" = "application/json"
}

Write-Host "Testing SimpleMDB Actor API..." -ForegroundColor Green

# Test 1: Get all actors
Write-Host "`nTest 1: Getting all actors..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors?page=1&size=5" -Method Get -Headers $headers
    Write-Host "Success! Found $($response.totalCount) actors" -ForegroundColor Green
    $response.data | Format-Table
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# Test 2: Create a new actor
Write-Host "`nTest 2: Creating new actor..." -ForegroundColor Yellow
$newActor = @{
    firstName = "Tom"
    lastName = "Hanks"
    bio = "Academy Award-winning actor"
    rating = 5.0
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors" -Method Post -Headers $headers -Body $newActor
    Write-Host "Success! Created actor with ID: $($response.id)" -ForegroundColor Green
    $actorId = $response.id
    $response | Format-List
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# Test 3: Get the created actor
Write-Host "`nTest 3: Getting actor by ID..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors/$actorId" -Method Get -Headers $headers
    Write-Host "Success! Retrieved actor:" -ForegroundColor Green
    $response | Format-List
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# Test 4: Update the actor
Write-Host "`nTest 4: Updating actor..." -ForegroundColor Yellow
$updatedActor = @{
    firstName = "Thomas"
    lastName = "Hanks"
    bio = "Updated bio for Tom Hanks"
    rating = 5.0
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors/$actorId" -Method Put -Headers $headers -Body $updatedActor
    Write-Host "Success! Updated actor:" -ForegroundColor Green
    $response | Format-List
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# Test 5: Get actor's movies
Write-Host "`nTest 5: Getting actor's movies..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors/$actorId/movies?page=1&size=5" -Method Get -Headers $headers
    Write-Host "Success! Found $($response.totalCount) movies" -ForegroundColor Green
    $response.data | Format-Table
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# Test 6: Delete the actor
Write-Host "`nTest 6: Deleting actor..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors/$actorId" -Method Delete -Headers $headers
    Write-Host "Success! Actor deleted" -ForegroundColor Green
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# Verify deletion
Write-Host "`nTest 7: Verifying deletion..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/actors/$actorId" -Method Get -Headers $headers
    Write-Host "Error: Actor still exists!" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 404) {
        Write-Host "Success! Actor no longer exists" -ForegroundColor Green
    } else {
        Write-Host "Error: $_" -ForegroundColor Red
    }
}