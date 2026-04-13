import { X, Download } from "lucide-react";
import { generateBookingPDF } from "../utils/bookingPDF";
import "../styles/bookingsModal.css";

function BookingsModal({ isOpen, onClose, bookings }) {
  if (!isOpen) return null;

  function getStatusInfo(statusId) {
    const statuses = {
      1: { text: "Новое", className: "status-new" },
      2: { text: "Ожидает подтверждения", className: "status-pending" },
      3: { text: "Подтверждено", className: "status-confirmed" },
      4: { text: "Оплачено", className: "status-paid" },
      5: { text: "Отменено", className: "status-canceled" },
      6: { text: "Завершено", className: "status-completed" },
    };
    return statuses[statusId] || { text: "Неизвестно", className: "" };
  }

  function formatDate(dateString) {
    if (!dateString) return "-";
    return new Date(dateString).toLocaleDateString("ru-RU", {
      day: "2-digit",
      month: "long",
      year: "numeric",
    });
  }

  function handleDownloadPDF(booking, property) {
    const bookingForPDF = {
      id: booking.id,
      arrivalDate: booking.arrivalDate,
      departureDate: booking.departureDate,
      guestsCount: booking.guestsCount,
      totalAmount: booking.totalAmount,
      prepaymentAmount: booking.prepaymentAmount,
      prepaymentPercent: booking.prepaymentPercent,
      statusId: booking.statusId,
      createdAt: booking.createdAt,
      needsContactlessCheckin: booking.needsContactlessCheckin,
    };
    generateBookingPDF(bookingForPDF, property, booking.owner || null);
  }

  return (
    <div className="bookings-overlay" onClick={onClose}>
      <div className="bookings-modal" onClick={(e) => e.stopPropagation()}>
        <div className="bookings-header">
          <h2 className="bookings-title">Мои бронирования</h2>
          <button className="bookings-close-btn" onClick={onClose}>
            <X size={24} />
          </button>
        </div>

        <div className="bookings-content">
          {!bookings || bookings.length === 0 ? (
            <div className="bookings-empty">
              <p>У вас пока нет бронирований</p>
            </div>
          ) : (
            <div className="bookings-list">
              {bookings.map((booking) => {
                const status = getStatusInfo(booking.statusId);
                return (
                  <div key={booking.id} className="booking-card">
                    <div className="booking-card-header">
                      <div className="booking-card-info">
                        <span className="booking-id">#{booking.id}</span>
                        <span className={`booking-status ${status.className}`}>
                          {status.text}
                        </span>
                      </div>
                      <button
                        className="booking-download-btn"
                        onClick={() => handleDownloadPDF(booking, booking.property)}
                        title="Скачать PDF"
                      >
                        <Download size={18} />
                      </button>
                    </div>

                    {booking.property && (
                      <div className="booking-property">
                        <h3 className="booking-property-title">{booking.property.title}</h3>
                        <p className="booking-property-address">{booking.property.address}</p>
                      </div>
                    )}

                    <div className="booking-dates">
                      <div className="booking-date-item">
                        <span className="booking-date-label">Заезд</span>
                        <span className="booking-date-value">{formatDate(booking.arrivalDate)}</span>
                      </div>
                      <div className="booking-date-item">
                        <span className="booking-date-label">Выезд</span>
                        <span className="booking-date-value">{formatDate(booking.departureDate)}</span>
                      </div>
                      <div className="booking-date-item">
                        <span className="booking-date-label">Гости</span>
                        <span className="booking-date-value">{booking.guestsCount}</span>
                      </div>
                    </div>

                    <div className="booking-price">
                      <div className="booking-price-row">
                        <span>Итого:</span>
                        <span className="booking-price-value">{parseFloat(booking.totalAmount || 0).toFixed(2)} ₽</span>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default BookingsModal;
