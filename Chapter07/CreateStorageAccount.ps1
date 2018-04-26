Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionId "********-****-****-****-***********"
New-AzureRmResourceGroup -Name PacktPubStorage -Location WestEurope
New-AzureRmStorageAccount -ResourceGroupName PacktPubStorage -AccountName packtpubstorage -Location WestEurope -SkuName "Standard_GRS"