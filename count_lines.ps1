# Define the root directory for the project
$rootDir = Get-Location

# Get the first-level directories in the root folder
$folders = Get-ChildItem -Path $rootDir -Directory

# Initialize total line count and folder-based summary
$totalLines = 0
$folderLineCounts = @{}
$fileTypes = @("cs", "razor", "css", "js", "py")
$fileTypeLineCounts = @{}

# Initialize line counts for each file type
foreach ($fileType in $fileTypes) {
    $fileTypeLineCounts[$fileType] = 0
}

# Function to count lines in a collection of files
function Count-LinesInFiles($files) {
    $lineCount = 0
    foreach ($file in $files) {
        $lines = (Get-Content $file.FullName).Count
        $lineCount += $lines
    }
    return $lineCount
}

# Count lines in specified file types in the root directory (direct files not in subfolders)
foreach ($fileType in $fileTypes) {
    $filesRoot = Get-ChildItem -Path $rootDir -Filter *.$fileType
    if ($filesRoot) {
        $linesRoot = Count-LinesInFiles($filesRoot)
        if ($folderLineCounts["Root"]) {
            $folderLineCounts["Root"] += $linesRoot
        } else {
            $folderLineCounts["Root"] = $linesRoot
        }
        $fileTypeLineCounts[$fileType] += $linesRoot
        $totalLines += $linesRoot
    }
}

# Loop through each first-level folder
foreach ($folder in $folders) {
    # Skip bin, obj, and node_modules folders
    if ($folder.Name -in @("bin", "obj", "node_modules")) {
        continue
    }

    foreach ($fileType in $fileTypes) {
        # Recursively get all files of the current type within the folder and all its subdirectories
        $files = Get-ChildItem -Path $folder.FullName -Recurse -Include *.$fileType | Where-Object { 
            $_.FullName -notmatch '\\bin\\|\\obj\\|\\node_modules\\'
        }
        
        # If the folder contains files of the current type, count the lines
        if ($files) {
            $folderLines = Count-LinesInFiles($files)
            if ($folderLineCounts[$folder.Name]) {
                $folderLineCounts[$folder.Name] += $folderLines
            } else {
                $folderLineCounts[$folder.Name] = $folderLines
            }
            $fileTypeLineCounts[$fileType] += $folderLines
            $totalLines += $folderLines
        }
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

# Output the total number of lines for each file type
Write-Host "`n--------------------------------------------------------------------------------------------------------------------------------------"
foreach ($fileType in $fileTypes) {
    Write-Host "`tTotal lines of code in .$fileType files: $($fileTypeLineCounts[$fileType])" -ForegroundColor Yellow
}
Write-Host "`tTotal lines of code in all files: $totalLines" -ForegroundColor Yellow
