<#
    custom_file_serializer.ps1
    custom file serializer
    reads an input file and generates a custom serialization of file
    for use as fallback content in the web read project
#>
$rootDr = "..\leads-ui\src\core\legallead.core\component\records.search\_shared"
$src = "$rootDr\xml-harrisCivilCaseType-json.txt"
$arr =@();
$template = 'sbb.AppendLine("{0}");'
Get-Content $src | ForEach-Object {
    $line = ([string]$_).Replace('"', '~');
    $transform = $template -f $line;
    $arr += ($transform);
}
$final = [string]::Join([Environment]::NewLine, $arr);
Write-Host $final -ForegroundColor Gray