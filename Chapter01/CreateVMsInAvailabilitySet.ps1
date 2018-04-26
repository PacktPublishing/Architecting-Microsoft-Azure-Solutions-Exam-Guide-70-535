Login-AzureRmAccount
# Optional
Select-AzureRmSubscription -SubscriptionId "put-in-your-subscription-id"

#"********-****-****-****-***********"

New-AzureRmResourceGroup -Name PacktPubPS -Location WestEurope

New-AzureRmAvailabilitySet -Location WestEurope -Name AvailabilitySet02 -ResourceGroupName PacktPubPS -Sku Aligned -PlatformFaultDomainCount 2 -PlatformUpdateDomainCount 2

$availabilitySet = Get-AzureRmAvailabilitySet -ResourceGroupName PacktPubPS -Name AvailabilitySet02

$cred = Get-Credential -Message "Enter a username and password for the virtual machine."

$subnetConfig = New-AzureRmVirtualNetworkSubnetConfig -Name PacktSubnet -AddressPrefix 192.168.1.0/24
$vnet = New-AzureRmVirtualNetwork -ResourceGroupName PacktPubPS -Location WestEurope -Name PacktVnet -AddressPrefix 192.168.0.0/16 -Subnet $subnetConfig

$nsgRuleRDP = New-AzureRmNetworkSecurityRuleConfig -Name PacktNetworkSecurityGroupRuleRDP -Protocol Tcp -Direction Inbound -Priority 1000 -SourceAddressPrefix * -SourcePortRange * -DestinationAddressPrefix * -DestinationPortRange 3389 -Access Allow

$nsg = New-AzureRmNetworkSecurityGroup -Location WestEurope -Name PacktSecurityGroup -ResourceGroupName PacktPubPS -SecurityRules $nsgRuleRDP

# Apply the network security group to a subnet
Set-AzureRmVirtualNetworkSubnetConfig -VirtualNetwork $vnet -Name PacktSubnet -NetworkSecurityGroup $nsg -AddressPrefix 192.168.1.0/24

# Update the virtual network
Set-AzureRmVirtualNetwork -VirtualNetwork $vnet

for ($i=1; $i -le 2; $i++)
{
   $pip = New-AzureRmPublicIpAddress -ResourceGroupName PacktPubPS -Location WestEurope -Name "$(Get-Random)" -AllocationMethod Static -IdleTimeoutInMinutes 4

   $nic = New-AzureRmNetworkInterface -Name PacktNic$i -ResourceGroupName PacktPubPS -Location WestEurope -SubnetId $vnet.Subnets[0].Id -PublicIpAddressId $pip.Id -NetworkSecurityGroupId $nsg.Id

   # Specify the availability set
   $vm = New-AzureRmVMConfig -VMName PacktVM$i -VMSize Standard_D2_v3 -AvailabilitySetId $availabilitySet.Id

   $vm = Set-AzureRmVMOperatingSystem -ComputerName myVM$i -Credential $cred -VM $vm -Windows -EnableAutoUpdate -ProvisionVMAgent
   $vm = Set-AzureRmVMSourceImage -VM $vm -PublisherName MicrosoftWindowsServer -Offer WindowsServer -Skus 2016-Datacenter -Version latest
   
   $vm = Add-AzureRmVMNetworkInterface -VM $vm -Id $nic.Id
   New-AzureRmVM -ResourceGroupName PacktPubPS -Location WestEurope -VM $vm
}