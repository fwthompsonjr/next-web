$moz = "C:\Program Files (x86)\Mozilla Firefox"
$pths = ($env:Path).Split(';')
$found = 0;
foreach($itm in $pths) {
    $p = [string]$itm;
    if ($p.IndexOf("firefox", [System.StringComparison]::OrdinalIgnoreCase) -ge 0) { $found++; }
}
if ($found -eq 0) {
    $pths += ($moz);
    $nwdata = [string]::Join(';', $pths);
    [Environment]::SetEnvironmentVariable( "PATH", $nwdata );
}
[string]::Join([Environment]::NewLine, $pths);