import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import TextField from "@mui/material/TextField";
import KeyboardArrowLeftRoundedIcon from "@mui/icons-material/KeyboardArrowLeftRounded";
import KeyboardArrowRightRoundedIcon from "@mui/icons-material/KeyboardArrowRightRounded";
import "../styles/propertyFilterSidebar.css";

function PropertyFilterSidebar({
  isCollapsed,
  onToggle,
  filters,
  onChange,
  onApply,
  onReset,
  disabled,
}) {
  function handleGuestsChange(event) {
    onChange({
      ...filters,
      guestsCount: event.target.value,
    });
  }

  function handleMinPriceChange(event) {
    onChange({
      ...filters,
      minPrice: event.target.value,
    });
  }

  function handleMaxPriceChange(event) {
    onChange({
      ...filters,
      maxPrice: event.target.value,
    });
  }

  function handleArrivalDateChange(newValue) {
    onChange({
      ...filters,
      arrivalDate: newValue,
    });
  }

  function handleDepartureDateChange(newValue) {
    onChange({
      ...filters,
      departureDate: newValue,
    });
  }

  function handleContactlessChange(event) {
    onChange({
      ...filters,
      needContactlessCheckIn: event.target.checked,
    });
  }

  return (
    <div className={`property-filter-shell ${isCollapsed ? "collapsed" : ""}`}>
      <aside className={`property-filter-sidebar ${isCollapsed ? "collapsed" : ""}`}>
        <div className="property-filter-content">
          <div className="property-filter-header">
            <h2 className="property-filter-title">Фильтры</h2>
          </div>

          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <div className="property-filter-block">
              <TextField
                label="Цена от"
                type="number"
                value={filters.minPrice}
                onChange={handleMinPriceChange}
                className="property-mui-field"
                fullWidth
                size="small"
                inputProps={{ min: 0 }}
              />
            </div>

            <div className="property-filter-block">
              <TextField
                label="Цена до"
                type="number"
                value={filters.maxPrice}
                onChange={handleMaxPriceChange}
                className="property-mui-field"
                fullWidth
                size="small"
                inputProps={{ min: 0 }}
              />
            </div>

            <div className="property-filter-block">
              <FormControl fullWidth size="small" className="property-mui-select">
                <InputLabel id="guests-select-label">Гостей</InputLabel>
                <Select
                  labelId="guests-select-label"
                  value={filters.guestsCount}
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

            <div className="property-filter-block">
              <DatePicker
                label="Заезд"
                value={filters.arrivalDate}
                onChange={handleArrivalDateChange}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    size: "small",
                    className: "property-mui-field",
                  },
                }}
              />
            </div>

            <div className="property-filter-block">
              <DatePicker
                label="Выезд"
                value={filters.departureDate}
                onChange={handleDepartureDateChange}
                minDate={filters.arrivalDate || undefined}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    size: "small",
                    className: "property-mui-field",
                  },
                }}
              />
            </div>
          </LocalizationProvider>

          <label className="property-filter-checkbox-row">
            <input
              type="checkbox"
              checked={filters.needContactlessCheckIn}
              onChange={handleContactlessChange}
            />
            <span>Бесконтактное заселение</span>
          </label>

          <div className="property-filter-actions">
            <button
              type="button"
              className="property-filter-apply-btn"
              onClick={onApply}
              disabled={disabled}
            >
              {disabled ? "Загрузка..." : "Применить"}
            </button>

            <button
              type="button"
              className="property-filter-reset-btn"
              onClick={onReset}
              disabled={disabled}
            >
              Сбросить
            </button>
          </div>
        </div>
      </aside>

      <button
        type="button"
        className="property-filter-toggle"
        onClick={onToggle}
        aria-label={isCollapsed ? "Открыть фильтры" : "Скрыть фильтры"}
        >
        {isCollapsed ? <KeyboardArrowRightRoundedIcon /> : <KeyboardArrowLeftRoundedIcon />}
        </button>
    </div>
  );
}

export default PropertyFilterSidebar;