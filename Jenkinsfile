pipeline {
    agent any

    environment {
        COMPOSE_FILE = 'compose.yaml'
        COMPOSE_PROJECT_NAME = 'riff'
        
        HOST_WORKSPACE = sh(returnStdout: true, script: 'echo $WORKSPACE').trim()
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Unit Tests') {
            steps {
                echo 'Running unit tests in AMD64 container via Rosetta...'
                
                sh """
                    docker run --rm --platform linux/amd64 \
                    -v "${HOST_WORKSPACE}":/app \
                    -w /app/backend/Riff.Tests \
                    mcr.microsoft.com/dotnet/sdk:10.0 dotnet test
                """
            }
        }

        stage('Build Images') {
            steps {
                echo 'Building Docker images (native ARM64)...'
                sh 'docker-compose -f compose.yaml build api playlist notification'
            }
        }

        stage('Deploy (Restart)') {
            steps {
                echo 'Deploying...'
                sh 'docker-compose -f compose.yaml up -d --build api playlist notification'
            }
        }
    }
}