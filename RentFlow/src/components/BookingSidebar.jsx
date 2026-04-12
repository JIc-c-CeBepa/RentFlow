import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import "../styles/bookingSidebar.css";

function BookingSidebar({
  property,
  bookingForm,
  setBookingForm,
  nightsCount,
  totalPrice,
  onBook,
}) {
  function handleArrivalDateChange(newValue) {
    setBookingForm((prev) => ({
      ...prev,
      arrivalDate: newValue,
    }));
  }

  function handleDepartureDateChange(newValue) {
    setBookingForm((prev) => ({
      ...prev,
      departureDate: newValue,
    }));
  }

  function handleGuestsChange(event) {
    setBookingForm((prev) => ({
      ...prev,
      guestsCount: event.target.value,
    }));
  }

  return (
    <aside className="booking-sidebar">
      <div className="booking-sidebar-card">
        <div className="booking-sidebar-price-row">
          <div className="booking-sidebar-price-main">
            {property.currentPrice} ₽
          </div>
          <div className="booking-sidebar-price-sub">за сутки</div>
        </div>

        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <div className="booking-sidebar-field">
            <DatePicker
              label="Заезд"
              value={bookingForm.arrivalDate}
              onChange={handleArrivalDateChange}
              slotProps={{
                textField: {
                  fullWidth: true,
                  size: "small",
                  className: "booking-mui-field",
                },
              }}
            />
          </div>

          <div className="booking-sidebar-field">
            <DatePicker
              label="Выезд"
              value={bookingForm.departureDate}
              onChange={handleDepartureDateChange}
              minDate={bookingForm.arrivalDate || undefined}
              slotProps={{
                textField: {
                  fullWidth: true,
                  size: "small",
                  className: "booking-mui-field",
                },
              }}
            />
          </div>
        </LocalizationProvider>

        <div className="booking-sidebar-field">
          <FormControl fullWidth size="small" className="booking-mui-select">
            <InputLabel id="booking-guests-select-label">Гостей</InputLabel>
            <Select
              labelId="booking-guests-select-label"
              value={bookingForm.guestsCount}
              label="Гостей"
              onChange={handleGuestsChange}
            >
              {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((value) => (
                <MenuItem key={value} value={value}>
                  {value}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </div>

        <div className="booking-sidebar-summary">
          <div className="booking-sidebar-summary-row">
            <span>Цена за сутки</span>
            <span>{property.currentPrice} ₽</span>
          </div>

          <div className="booking-sidebar-summary-row">
            <span>Ночей</span>
            <span>{nightsCount}</span>
          </div>

          <div className="booking-sidebar-summary-row total">
            <span>Итого</span>
            <span>{totalPrice} ₽</span>
          </div>
        </div>

        <button
          type="button"
          className="booking-sidebar-book-btn"
          onClick={onBook}
        >
          Забронировать
        </button>
      </div>
    </aside>
  );
}

export default BookingSidebar;