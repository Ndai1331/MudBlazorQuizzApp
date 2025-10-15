#!/bin/bash

# Docker Build and Push Script for Quiz App
# Usage: ./docker-build-push.sh [version]

set -e  # Exit on any error

# Configuration
DOCKER_USERNAME="longnguyen1331"
API_IMAGE_NAME="qc-quizz-api"
CLIENT_IMAGE_NAME="qc-quizz-client"
VERSION=${1:-"latest"}

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
}

# Function to check if logged in to Docker Hub
check_docker_login() {
    if ! docker info | grep -q "Username"; then
        print_warning "Not logged in to Docker Hub. Please login first:"
        echo "docker login"
        exit 1
    fi
}

# Function to build and push API
build_push_api() {
    print_status "Building API Docker image..."
    
    # Build API image
    docker build \
        --platform linux/amd64 \
        -t "${DOCKER_USERNAME}/${API_IMAGE_NAME}:${VERSION}" \
        -f API/Dockerfile .
    
    if [ $? -eq 0 ]; then
        print_success "API image built successfully"
    else
        print_error "Failed to build API image"
        exit 1
    fi
    
    # Push API image
    print_status "Pushing API image to Docker Hub..."
    docker push "${DOCKER_USERNAME}/${API_IMAGE_NAME}:${VERSION}"
    
    if [ $? -eq 0 ]; then
        print_success "API image pushed successfully"
    else
        print_error "Failed to push API image"
        exit 1
    fi
}

# Function to build and push Client
build_push_client() {
    print_status "Building Client Docker image..."
    
    # Build Client image
    docker build \
        --platform linux/amd64 \
        -t "${DOCKER_USERNAME}/${CLIENT_IMAGE_NAME}:${VERSION}" \
        -f Client/Dockerfile .
    
    if [ $? -eq 0 ]; then
        print_success "Client image built successfully"
    else
        print_error "Failed to build Client image"
        exit 1
    fi
    
    # Push Client image
    print_status "Pushing Client image to Docker Hub..."
    docker push "${DOCKER_USERNAME}/${CLIENT_IMAGE_NAME}:${VERSION}"
    
    if [ $? -eq 0 ]; then
        print_success "Client image pushed successfully"
    else
        print_error "Failed to push Client image"
        exit 1
    fi
}

# Function to clean up local images (optional)
cleanup_local_images() {
    read -p "Do you want to remove local images after pushing? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_status "Cleaning up local images..."
        docker rmi "${DOCKER_USERNAME}/${API_IMAGE_NAME}:${VERSION}" 2>/dev/null || true
        docker rmi "${DOCKER_USERNAME}/${CLIENT_IMAGE_NAME}:${VERSION}" 2>/dev/null || true
        print_success "Local images cleaned up"
    fi
}

# Function to show image info
show_image_info() {
    print_success "Docker images created:"
    echo "  API:   ${DOCKER_USERNAME}/${API_IMAGE_NAME}:${VERSION}"
    echo "  Client: ${DOCKER_USERNAME}/${CLIENT_IMAGE_NAME}:${VERSION}"
    echo ""
    print_status "You can pull these images using:"
    echo "  docker pull ${DOCKER_USERNAME}/${API_IMAGE_NAME}:${VERSION}"
    echo "  docker pull ${DOCKER_USERNAME}/${CLIENT_IMAGE_NAME}:${VERSION}"
}

# Main execution
main() {
    echo "=========================================="
    echo "  Quiz App Docker Build & Push Script"
    echo "=========================================="
    echo ""
    
    print_status "Starting Docker build and push process..."
    print_status "Version: ${VERSION}"
    print_status "Docker Hub: ${DOCKER_USERNAME}"
    echo ""
    
    # Pre-flight checks
    check_docker
    check_docker_login
    
    # Build and push
    build_push_api
    echo ""
    build_push_client
    echo ""
    
    # Show results
    show_image_info
    
    # Optional cleanup
    cleanup_local_images
    
    print_success "All done! Images are now available on Docker Hub."
}

# Help function
show_help() {
    echo "Usage: $0 [VERSION]"
    echo ""
    echo "Arguments:"
    echo "  VERSION    Docker image tag (default: latest)"
    echo ""
    echo "Examples:"
    echo "  $0                    # Build with 'latest' tag"
    echo "  $0 v1.0.0           # Build with 'v1.0.0' tag"
    echo "  $0 $(date +%Y%m%d)   # Build with date tag"
    echo ""
    echo "Prerequisites:"
    echo "  - Docker must be running"
    echo "  - Must be logged in to Docker Hub (docker login)"
    echo "  - Dockerfile must exist in API/ and Client/ directories"
}

# Check for help flag
if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Run main function
main
