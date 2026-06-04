# Monitoring

## Overview

Monitoring is used in the Realm Architect Lab to observe service availability and detect outages early.

The project uses Uptime Kuma as the main monitoring dashboard.

Uptime Kuma runs as an internal Docker service and is intended to be accessed only through the local network or WireGuard VPN.

## Goal

The goal of monitoring is to verify that important infrastructure components are reachable and working as expected.

Monitoring helps answer questions such as:

* Is the Debian server reachable?
* Is SSH available?
* Is the Minecraft server online?
* Is the public lab website reachable through HTTPS?
* Does the public Minecraft domain resolve and connect correctly?
* Are public DNS names pointing to the correct services?

## Uptime Kuma

Uptime Kuma is deployed with Docker Compose.

Repository path:

```text
docker/internal/uptime-kuma/
```

Server-side path:

```text
/srv/docker/internal/uptime-kuma/
```

Uptime Kuma is treated as an internal administration service.

It should not be exposed directly to the public internet.

Access should happen through:

```text
Local network
WireGuard VPN
```

## Current Monitoring Targets

The current monitoring setup includes internal and public checks.

| Monitor            | Type              | Target                            | Purpose                                               |
| ------------------ | ----------------- | --------------------------------- | ----------------------------------------------------- |
| Debian Server      | Ping / Host check | `192.168.2.49`                    | Checks if the server is reachable inside the network  |
| SSH Server         | TCP Port          | `192.168.2.49:22`                 | Checks if SSH is reachable internally                 |
| Minecraft Server   | TCP Port          | `mc.realm-architect.dev:25566`    | Checks if the Minecraft server port is reachable      |
| Public Lab Website | HTTP(s)           | `https://lab.realm-architect.dev` | Checks the full public HTTPS path                     |
| Minecraft Domain   | TCP Port          | `mc.realm-architect.dev:25566`    | Checks the public Minecraft domain and forwarded port |
| VPN DNS            | DNS               | `vpn.realm-architect.dev`         | Checks whether the VPN hostname resolves correctly    |

## Public Website Monitoring

The public lab website is monitored through:

```text
https://lab.realm-architect.dev
```

This check verifies the full public request path:

```text
DNS
   |
   v
Public IP
   |
   v
Router Port Forwarding
   |
   v
Nginx Reverse Proxy
   |
   v
realm-architect-site Docker Container
```

This is more valuable than only checking the internal container port because it confirms that the public HTTPS endpoint works from the outside.

## Minecraft Monitoring

The Minecraft server is reachable through:

```text
mc.realm-architect.dev
```

The public Minecraft service uses a Minecraft SRV record so that players do not need to manually enter the port.

Internally, the public Minecraft port is:

```text
25566
```

A TCP port monitor can be used for:

```text
mc.realm-architect.dev:25566
```

This verifies that the public Minecraft endpoint is reachable.

## VPN Monitoring

WireGuard uses:

```text
vpn.realm-architect.dev
```

WireGuard runs on:

```text
UDP 51820
```

A normal TCP port monitor is not suitable for WireGuard because WireGuard does not listen on TCP.

For this reason, the VPN endpoint is monitored through a DNS check instead.

Recommended monitor:

```text
Type: DNS
Hostname: vpn.realm-architect.dev
Record Type: A
Resolver: 1.1.1.1
```

This does not prove that WireGuard itself is working, but it verifies that the VPN hostname resolves correctly.

A real WireGuard availability test would require a UDP-capable check or an external client test.

## Internal vs Public Monitoring

Internal checks verify local network availability.

Examples:

```text
192.168.2.49
192.168.2.49:22
192.168.2.49:3001
192.168.2.49:9443
```

Public checks verify the real external path.

Examples:

```text
https://lab.realm-architect.dev
mc.realm-architect.dev:25566
vpn.realm-architect.dev DNS
```

Both types are useful.

Internal monitoring helps detect local service failures.

Public monitoring helps detect problems with DNS, router forwarding, HTTPS, reverse proxy configuration or public service availability.

## Recommended Monitor Names

Recommended Uptime Kuma monitor names:

```text
Debian Server
SSH Server
Public Lab Website
Minecraft Server
Minecraft Domain
VPN DNS
```

Clear monitor names make the dashboard easier to understand later.

## Useful Checks

Check if Uptime Kuma is running:

```bash
docker ps
```

Check Uptime Kuma logs:

```bash
cd /srv/docker/internal/uptime-kuma
docker compose logs
```

Check the public lab website manually:

```bash
curl -I https://lab.realm-architect.dev
```

Check the Minecraft public port:

```bash
nc -vz mc.realm-architect.dev 25566
```

Check DNS resolution:

```bash
nslookup vpn.realm-architect.dev
nslookup mc.realm-architect.dev
nslookup lab.realm-architect.dev
```

Check the current public IP:

```bash
curl -4 ifconfig.me
```

## Security Notes

Uptime Kuma contains information about internal services and infrastructure.

It should not be exposed publicly without a strong authentication and access-control concept.

Current design decision:

```text
Uptime Kuma stays private.
Access only through LAN or WireGuard VPN.
```

Public users should only see intentionally exposed services such as:

```text
lab.realm-architect.dev
mc.realm-architect.dev
```

## Future Improvements

Planned monitoring improvements:

* add notification alerts
* add public HTTPS certificate expiry monitoring
* add DNS checks for all important subdomains
* add backup freshness monitoring
* add Minecraft process or RCON-based status checks
* add external monitoring from outside the home network
* document alerting rules and response steps
