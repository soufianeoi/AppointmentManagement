# Script PowerShell pour v�rifier l'�tat des services Docker

Write-Host "?? V�rification de l'�tat des services Docker" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Green

# Fonction pour v�rifier si un port est ouvert
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
        Write-Host "? $Service : $Url r�pond correctement" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "? $Service : $Url ne r�pond pas" -ForegroundColor Red
        return $false
    }
}

Write-Host ""
Write-Host "?? �tat des conteneurs Docker:" -ForegroundColor Cyan
docker ps --filter "name=appointmentmanagement" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

Write-Host ""
Write-Host "?? Test des services r�seau:" -ForegroundColor Cyan

# Test des ports de base de donn�es
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
Write-Host "?? Logs r�cents des conteneurs:" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

$containers = docker ps --filter "name=appointmentmanagement" --format "{{.Names}}" | Where-Object { $_ }

foreach ($container in $containers) {
    Write-Host ""
    Write-Host "?? Logs de $container (10 derni�res lignes):" -ForegroundColor Yellow
    try {
        docker logs --tail 10 $container 2>$null
    }
    catch {
        Write-Host "   Impossible de r�cup�rer les logs de $container" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Commandes utiles:" -ForegroundColor Cyan
Write-Host "====================" -ForegroundColor Cyan
Write-Host "� Voir tous les logs: docker-compose logs -f" -ForegroundColor White
Write-Host "� Red�marrer les services: docker-compose restart" -ForegroundColor White
Write-Host "� Arr�ter les services: docker-compose down" -ForegroundColor White
Write-Host "� Voir l'�tat d�taill�: docker-compose ps" -ForegroundColor White