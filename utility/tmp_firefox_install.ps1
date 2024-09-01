if ($IsWindows -eq $false) { return; }
$vrsn = "129.0.2"
$uri = "https://download-installer.cdn.mozilla.net/pub/firefox/releases/{0}/win64/en-US/Firefox%20Setup%20{0}.exe";
$address = ($uri -f $vrsn);
$installDir = $env:LOCALAPPDATA
$children = @('mozilla-win', 'install')
foreach($child in $children) {
    $installDir = [System.IO.Path]::Combine( $installDir, $child );
    if ([System.IO.Directory]::Exists( $installDir ) -eq $false ) {
        [System.IO.Directory]::CreateDirectory( $installDir );
    }
}
$fullName = [System.IO.Path]::Combine( $installDir, "firefox_setup.exe");
if ([System.IO.File]::Exists( $fullName ) -eq $false )
{
    $httpClient = [System.Net.Http.HttpClient]::new();
    $ms = [System.IO.MemoryStream]::new();
    $responseStream = $httpClient.GetStreamAsync($address).GetAwaiter().GetResult();
    $responseStream.CopyTo($ms);
    $contents = $ms.ToArray();
    [System.IO.File]::WriteAllBytes( $fullName, $contents );
}
$obj = @{
        status = "ok"
        installationDate = (Get-Date).Date.ToString("s")
    } | ConvertTo-Json

$confirmationName = [System.IO.Path]::Combine( $installDir, "firefox_confirmation.txt");
if ([System.IO.File]::Exists( $confirmationName ) -eq $false ) {
    $psi = [System.Diagnostics.ProcessStartInfo]::new($fullName);
    $psi.RedirectStandardOutput = $true;
    $psi.RedirectStandardError = $true;
    $psi.UseShellExecute = $false;
    $psi.CreateNoWindow = $true;
    $psi.Arguments = "/S";
    $psi.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden;
    ## start process
    $process = [System.Diagnostics.Process]::new();
    $process.StartInfo = $psi;
    $process.Start();
    $process.WaitForExit();
    ## read results
    
    $errors = $process.StandardError.ReadToEnd();
    if ($process.ExitCode -ne 0) { return  }
    if ($errors.Length > 0) { return  }
    
    [System.IO.File]::WriteAllText( $confirmationName, $obj );
}