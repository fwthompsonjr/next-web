<#
File: C:\_d\next-web\dat\post_deployment_dns_check.ps1

after deployment of website
1. goto aws console
2. find load balancer for next-web application
3. get public url for load balancer
	e.g: awseb--awseb-slz37rtj56jw-1244023339.us-east-2.elb.amazonaws.com
4. ping load balancer url
5. if ip address has changed then
    a. login to go-daddy
    b. update dns record to point to ip 
#>
$current_ip = "3.22.23.7"
$load_balancer = "awseb--awseb-slz37rtj56jw-1244023339.us-east-2.elb.amazonaws.com";
$ip = Resolve-DNSName $load_balancer
if ($ip -eq $null) {
    Write-Host -ForegroundColor Red "$load_balancer : Unable to resolve dns"
    return;
}
foreach( $item in $ip ) {
    $resolved_ip = $item.IPAddress;
    Write-Host -ForegroundColor Gray "$resolved_ip";
    if( $resolved_ip -eq $current_ip ) {        
        Write-Host -ForegroundColor Green "$load_balancer : match found for : $current_ip";
        return;
    }
}

Write-Host -ForegroundColor Red "$load_balancer : Unable to match dns entry"