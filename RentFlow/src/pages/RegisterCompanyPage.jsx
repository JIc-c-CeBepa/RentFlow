import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import "../styles/registerCompanyPage.css";
import { API_BASE_URL, authFetch, saveTokens } from "../api/authFetch";



function RegisterCompanyPage() {
  const navigate = useNavigate();
  const token = localStorage.getItem("token");

  const [form, setForm] = useState({
    companyName: "",
    description: "",
    firstName: "",
    lastName: "",
    middleName: "",
    telegram: "",
    email: "",
  });

  const [loading, setLoading] = useState(false);

  function handleChange(e) {
    const { name, value } = e.target;

    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  }

  async function handleSubmit(e) {
    e.preventDefault();

    try {
      setLoading(true);

      if (!form.companyName.trim()) {
        toast.error("Введите название компании");
        return;
      }

      if (!form.firstName.trim()) {
        toast.error("Введите имя");
        return;
      }

      const response = await authFetch(`${API_BASE_URL}/api/Auth/register-company`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          companyName: form.companyName,
          description: form.description,
          firstName: form.firstName,
          lastName: form.lastName,
          middleName: form.middleName,
          telegram: form.telegram,
          email: form.email,
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data?.message || data?.title || "Ошибка регистрации компании");
      }

      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      toast.success("Компания успешно зарегистрирована");

      navigate("/");
    } catch (err) {
      toast.error(err.message || "Ошибка регистрации компании");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="register-company-page">
      <div className="register-company-card">
        <h1 className="register-company-title">Регистрация компании</h1>
        <p className="register-company-subtitle">
          Заполните данные компании и личные данные владельца
        </p>

        <form className="register-company-form" onSubmit={handleSubmit}>
          <div className="register-company-group">
            <label>Название компании</label>
            <input
              type="text"
              name="companyName"
              value={form.companyName}
              onChange={handleChange}
              placeholder="Введите название компании"
            />
          </div>

          <div className="register-company-group">
            <label>Описание</label>
            <textarea
              name="description"
              value={form.description}
              onChange={handleChange}
              placeholder="Кратко опишите компанию"
              rows="4"
            />
          </div>

          <div className="register-company-group">
            <label>Имя</label>
            <input
              type="text"
              name="firstName"
              value={form.firstName}
              onChange={handleChange}
              placeholder="Введите имя"
            />
          </div>

          <div className="register-company-group">
            <label>Фамилия</label>
            <input
              type="text"
              name="lastName"
              value={form.lastName}
              onChange={handleChange}
              placeholder="Введите фамилию"
            />
          </div>

          <div className="register-company-group">
            <label>Отчество</label>
            <input
              type="text"
              name="middleName"
              value={form.middleName}
              onChange={handleChange}
              placeholder="Введите отчество"
            />
          </div>

          <div className="register-company-group">
            <label>Telegram</label>
            <input
              type="text"
              name="telegram"
              value={form.telegram}
              onChange={handleChange}
              placeholder="@username"
            />
          </div>

          

          <div className="register-company-actions">
            <button
              type="button"
              className="register-company-back-btn"
              onClick={() => navigate(-1)}
            >
              Назад
            </button>

            <button
              type="submit"
              className="register-company-submit-btn"
              disabled={loading}
            >
              {loading ? "Сохранение..." : "Зарегистрировать компанию"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default RegisterCompanyPage;