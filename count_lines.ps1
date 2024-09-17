# Define the root directory for the project
$rootDir = Get-Location

# Recursively get all .cs files in the project
$csFiles = Get-ChildItem -Path $rootDir -Recurse -Filter *.cs

# Initialize total line count
$totalLines = 0

# Loop through each .cs file and count the lines
foreach ($file in $csFiles) {
    # Count the number of lines in the current .cs file and add it to the total
    $lines = (Get-Content $file.FullName).Count
    $totalLines += $lines
}

# Output the total number of lines
Write-Host "Total lines of code in .cs files: $totalLines"

