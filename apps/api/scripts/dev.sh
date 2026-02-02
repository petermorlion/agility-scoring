#!/usr/bin/env sh
set -e

echo "Starting local Mongo container (docker compose up -d mongo) ..."
docker compose up -d mongo

echo "Waiting for MongoDB to be ready..."
node ./scripts/wait-for-mongo.js

echo "Starting API dev server (tsx watch)..."
npx tsx watch src/index.ts
