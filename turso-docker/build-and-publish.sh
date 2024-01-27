#!/usr/bin/env bash
set -euo pipefail

registry="tvandinther/turso"
latest=""; [[ "$@" == *"--latest"* ]] && latest="--tag ${registry}:latest"
version="v0.87.7"

docker build --tag "${registry}:${version}" $latest --build-arg VERSION=$version .
docker push ${registry} --all-tags
