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
                sh 'dotnet test backend/Riff.Tests/Riff.Tests.csproj'
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
