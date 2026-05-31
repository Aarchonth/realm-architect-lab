# Monitoring

## Overview

Monitoring is used in the Realm Architect Lab to track whether important services are online and reachable.

The project currently uses Uptime Kuma as a lightweight self-hosted monitoring dashboard.

Monitoring is important because running services is not enough. A server should also provide visibility into whether its services are actually available.

## Monitoring Tool

The monitoring service is deployed with Docker Compose.

Tool:

```text
Uptime Kuma
```

Docker Compose example:

```text
docker/uptime-kuma/docker-compose.yml
```

Uptime Kuma provides a web dashboard for checking service availability and uptime history.

## Monitored Services

The current monitoring setup includes checks for:

* Debian server availability
* SSH service
* Minecraft server
* Nginx web lab

## Debian Server Check

The Debian server can be monitored with a ping check.

Example:

```text
Type: Ping
Host: 192.168.2.49
```

This confirms that the server itself is reachable on the local network or through WireGuard VPN.

## SSH Check

SSH can be monitored with a TCP port check.

Example:

```text
Type: TCP Port
Host: 192.168.2.49
Port: 22
```

This confirms that the SSH service is reachable.

SSH is not exposed directly to the public internet. Remote access is done through WireGuard VPN.

## Minecraft Check

The Minecraft server can be monitored with a TCP port check.

Example:

```text
Type: TCP Port
Host: 192.168.2.49
Port: 25565
```

This confirms that the Minecraft server port is reachable.

When the Minecraft service is stopped, Uptime Kuma marks the monitor as down.

When the service is started again, Uptime Kuma marks it as up.

## Nginx Web Lab Check

The internal Nginx test webserver can be monitored with an HTTP check.

Example:

```text
Type: HTTP(s)
URL: http://192.168.2.49:8080
```

This confirms that the Docker-based Nginx webserver is responding.

## Access Model

The Uptime Kuma dashboard is intended to be accessed through:

* local network
* WireGuard VPN

It should not be exposed publicly without additional security measures such as HTTPS, authentication hardening, and reverse proxy configuration.

## Useful Docker Commands

Check if the Uptime Kuma container is running:

```bash
docker ps
```

View Uptime Kuma logs:

```bash
docker logs uptime-kuma
```

Restart Uptime Kuma:

```bash
docker restart uptime-kuma
```

Start Uptime Kuma from the Compose directory:

```bash
cd /srv/docker/uptime-kuma
docker compose up -d
```

Stop Uptime Kuma from the Compose directory:

```bash
cd /srv/docker/uptime-kuma
docker compose down
```

## Why Monitoring Matters

Monitoring helps detect problems early.

Without monitoring, a service can be offline for hours or days before anyone notices.

With monitoring, service failures become visible immediately through a dashboard.

This is especially useful for:

* game server hosting
* remote administration
* web services
* Docker containers
* future public services

## Current Result

The current monitoring setup provides visibility into the health of the main infrastructure components:

```text
Debian Server  -> monitored
SSH            -> monitored
Minecraft      -> monitored
Nginx Web Lab  -> monitored
```

## Future Improvements

Planned improvements:

* add notification alerts
* add Discord notifications
* add email notifications
* monitor disk usage
* monitor backup success
* monitor Docker containers
* create a public status page
* add HTTPS reverse proxy monitoring
