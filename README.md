# Realm Architect Lab

**Self-Hosted Game Server Infrastructure Lab by Aaron**

A self-hosted infrastructure lab focused on Linux administration, secure remote access, Docker-based services, monitoring, automation, backups, and game server hosting.

## Project Highlights

* Debian-based self-hosted server
* SSH key authentication with disabled password login
* WireGuard VPN for secure remote administration
* Minecraft server managed through systemd
* Automated backups with restore verification
* Automated safe restarts with player warnings
* Docker-based services using Docker Compose
* Uptime Kuma monitoring dashboard
* Portainer container management
* Nginx test webserver

## Stack

### Host System

* Debian Linux
* systemd
* UFW

### Secure Access

* SSH key authentication
* WireGuard VPN
* DDNS

### Game Server Hosting

* Minecraft server
* RCON
* automated restarts
* automated backups

### Docker Services

* Uptime Kuma
* Portainer
* Nginx test webserver

## Architecture Overview

```text
Internet
   |
   | DDNS
   v
Edge Router / Home Gateway
   |
   | NAT / Port Forwarding
   v
Debian Server
   ├── WireGuard VPN
   ├── SSH Key Authentication
   ├── Minecraft systemd Service
   ├── Automated Backups / Restarts
   └── Docker Services
       ├── Uptime Kuma
       ├── Portainer
       └── Nginx Web Lab
```

## Documentation

| Topic                                  | Description                                |
| -------------------------------------- | ------------------------------------------ |
| [Architecture](docs/architecture.md)   | High-level infrastructure overview         |
| [Networking](docs/networking.md)       | LAN, DDNS, VPN and port forwarding design  |
| [SSH Hardening](docs/ssh-hardening.md) | Key-based login and SSH security decisions |
| [Backup Strategy](docs/backups.md)     | Automated backups and restore testing      |
| [Docker](docs/docker.md)               | Container services and Compose structure   |
| [Monitoring](docs/monitoring.md)       | Uptime Kuma checks and service visibility  |

## What I Learned

* Managing Linux services with systemd
* Securing SSH access with public key authentication
* Setting up WireGuard VPN for remote administration
* Using Docker Compose for reproducible service deployment
* Building automated backup and restart workflows
* Verifying backup usability through a non-destructive restore test
* Monitoring self-hosted services with Uptime Kuma
* Documenting infrastructure in a GitHub-based portfolio repository

## Project Goal

The goal of this project is to build and document a professional self-hosted infrastructure lab around game server hosting and Linux administration.

This lab is designed as a long-term portfolio project for learning and demonstrating practical skills in:

* Linux server administration
* secure remote access
* networking and VPNs
* service management with systemd
* backup automation
* Docker and containerized services
* monitoring and uptime checks
* webserver deployment

## Roadmap

* [x] Debian server setup
* [x] SSH key authentication
* [x] WireGuard VPN
* [x] DDNS
* [x] Minecraft systemd service
* [x] Automated backups
* [x] Backup restore test
* [x] Automated safe restarts
* [x] Docker setup
* [x] Uptime Kuma monitoring
* [x] Portainer
* [x] Nginx test webserver
* [ ] Custom domain setup
* [ ] Reverse proxy with HTTPS
* [ ] Public portfolio/status page
* [ ] Backup storage on a second disk
* [ ] Docker-based Minecraft test server
* [ ] Basic server provisioning panel concept

## Status

This project is actively being built and documented.
