import { BrowserRouter, Routes, Route } from "react-router-dom";
import AuthPage from "./pages/AuthPage";
import ClientPage from "./pages/ClientPage";
import RegisterCompanyPage from "./pages/RegisterCompanyPage";
import PropertyDetailsPage from "./pages/PropertyDetailsPage";
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AuthPage />} />
        <Route path="/client" element={<ClientPage />} />
        <Route path="/register-company" element={<RegisterCompanyPage />} />
        <Route path="/client/property/:id" element={<PropertyDetailsPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;