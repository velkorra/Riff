pipeline {
    agent any

    environment {
        COMPOSE_FILE = 'compose.exam.yaml'
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
                script {
                    echo 'Force creating .env file...'
                    
                    sh '''
                        echo "POSTGRES_USER=riff" > .env
                        echo "POSTGRES_PASSWORD=riff" >> .env
                        echo "POSTGRES_DB=riff" >> .env
                        
                        echo "RABBIT_USER=guest" >> .env
                        echo "RABBIT_PASS=guest" >> .env
                        
                        echo "POSTGRES_PORT=5432" >> .env
                        echo "RABBIT_UI_PORT=15672" >> .env
                        echo "GRAFANA_PORT=3000" >> .env
                        echo "PROMETHEUS_PORT=9090" >> .env
                        
                        echo "CONNECTION_STRING=Host=db;Database=riff;Username=riff;Password=riff" >> .env
                        echo "RABBIT_CONN_STRING=amqp://guest:guest@rabbitmq" >> .env
                    '''
                    
                    echo 'Deploying application services...'
                    sh 'docker-compose -f compose.yaml up -d --build --no-deps api playlist notification front'
                }
            }
        }
    }
}