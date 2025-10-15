#!/bin/bash

# Quick Docker Build Script
# Simple version for quick builds

set -e

VERSION=${1:-"latest"}
DOCKER_USERNAME="longnguyen1331"

echo "🚀 Quick Build - Quiz App Docker Images"
echo "Version: $VERSION"
echo ""

# Build API
echo "📦 Building API..."
docker build --platform linux/amd64 -t "${DOCKER_USERNAME}/qc-quizz-api:${VERSION}" -f API/Dockerfile .

# Build Client  
echo "📦 Building Client..."
docker build --platform linux/amd64 -t "${DOCKER_USERNAME}/qc-quizz-client:${VERSION}" -f Client/Dockerfile .

# Push both
echo "⬆️  Pushing to Docker Hub..."
docker push "${DOCKER_USERNAME}/qc-quizz-api:${VERSION}"
docker push "${DOCKER_USERNAME}/qc-quizz-client:${VERSION}"

echo "✅ Done! Images pushed successfully."
echo "API:   ${DOCKER_USERNAME}/qc-quizz-api:${VERSION}"
echo "Client: ${DOCKER_USERNAME}/qc-quizz-client:${VERSION}"
