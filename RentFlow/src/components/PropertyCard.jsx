import "../styles/propertyCard.css";

function PropertyCard({ property, onOpen }) {
  const shortDescription = property.description
    ? property.description.length > 140
      ? `${property.description.slice(0, 140)}...`
      : property.description
    : "Описание пока не добавлено";

  const bookingModeText =
    property.bookingMode === "instant"
      ? "Мгновенное бронирование"
      : property.bookingMode === "confirmation"
      ? "Подтверждение владельцем"
      : "Не указано";

  return (
    <div className="property-card">
      <div className="property-image-wrap">
        {property.mainImageUrl ? (
          <img
            src={property.mainImageUrl}
            alt={property.title}
            className="property-image"
          />
        ) : (
          <div className="property-image-placeholder">Нет фото</div>
        )}
      </div>

      <div className="property-card-center">
        <h3 className="property-title">{property.title}</h3>
        <p className="property-address">{property.address}</p>

        <div className="property-meta-list">
          <div className="property-meta-item">
            <span className="property-meta-label">Бесконтактное заселение:</span>
            <span className="property-meta-value">
              {property.isContactlessCheckInAvailable ? "Есть" : "Нет"}
            </span>
          </div>

          <div className="property-meta-item">
            <span className="property-meta-label">Режим бронирования:</span>
            <span className="property-meta-value">{bookingModeText}</span>
          </div>
        </div>

        <p className="property-description">{shortDescription}</p>
      </div>

      <div className="property-card-right">
        <div className="property-price-block">
          <div className="property-price-label">Цена за сутки</div>
          <div className="property-price-value">
            {property.currentPrice ? `${property.currentPrice} ₽` : "Не указана"}
          </div>
        </div>

        <button
          className="property-open-btn"
          type="button"
          onClick={() => onOpen?.(property)}
        >
          Открыть
        </button>
      </div>
    </div>
  );
}

export default PropertyCard;