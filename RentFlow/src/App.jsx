import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ThemeProvider } from "./context/ThemeContext";
import AuthPage from "./pages/AuthPage";
import ClientPage from "./pages/ClientPage";
import RegisterCompanyPage from "./pages/RegisterCompanyPage";
import PropertyDetailsPage from "./pages/PropertyDetailsPage";
import "./index.css";

function App() {
  return (
    <ThemeProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<AuthPage />} />
          <Route path="/client" element={<ClientPage />} />
          <Route path="/register-company" element={<RegisterCompanyPage />} />
          <Route path="/client/property/:id" element={<PropertyDetailsPage />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;