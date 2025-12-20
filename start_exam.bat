@echo off
echo Creating .env file...
(
echo POSTGRES_USER=riff
echo POSTGRES_PASSWORD=riff
echo POSTGRES_DB=riff
echo RABBIT_USER=guest
echo RABBIT_PASS=guest
echo POSTGRES_PORT=5432
echo RABBIT_UI_PORT=15672
echo GRAFANA_PORT=3000
echo PROMETHEUS_PORT=9090
echo CONNECTION_STRING=Host=db;Database=riff;Username=riff;Password=riff
echo RABBIT_CONN_STRING=amqp://guest:guest@rabbitmq
) > .env

echo Starting Infrastructure and Apps...
docker-compose -f compose.exam.yaml up -d --build

echo.
echo ========================================================
echo SYSTEM IS READY!
echo App:      http://localhost:80/login
echo Grafana:  http://localhost:3000
echo Jenkins:  http://localhost:8088
echo Gitea:    http://localhost:3001
echo ========================================================
pause
