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
                echo 'Building a temporary image to run tests in an isolated AMD64 environment...'
                
                sh 'docker build -t riff-test-runner -f backend/Riff.Tests/Dockerfile .'
                
                sh 'docker run --rm riff-test-runner'
            }
        }

        stage('Build Images') {
            steps {
                echo 'Building application Docker images...'
                sh 'docker-compose -f compose.yaml build api playlist notification front'
            }
        }

        stage('Deploy (Restart)') {
            steps {
                echo 'Deploying application services...'
                sh 'docker-compose -f compose.yaml up -d --build --no-deps api playlist notification front'
            }
        }
    }
}