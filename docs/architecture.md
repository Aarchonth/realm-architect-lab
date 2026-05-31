# Architecture

## Overview

Realm Architect Lab is a self-hosted infrastructure project built on a Debian server.

The goal is to create a practical homelab environment for learning and demonstrating Linux administration, secure remote access, game server hosting, automation, monitoring, and Docker-based services.

## High-Level Architecture

```text
Internet
   |
   | DDNS
   v
Speedport Router
   |
   | Port Forwarding
   | - WireGuard UDP 51820
   | - Minecraft TCP 25565
   v
Debian Server
   |
   ├── SSH Key Authentication
   ├── WireGuard VPN
   ├── Minecraft Server (systemd)
   ├── Automated Backups (systemd timer)
   ├── Automated Restarts (systemd timer)
   └── Docker
       ├── Uptime Kuma
       ├── Portainer
       └── Nginx Test Webserver