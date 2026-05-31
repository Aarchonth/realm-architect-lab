# Docker

## Overview

Docker is used in the Realm Architect Lab to run supporting infrastructure services in isolated containers.

The main Minecraft server currently runs as a native systemd service, while Docker is used for additional services such as monitoring, container management, and webserver testing.

## Why Docker Is Used

Docker makes it easier to run services without installing every application directly on the Debian host system.

Benefits:

* isolated services
* reproducible deployments
* easier cleanup
* simple service updates
* clear separation between host system and applications

## Core Concepts

### Image

An image is a template or blueprint for a container.

Example:

```text
nginx:latest
```

### Container

A container is a running instance of an image.

Examples:

```text
nginx-test
uptime-kuma
portainer
```

### Volume

A volume or bind mount stores persistent data outside the container.

This is important because containers can be recreated without losing application data.

Example:

```yaml
volumes:
  - ./data:/app/data
```

### Docker Compose

Docker Compose is used to define services in a `docker-compose.yml` file.

This makes services easier to reproduce, update, document, and move between systems.

## Current Docker Services

### Uptime Kuma

Uptime Kuma is used for monitoring service availability.

Repository example:

```text
docker/uptime-kuma/docker-compose.yml
```

It monitors services such as:

* Debian server availability
* SSH
* Minecraft server
* Nginx web lab

### Portainer

Portainer provides a web interface for managing Docker containers, images, volumes, and networks.

Repository example:

```text
docker/portainer/docker-compose.yml
```

Portainer is useful for visualizing Docker resources and learning how containers, images, volumes, and networks relate to each other.

### Nginx Test Webserver

Nginx is used as a simple test webserver for serving a static HTML page.

Repository example:

```text
docker/nginx-test/docker-compose.yml
```

The local `html` directory is mounted into the Nginx container as the web root.

## Useful Commands

List running containers:

```bash
docker ps
```

List all containers, including stopped ones:

```bash
docker ps -a
```

List downloaded images:

```bash
docker images
```

Start services from a Compose file:

```bash
docker compose up -d
```

Stop services from a Compose file:

```bash
docker compose down
```

View container logs:

```bash
docker logs <CONTAINER_NAME>
```

View Compose service logs:

```bash
docker compose logs
```

Remove stopped test containers:

```bash
docker container prune
```

## Project Structure

Docker service examples are stored in:

```text
docker/
├── nginx-test/
├── portainer/
└── uptime-kuma/
```

Each service contains its own `docker-compose.yml` file.

## Design Decision

The main Minecraft server is currently not containerized.

It runs as a systemd service because this provides direct control, simple logging through `journalctl`, and a good learning path for Linux service management.

Docker is currently used for supporting infrastructure services.

Future Minecraft test servers may be deployed in Docker to explore automated game server provisioning.

## Security Notes

Docker management interfaces should not be exposed publicly without proper authentication, HTTPS, and access control.

Portainer is currently intended to be accessed through the local network or WireGuard VPN.

Sensitive values should not be committed to GitHub.

Examples of sensitive values:

* real passwords
* API tokens
* private keys
* database credentials
* production secrets

## Future Improvements

Planned Docker-related improvements:

* add reverse proxy
* add HTTPS support
* add additional monitoring targets
* create a Docker-based Minecraft test server
* experiment with automated container provisioning
* document container backup strategies
* explore a small game server provisioning panel
