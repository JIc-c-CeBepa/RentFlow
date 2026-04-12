import GppGoodRoundedIcon from "@mui/icons-material/GppGoodRounded";
import "../styles/propertyRules.css";

function PropertyRules({ rules }) {
  if (!rules.length) {
    return <div className="property-rules-empty">Правила не указаны</div>;
  }

  return (
    <div className="property-rules-list">
      {rules.map((rule) => (
        <div key={rule.id} className="property-rule-card">
          <div className="property-rule-icon">
            <GppGoodRoundedIcon />
          </div>

          <div className="property-rule-content">
            <div className="property-rule-name">{rule.name}</div>
            {rule.description && (
              <div className="property-rule-description">{rule.description}</div>
            )}
          </div>
        </div>
      ))}
    </div>
  );
}

export default PropertyRules;