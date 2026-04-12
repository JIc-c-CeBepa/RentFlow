import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import PropertyCard from "../components/PropertyCard";
import ProfileSidebar from "../components/ProfileSidebar";
import PropertyFilterSidebar from "../components/PropertyFilterSidebar";
import "../styles/clientPage.css";
import { toast } from "react-toastify";
import { API_BASE_URL, authFetch, logoutRequest } from "../api/authFetch";

function ClientPage() {
  const fileInputRef = useRef(null);
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [propertiesLoading, setPropertiesLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [avatarUploading, setAvatarUploading] = useState(false);
  const [profileSaving, setProfileSaving] = useState(false);

  const [ownerId, setOwnerId] = useState(null);
  const [profileOpen, setProfileOpen] = useState(false);
  const [filtersCollapsed, setFiltersCollapsed] = useState(false);

  const [code, setCode] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [properties, setProperties] = useState([]);

  const [filters, setFilters] = useState({
    minPrice: "",
    maxPrice: "",
    guestsCount: 1,
    needContactlessCheckIn: false,
    arrivalDate: null,
    departureDate: null,
  });

  const [user, setUser] = useState({
    id: null,
    firstName: "",
    lastName: "",
    middleName: "",
    phone: "",
    telegram: "",
    photo: "",
    roleId: null,
  });

  useEffect(() => {
    loadUser();
  }, []);

  useEffect(() => {
    if (ownerId) {
      loadProperties();
    } else {
      setProperties([]);
    }
  }, [ownerId]);

  async function handleLogout() {
    try {
      await logoutRequest();
    } finally {
      setProfileOpen(false);
      navigate("/");
    }
  }

  async function loadUser() {
    try {
      setLoading(true);
      setError("");

      const response = await authFetch(`${API_BASE_URL}/api/Auth/me`, {
        method: "GET",
      });

      if (!response.ok) {
        throw new Error("Не удалось получить данные пользователя");
      }

      const data = await response.json();

      setOwnerId(data.ownerId ?? null);
      setUser({
        id: data.id ?? null,
        firstName: data.firstName ?? "",
        lastName: data.lastName ?? "",
        middleName: data.middleName ?? "",
        phone: data.phone ?? "",
        telegram: data.telegram ?? "",
        photo: data.photo ?? "",
        roleId: data.roleId ?? null,
      });
    } catch (err) {
      setError(err.message || "Ошибка загрузки пользователя");
      toast.error(err.message || "Ошибка загрузки пользователя");
    } finally {
      setLoading(false);
    }
  }

  async function loadProperties(customFilters = filters) {
    try {
      setPropertiesLoading(true);
      setError("");

      const params = new URLSearchParams();

      if (customFilters.minPrice !== "" && customFilters.minPrice !== null) {
        params.append("minPrice", customFilters.minPrice);
      }

      if (customFilters.maxPrice !== "" && customFilters.maxPrice !== null) {
        params.append("maxPrice", customFilters.maxPrice);
      }

      if (customFilters.guestsCount) {
        params.append("guestsCount", customFilters.guestsCount);
      }

      if (customFilters.needContactlessCheckIn) {
        params.append("needContactlessCheckIn", "true");
      }

      if (customFilters.arrivalDate && customFilters.departureDate) {
        const arrivalDate = new Date(customFilters.arrivalDate)
          .toISOString()
          .split("T")[0];
        const departureDate = new Date(customFilters.departureDate)
          .toISOString()
          .split("T")[0];

        params.append("arrivalDate", arrivalDate);
        params.append("departureDate", departureDate);
      }

      const queryString = params.toString();
      const url = queryString
        ? `${API_BASE_URL}/api/Property/my-properties/filter?${queryString}`
        : `${API_BASE_URL}/api/Property/my-properties`;

      const response = await authFetch(url, {
        method: "GET",
      });

      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || "Не удалось загрузить квартиры");
      }

      const data = await response.json();
      setProperties(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message || "Ошибка загрузки квартир");
      toast.error(err.message || "Ошибка загрузки квартир");
    } finally {
      setPropertiesLoading(false);
    }
  }

  function handleApplyFilters() {
    const { arrivalDate, departureDate, minPrice, maxPrice } = filters;

    if ((arrivalDate && !departureDate) || (!arrivalDate && departureDate)) {
      toast.error("Укажи и дату заезда, и дату выезда");
      return;
    }

    if (
      arrivalDate &&
      departureDate &&
      new Date(arrivalDate) >= new Date(departureDate)
    ) {
      toast.error("Дата выезда должна быть позже даты заезда");
      return;
    }

    if (
      minPrice !== "" &&
      maxPrice !== "" &&
      Number(minPrice) > Number(maxPrice)
    ) {
      toast.error("Минимальная цена не может быть больше максимальной");
      return;
    }

    loadProperties(filters);
  }

  function handleResetFilters() {
    const emptyFilters = {
      minPrice: "",
      maxPrice: "",
      guestsCount: 1,
      needContactlessCheckIn: false,
      arrivalDate: null,
      departureDate: null,
    };

    setFilters(emptyFilters);
    loadProperties(emptyFilters);
  }

  async function handleAttachOwner() {
    try {
      if (!code.trim()) {
        toast.error("Введите ссылку или код");
        return;
      }

      let preparedCode = code.trim();

      if (preparedCode.includes("/")) {
        const parts = preparedCode.split("/").filter(Boolean);
        preparedCode = parts[parts.length - 1];
      }

      setIsSubmitting(true);

      const response = await authFetch(
        `${API_BASE_URL}/api/Property/AddOwnerLinkToUser`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            code: preparedCode,
          }),
        }
      );

      let data = null;
      try {
        data = await response.json();
      } catch {
        data = null;
      }

      if (!response.ok) {
        throw new Error(data?.message || data?.title || "Ошибка привязки");
      }

      setCode("");
      toast.success("Владелец успешно прикреплен");
      await loadUser();
    } catch (err) {
      toast.error(err.message || "Ошибка привязки");
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleAvatarChange(event) {
    try {
      const file = event.target.files?.[0];
      if (!file) return;

      setAvatarUploading(true);

      const formData = new FormData();
      formData.append("Avatar", file);

      const response = await authFetch(`${API_BASE_URL}/api/Auth/upload-avatar`, {
        method: "POST",
        body: formData,
      });

      let data = null;
      try {
        data = await response.json();
      } catch {
        data = null;
      }

      if (!response.ok) {
        throw new Error(data?.message || data?.title || "Не удалось загрузить фото");
      }

      setUser((prev) => ({
        ...prev,
        photo: data?.photo ?? prev.photo,
      }));

      toast.success("Фото профиля обновлено");
    } catch (err) {
      toast.error(err.message || "Ошибка загрузки фото");
    } finally {
      setAvatarUploading(false);
      event.target.value = "";
    }
  }

  async function handleSaveProfile(form) {
    try {
      setProfileSaving(true);

      const response = await authFetch(`${API_BASE_URL}/api/Auth/profile`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          firstName: form.firstName,
          lastName: form.lastName,
          middleName: form.middleName,
          telegram: form.telegram,
        }),
      });

      let data = null;
      try {
        data = await response.json();
      } catch {
        data = null;
      }

      if (!response.ok) {
        throw new Error(data?.message || data?.title || "Не удалось сохранить профиль");
      }

      setUser(data.user);
      toast.success("Профиль обновлен");
      return true;
    } catch (err) {
      toast.error(err.message || "Ошибка сохранения профиля");
      return false;
    } finally {
      setProfileSaving(false);
    }
  }

  function openFileDialog() {
    fileInputRef.current?.click();
  }

  function handleOpenProperty(property) {
  navigate(`/client/property/${property.id}`);
  }

  if (loading) {
    return <div className="client-page-loading">Загрузка...</div>;
  }

  if (error && user.roleId === null) {
    return <div className="client-page-loading">{error}</div>;
  }

  if (user.roleId !== 3) {
    return (
      <div className="client-page-loading">
        Эта страница доступна только для клиента
      </div>
    );
  }

  return (
    <div className="client-page">
      <header className="client-topbar">
        <div className="client-logo">RentFlow</div>

        <button
          className="client-profile-btn"
          type="button"
          onClick={() => setProfileOpen(true)}
        >
          👤
        </button>
      </header>

      <main className="client-page-content">
        {!ownerId ? (
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

            <button
              className="client-attach-btn"
              type="button"
              onClick={handleAttachOwner}
              disabled={isSubmitting}
            >
              {isSubmitting ? "Отправка..." : "Прикрепить"}
            </button>
          </div>
        ) : (
          <div className="client-catalog-layout">
            <PropertyFilterSidebar
              isCollapsed={filtersCollapsed}
              onToggle={() => setFiltersCollapsed((prev) => !prev)}
              filters={filters}
              onChange={setFilters}
              onApply={handleApplyFilters}
              onReset={handleResetFilters}
              disabled={propertiesLoading}
            />

            <div className="client-properties-section">
              <div className="client-properties-header">
                <h1 className="client-properties-title">Доступные квартиры</h1>
                <p className="client-properties-subtitle">
                  Выберите подходящий вариант для бронирования
                </p>
              </div>

              {error && <div className="client-message client-error">{error}</div>}
              {success && <div className="client-message client-success">{success}</div>}

              {propertiesLoading ? (
                <div className="client-page-loading-inner">Загрузка квартир...</div>
              ) : properties.length === 0 ? (
                <div className="client-empty-box">Пока нет доступных квартир</div>
              ) : (
                <div className="client-properties-grid">
                  {properties.map((property) => (
                    <PropertyCard
                      key={property.id}
                      property={property}
                      onOpen={handleOpenProperty}
                    />
                  ))}
                </div>
              )}
            </div>
          </div>
        )}
      </main>

      <ProfileSidebar
        isOpen={profileOpen}
        onClose={() => setProfileOpen(false)}
        user={user}
        avatarUploading={avatarUploading}
        fileInputRef={fileInputRef}
        onAvatarChange={handleAvatarChange}
        onOpenFileDialog={openFileDialog}
        onSaveProfile={handleSaveProfile}
        profileSaving={profileSaving}
        onLogout={handleLogout}
        onRegisterCompany={() => navigate("/register-company")}
      />
    </div>
  );
}

export default ClientPage;