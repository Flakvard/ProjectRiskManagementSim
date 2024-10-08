# Define the root directory for the project
$rootDir = Get-Location

# Get the first-level directories in the root folder
$folders = Get-ChildItem -Path $rootDir -Directory

# Initialize total line count and folder-based summary
$totalLines = 0
$folderLineCounts = @{}

# Function to count lines in a collection of files
function Count-LinesInFiles($files) {
    $lineCount = 0
    foreach ($file in $files) {
        $lines = (Get-Content $file.FullName).Count
        $lineCount += $lines
    }
    return $lineCount
}

# Count lines in .cs files in the root directory (direct files not in subfolders)
$csFilesRoot = Get-ChildItem -Path $rootDir -Filter *.cs
if ($csFilesRoot) {
    $linesRoot = Count-LinesInFiles $csFilesRoot
    $folderLineCounts["Root"] = $linesRoot
    $totalLines += $linesRoot
}

# Loop through each first-level folder
foreach ($folder in $folders) {
    # Recursively get all .cs files within the folder and all its subdirectories
    $csFiles = Get-ChildItem -Path $folder.FullName -Recurse -Filter *.cs
    
    # If the folder contains .cs files, count the lines
    if ($csFiles) {
        $folderLines = Count-LinesInFiles $csFiles
        $folderLineCounts[$folder.Name] = $folderLines
        $totalLines += $folderLines
    }
}

# Remove double-counting of root files (subtract root lines from total)
$totalLines -= $folderLineCounts["Root"]

# Sort folder line counts by the number of lines in descending order
$sortedFolders = $folderLineCounts.GetEnumerator() | Sort-Object Value -Descending

# Output the sorted number of lines and folder-based counts with percentage distribution
Write-Host "`nLines of code by folder:" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------------------------------------------------------------------------------------"

# Print the header of the table
$headerFormat = "{0,-80} {1,-20} {2,-15}"
Write-Host ($headerFormat -f "Folder", "Lines of Code", "Percentage")
Write-Host "--------------------------------------------------------------------------------------------------------------------------------------"

# Print the sorted folder statistics
foreach ($entry in $sortedFolders) {
    $folderName = $entry.Key
    $folderLines = $entry.Value
    $percentage = [math]::Round(($folderLines / $totalLines) * 100, 2)

    # Output folder name in green and lines of code and percentage in normal color
    Write-Host -NoNewline ("`t{0,-80}" -f $folderName) -ForegroundColor Green
    Write-Host -NoNewline ("{0,-20}" -f $folderLines)
    Write-Host ("{0,-5}%" -f $percentage)
}
# Output the total number of lines
Write-Host "`n--------------------------------------------------------------------------------------------------------------------------------------"
Write-Host "`tTotal lines of code in all .cs files: $totalLines" -ForegroundColor Yellow
