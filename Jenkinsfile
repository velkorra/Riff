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
                
                sh 'docker build -t riff-test-runner -f backend/Riff.Tests/Dockerfile .'

                sh 'docker run --rm riff-test-runner'
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