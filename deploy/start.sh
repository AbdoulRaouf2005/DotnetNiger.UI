#!/bin/bash
set -e

# Remplacer __BACKEND_URL__ par la variable d'environnement (ou valeur par défaut)
# Utile sur HF : on peut définir BACKEND_URL dans les secrets sans rebuild
if [ -n "$BACKEND_URL" ]; then
    sed -i "s|__BACKEND_URL__|${BACKEND_URL}|g" /etc/nginx/nginx.conf
fi

exec nginx -g "daemon off;"
