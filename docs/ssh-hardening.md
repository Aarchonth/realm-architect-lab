# SSH Hardening

## Overview

SSH is the primary administration method for the Debian server in the Realm Architect Lab.

The goal of the SSH setup is to allow secure remote administration while reducing the risk of unauthorized access.

## Authentication Method

The server uses SSH key-based authentication.

Password-based SSH login is disabled.

This means that only devices with an authorized private key can access the server.

## Client Devices

The following device types are configured as trusted SSH clients:

* main laptop
* secondary laptop
* mobile client

Each device has its own SSH key pair.

The public keys are stored on the server in:

```text
~/.ssh/authorized_keys
```

The private keys remain on the client devices and must not be uploaded to GitHub or shared.

## Server Configuration

The SSH server configuration is managed through:

```text
/etc/ssh/sshd_config
```

Important settings:

```text
PubkeyAuthentication yes
PasswordAuthentication no
PermitRootLogin no
```

## Why Key-Based Authentication Is Used

SSH keys are more secure than password-based login because access requires possession of the correct private key.

A password can be guessed, reused, leaked, or brute-forced.

A private key should remain stored only on the trusted client device.

## Why Password Login Is Disabled

Password login is disabled to reduce the risk of brute-force attacks and leaked-password access.

With password authentication disabled, an attacker cannot log in through SSH using only a username and password.

## Why Root Login Is Disabled

Root login is disabled to prevent direct administrative login as the root user.

Instead, administration is performed through a regular user account with sudo permissions.

This provides better accountability and reduces the risk of accidental destructive actions.

## Access Model

SSH is not exposed directly to the public internet.

Administration from outside the local network is done through WireGuard VPN first.

```text
Remote Client
   |
   | WireGuard VPN
   v
Internal Network
   |
   | SSH
   v
Debian Server
```

This keeps SSH reachable for trusted devices while reducing the public attack surface.

## Local and Remote Access

SSH can be used from inside the local network:

```bash
ssh <USER>@<SERVER_IP>
```

Remote access works by connecting to the WireGuard VPN first and then using the same internal server IP.

Example:

```bash
ssh <USER>@192.168.2.49
```

## Useful Commands

Check SSH service status:

```bash
sudo systemctl status ssh
```

Restart the SSH service:

```bash
sudo systemctl restart ssh
```

View the SSH server configuration:

```bash
sudo nano /etc/ssh/sshd_config
```

Check active SSH sessions:

```bash
who
```

View recent SSH-related logs:

```bash
journalctl -u ssh
```

Follow SSH logs live:

```bash
journalctl -u ssh -f
```

## Recommended SSH Configuration Example

Example `/etc/ssh/sshd_config` hardening settings:

```text
PubkeyAuthentication yes
PasswordAuthentication no
PermitRootLogin no
```

After changing the configuration, restart SSH:

```bash
sudo systemctl restart ssh
```

Before closing the current SSH session, always test a second login session to avoid locking yourself out.

## Security Notes

Never commit the following to GitHub:

* SSH private keys
* real passwords
* tokens
* full private configuration files containing secrets
* private device identifiers if privacy is required

The following file should not be uploaded directly if it contains real keys tied to personal devices:

```text
~/.ssh/authorized_keys
```

Public documentation should use placeholders such as:

```text
<USER>
<SERVER_IP>
<SSH_PUBLIC_KEY>
<PRIVATE_KEY>
```

## Result

The final SSH setup provides:

* secure key-based login
* disabled password authentication
* disabled root login
* administration through a regular sudo user
* remote administration through WireGuard VPN
* reduced public attack surface
