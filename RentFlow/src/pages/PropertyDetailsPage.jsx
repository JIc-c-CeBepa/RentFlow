import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import PropertyGallery from "../components/PropertyGallery";
import PropertyAmenities from "../components/PropertyAmenities";
import PropertyRules from "../components/PropertyRules";
import BookingSidebar from "../components/BookingSidebar";
import { API_BASE_URL, authFetch } from "../api/authFetch";
import { generateBookingPDF } from "../utils/bookingPDF";
import "../styles/propertyDetailsPage.css";

function PropertyDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [property, setProperty] = useState(null);

  const [bookingForm, setBookingForm] = useState({
    arrivalDate: null,
    departureDate: null,
    guestsCount: 1,
  });

  useEffect(() => {
    loadProperty();
  }, [id]);

  async function loadProperty() {
    try {
      setLoading(true);

      const response = await authFetch(`${API_BASE_URL}/api/Property/${id}`, {
        method: "GET",
      });

      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || "Не удалось загрузить квартиру");
      }

      const data = await response.json();
      setProperty(data);
    } catch (err) {
      toast.error(err.message || "Ошибка загрузки квартиры");
    } finally {
      setLoading(false);
    }
  }

  const nightsCount = useMemo(() => {
    if (!bookingForm.arrivalDate || !bookingForm.departureDate) return 0;

    const start = new Date(bookingForm.arrivalDate);
    const end = new Date(bookingForm.departureDate);
    const diff = end.getTime() - start.getTime();

    if (diff <= 0) return 0;

    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  }, [bookingForm.arrivalDate, bookingForm.departureDate]);

  const totalPrice = useMemo(() => {
    if (!property?.currentPrice || !nightsCount) return 0;
    return Number(property.currentPrice) * nightsCount;
  }, [property, nightsCount]);

  async function handleBook() {
  try {
    if (!bookingForm.arrivalDate || !bookingForm.departureDate) {
      toast.error("Выберите даты бронирования");
      return;
    }

    const response = await authFetch(`${API_BASE_URL}/api/Booking`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        propertyId: property.id,
        arrivalDate: new Date(bookingForm.arrivalDate).toISOString().split("T")[0],
        departureDate: new Date(bookingForm.departureDate).toISOString().split("T")[0],
        guestsCount: bookingForm.guestsCount,
        needsContactlessCheckin: false,
      }),
    });

    let data = null;
    try {
      data = await response.json();
    } catch {
      data = null;
    }

    if (!response.ok) {
      throw new Error(data?.message || data?.title || "Не удалось создать бронь");
    }

    toast.success("Бронирование успешно создано! PDF скачивается...");

    const bookingData = data.booking || data;
    
    const bookingForPDF = {
      id: bookingData.id || Date.now(),
      arrivalDate: bookingData.arrivalDate || bookingForm.arrivalDate,
      departureDate: bookingData.departureDate || bookingForm.departureDate,
      guestsCount: bookingForm.guestsCount,
      totalAmount: bookingData.totalAmount || totalPrice,
      prepaymentAmount: bookingData.prepaymentAmount || 0,
      prepaymentPercent: bookingData.prepaymentPercent || 0,
      statusId: bookingData.statusId || 1,
      createdAt: bookingData.createdAt || new Date().toISOString(),
      needsContactlessCheckin: bookingData.needsContactlessCheckin || false,
    };

    generateBookingPDF(bookingForPDF, property, property.owner || null);

  } catch (err) {
    toast.error(err.message || "Ошибка бронирования");
  }
}

  if (loading) {
    return <div className="property-details-loading">Загрузка...</div>;
  }

  if (!property) {
    return <div className="property-details-loading">Квартира не найдена</div>;
  }

  return (
    <div className="property-details-page">
      <div className="property-details-topbar">
        <button
          type="button"
          className="property-details-back-btn"
          onClick={() => navigate(-1)}
        >
          ← Назад
        </button>
      </div>

      <div className="property-details-container">
        <div className="property-details-main">
          <h1 className="property-details-title">{property.title}</h1>

          <PropertyGallery
            title={property.title}
            images={property.images || []}
            mainImageUrl={property.mainImageUrl}
          />

          <div className="property-details-section">
            <h2 className="property-details-section-title">Удобства</h2>
            <PropertyAmenities amenities={property.amenities || []} />
          </div>

          <div className="property-details-section">
            <h2 className="property-details-section-title">Правила</h2>
            <PropertyRules rules={property.rules || []} />
          </div>

          <div className="property-details-section">
            <h2 className="property-details-section-title">Расположение</h2>
            <div className="property-details-address-card">{property.address}</div>
          </div>

          <div className="property-details-section">
            <h2 className="property-details-section-title">Описание</h2>
            <div className="property-details-description-card">
              {property.description || "Описание пока не добавлено"}
            </div>
          </div>
        </div>

        <BookingSidebar
          property={property}
          bookingForm={bookingForm}
          setBookingForm={setBookingForm}
          nightsCount={nightsCount}
          totalPrice={totalPrice}
          onBook={handleBook}
        />
      </div>
    </div>
  );
}

export default PropertyDetailsPage;