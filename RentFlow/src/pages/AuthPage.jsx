import { useState } from "react";
import { Eye, EyeOff } from "lucide-react";
import Toast from "../components/Toast";
import { formatPhone, isPhoneComplete } from "../utils/phoneMask";
import { loginUser, registerUser } from "../services/AuthApi";
import { useNavigate } from "react-router-dom";
import { saveTokens, API_BASE_URL } from "../api/authFetch";

function AuthPage() {
  const [mode, setMode] = useState("login");

  const navigate = useNavigate();

  const [loginForm, setLoginForm] = useState({
    phone: "",
    password: "",
  });

  const [registerForm, setRegisterForm] = useState({
    firstName: "",
    phone: "",
    password: "",
    confirmPassword: "",
  });

  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);

  const [toast, setToast] = useState({
    message: "",
    type: "success",
  });

  const [showLoginPassword, setShowLoginPassword] = useState(false);
  const [showRegisterPassword, setShowRegisterPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  function showToast(message, type = "success") {
    setToast({ message, type });

    setTimeout(() => {
      setToast({ message: "", type: "success" });
    }, 3500);
  }

  function closeToast() {
    setToast({ message: "", type: "success" });
  }

  function validatePassword(password) {
    const errors = [];

    if (password.length < 8) {
      errors.push("Пароль должен содержать минимум 8 символов");
    }

    if (!/[a-zа-я]/.test(password)) {
      errors.push("Пароль должен содержать строчную букву");
    }

    if (!/[A-ZА-Я]/.test(password)) {
      errors.push("Пароль должен содержать заглавную букву");
    }

    if (!/\d/.test(password)) {
      errors.push("Пароль должен содержать цифру");
    }

    if (!/[!@#$%^&*()_\-+=\[{\]};:'",.<>/?\\|`~]/.test(password)) {
      errors.push("Пароль должен содержать спецсимвол");
    }

    return errors;
  }

  function validateLogin() {
    const newErrors = {};

    if (!loginForm.phone.trim()) {
      newErrors.loginPhone = "Введите телефон";
    } else if (!isPhoneComplete(loginForm.phone)) {
      newErrors.loginPhone = "Телефон введен не полностью";
    }

    if (!loginForm.password.trim()) {
      newErrors.loginPassword = "Введите пароль";
    }

    return newErrors;
  }

  function validateRegister() {
    const newErrors = {};

    if (!registerForm.firstName.trim()) {
      newErrors.firstName = "Введите имя";
    }

    if (!registerForm.phone.trim()) {
      newErrors.phone = "Введите телефон";
    } else if (!isPhoneComplete(registerForm.phone)) {
      newErrors.phone = "Телефон введен не полностью";
    }

    if (!registerForm.password.trim()) {
      newErrors.password = "Введите пароль";
    } else {
      const passwordErrors = validatePassword(registerForm.password);
      if (passwordErrors.length > 0) {
        newErrors.password = passwordErrors[0];
      }
    }

    if (!registerForm.confirmPassword.trim()) {
      newErrors.confirmPassword = "Подтвердите пароль";
    } else if (registerForm.password !== registerForm.confirmPassword) {
      newErrors.confirmPassword = "Пароли не совпадают";
    }

    return newErrors;
  }

  function handleLoginPhoneChange(e) {
    const formatted = formatPhone(e.target.value);
    setLoginForm((prev) => ({ ...prev, phone: formatted }));
  }

  function handleRegisterPhoneChange(e) {
    const formatted = formatPhone(e.target.value);
    setRegisterForm((prev) => ({ ...prev, phone: formatted }));
  }

  function handleLoginChange(e) {
    const { name, value } = e.target;
    setLoginForm((prev) => ({ ...prev, [name]: value }));
  }

  function handleRegisterChange(e) {
    const { name, value } = e.target;
    setRegisterForm((prev) => ({ ...prev, [name]: value }));
  }

  async function handleLoginSubmit(e) {
    e.preventDefault();

    const validationErrors = validateLogin();
    setErrors(validationErrors);

    if (Object.keys(validationErrors).length > 0) {
      showToast("Исправь ошибки в форме входа", "error");
      return;
    }

    try {
      setLoading(true);

      const payload = {
        phone: loginForm.phone,
        password: loginForm.password,
      };

      const data = await loginUser(payload);

      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      localStorage.setItem("userId", data.userId);
      localStorage.setItem("role", JSON.stringify(data.role));

      showToast("Вход выполнен успешно", "success");

      console.log("LOGIN RESPONSE:", data);
      console.log("LOGIN RESPONSE:", data.roleId);
      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      if (data.roleId === 3) {
        navigate("/client");
      }
    } catch (error) {
      showToast(error.message || "Ошибка авторизации", "error");
    } finally {
      setLoading(false);
    }
  }

  async function handleRegisterSubmit(e) {
    e.preventDefault();

    const validationErrors = validateRegister();
    setErrors(validationErrors);

    if (Object.keys(validationErrors).length > 0) {
      showToast("Исправь ошибки в форме регистрации", "error");
      return;
    }

    try {
      setLoading(true);

      const payload = {
        firstName: registerForm.firstName,
        phone: registerForm.phone,
        password: registerForm.password,
        leadSourceId: 1,
      };

      const data = await registerUser(payload);

      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);

      showToast("Регистрация прошла успешно", "success");

      setRegisterForm({
        firstName: "",
        phone: "",
        password: "",
        confirmPassword: "",
      });

      setErrors({});
      setMode("login");
    } catch (error) {
      showToast(error.message || "Ошибка регистрации", "error");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <Toast message={toast.message} type={toast.type} onClose={closeToast} />

      <div className="auth-card">
        {mode === "login" ? (
          <form className="auth-form" onSubmit={handleLoginSubmit}>
            <h2>Авторизация</h2>

            <div className="form-group">
              <label>Телефон</label>
              <input
                type="text"
                name="phone"
                placeholder="+7 (917)-788-22-18"
                value={loginForm.phone}
                onChange={handleLoginPhoneChange}
                maxLength={18}
              />
              {errors.loginPhone && (
                <span className="error-text">{errors.loginPhone}</span>
              )}
            </div>

            <div className="form-group">
              <label>Пароль</label>
              <div className="password-wrapper">
                <input
                  type={showLoginPassword ? "text" : "password"}
                  name="password"
                  placeholder="Введите пароль"
                  value={loginForm.password}
                  onChange={handleLoginChange}
                  className="password-input"
                />
                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowLoginPassword((prev) => !prev)}
                >
                  {showLoginPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
              {errors.loginPassword && (
                <span className="error-text">{errors.loginPassword}</span>
              )}
            </div>

            <button className="submit-btn" type="submit" disabled={loading}>
              {loading ? "Загрузка..." : "Войти"}
            </button>
          </form>
        ) : (
          <form className="auth-form" onSubmit={handleRegisterSubmit}>
            <h2>Регистрация</h2>

            <div className="form-group">
              <label>Имя</label>
              <input
                type="text"
                name="firstName"
                placeholder="Введите имя"
                value={registerForm.firstName}
                onChange={handleRegisterChange}
              />
              {errors.firstName && (
                <span className="error-text">{errors.firstName}</span>
              )}
            </div>

            <div className="form-group">
              <label>Телефон</label>
              <input
                type="text"
                name="phone"
                placeholder="+7 (917)-788-22-18"
                value={registerForm.phone}
                onChange={handleRegisterPhoneChange}
                maxLength={18}
              />
              {errors.phone && (
                <span className="error-text">{errors.phone}</span>
              )}
            </div>

            <div className="form-group">
              <label>Пароль</label>
              <div className="password-wrapper">
                <input
                  type={showRegisterPassword ? "text" : "password"}
                  name="password"
                  placeholder="Введите пароль"
                  value={registerForm.password}
                  onChange={handleRegisterChange}
                  className="password-input"
                />
                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowRegisterPassword((prev) => !prev)}
                >
                  {showRegisterPassword ? (
                    <EyeOff size={20} />
                  ) : (
                    <Eye size={20} />
                  )}
                </button>
              </div>
              {errors.password && (
                <span className="error-text">{errors.password}</span>
              )}
            </div>

            <div className="password-hint">
              Пароль: минимум 8 символов, заглавная, строчная, цифра и
              спецсимвол
            </div>

            <div className="form-group">
              <label>Подтверждение пароля</label>
              <div className="password-wrapper">
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  name="confirmPassword"
                  placeholder="Повторите пароль"
                  value={registerForm.confirmPassword}
                  onChange={handleRegisterChange}
                  className="password-input"
                />
                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowConfirmPassword((prev) => !prev)}
                >
                  {showConfirmPassword ? (
                    <EyeOff size={20} />
                  ) : (
                    <Eye size={20} />
                  )}
                </button>
              </div>
              {errors.confirmPassword && (
                <span className="error-text">{errors.confirmPassword}</span>
              )}
            </div>

            <button className="submit-btn" type="submit" disabled={loading}>
              {loading ? "Загрузка..." : "Зарегистрироваться"}
            </button>
          </form>
        )}

        <div className="auth-tabs">
          <button
            type="button"
            className={mode === "login" ? "tab active" : "tab"}
            onClick={() => {
              setMode("login");
              setErrors({});
            }}
          >
            Вход
          </button>
          <button
            type="button"
            className={mode === "register" ? "tab active" : "tab"}
            onClick={() => {
              setMode("register");
              setErrors({});
            }}
          >
            Регистрация
          </button>
        </div>
      </div>
    </div>
  );
}

export default AuthPage;