#!/usr/bin/env sh
set -eu

OSRM_IMAGE=ghcr.io/project-osrm/osrm-backend:v6.0.0
SOURCE_URL=https://download.geofabrik.de/asia/kazakhstan-latest.osm.pbf
ALMATY_BBOX=76.65,43.05,77.25,43.45

cd "$(dirname "$0")/../.."
mkdir -p osrm-data

if [ ! -f osrm-data/kazakhstan-latest.osm.pbf ]; then
  curl -fL -o osrm-data/kazakhstan-latest.osm.pbf "$SOURCE_URL"
fi

docker run --rm -v "$PWD/osrm-data:/data" debian:bookworm-slim sh -c \
  "apt-get update -qq && apt-get install -y -qq osmium-tool >/dev/null && \
   osmium extract --overwrite -b $ALMATY_BBOX /data/kazakhstan-latest.osm.pbf -o /data/almaty.osm.pbf"

docker run --rm -v "$PWD/osrm-data:/data" "$OSRM_IMAGE" osrm-extract -p /opt/car.lua /data/almaty.osm.pbf
docker run --rm -v "$PWD/osrm-data:/data" "$OSRM_IMAGE" osrm-partition /data/almaty.osrm
docker run --rm -v "$PWD/osrm-data:/data" "$OSRM_IMAGE" osrm-customize /data/almaty.osrm
