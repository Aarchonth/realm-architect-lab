# Backend API

## Overview

The Realm Architect Lab includes a small backend API built with ASP.NET Core Minimal API.

The backend is used to expose selected infrastructure status information to the public lab website.

The API is available through the existing public domain:

```text
https://lab.realm-architect.dev/api/
```

The API itself does not listen publicly. It runs locally on the Debian server and is exposed through the Nginx reverse proxy.

## Goal

The goal of the backend API is to turn the public lab website from a static page into a small live infrastructure dashboard.

The backend currently provides:

* API health status
* project information
* live service status checks
* Minecraft server status
* Minecraft current player count
* Minecraft online player names
* Minecraft skin head avatar URLs

## Architecture

```text
Browser
   |
   | HTTPS
   v
lab.realm-architect.dev
   |
   v
Nginx Reverse Proxy
   |
   | /          -> 127.0.0.1:8080
   | /api/      -> 127.0.0.1:5050
   v
ASP.NET Core Backend API
```

The static website is served by the `realm-architect-site` Docker container.

The backend API runs as a native systemd service on the Debian host.

## Runtime

The backend uses:

```text
ASP.NET Core Minimal API
.NET 8 SDK / Runtime
Debian 13 / Trixie
systemd
Nginx reverse proxy
```

.NET was installed manually under:

```text
/opt/dotnet
```

The `dotnet` binary is linked through:

```text
/usr/local/bin/dotnet
```

This avoids using an incompatible Microsoft APT repository on Debian Trixie.

## Server-Side Path

The backend application is stored on the Debian server at:

```text
/srv/apps/realm-architect-api
```

The published release build is stored at:

```text
/srv/apps/realm-architect-api/publish
```

## API Endpoints

### Health

```text
GET /api/health
```

Example response:

```json
{
  "status": "ok",
  "service": "realm-architect-api",
  "version": "0.2.0",
  "timestampUtc": "2026-06-04T19:06:00.9951722Z"
}
```

This endpoint is used by the public website to show whether the backend API is online.

### Info

```text
GET /api/info
```

Example response:

```json
{
  "project": "Realm Architect Lab",
  "description": "Self-hosted infrastructure API",
  "environment": "Production"
}
```

This endpoint provides basic project and runtime information.

### Services

```text
GET /api/services
```

Example response:

```json
{
  "generatedAtUtc": "2026-06-04T19:08:41.785165Z",
  "services": [
    {
      "name": "Backend API",
      "status": "online",
      "type": "api",
      "publicName": "lab.realm-architect.dev/api/health"
    },
    {
      "name": "Public Lab Website",
      "status": "online",
      "type": "https",
      "publicName": "lab.realm-architect.dev"
    },
    {
      "name": "Minecraft Server",
      "status": "online",
      "type": "tcp",
      "publicName": "mc.realm-architect.dev"
    },
    {
      "name": "SSH Server",
      "status": "online",
      "type": "tcp",
      "publicName": "Private / VPN only"
    },
    {
      "name": "Nginx Reverse Proxy",
      "status": "online",
      "type": "https",
      "publicName": "lab.realm-architect.dev"
    }
  ]
}
```

This endpoint performs live checks for important services.

The public website uses this endpoint to render the live service status cards on the index page.

### Minecraft

```text
GET /api/minecraft
```

Example response without online players:

```json
{
  "name": "Realm Architect Minecraft",
  "address": "mc.realm-architect.dev",
  "status": "online",
  "currentPlayers": 0,
  "maxPlayers": 20,
  "players": [],
  "playerStatusAvailable": true,
  "checkedAtUtc": "2026-06-04T19:08:41.4880415Z"
}
```

Example response with online players:

```json
{
  "name": "Realm Architect Minecraft",
  "address": "mc.realm-architect.dev",
  "status": "online",
  "currentPlayers": 1,
  "maxPlayers": 20,
  "players": [
    {
      "name": "PlayerName",
      "avatarUrl": "https://mc-heads.net/avatar/PlayerName/48"
    }
  ],
  "playerStatusAvailable": true,
  "checkedAtUtc": "2026-06-04T19:08:41.4880415Z"
}
```

The public Minecraft page uses this endpoint to show:

* Minecraft online/offline state
* current player count
* maximum player count
* online player names
* Minecraft skin head images

## Service Checks

The backend performs simple live checks.

### Backend API

The backend reports itself as online when `/api/health` responds.

### Public Lab Website

The backend checks:

```text
https://lab.realm-architect.dev
```

This verifies the public HTTPS path.

### Minecraft Server

The backend checks the local Minecraft TCP port.

Internal Minecraft service port:

```text
127.0.0.1:25565
```

Public Minecraft port:

```text
mc.realm-architect.dev:25566
```

The public port is used by external players through router port forwarding and the Minecraft SRV record.

The backend checks the internal port because it runs on the same Debian server as the Minecraft server.

### SSH Server

The backend checks:

```text
127.0.0.1:22
```

The public website only displays this as a private/VPN-only service.

It does not expose internal SSH URLs.

### Nginx Reverse Proxy

The backend checks whether the local HTTPS port is reachable.

```text
127.0.0.1:443
```

## Minecraft Player Status

Minecraft player status is retrieved through RCON.

RCON is used to run the Minecraft command:

```text
list
```

The response is parsed to extract:

* current player count
* maximum player count
* online player names

The backend then generates Minecraft skin head avatar URLs for each player.

Example avatar URL pattern:

```text
https://mc-heads.net/avatar/<PLAYER_NAME>/48
```

## Environment Configuration

Sensitive Minecraft RCON values are not stored in the source code.

They are stored in a protected environment file:

```text
/etc/realm-architect/api.env
```

Example structure:

```text
MINECRAFT_RCON_PASSWORD=<RCON_PASSWORD>
MINECRAFT_RCON_PORT=25575
MINECRAFT_RCON_HOST=127.0.0.1
MINECRAFT_TCP_HOST=127.0.0.1
MINECRAFT_TCP_PORT=25565
```

This file must not be committed to GitHub.

Recommended permissions:

```bash
sudo chmod 600 /etc/realm-architect/api.env
sudo chown root:root /etc/realm-architect/api.env
```

## systemd Service

The API runs as a systemd service.

Service name:

```text
realm-architect-api.service
```

Service file:

```text
/etc/systemd/system/realm-architect-api.service
```

Example structure:

```ini
[Unit]
Description=Realm Architect API
After=network.target

[Service]
WorkingDirectory=/srv/apps/realm-architect-api/publish
ExecStart=/usr/local/bin/dotnet /srv/apps/realm-architect-api/publish/realm-architect-api.dll
Restart=always
RestartSec=5
User=adminaaron
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_ROOT=/opt/dotnet
EnvironmentFile=/etc/realm-architect/api.env

[Install]
WantedBy=multi-user.target
```

Useful commands:

```bash
sudo systemctl daemon-reload
sudo systemctl enable realm-architect-api
sudo systemctl start realm-architect-api
sudo systemctl restart realm-architect-api
sudo systemctl status realm-architect-api
```

View logs:

```bash
journalctl -u realm-architect-api -n 50
journalctl -u realm-architect-api -f
```

## Build and Deploy

The backend is published with:

```bash
cd /srv/apps/realm-architect-api
dotnet publish -c Release -o /srv/apps/realm-architect-api/publish
```

After publishing, restart the service:

```bash
sudo systemctl restart realm-architect-api
```

Test locally:

```bash
curl http://127.0.0.1:5050/api/health
curl http://127.0.0.1:5050/api/services
curl http://127.0.0.1:5050/api/minecraft
```

Test publicly:

```bash
curl https://lab.realm-architect.dev/api/health
curl https://lab.realm-architect.dev/api/services
curl https://lab.realm-architect.dev/api/minecraft
```

## Nginx Reverse Proxy Integration

The Nginx reverse proxy routes static website traffic and API traffic differently.

Website traffic:

```text
/ -> 127.0.0.1:8080
```

API traffic:

```text
/api/ -> 127.0.0.1:5050
```

Example Nginx location block:

```nginx
location /api/ {
    proxy_pass http://127.0.0.1:5050;

    proxy_http_version 1.1;

    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

This block must be placed inside the HTTPS server block for:

```text
lab.realm-architect.dev
```

## Frontend Integration

The public website uses JavaScript to call the backend API.

Frontend file:

```text
docker/public/realm-architect-site/html/js/app.js
```

The index page uses:

```text
/api/health
/api/info
/api/services
```

The Minecraft page uses:

```text
/api/minecraft
```

The frontend displays:

* API online/offline state
* backend version
* production environment
* last API check time
* live service status cards
* Minecraft online/offline state
* current Minecraft players
* player skin head images

## Public Website Changes

The public lab website now includes live data from the backend API.

### Index Page

```text
Live Project card
Live API status
Current service status section
Service cards loaded from /api/services
```

### Minecraft Page

```text
Live Minecraft server status
Current player count
Online player list
Minecraft skin head images
```

## Security Notes

The API exposes only selected non-sensitive infrastructure information.

The API must not expose:

* internal IP addresses
* private service URLs
* RCON passwords
* API keys
* SSH keys
* Dynamic DNS update URLs
* internal backup paths
* private admin endpoints

Private services such as Portainer and Uptime Kuma remain behind the local network or WireGuard VPN.

## Current Limitations

The current API is intentionally simple.

Limitations:

* no authentication
* no database
* no user accounts
* no admin actions
* no write operations
* simple TCP and HTTP checks only
* Minecraft player data depends on RCON availability
* avatar images depend on an external Minecraft avatar service

## Future Improvements

Planned backend improvements:

* add `/api/backup-status`
* check latest Minecraft backup freshness
* add Docker container status checks
* add Minecraft query/player details
* add internal-only admin endpoints
* add whitelist request form
* add SQLite database
* add admin login
* add small Realm Architect Panel
