# Reverse Proxy and HTTPS

## Overview

This document describes the reverse proxy and HTTPS setup used in the Realm Architect Lab.

The public lab website is available at:

```text
https://lab.realm-architect.dev
```

The site is served through a raw Nginx reverse proxy running directly on the Debian server.

## Goal

The goal of this setup is to expose a public HTTPS website while keeping the actual Docker service behind an internal port.

Public access:

```text
https://lab.realm-architect.dev
```

Internal service:

```text
http://127.0.0.1:8080
```

## Architecture

```text
Browser
   |
   | HTTPS
   v
lab.realm-architect.dev
   |
   | DNS
   v
Public Home IP
   |
   | Router Port Forwarding
   | TCP 80  -> Debian Server
   | TCP 443 -> Debian Server
   v
Debian Server
   |
   | Nginx Reverse Proxy
   v
127.0.0.1:8080
   |
   v
realm-architect-site Docker Container
```

## Components

### Nginx

Nginx runs directly on the Debian host and acts as the reverse proxy.

It listens on public web ports:

```text
80
443
```

Nginx forwards requests for:

```text
lab.realm-architect.dev
```

to the internal Docker web service:

```text
http://127.0.0.1:8080
```

### Certbot

Certbot is used to request and manage Let's Encrypt TLS certificates.

The certificate enables HTTPS for:

```text
lab.realm-architect.dev
```

Certbot also configures Nginx to redirect HTTP traffic to HTTPS.

### Docker Web Service

The public lab website runs as a Docker-based Nginx container.

Repository path:

```text
docker/public/realm-architect-site/
```

The container serves static HTML, CSS and image assets.

It is reachable internally through:

```text
127.0.0.1:8080
```

## Nginx Configuration

The example Nginx reverse proxy configuration is stored in:

```text
nginx/sites-available/lab.realm-architect.dev.conf.example
```

Example structure:

```nginx
server {
    listen 80;
    listen [::]:80;

    server_name lab.realm-architect.dev;

    location / {
        proxy_pass http://127.0.0.1:8080;

        proxy_http_version 1.1;

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## Important Headers

The reverse proxy forwards important request information to the backend service.

```nginx
proxy_set_header Host $host;
```

Preserves the original domain name.

```nginx
proxy_set_header X-Real-IP $remote_addr;
```

Passes the client IP address to the backend.

```nginx
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
```

Keeps track of the forwarding chain.

```nginx
proxy_set_header X-Forwarded-Proto $scheme;
```

Tells the backend whether the original request used HTTP or HTTPS.

## Port Forwarding

The edge router forwards public web traffic to the Debian server.

```text
TCP 80  -> 192.168.2.49
TCP 443 -> 192.168.2.49
```

Port 80 is required for HTTP traffic and Let's Encrypt validation.

Port 443 is required for HTTPS traffic.

## Testing

Check if the internal Docker web service responds:

```bash
curl -I http://127.0.0.1:8080
```

Check if the public HTTP endpoint responds:

```bash
curl -I http://lab.realm-architect.dev
```

Check if the public HTTPS endpoint responds:

```bash
curl -I https://lab.realm-architect.dev
```

Test the Nginx configuration:

```bash
sudo nginx -t
```

Reload Nginx after configuration changes:

```bash
sudo systemctl reload nginx
```

Check Nginx status:

```bash
sudo systemctl status nginx
```

View Nginx access logs:

```bash
sudo tail -f /var/log/nginx/access.log
```

View Nginx error logs:

```bash
sudo tail -f /var/log/nginx/error.log
```

## Logs

Nginx access and error logs are used to inspect public web traffic and troubleshoot problems.

The access log shows incoming requests, requested paths, HTTP status codes, client IPs, referrers and user agents.

Access log:

```bash
sudo tail -f /var/log/nginx/access.log
```

The error log shows Nginx errors, failed requests, proxy problems and configuration-related issues.

Error log:

```bash
sudo tail -f /var/log/nginx/error.log
```

Useful checks:

```bash
sudo tail -n 50 /var/log/nginx/access.log
sudo grep " 404 " /var/log/nginx/access.log
sudo grep "minecraft.html" /var/log/nginx/access.log
sudo grep -i "bot" /var/log/nginx/access.log
```

Example use cases:

* checking whether the public website receives requests
* finding broken links or missing files
* identifying HTTP errors such as `404` or `500`
* spotting bots or automated scanners
* troubleshooting reverse proxy issues

Log data can include IP addresses and user agents.

These values should be treated carefully and should not be published in the repository.

## Certificate Renewal

Certbot configures automatic certificate renewal.

A renewal test can be performed with:

```bash
sudo certbot renew --dry-run
```

If the dry run succeeds, certificates should renew automatically.

## Public and Private Boundaries

Public:

```text
lab.realm-architect.dev
mc.realm-architect.dev
vpn.realm-architect.dev
```

Private:

```text
SSH
Portainer
Uptime Kuma admin interface
Internal Docker services
```

Administrative services remain behind the local network or WireGuard VPN.

## Security Notes

The reverse proxy exposes only selected services to the public internet.

Management interfaces such as Portainer and Uptime Kuma should not be exposed directly.

Sensitive values such as API keys, Dynamic DNS update URLs, private keys and passwords must not be committed to GitHub.

## Result

The public lab website is available through HTTPS:

```text
https://lab.realm-architect.dev
```

The setup demonstrates:

- raw Nginx reverse proxy configuration
- HTTPS with Let's Encrypt and Certbot
- public domain routing
- Docker-backed static website hosting
- separation between public services and private administration tools