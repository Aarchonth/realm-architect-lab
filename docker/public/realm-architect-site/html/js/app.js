async function loadApiStatus() {
  const statusLabel = document.getElementById("api-status-label");
  const statusDot = document.getElementById("api-status-dot");
  const apiService = document.getElementById("api-service");
  const apiEnvironment = document.getElementById("api-environment");
  const apiLastCheck = document.getElementById("api-last-check");

  if (!statusLabel || !statusDot || !apiService || !apiEnvironment || !apiLastCheck) {
    return;
  }

  try {
    const healthResponse = await fetch("/api/health");
    const infoResponse = await fetch("/api/info");

    if (!healthResponse.ok || !infoResponse.ok) {
      throw new Error("API responded with an error");
    }

    const health = await healthResponse.json();
    const info = await infoResponse.json();

    statusLabel.textContent = "API Online";
    statusDot.classList.remove("offline");

    apiService.textContent = health.service + " · v" + health.version;
    apiEnvironment.textContent = info.environment;
    apiLastCheck.textContent = formatDateTime(health.timestampUtc);
  } catch (error) {
    statusLabel.textContent = "API Offline";
    statusDot.classList.add("offline");

    apiService.textContent = "unavailable";
    apiEnvironment.textContent = "unknown";
    apiLastCheck.textContent = "failed";
  }
}

async function loadServicesStatus() {
  const servicesGrid = document.getElementById("services-grid");

  if (!servicesGrid) {
    return;
  }

  try {
    const response = await fetch("/api/services");

    if (!response.ok) {
      throw new Error("Services API responded with an error");
    }

    const data = await response.json();

    servicesGrid.innerHTML = "";

    data.services.forEach(function (service) {
      const card = document.createElement("article");
      card.className = "card service-card";

      const statusClass = service.status === "online" ? "online" : "offline";

      card.innerHTML = `
        <div class="service-status-row">
          <span class="mini-status-dot ${statusClass}"></span>
          <span>${service.status}</span>
        </div>

        <h3>${escapeHtml(service.name)}</h3>
        <p>${escapeHtml(service.publicName)}</p>
        <small>${escapeHtml(service.type)}</small>
      `;

      servicesGrid.appendChild(card);
    });
  } catch (error) {
    servicesGrid.innerHTML = `
      <article class="card service-card">
        <div class="service-status-row">
          <span class="mini-status-dot offline"></span>
          <span>offline</span>
        </div>

        <h3>Service Status Unavailable</h3>
        <p>The API could not load the service status right now.</p>
      </article>
    `;
  }
}

async function loadMinecraftStatus() {
  const title = document.getElementById("minecraft-status-title");
  const text = document.getElementById("minecraft-status-text");
  const count = document.getElementById("minecraft-player-count");
  const list = document.getElementById("minecraft-player-list");

  if (!title || !text || !count || !list) {
    return;
  }

  try {
    const response = await fetch("/api/minecraft");

    if (!response.ok) {
      throw new Error("Minecraft API responded with an error");
    }

    const data = await response.json();

    const isOnline = data.status === "online";

    title.textContent = isOnline ? "Minecraft is online" : "Minecraft is offline";
    text.textContent = isOnline
      ? "The server is reachable and player status is available."
      : "The server is currently not reachable.";

    const currentPlayers = data.currentPlayers ?? 0;
    const maxPlayers = data.maxPlayers ?? "--";

    count.textContent = `${currentPlayers} / ${maxPlayers}`;

    if (data.players && data.players.length > 0) {
  list.innerHTML = "";

  data.players.forEach(function (player) {
    const playerChip = document.createElement("div");
    playerChip.className = "player-chip";

    playerChip.innerHTML = `
      <img src="${escapeHtml(player.avatarUrl)}" alt="${escapeHtml(player.name)} skin head" />
      <span>${escapeHtml(player.name)}</span>
    `;

    list.appendChild(playerChip);
  });
} else {
  list.textContent = isOnline ? "No players online right now." : "Player list unavailable.";
}
  } catch (error) {
    title.textContent = "Minecraft status unavailable";
    text.textContent = "The API could not load Minecraft status right now.";
    count.textContent = "-- / --";
    list.textContent = "Unavailable.";
  }
}

function formatDateTime(timestampUtc) {
  const date = new Date(timestampUtc);

  const day = String(date.getDate()).padStart(2, "0");

  const month = date.toLocaleString("en-US", {
    month: "short"
  });

  const year = date.getFullYear();

  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  return `${day} ${month} ${year} · ${hours}:${minutes}`;
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

loadApiStatus();
loadServicesStatus();
loadMinecraftStatus();
