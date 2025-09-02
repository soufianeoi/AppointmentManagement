# Script PowerShell pour vérifier l'état des services Docker

Write-Host "?? Vérification de l'état des services Docker" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Green

# Fonction pour vérifier si un port est ouvert
function Test-Port {
    param([string]$Host, [int]$Port, [string]$Service)
    
    try {
        $connection = New-Object System.Net.Sockets.TcpClient($Host, $Port)
        $connection.Close()
        Write-Host "? $Service : $Host`:$Port est accessible" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "? $Service : $Host`:$Port n'est pas accessible" -ForegroundColor Red
        return $false
    }
}

# Fonction pour tester une URL HTTP
function Test-Http {
    param([string]$Url, [string]$Service)
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Head -TimeoutSec 5 -ErrorAction Stop
        Write-Host "? $Service : $Url répond correctement" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "? $Service : $Url ne répond pas" -ForegroundColor Red
        return $false
    }
}

Write-Host ""
Write-Host "?? État des conteneurs Docker:" -ForegroundColor Cyan
docker ps --filter "name=appointmentmanagement" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

Write-Host ""
Write-Host "?? Test des services réseau:" -ForegroundColor Cyan

# Test des ports de base de données
Test-Port "localhost" 1433 "SQL Server Production"
Test-Port "localhost" 1434 "SQL Server Development"

# Test des ports API
Test-Port "localhost" 8080 "API Production"
Test-Port "localhost" 5000 "API Development"

# Test des services additionnels
Test-Port "localhost" 8025 "MailHog Web UI"
Test-Port "localhost" 1025 "MailHog SMTP"
Test-Port "localhost" 6379 "Redis"

Write-Host ""
Write-Host "?? Test des endpoints HTTP:" -ForegroundColor Cyan

# Test des endpoints HTTP
Test-Http "http://localhost:8080/health" "API Production Health"
Test-Http "http://localhost:5000/health" "API Development Health"
Test-Http "http://localhost:8080/swagger" "Swagger Production"
Test-Http "http://localhost:5000/swagger" "Swagger Development"
Test-Http "http://localhost:8025" "MailHog Web UI"

Write-Host ""
Write-Host "?? Logs récents des conteneurs:" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

$containers = docker ps --filter "name=appointmentmanagement" --format "{{.Names}}" | Where-Object { $_ }

foreach ($container in $containers) {
    Write-Host ""
    Write-Host "?? Logs de $container (10 dernières lignes):" -ForegroundColor Yellow
    try {
        docker logs --tail 10 $container 2>$null
    }
    catch {
        Write-Host "   Impossible de récupérer les logs de $container" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Commandes utiles:" -ForegroundColor Cyan
Write-Host "====================" -ForegroundColor Cyan
Write-Host "• Voir tous les logs: docker-compose logs -f" -ForegroundColor White
Write-Host "• Redémarrer les services: docker-compose restart" -ForegroundColor White
Write-Host "• Arrêter les services: docker-compose down" -ForegroundColor White
Write-Host "• Voir l'état détaillé: docker-compose ps" -ForegroundColor White