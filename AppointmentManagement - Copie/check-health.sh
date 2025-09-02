#!/bin/bash

echo "?? Vérification de l'état des services Docker"
echo "=============================================="

# Fonction pour vérifier si un port est ouvert
check_port() {
    local host=$1
    local port=$2
    local service=$3
    
    if nc -z $host $port 2>/dev/null; then
        echo "? $service: $host:$port est accessible"
        return 0
    else
        echo "? $service: $host:$port n'est pas accessible"
        return 1
    fi
}

# Fonction pour tester une URL HTTP
check_http() {
    local url=$1
    local service=$2
    
    if curl -f -s "$url" > /dev/null 2>&1; then
        echo "? $service: $url répond correctement"
        return 0
    else
        echo "? $service: $url ne répond pas"
        return 1
    fi
}

echo ""
echo "?? État des conteneurs Docker:"
docker ps --filter "name=appointmentmanagement" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

echo ""
echo "?? Test des services réseau:"

# Test des ports de base de données
check_port "localhost" "1433" "SQL Server Production"
check_port "localhost" "1434" "SQL Server Development"

# Test des ports API
check_port "localhost" "8080" "API Production"
check_port "localhost" "5000" "API Development"

# Test des services additionnels
check_port "localhost" "8025" "MailHog Web UI"
check_port "localhost" "1025" "MailHog SMTP"
check_port "localhost" "6379" "Redis"

echo ""
echo "?? Test des endpoints HTTP:"

# Test des endpoints HTTP
check_http "http://localhost:8080/health" "API Production Health"
check_http "http://localhost:5000/health" "API Development Health"
check_http "http://localhost:8080/swagger" "Swagger Production"
check_http "http://localhost:5000/swagger" "Swagger Development"
check_http "http://localhost:8025" "MailHog Web UI"

echo ""
echo "?? Logs récents des conteneurs:"
echo "================================"

for container in $(docker ps --filter "name=appointmentmanagement" --format "{{.Names}}"); do
    echo ""
    echo "?? Logs de $container (10 dernières lignes):"
    docker logs --tail 10 "$container" 2>/dev/null || echo "   Impossible de récupérer les logs de $container"
done

echo ""
echo "?? Commandes utiles:"
echo "===================="
echo "• Voir tous les logs: docker-compose logs -f"
echo "• Redémarrer les services: docker-compose restart"
echo "• Arrêter les services: docker-compose down"
echo "• Voir l'état détaillé: docker-compose ps"