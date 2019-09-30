/usr/bin/openssl req \
-newkey rsa:2048 \
-x509 \
-nodes \
-keyout $3/$2.key \
-new \
-out $3/$2.crt \
-subj /CN=$1 \
-reqexts SAN \
-extensions SAN \
-config <(cat /etc/ssl/openssl.cnf \
    <(printf '[SAN]\nsubjectAltName=DNS:$1')) \
-sha256 \
-days 3650