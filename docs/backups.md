# Backup Strategy
# Backup Strategy

## Overview

Backups are a core part of the Realm Architect Lab.

The Minecraft server is automatically backed up using a custom Bash script and a systemd timer.

The goal is to protect world data and server configuration from accidental loss, corruption, or failed changes.

## What Gets Backed Up

The current backup strategy archives the full Minecraft server directory:

```text
/srv/games/minecraft
```

This includes:

* world data
* server configuration
* whitelist and operator files
* logs
* server icon
* server jar
* supporting files

Backing up the full directory makes restore operations simpler because the complete server state is preserved.

## Backup Location

Backups are stored in:

```text
/srv/backups/minecraft
```

Each backup is stored as a compressed `.tar.gz` archive with a timestamp in the filename.

Example:

```text
minecraft-2026-05-29_17-31.tar.gz
```

## Backup Script

The backup process is handled by a custom Bash script:

```text
/usr/local/bin/minecraft-backup.sh
```

An example version of this script is included in this repository:

```text
systemd/minecraft-backup.sh.example
```

The script performs the following steps:

1. creates a timestamp
2. triggers a Minecraft save through RCON
3. waits briefly
4. creates a compressed archive
5. deletes backups older than the retention period

## Minecraft Save Process

Before creating the archive, the script runs:

```text
save-all flush
```

This forces Minecraft to write world data to disk before the backup archive is created.

This reduces the risk of backing up incomplete or outdated world data.

## Backup Rotation

Old backups are automatically removed after a retention period.

Current retention example:

```text
14 days
```

This prevents the backup directory from growing endlessly.

## systemd Automation

Backups are scheduled with a systemd timer.

The backup timer runs daily at:

```text
02:00
```

The corresponding files are:

```text
systemd/minecraft-backup.service
systemd/minecraft-backup.timer
```

The service is configured as a `oneshot` service because it performs one backup task and then exits.

## Manual Backup

A manual backup can be triggered with:

```bash
sudo systemctl start minecraft-backup.service
```

or by running the script directly:

```bash
sudo /usr/local/bin/minecraft-backup.sh
```

## Checking Backups

List existing backups:

```bash
ls -lh /srv/backups/minecraft
```

Check the backup timer:

```bash
systemctl list-timers | grep minecraft
```

Check backup service logs:

```bash
journalctl -u minecraft-backup.service
```

## Restore Concept

A restore process would generally follow this idea:

1. stop the Minecraft service
2. move or archive the current server directory
3. extract the selected backup archive
4. restore ownership and permissions
5. start the Minecraft service again
6. verify server startup and world integrity

Example restore command pattern:

```bash
sudo systemctl stop minecraft
sudo tar -xzf /srv/backups/minecraft/<BACKUP_FILE>.tar.gz -C /
sudo systemctl start minecraft
```

## Restore Test

A non-destructive restore test was performed to verify that the backup archive can be extracted successfully.

The backup was not restored directly into the live Minecraft server directory. Instead, it was extracted into a separate test directory.

Test directory:

```text
/srv/backups/Testdirectory
```

Test command pattern:

```bash
sudo mkdir -p /srv/backups/Testdirectory
sudo tar -xzf /srv/backups/minecraft/<BACKUP_FILE>.tar.gz -C /srv/backups/Testdirectory
```

Because the backup archive was created from an absolute path, the extracted structure inside the test directory looked like this:

```text
/srv/backups/Testordner/srv/games/minecraft
```

The following files and directories were verified after extraction:

```text
world/
server.properties
server.jar
ops.json
whitelist.json
logs/
libraries/
versions/
```

This confirms that the backup archive contains the full Minecraft server directory and can be extracted successfully.

After verification, the temporary restore test directory can be removed:

```bash
sudo rm -rf /srv/backups/Testdirectory
```

### Result

The restore test was successful.

The backup archive could be extracted into a safe test directory, and the expected Minecraft server files were present.

This verifies that the backup process produces usable restore data.


## Important Notes

Backups currently live on the same physical machine as the Minecraft server.

This protects against accidental changes or world corruption, but it does not protect against full disk failure.

A future improvement is to store backups on a second disk or external system.

## Security Notes

The backup script uses RCON to trigger a Minecraft save before creating the archive.

The real RCON password must not be committed to GitHub.

Public examples should use placeholders such as:

```text
<RCON_PASSWORD>
```

## Future Improvements

Planned improvements:

* backup restore testing
* backup storage on a second physical disk
* offsite backup target
* backup integrity checks
* backup size monitoring
* optional compressed world-only backups
