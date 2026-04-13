import { useEffect, useState } from "react";
import { UserPlus, CalendarCheck } from "lucide-react";
import "../styles/profileSidebar.css";

function ProfileSidebar({
  isOpen,
  onClose,
  user,
  avatarUploading,
  fileInputRef,
  onAvatarChange,
  onOpenFileDialog,
  onSaveProfile,
  profileSaving,
  onLogout,
  onRegisterCompany,
  onViewBookings,
}) {
  const [isEditing, setIsEditing] = useState(false);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    middleName: "",
    telegram: "",
  });

  useEffect(() => {
    setForm({
      firstName: user.firstName ?? "",
      lastName: user.lastName ?? "",
      middleName: user.middleName ?? "",
      telegram: user.telegram ?? "",
    });
  }, [user]);

  function buildFullName(userData) {
    const firstName = userData?.firstName?.trim() || "";
    const lastName = userData?.lastName?.trim() || "";
    const middleName = userData?.middleName?.trim() || "";

    if (!lastName && !middleName) {
      return firstName || "Пользователь";
    }

    return [lastName, firstName, middleName].filter(Boolean).join(" ");
  }

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  }

  async function handleSave() {
    const success = await onSaveProfile?.(form);
    if (success) {
      setIsEditing(false);
    }
  }

  function handleCancel() {
    setForm({
      firstName: user.firstName ?? "",
      lastName: user.lastName ?? "",
      middleName: user.middleName ?? "",
      telegram: user.telegram ?? "",
    });
    setIsEditing(false);
  }

  if (!isOpen) return null;

  return (
    <div className="profile-overlay" onClick={onClose}>
      <div className="profile-panel" onClick={(e) => e.stopPropagation()}>
        <div className="profile-header">
          <h2 className="profile-title">Профиль</h2>

          <button className="profile-close-btn" type="button" onClick={onClose}>
            ✕
          </button>
        </div>

        <div className="profile-edit-row">
          {!isEditing ? (
            <button
              className="profile-edit-btn"
              type="button"
              onClick={() => setIsEditing(true)}
            >
              Редактировать
            </button>
          ) : (
            <div className="profile-edit-actions">
              <button
                className="profile-save-btn"
                type="button"
                onClick={handleSave}
                disabled={profileSaving}
              >
                {profileSaving ? "Сохранение..." : "Сохранить"}
              </button>

              <button
                className="profile-cancel-btn"
                type="button"
                onClick={handleCancel}
                disabled={profileSaving}
              >
                Отмена
              </button>
            </div>
          )}
        </div>

        <div className="profile-avatar-section">
          <div
            className="profile-avatar-wrapper"
            onClick={onOpenFileDialog}
            role="button"
            tabIndex={0}
            onKeyDown={(e) => {
              if (e.key === "Enter" || e.key === " ") {
                onOpenFileDialog();
              }
            }}
          >
            {user.photo ? (
              <img
                src={user.photo}
                alt="Аватар"
                className="profile-avatar-image"
              />
            ) : (
              <div className="profile-avatar-placeholder">
                <UserPlus size={48} />
              </div>
            )}

            <div className="profile-avatar-overlay">
              {avatarUploading ? "Загрузка..." : "Сменить фото"}
            </div>
          </div>

          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            className="profile-file-input"
            onChange={onAvatarChange}
          />
        </div>

        {!isEditing ? (
          <>
            <div className="profile-user-name">{buildFullName(user)}</div>

            <div className="profile-info-block">
              <div className="profile-info-label">Телефон</div>
              <div className="profile-info-value">{user.phone || "Не указан"}</div>
            </div>

            <div className="profile-info-block">
              <div className="profile-info-label">Telegram</div>
              {user.telegram ? (
                <div className="profile-info-value">{user.telegram}</div>
              ) : (
                <div className="profile-info-value">Не указан</div>
              )}
            </div>
          </>
        ) : (
          <>
            <div className="profile-form-block">
              <label className="profile-form-label">Имя</label>
              <input
                className="profile-input"
                name="firstName"
                value={form.firstName}
                onChange={handleChange}
                placeholder="Имя"
              />
            </div>

            <div className="profile-form-block">
              <label className="profile-form-label">Фамилия</label>
              <input
                className="profile-input"
                name="lastName"
                value={form.lastName}
                onChange={handleChange}
                placeholder="Фамилия"
              />
            </div>

            <div className="profile-form-block">
              <label className="profile-form-label">Отчество</label>
              <input
                className="profile-input"
                name="middleName"
                value={form.middleName}
                onChange={handleChange}
                placeholder="Отчество"
              />
            </div>

            <div className="profile-form-block">
              <label className="profile-form-label">Телефон</label>
              <input
                className="profile-input profile-input-disabled"
                value={user.phone || ""}
                disabled
              />
            </div>

            <div className="profile-form-block">
              <label className="profile-form-label">Telegram</label>
              <input
                className="profile-input"
                name="telegram"
                value={form.telegram}
                onChange={handleChange}
                placeholder="@username"
              />
            </div>
          </>
        )}

        <div className="profile-footer">
          {user.roleId === 3 && onViewBookings && (
            <button className="profile-bookings-btn" type="button" onClick={onViewBookings}>
              <CalendarCheck size={20} />
              Мои бронирования
            </button>
          )}
          {user.roleId === 3 && (
            <button className="register-company-btn" type="button" onClick={onRegisterCompany}>
              Зарегистрировать компанию
            </button>
          )}
          <button className="profile-logout-btn" type="button" onClick={onLogout}>
            Выйти из аккаунта
          </button>
        </div>

        
  
      </div>
    </div>
  );
}

export default ProfileSidebar;