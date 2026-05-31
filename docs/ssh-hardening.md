# SSH Hardening

## Overview

SSH is the primary administration method for the Debian server.

The goal of the SSH setup is to allow secure remote administration while reducing the risk of unauthorized access.

## Authentication Method

The server uses SSH key-based authentication.

Password-based SSH login is disabled.

This means that only devices with an authorized private key can access the server.

## Client Devices

The following device types are configured as trusted SSH clients:

- main laptop
- secondary laptop
- mobile client

Each device has its own SSH key pair.

The public keys are stored on the server in:

```text
~/.ssh/authorized_keys