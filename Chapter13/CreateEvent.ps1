Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionId "********-****-****-****-***********"



$endpoint = (Get-AzureRmEventGridTopic -ResourceGroupName PacktEventGrid -Name PacktEventGridTopic).Endpoint
$keys = Get-AzureRmEventGridTopicKey -ResourceGroupName PacktEventGrid -Name PacktEventGridTopic


$eventID = Get-Random 99999

#Date format should be SortableDateTimePattern (ISO 8601)
$eventDate = Get-Date -Format s

#Construct body using Hashtable
$htbody = @{
    id= $eventID
    eventType="recordInserted"
    subject="myapp/packtpub/books"
    eventTime= $eventDate   
    data= @{
        title="Architecting Microsoft Solutions"
        eventtype="Ebook"
    }
    dataVersion="1.0"
}

#Use ConvertTo-Json to convert event body from Hashtable to JSON Object
#Append square brackets to the converted JSON payload since they are expected in the event's JSON payload syntax
$body = "["+(ConvertTo-Json $htbody)+"]"

Invoke-WebRequest -Uri $endpoint -Method POST -Body $body -Headers @{"aeg-sas-key" = $keys.Key1}