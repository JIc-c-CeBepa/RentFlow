import { useMemo, useState } from "react";
import "../styles/propertyGallery.css";

function PropertyGallery({ title, images, mainImageUrl }) {
  const preparedImages = useMemo(() => {
    if (images?.length) {
      return images.map((x) => x.imageUrl);
    }

    return mainImageUrl ? [mainImageUrl] : [];
  }, [images, mainImageUrl]);

  const [activeIndex, setActiveIndex] = useState(0);

  if (!preparedImages.length) {
    return <div className="property-gallery-empty">Нет фотографий</div>;
  }

  const activeImage = preparedImages[activeIndex];

  function goPrev() {
    setActiveIndex((prev) =>
      prev === 0 ? preparedImages.length - 1 : prev - 1
    );
  }

  function goNext() {
    setActiveIndex((prev) =>
      prev === preparedImages.length - 1 ? 0 : prev + 1
    );
  }

  return (
    <div className="property-gallery">
      <div className="property-gallery-main-wrap">
        <img
          src={activeImage}
          alt={title}
          className="property-gallery-main-image"
        />

        {preparedImages.length > 1 && (
          <>
            <button
              type="button"
              className="property-gallery-nav prev"
              onClick={goPrev}
            >
              ‹
            </button>
            <button
              type="button"
              className="property-gallery-nav next"
              onClick={goNext}
            >
              ›
            </button>
          </>
        )}
      </div>

      {preparedImages.length > 1 && (
        <div className="property-gallery-thumbs">
          {preparedImages.map((image, index) => (
            <button
              key={index}
              type="button"
              className={`property-gallery-thumb-btn ${
                index === activeIndex ? "active" : ""
              }`}
              onClick={() => setActiveIndex(index)}
            >
              <img
                src={image}
                alt={`${title} ${index + 1}`}
                className="property-gallery-thumb-image"
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

export default PropertyGallery;