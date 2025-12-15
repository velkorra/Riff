pipeline {
    agent any

    environment {
        COMPOSE_FILE = 'compose.yaml'
        COMPOSE_PROJECT_NAME = 'riff'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Unit Tests') {
            steps {
                echo 'Running mega tests...'
                sh 'docker run --rm -v $(pwd):/app -w /app/Riff.Tests mcr.microsoft.com/dotnet/sdk:10.0 dotnet test'
            }
        }

        stage('Build Images') {
            steps {
                echo 'Building Docker images...'
                sh 'docker-compose -f compose.yaml build api playlist notification'
            }
        }

        stage('Deploy (Restart)') {
            steps {
                echo 'Deploying...'
                sh 'docker-compose -f compose.yaml up -d --no-deps api playlist notification'
                
                sh 'docker image prune -f'
            }
        }
    }
}
