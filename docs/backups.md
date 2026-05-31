# Backup Strategy

## Overview

Backups are a core part of the Realm Architect Lab.

The Minecraft server is automatically backed up using a custom Bash script and a systemd timer.

The goal is to protect world data and server configuration from accidental loss, corruption, or failed changes.

## What Gets Backed Up

The current backup strategy archives the full Minecraft server directory:

```text
/srv/games/minecraft