#!/bin/bash 

set -e

# docker compose build


if [ "$EUID" -ne 0 ]; then
    echo "Please, run with sudo command"
    exit 1;
fi


current_dir=$(pwd)

cd "$current_dir"/devcerts
mkcert -key-file carsties.local.key -cert-file carsties.local.crt app.carsties.local api.carsties.local id.carsties.local

cd "$current_dir"/infra/devcerts
mkcert -key-file carsties-app-tls.key -cert-file carsties-app-tls.crt app.carsties.local api.carsties.local id.carsties.local

cd "$current_dir"


CERTS_DIR="$current_dir"/infra/devcerts

if [ ! -d "$CERTS_DIR" ]; then
    echo "Cert folder not found";
    exit 1
fi

for cert in "$CERTS_DIR"/*.crt; do
    [ -e "$cert" ] || {
        echo "No certificate found in $CERTS_DIR";
        exit 1;
    }

    secret_name=$(basename "$cert" .crt)
    key="${CERTS_DIR}/${secret_name}.key"
    
    if [ ! -f "$key" ]; then
        echo "WARNING! Key for $cert not found"
        continue;
    fi

    kubectl delete secret "$secret_name" || true
    kubectl create secret tls "$secret_name" --key="$key" --cert="$cert"
    echo "Secret $secret_name created from $key and $cert"
done;


IP="127.0.0.1"
DOMAIN="id.carsties.local app.carsties.local api.carsties.local"
LINE="$IP $DOMAIN"

if grep -Fxq "$LINE" /etc/hosts; then
    echo -e "Domains already added: $DOMAIN"
else
    echo -e "\n#Custom\n$LINE" | tee -a /etc/hosts > /dev/null
    echo "Domains writed: $DOMAIN"
fi


folders=("infra/dev-k8s" "infra/K8S" "infra/ingress");

for folder in "${folders[@]}"; do
    kubectl apply -f "$folder"
done