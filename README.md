# Realm Architect Lab

**Self-Hosted Infrastructure Lab by Aaron**

A self-hosted infrastructure lab for learning Linux, networking, automation and service hosting.

The lab currently powers a public HTTPS website, a live German-language client website (Naturhof Bocholt – a real organic farm in Germany), a Minecraft server, VPN access, monitoring, backups, Dynamic DNS and Docker-based services.

## Live Services

| Service          | Address                         | Purpose                                      |
| ---------------- | ------------------------------- | -------------------------------------------- |
| Public Lab Site  | https://lab.realm-architect.dev | Public project landing page                  |
| Minecraft Server | `mc.realm-architect.dev`        | Self-hosted Minecraft server with SRV record |
| VPN Endpoint     | `vpn.realm-architect.dev`       | WireGuard remote access endpoint             |

Administrative services such as SSH, Portainer and Uptime Kuma are kept private behind the local network or WireGuard VPN.

## Featured: Real Client Project

The lab hosts a live production website for **Naturhof Bocholt**, a local organic-vegetable farm in Germany — the first paying client running on this infrastructure.

- Self-service blog and seasonal vegetable calendar the client edits herself (WYSIWYG admin panel, no code required)
- ASP.NET Core Web API (.NET 8) with JWT-protected admin endpoints + MariaDB
- Full recipe collection migrated from an **offline legacy WordPress site via the Internet Archive**
- GDPR compliance: self-hosted fonts, two-click consent for Google Maps, Impressum (§ 5 DDG)
- German-language site deployed behind the lab's Nginx reverse proxy on its own subdomain

[Full case study →](docs/client-naturhof.md)

## Project Highlights

* Debian-based self-hosted server
* SSH key authentication with disabled password login
* WireGuard VPN for secure remote administration
* Custom domain with IONOS DNS and Dynamic DNS automation
* Minecraft server reachable through `mc.realm-architect.dev` without manually entering a port
* Minecraft server managed as a native systemd service
* Automated backups with restore verification
* Automated safe restarts with player warnings
* Docker-based internal and public services using Docker Compose
* Uptime Kuma monitoring dashboard
* Portainer container management
* Raw Nginx reverse proxy with HTTPS via Certbot
* Public lab website with dedicated Minecraft page

## Stack

### Host System

* Debian Linux
* systemd
* UFW
* Nginx
* Certbot

### Secure Access

* SSH key authentication
* WireGuard VPN
* VPN-based administration
* Disabled public access to admin services

### Domain and Networking

* IONOS domain
* IONOS Dynamic DNS
* DNS A records
* Minecraft SRV record
* Router port forwarding
* Public/private service separation

### Self-Hosted Services

* Public lab website
* Minecraft server
* Monitoring dashboard
* Docker management interface
* Future internal services
* Future client/demo deployments

### Docker Services

* Uptime Kuma
* Portainer
* Realm Architect Site

### Automation and Reliability

* systemd services
* systemd timers
* automated Minecraft backups
* tested restore workflow
* safe restart automation
* Dynamic DNS update timer
* monitoring checks

## Architecture Overview

```text
Internet
   |
   | DNS / Dynamic DNS
   v
realm-architect.dev
   |
   | Router Port Forwarding
   v
Debian Server
   ├── WireGuard VPN
   ├── SSH Key Authentication
   ├── Raw Nginx Reverse Proxy
   │   └── HTTPS via Certbot
   ├── Minecraft systemd Service
   ├── Automated Backups / Restarts
   └── Docker Services
       ├── Uptime Kuma
       ├── Portainer
       └── Realm Architect Site
```

## Public Website Architecture

```text
Browser
   |
   | HTTPS
   v
lab.realm-architect.dev
   |
   v
Router TCP 443
   |
   v
Debian Server
   |
   v
Nginx Reverse Proxy
   |
   v
127.0.0.1:8080
   |
   v
realm-architect-site Docker Container
```

## Minecraft Architecture

```text
Minecraft Client
   |
   | mc.realm-architect.dev
   v
Minecraft SRV Record
   |
   | Port 25566
   v
Router Port Forwarding
   |
   v
Debian Server
   |
   v
Minecraft systemd Service
```

## Documentation

| Topic                                  | Description                                              |
| -------------------------------------- | -------------------------------------------------------- |
| [Architecture](docs/architecture.md)   | High-level infrastructure overview                       |
| [Networking](docs/networking.md)       | LAN, VPN and port forwarding design                      |
| [Domain and DNS](docs/domain-dns.md)   | Custom domain, subdomains, DDNS and Minecraft SRV record |
| [Reverse Proxy](docs/reverse-proxy.md) | Raw Nginx reverse proxy and HTTPS setup                  |
| [SSH Hardening](docs/ssh-hardening.md) | Key-based login and SSH security decisions               |
| [Backup Strategy](docs/backups.md)     | Automated backups and restore testing                    |
| [Docker](docs/docker.md)               | Container services and Compose structure                 |
| [Monitoring](docs/monitoring.md)       | Uptime Kuma checks and service visibility                |
| [Backend API](docs/backend.md) | ASP.NET Core API, live service status and Minecraft player status |
| [Client: Naturhof Bocholt](docs/client-naturhof.md) | Real German-language client website hosted on the lab |

## Configuration Examples

This repository includes example configuration files for the main infrastructure components.

### systemd Examples

| Area       | Path                                         | Purpose                                                     |
| ---------- | -------------------------------------------- | ----------------------------------------------------------- |
| Minecraft  | [`systemd/minecraft/`](systemd/minecraft/)   | Minecraft service, backup timer and safe restart automation |
| IONOS DDNS | [`systemd/ionos-ddns/`](systemd/ionos-ddns/) | Dynamic DNS update service and timer                        |
| Backend API | [`systemd/backend/`](systemd/backend/) | ASP.NET Core backend API service |

### Docker Compose Examples

Docker services are organized by responsibility.

| Area              | Path                                   | Purpose                                   |
| ----------------- | -------------------------------------- | ----------------------------------------- |
| Internal Services | [`docker/internal/`](docker/internal/) | Private admin and monitoring services     |
| Public Services   | [`docker/public/`](docker/public/)     | Public-facing services owned by the lab   |
| Client Services   | [`docker/clients/`](docker/clients/)   | Future client or demo website deployments |

Current Docker service examples:

| Service              | Path                                                                         | Purpose                                                 |
| -------------------- | ---------------------------------------------------------------------------- | ------------------------------------------------------- |
| Uptime Kuma          | [`docker/internal/uptime-kuma/`](docker/internal/uptime-kuma/)               | Internal monitoring dashboard                           |
| Portainer            | [`docker/internal/portainer/`](docker/internal/portainer/)                   | Internal Docker management interface                    |
| Realm Architect Site | [`docker/public/realm-architect-site/`](docker/public/realm-architect-site/) | Public lab landing page served behind the reverse proxy |

### Nginx Examples

| Area          | Path                                               | Purpose                                                               |
| ------------- | -------------------------------------------------- | --------------------------------------------------------------------- |
| Reverse Proxy | [`nginx/sites-available/`](nginx/sites-available/) | Example raw Nginx reverse proxy configuration for the public lab site |

## Repository Structure

```text
realm-architect-lab/
├── docker/
│   ├── internal/
│   │   ├── uptime-kuma/
│   │   └── portainer/
│   ├── public/
│   │   └── realm-architect-site/
│   └── clients/
├── docs/
├── nginx/
│   └── sites-available/
├── systemd/
│   ├── minecraft/
│   └── ionos-ddns/
├── screenshots/
└── notes/
```

## What I Learned

* Managing Linux services with systemd
* Creating and testing systemd timers
* Securing SSH access with public key authentication
* Setting up WireGuard VPN for remote administration
* Using custom DNS records and Dynamic DNS
* Configuring a Minecraft SRV record for clean server access
* Running Docker services with Docker Compose
* Organizing services by responsibility
* Building automated backup and restart workflows
* Verifying backup usability through a non-destructive restore test
* Setting up raw Nginx as a reverse proxy
* Enabling HTTPS with Let's Encrypt and Certbot
* Monitoring self-hosted services with Uptime Kuma
* Documenting infrastructure in a GitHub-based portfolio repository

## Project Goal

The goal of this project is to build and document a professional self-hosted infrastructure lab around Linux administration, service hosting, automation and secure remote access.

Minecraft is used as the first major real-world service because it combines networking, process management, backups, monitoring, public DNS and user-facing availability.

The lab is designed as a long-term portfolio project for learning and demonstrating practical skills in:

* Linux server administration
* secure remote access
* networking and VPNs
* DNS and Dynamic DNS
* reverse proxy configuration
* HTTPS certificate management
* service management with systemd
* backup automation
* Docker and containerized services
* monitoring and uptime checks
* webserver deployment
* future internal service and panel development

## Roadmap

* [x] Debian server setup
* [x] SSH key authentication
* [x] WireGuard VPN
* [x] DDNS
* [x] Custom domain setup
* [x] Minecraft SRV record for clean server address
* [x] IONOS Dynamic DNS automation
* [x] Minecraft systemd service
* [x] Automated backups
* [x] Backup restore test
* [x] Automated safe restarts
* [x] Docker setup
* [x] Docker service structure by responsibility
* [x] Uptime Kuma monitoring
* [x] Portainer
* [x] Public Realm Architect site
* [x] Raw Nginx reverse proxy
* [x] HTTPS with Certbot
* [x] Dedicated Minecraft page
* [x] Public HTTPS monitoring in Uptime Kuma
* [ ] Reverse proxy documentation review
* [ ] Backup storage on a second disk
* [ ] Docker-based Minecraft test server
* [x] Basic backend health API
* [x] Live service status API
* [x] Minecraft player status API
* [x] Frontend API integration
* [ ] Minecraft whitelist request concept
* [ ] Basic server provisioning panel concept

## Future Direction

The long-term direction of this lab is to grow from a single self-hosted infrastructure setup into a small service hosting and automation environment.

Possible future additions:

* Pi-hole or local DNS service
* Samba or private file storage
* public status page
* internal dashboard
* backend API
* Minecraft whitelist request system
* Docker-based test servers
* small server management panel
* client/demo website hosting structure
* second-disk backup target

## Security Boundaries

Publicly exposed:

* `lab.realm-architect.dev`
* `mc.realm-architect.dev`
* `vpn.realm-architect.dev`

Kept private:

* SSH
* Portainer
* Uptime Kuma admin interface
* internal Docker services
* private keys
* API keys
* Dynamic DNS update URLs
* RCON passwords

Sensitive values are never committed to this repository. Public examples use placeholders where required.

## Status

This project is actively being built, tested and documented.
