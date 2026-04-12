const API_BASE_URL = "https://localhost:7193";

function getAccessToken() {
  return localStorage.getItem("accessToken");
}

function getRefreshToken() {
  return localStorage.getItem("refreshToken");
}

function saveTokens(accessToken, refreshToken) {
  localStorage.setItem("accessToken", accessToken);
  localStorage.setItem("refreshToken", refreshToken);
}

function clearTokens() {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
}

async function tryRefreshToken() {
  const refreshToken = getRefreshToken();

  if (!refreshToken) {
    clearTokens();
    return null;
  }

  const response = await fetch(`${API_BASE_URL}/api/Auth/refresh`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      refreshToken,
    }),
  });

  if (!response.ok) {
    clearTokens();
    return null;
  }

  const data = await response.json();

  if (!data?.accessToken || !data?.refreshToken) {
    clearTokens();
    return null;
  }

  saveTokens(data.accessToken, data.refreshToken);
  return data.accessToken;
}

export async function authFetch(url, options = {}) {
  let accessToken = getAccessToken();

  const makeRequest = (token) =>
    fetch(url, {
      ...options,
      headers: {
        ...(options.headers || {}),
        Authorization: `Bearer ${token}`,
      },
    });

  let response = await makeRequest(accessToken);

  if (response.status !== 401) {
    return response;
  }

  const newAccessToken = await tryRefreshToken();

  if (!newAccessToken) {
    window.location.href = "/";
    throw new Error("Сессия истекла");
  }

  response = await makeRequest(newAccessToken);
  return response;
}

export async function logoutRequest() {
  const refreshToken = getRefreshToken();

  try {
    if (refreshToken) {
      await fetch(`${API_BASE_URL}/api/Auth/logout`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${getAccessToken()}`,
        },
        body: JSON.stringify({
          refreshToken,
        }),
      });
    }
  } finally {
    clearTokens();
  }
}

export { API_BASE_URL, saveTokens, clearTokens, getAccessToken, getRefreshToken };