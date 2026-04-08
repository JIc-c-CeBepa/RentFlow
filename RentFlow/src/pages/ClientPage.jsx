import { useEffect, useState } from "react";
import "../styles/clientPage.css";

function ClientPage() {
  const [loading, setLoading] = useState(true);
  const [ownerId, setOwnerId] = useState(null);
  const [code, setCode] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const token = localStorage.getItem("token");

  useEffect(() => {
    loadUser();
  }, []);

  async function loadUser() {
    try {
      setLoading(true);
      setError("");

      const response = await fetch("https://localhost:7182/api/Auth/me", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Не удалось получить данные пользователя");
      }

      const data = await response.json();
      setOwnerId(data.ownerId ?? null);
    } catch (err) {
      setError(err.message || "Ошибка загрузки пользователя");
    } finally {
      setLoading(false);
    }
  }

  async function handleAttachOwner() {
  try {
    setError("");
    setSuccess("");

    if (!code.trim()) {
      setError("Введите ссылку или код");
      return;
    }

    let preparedCode = code.trim();

    if (preparedCode.includes("/")) {
      const parts = preparedCode.split("/").filter(Boolean);
      preparedCode = parts[parts.length - 1];
    }

    setIsSubmitting(true);

    const response = await fetch("https://localhost:7182/api/Property/AddOwnerLinkToUser", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        code: preparedCode,
      }),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data?.message || data?.title || "Ошибка привязки");
    }

    setSuccess("Владелец успешно прикреплен");
    setOwnerId(1);
    setCode("");
  } catch (err) {
    setError(err.message || "Ошибка привязки");
  } finally {
    setIsSubmitting(false);
  }
}

  if (loading) {
    return (
      <div className="client-page-loading">
        Загрузка...
      </div>
    );
  }

  if (ownerId) {
    return (
      <div className="client-page">
        <header className="client-topbar">
          <div className="client-logo">RentFlow</div>
          <button className="client-profile-btn">👤</button>
        </header>

        <main className="client-page-content">
          <div className="client-connected-box">
            <h1>Личный кабинет клиента</h1>
            <p>Пользователь уже привязан к владельцу.</p>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="client-page">
      <header className="client-topbar">
        <div className="client-logo">RentFlow</div>
        <button className="client-profile-btn">👤</button>
      </header>

      <main className="client-page-content">
        <div className="client-link-card">
          <h2 className="client-link-title">Подключение к владельцу</h2>
          <p className="client-link-subtitle">
            Введите ссылку или код, который вам отправил владелец
          </p>

          <input
            type="text"
            className="client-link-input"
            placeholder="Введите ссылку или код"
            value={code}
            onChange={(e) => setCode(e.target.value)}
          />

          {error && <div className="client-message client-error">{error}</div>}
          {success && <div className="client-message client-success">{success}</div>}

          <button
            className="client-attach-btn"
            onClick={handleAttachOwner}
            disabled={isSubmitting}
          >
            {isSubmitting ? "Отправка..." : "Прикрепить"}
          </button>
        </div>
      </main>
    </div>
  );
}

export default ClientPage;