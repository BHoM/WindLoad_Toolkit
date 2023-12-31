using namespace System.IO
using namespace System.Collections.Generic

$WindLoad = Read-Host "`nPlease enter the software name."

try 
{
    # Replace occurrences of "WindLoad" in all files
    Write-Host "`nReplacing occurrences of 'WindLoad' with "$WindLoad" in all files.`n"

    $replace_successful = $true
    Get-ChildItem -File -Recurse | ForEach-Object {
        try {
            If ((Get-Content $_.Extension -eq ".ps1") -or (Get-Content $_.Extension -eq ".bat")) 
                {
                    continue
                }
        } catch {}

        try
        {
            (Get-Content $_.FullName) -replace 'WindLoad', $WindLoad | Set-Content $_.FullName
        } 
        catch 
        {
            Write-Host "`nAn error occurred when replacing file contents of " $_.FullName ":"
            Write-Host $_
            $replace_successful = $false
        }
     }

    If($replace_successful)
    {
        Write-Host "Successfully replaced files contents.`n"
    }
    else
    {
        Write-Host "`nERROR: Could not replace some file contents.`n"
    }
    

    Write-Host "`nRenaming files and directories using "$WindLoad":`n"


    # Rename files and folders
    $stack = [Stack[string]]::new()
    $allPaths = [List[string]]::new()


    # Get all files and directories containing "WindLoad" recursively
    Get-ChildItem -Recurse -Directory | ForEach-Object {
        $dirpath = $_.FullName
        $dirname = Split-Path  $dirpath -Leaf
        if ($dirname.Contains("WindLoad"))
        {
            $stack.Push($dirpath)
            $allPaths.Add($dirpath)
        }

        # Write-Host $_.FullName

        foreach ($file in [Directory]::EnumerateFiles($dirpath)) 
        {
            $filename = [Path]::GetFileName($file)
            if ($filename.Contains('WindLoad') -and -not $allPaths.Contains($file))
            {
                $stack.Push($file)
                $allPaths.Add($file)
            }
        }
    }

    # Add root files
    Get-ChildItem -File | ForEach-Object {
        if ($_.FullName.Contains("WindLoad")) {
            $stack.Push($_.FullName)
        }
    }

    # Rename files and folders
    while ($stack.Count) {
        $poppedFullName = $stack.Pop()
        $pathExists = (-not ([string]::IsNullOrEmpty($poppedFullName))) -and (Test-Path -Path $poppedFullName)

        $filename = [Path]::GetFileName($poppedFullName)

        if($filename.Contains('WindLoad') -and $pathExists)
        {
            $newName = $filename.Replace('WindLoad', $WindLoad)

            Write-Host "Renaming: " $poppedFullName " to: " $newName

            Rename-Item -LiteralPath $poppedFullName -NewName $newName #-WhatIf
        }
    }

    Write-Host "`nAll files and folders renamed successfully."
}
catch 
{
    Write-Host "`nERROR:"
    Write-Host $_
}

