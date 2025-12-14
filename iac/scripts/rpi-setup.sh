#!/usr/bin/env bash



# Removes any existing "Port" lines from /etc/ssh/sshd_config and appends a single commented line "#Port 2300".
set -euo pipefail

if [ "$EUID" -ne 0 ]; then
    exec sudo "$0" "$@"
fi

cfg="/etc/ssh/sshd_config"
bak="${cfg}.bak.$(date +%s)"
cp -a -- "$cfg" "$bak"

tmp="$(mktemp)"
# remove any line that is "Port ..." whether commented or not, then append the commented port
sed -E '/^[[:space:]]*#?[[:space:]]*Port\b.*$/d' "$cfg" > "$tmp"
printf '%s\n' "#Port 2300" >> "$tmp"

mv -- "$tmp" "$cfg"
# preserve original ownership/permissions if possible
chown --reference="$bak" "$cfg" || true
chmod --reference="$bak" "$cfg" || true

echo "Updated $cfg (backup at $bak)."


## Setup UFW
sudo apt install ufw -y
sudo ufw allow 2300/tcp
sudo ufw delete allow 22/tcp
sudo ufw enable