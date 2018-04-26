Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionId "********-****-****-****-***********"

$RGName = "PacktPub"
$VMName = "W16PacktServer"
$AADClientID = "PacktADApp"
$AADClientSecret = "PacktSecret"
$VaultName= "PacktKeyVault"

$KeyVault = Get-AzureRmKeyVault -VaultName $VaultName -ResourceGroupName $RGName
$DiskEncryptionKeyVaultUrl = $KeyVault.VaultUri
$KeyVaultResourceId = $KeyVault.ResourceId

Set-AzureRmVMDiskEncryptionExtension -ResourceGroupName $RGName -VMName $VMName -AadClientID $AADClientID -AadClientSecret $AADClientSecret -DiskEncryptionKeyVaultUrl $DiskEncryptionKeyVaultUrl -DiskEncryptionKeyVaultId $KeyVaultResourceId