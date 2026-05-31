# Networking

## Overview

This document describes the networking setup used in the Realm Architect Lab.

The goal of the network design is to provide stable local access, secure remote administration, and controlled public access for selected services.

## Network Layout

```text
Internet
   |
   | DDNS
   v
Edge Router / Home Gateway
   |
   | NAT / Port Forwarding
   | - WireGuard UDP 51820
   | - Minecraft TCP 25565
   v
Debian Server
   |
   ├── SSH Key Authentication
   ├── WireGuard VPN
   ├── Minecraft Server
   └── Docker Services
```

## Local Network

The Debian server uses a fixed local IP address assigned through the router's DHCP reservation feature.

```text
Server IP: 192.168.2.49
```

Using a router-based DHCP reservation keeps the server reachable at the same local IP address without manually hardcoding network settings inside Debian.

This makes the setup easier to maintain and reduces the risk of incorrect gateway or DNS configuration.

## DDNS

Dynamic DNS is used to make the server reachable from outside the local network, even if the public IP address changes.

Instead of connecting directly to a changing public IP address, clients use a DDNS hostname.

```text
<DDNS_HOSTNAME>
```

The DDNS hostname points to the current public IP address of the home network.

## NAT and Port Forwarding

The edge router forwards selected incoming traffic to the Debian server.

Current public-facing services:

```text
WireGuard VPN: UDP 51820
Minecraft Server: TCP 25565
```

SSH is intentionally not exposed directly to the internet.

Remote administration is done through WireGuard VPN first, then SSH is accessed through the internal server IP.

## WireGuard VPN

WireGuard provides secure remote access to the internal network.

Remote devices connect to the VPN and can then reach internal services as if they were inside the local network.

Example internal access after VPN connection:

```text
SSH:        admin@192.168.2.49
Minecraft: 192.168.2.49:25565
Web Lab:   http://192.168.2.49:8080
```

Each client device has its own WireGuard key pair and its own VPN IP address.

Example VPN client layout:

```text
VPN Server:      10.8.0.1
Main Laptop:     10.8.0.2
Secondary Laptop:10.8.0.3
Mobile Client:   10.8.0.4
```

## SSH Access

SSH access is secured with public key authentication.

Password-based login and root login are disabled.

This means only devices with an authorized SSH private key can log in to the server.

SSH is available only from:

* the local network
* trusted VPN clients

## Internal Services

Some services are intentionally kept internal and are only reachable through LAN or VPN.

Examples:

```text
Portainer:    https://192.168.2.49:9443
Uptime Kuma:  http://192.168.2.49:3001
Nginx Lab:    http://192.168.2.49:8080
```

Keeping management interfaces private reduces the exposed attack surface.

## Security Notes

The following values should not be committed to this repository:

* real WireGuard private keys
* SSH private keys
* real DDNS credentials
* router login information
* RCON passwords
* public IP history
* personal device identifiers

Public documentation should use placeholders such as:

```text
<DDNS_HOSTNAME>
<CLIENT_PUBLIC_KEY>
<SERVER_PUBLIC_KEY>
<PRIVATE_KEY>
<RCON_PASSWORD>
```

## Design Decision

The main security decision in this setup is:

```text
Expose only what needs to be public.
Keep administration behind VPN.
```

Minecraft is exposed publicly for players.

WireGuard is exposed publicly for secure remote access.

SSH, Portainer, Uptime Kuma, and internal web services are kept private behind the VPN.
