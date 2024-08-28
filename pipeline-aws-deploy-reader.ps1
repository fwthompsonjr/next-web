param (
    [string]$searchPattern,
    [string]$versionLabel,
    [string]$buildNumber = '',
    [bool]$getTools = $false
)
if( [string]::IsNullOrWhiteSpace( $searchPattern ) -eq $true ) {
    $searchPattern = '*next.processor.api.csproj';
}
if( [string]::IsNullOrWhiteSpace( $buildNumber ) -eq $true ) {
    $buildNumber = '0';
}

function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}
function installTools() {
    try
    { 
        dotnet tool install -g Amazon.ElasticBeanstalk.Tools
    } catch {
        # no action taken on fail to install tools
    }
}

function getVersionNumber( $source ){
    $currentLocation = Get-Location
    try {
        $dstamp = (Get-Date).Date.ToString("s").Split("T")[0].Replace("-","");
        $tstamp  = (Get-Date -Format hhmm)
        $configFile = [System.IO.Path]::Combine( $source, "web.reader.version.json" );
        if ( [System.IO.File]::Exists( $configFile ) -eq $false ) {
            $v = [string]::Concat("v.3.2.40x.", $dstamp, ".", $tstamp );
            return $v;
        }
        $content = [System.IO.File]::ReadAllText( $configFile ) | ConvertFrom-Json
        $v = [string]::Concat("v.", [string]($content.Item(0).name).Replace("--", $dstamp).Replace("vv", $buildNumber));
        return $v;
    } finally {
        Set-Location $currentLocation
    }
}

function executeDeployment( $source ){
    $currentLocation = Get-Location
    try {
        $projectName = [System.IO.Path]::GetFileName( $source );
        $projectDirectory = [System.IO.Path]::GetDirectoryName( $source );
        $configurationFile = [System.IO.Path]::Combine( $projectDirectory, "aws-beanstalk-tools-defaults.json" );
        if ([System.IO.File]::Exists( $configurationFile ) -eq $false ) {
            Write-Output "No configuration file is found."
            return;
        }
        Set-Location $projectDirectory
        Write-Output "Executing Deployment: $projectName"
        dotnet eb deploy-environment -c Release -cfg $configurationFile --version-label $versionLabel
    } finally {
        Set-Location $currentLocation
    }
}

$workingDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$versionLabel = getVersionNumber -source $workingDir
$di = [System.IO.DirectoryInfo]::new( $workingDir );
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories)
if($getTools -eq $true ) { installTools }
if( ( canEnumerate -obj $found ) -eq $false ) {
    $solutionFile = $found.FullName
    executeDeployment -source $solutionFile
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        executeDeployment -source $solutionFile
    }
}