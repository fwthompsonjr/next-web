$keys = @{
    "AWS_PROD_INTERNAL_URL" = "oxfordleads.us-east-2.elasticbeanstalk.com"
    "AWS_PROD_EXTERNAL_URL" = "awseb--awseb-slz37rtj56jw-1244023339.us-east-2.elb.amazonaws.com"
    "AWS_PROD_LOAD_BALANCER_IP" = "18.118.182.232"
    "GODADDY_DOMAIN" = "oxfordlegalleads.com"
    "GODADDY_INDEX" = "-- redacted --"
    "GODADDY_SECRET" = "-- redacted --"
}
$apiKey = @{
    key = $keys["GODADDY_INDEX"]
    secret = $keys["GODADDY_SECRET"]
}
$domain = $keys["GODADDY_DOMAIN"]
#---- Build authorization header ----#
$headers = @{}
$headers["Authorization"] = 'sso-key ' + $apiKey.key + ':' + $apiKey.secret
$headers["accept"] = "application/json"
        
#---- Build the request URI based on domain ----#
$uri = "https://api.godaddy.com/v1/domains/$domain/records"

#---- Make the request ----#
$request = Invoke-WebRequest -Uri $uri -Method Get -Headers $headers -UseBasicParsing | ConvertFrom-Json

#---- Convert the request data into an object ----#
foreach ($item in $request) {
    [PSCustomObject]@{
        data = $item.data
        name = $item.name
        ttl  = $item.ttl
        type = $item.type
    }
}