import { BrowserRouter, Routes, Route } from "react-router-dom";
import AuthPage from "./pages/AuthPage";
import ClientPage from "./pages/ClientPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AuthPage />} />
        <Route path="/client" element={<ClientPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;