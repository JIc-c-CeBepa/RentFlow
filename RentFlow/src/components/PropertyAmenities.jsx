import WifiRoundedIcon from "@mui/icons-material/WifiRounded";
import TvRoundedIcon from "@mui/icons-material/TvRounded";
import AcUnitRoundedIcon from "@mui/icons-material/AcUnitRounded";
import KitchenRoundedIcon from "@mui/icons-material/KitchenRounded";
import LocalLaundryServiceRoundedIcon from "@mui/icons-material/LocalLaundryServiceRounded";
import LocalParkingRoundedIcon from "@mui/icons-material/LocalParkingRounded";
import HealthAndSafetyRoundedIcon from "@mui/icons-material/HealthAndSafetyRounded";
import MeetingRoomRoundedIcon from "@mui/icons-material/MeetingRoomRounded";
import CheckCircleOutlineRoundedIcon from "@mui/icons-material/CheckCircleOutlineRounded";
import "../styles/propertyAmenities.css";

const amenityIcons = {
  wifi: WifiRoundedIcon,
  tv: TvRoundedIcon,
  ac_unit: AcUnitRoundedIcon,
  kitchen: KitchenRoundedIcon,
  local_laundry_service: LocalLaundryServiceRoundedIcon,
  local_parking: LocalParkingRoundedIcon,
  health_and_safety: HealthAndSafetyRoundedIcon,
  meeting_room: MeetingRoomRoundedIcon,
};

function PropertyAmenities({ amenities }) {
  if (!amenities.length) {
    return <div className="property-amenities-empty">Удобства не указаны</div>;
  }

  return (
    <div className="property-amenities-grid">
      {amenities.map((item) => {
        const IconComponent = amenityIcons[item.icon] || CheckCircleOutlineRoundedIcon;

        return (
          <div key={item.id} className="property-amenity-card">
            <div className="property-amenity-icon-wrap">
              <IconComponent />
            </div>
            <div className="property-amenity-name">{item.name}</div>
          </div>
        );
      })}
    </div>
  );
}

export default PropertyAmenities;