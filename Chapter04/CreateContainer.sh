az account show
az account set -s "Your-Subscription-Name"

az group create --name packtcontainergroup --location "West Europe"
az container create -g packtcontainergroup --name packtcontainer --image library/nginx --ip-address public