Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionId "********-****-****-****-***********"
New-AzureRmPublicIpAddress -Name PublicPacktIP -ResourceGroupName PacktPub -AllocationMethod Static -Location "West Europe"