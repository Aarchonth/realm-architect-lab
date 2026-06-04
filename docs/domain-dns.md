# Domain and DNS

## Overview

This document describes the domain and DNS setup used in the Realm Architect Lab.

The goal is to provide clean public hostnames for selected services while keeping administrative interfaces private.

The project uses the domain:

```text
realm-architect.dev
```

This domain is used as part of the public-facing identity of the lab.

## Domain Structure

The current domain structure is planned around clear service-specific subdomains.

```text
vpn.realm-architect.dev
mc.realm-architect.dev
lab.realm-architect.dev
```

Each subdomain has a specific role.

| Subdomain                 | Purpose                                |
| ------------------------- | -------------------------------------- |
| `vpn.realm-architect.dev` | WireGuard VPN endpoint                 |
| `mc.realm-architect.dev`  | Minecraft server address               |
| `lab.realm-architect.dev` | Future public lab / portfolio web page |

## DNS Records

The current setup uses A records for the main service subdomains.

Example structure:

```text
vpn.realm-architect.dev  -> public home IP
mc.realm-architect.dev   -> public home IP
lab.realm-architect.dev  -> public home IP
```

The public IP points to the home network. The edge router then forwards selected ports to the internal Debian server.

## Internal Server Address

The Debian server uses a fixed local IP address assigned through the router's DHCP reservation feature.

```text
Debian Server: 192.168.2.49
```

This local IP is used internally by services, monitoring, SSH access, and port forwarding rules.

## Port Forwarding

The edge router forwards only selected public ports to the Debian server.

Current public-facing port forwards:

```text
WireGuard VPN: UDP 51820 -> 192.168.2.49
Minecraft:     TCP 25566 -> 192.168.2.49
```

Future web services may use:

```text
HTTP:  TCP 80  -> reverse proxy
HTTPS: TCP 443 -> reverse proxy
```

Administrative services should not be exposed directly to the public internet.

## WireGuard DNS

The WireGuard VPN uses:

```text
vpn.realm-architect.dev
```

Example WireGuard client endpoint:

```text
Endpoint = vpn.realm-architect.dev:51820
```

The VPN provides secure remote access to internal services.

After connecting to WireGuard, internal services can be accessed through the private server IP.

Examples:

```text
SSH:        admin@192.168.2.49
Portainer:  https://192.168.2.49:9443
Uptime Kuma: http://192.168.2.49:3001
```

## Minecraft DNS

The Minecraft server uses:

```text
mc.realm-architect.dev
```

The Minecraft server runs on a non-default public port:

```text
25566
```

To allow players to connect without entering the port manually, an SRV record is used.

## Minecraft SRV Record

The SRV record allows players to join using only:

```text
mc.realm-architect.dev
```

instead of:

```text
mc.realm-architect.dev:25566
```

The SRV record points Minecraft clients to the correct port.

Example SRV record:

```text
_minecraft._tcp.mc.realm-architect.dev
```

SRV target:

```text
mc.realm-architect.dev
```

SRV settings:

```text
Priority: 0
Weight: 5
Port: 25566
Target: mc.realm-architect.dev
```

This was tested successfully with:

```powershell
nslookup -type=SRV _minecraft._tcp.mc.realm-architect.dev
```

Expected result:

```text
priority = 0
weight   = 5
port     = 25566
hostname = mc.realm-architect.dev
```

The Minecraft client can successfully join using:

```text
mc.realm-architect.dev
```

## Dynamic DNS

Because the public home IP can change, the domain setup uses Dynamic DNS.

IONOS Dynamic DNS is configured through an update URL generated through the IONOS DNS API.

The Debian server calls this update URL regularly so that the DNS records stay updated with the current public IP.

The update URL is treated as a secret and must not be committed to GitHub.

## Dynamic DNS Update Script

The update URL is stored in a protected environment file:

```text
/etc/realm-architect/ionos-ddns.env
```

Example placeholder:

```bash
IONOS_DDNS_UPDATE_URL='<IONOS_DDNS_UPDATE_URL>'
```

The update script is stored at:

```text
/usr/local/bin/ionos-ddns-update.sh
```

The script loads the update URL from the protected environment file and calls it with `curl`.

Example script structure:

```bash
#!/bin/bash

set -e

source /etc/realm-architect/ionos-ddns.env

if [ -z "$IONOS_DDNS_UPDATE_URL" ]; then
  echo "IONOS_DDNS_UPDATE_URL is not set."
  exit 1
fi

curl -fsS "$IONOS_DDNS_UPDATE_URL" >/dev/null

echo "IONOS Dynamic DNS update completed."
```

## Dynamic DNS systemd Timer

The Dynamic DNS update is automated with a systemd service and timer.

Service file:

```text
/etc/systemd/system/ionos-ddns-update.service
```

Timer file:

```text
/etc/systemd/system/ionos-ddns-update.timer
```

The timer runs shortly after boot and then regularly afterwards.

Example timer behavior:

```text
OnBootSec=2min
OnUnitActiveSec=10min
Persistent=true
```

This keeps the DNS records updated without manual work.

## Testing DNS

A records can be tested with:

```powershell
nslookup vpn.realm-architect.dev
nslookup mc.realm-architect.dev
nslookup lab.realm-architect.dev
```

The Minecraft SRV record can be tested with:

```powershell
nslookup -type=SRV _minecraft._tcp.mc.realm-architect.dev
```

The current public IP can be checked from the Debian server with:

```bash
curl -4 ifconfig.me
```

The DNS results should match the current public IP.

## Public vs Private Services

Only selected services are intended to be publicly reachable.

Public services:

```text
WireGuard VPN
Minecraft server
Future HTTPS web page
```

Private services:

```text
SSH
Portainer
Uptime Kuma admin interface
Internal Docker services
```

Private services should only be reachable through the local network or WireGuard VPN.

## Security Notes

The following values must not be committed to GitHub:

* IONOS API keys
* IONOS Dynamic DNS update URLs
* private WireGuard keys
* SSH private keys
* router login credentials
* public IP history if privacy is required
* passwords or tokens

Public documentation should use placeholders such as:

```text
<IONOS_DDNS_UPDATE_URL>
<PRIVATE_KEY>
<API_KEY>
<DDNS_HOSTNAME>
<RCON_PASSWORD>
```

## Design Decision

The main design decision is:

```text
Use public DNS names for clean service access.
Expose only necessary public services.
Keep administration behind WireGuard VPN.
```

This keeps the setup clean for users while reducing the public attack surface.

## Future Improvements

Planned improvements:

* reverse proxy with HTTPS
* Let's Encrypt certificates
* public lab / portfolio page
* dedicated status page
* documentation for DNS automation files
* optional migration away from temporary legacy DDNS fallback
