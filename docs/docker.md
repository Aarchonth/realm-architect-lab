# Docker

## Overview

Docker is used in the Realm Architect Lab to run supporting infrastructure services in isolated containers.

The main Minecraft server currently runs as a native systemd service, while Docker is used for additional services such as monitoring, container management, and public web services.

## Why Docker Is Used

Docker makes it easier to run services without installing every application directly on the Debian host system.

Benefits:

* isolated services
* reproducible deployments
* easier cleanup
* simple service updates
* clear separation between the host system and applications
* organized service deployment with Docker Compose

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
realm-architect-site
uptime-kuma
portainer
```

### Volume / Bind Mount

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

## Service Responsibility Structure

Docker services are organized by responsibility.

```text
docker/
├── internal/
│   ├── uptime-kuma/
│   └── portainer/
├── public/
│   └── realm-architect-site/
└── clients/
```

This separation makes the infrastructure easier to understand and maintain.

| Area        | Purpose                                   |
| ----------- | ----------------------------------------- |
| `internal/` | Private admin and monitoring services     |
| `public/`   | Public-facing services owned by the lab   |
| `clients/`  | Future client or demo website deployments |

## Current Docker Services

### Uptime Kuma

Uptime Kuma is used for monitoring service availability.

Repository example:

```text
docker/internal/uptime-kuma/docker-compose.yml
```

It monitors services such as:

* Debian server availability
* SSH
* Minecraft server
* internal web services
* future public HTTPS endpoints

Uptime Kuma is an internal service and should only be accessed through the local network or WireGuard VPN.

### Portainer

Portainer provides a web interface for managing Docker containers, images, volumes, and networks.

Repository example:

```text
docker/internal/portainer/docker-compose.yml
```

Portainer is useful for visualizing Docker resources and learning how containers, images, volumes, and networks relate to each other.

Portainer is an internal administration tool and should not be exposed directly to the public internet.

### Realm Architect Site

The Realm Architect Site is the public-facing lab landing page.

Repository example:

```text
docker/public/realm-architect-site/docker-compose.yml
```

This service is based on Nginx and serves a static HTML page.

The local `html` directory is mounted into the Nginx container as the web root.

The site currently listens internally on:

```text
127.0.0.1:8080
```

A reverse proxy will later expose it publicly through:

```text
https://lab.realm-architect.dev
```

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

## Example Server-Side Directory Layout

On the Debian server, Docker services are organized under:

```text
/srv/docker/
├── internal/
│   ├── uptime-kuma/
│   └── portainer/
├── public/
│   └── realm-architect-site/
└── clients/
```

This mirrors the repository structure and keeps services grouped by responsibility.

## Design Decision

The main Minecraft server is currently not containerized.

It runs as a systemd service because this provides direct control, simple logging through `journalctl`, and a good learning path for Linux service management.

Docker is currently used for supporting infrastructure services and web services.

Future Minecraft test servers may be deployed in Docker to explore automated game server provisioning.

## Security Notes

Docker management interfaces should not be exposed publicly without proper authentication, HTTPS, and access control.

Portainer and Uptime Kuma are currently intended to be accessed through the local network or WireGuard VPN.

Sensitive values should not be committed to GitHub.

Examples of sensitive values:

* real passwords
* API tokens
* private keys
* database credentials
* production secrets
* Dynamic DNS update URLs

## Future Improvements

Planned Docker-related improvements:

* add raw Nginx reverse proxy
* add HTTPS support with Certbot
* monitor public HTTPS endpoints
* create a Docker-based Minecraft test server
* experiment with automated container provisioning
* document container backup strategies
* explore a small game server provisioning panel
