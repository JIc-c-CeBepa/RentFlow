import html2pdf from "html2pdf.js";

export function generateBookingPDF(booking, property, owner) {
  const arrivalDate = booking.arrivalDate 
    ? formatDate(booking.arrivalDate)
    : "-";
  const departureDate = booking.departureDate 
    ? formatDate(booking.departureDate)
    : "-";
  const createdDate = booking.createdAt 
    ? formatDate(booking.createdAt)
    : "-";
  const nightsCount = getNightsCount(booking.arrivalDate, booking.departureDate);
  const totalAmount = booking.totalAmount ? parseFloat(booking.totalAmount).toFixed(2) : "0.00";
  const prepaymentAmount = booking.prepaymentAmount ? parseFloat(booking.prepaymentAmount).toFixed(2) : "0.00";

  const htmlContent = `
    <div style="width: 595px; padding: 30px; font-family: 'Segoe UI', Arial, sans-serif; background: #f3efe7; color: #3d3428;">
      <div style="background: #f3efe7; padding: 15px 0; border-bottom: 2px solid #c9a86c; margin-bottom: 20px; display: flex; justify-content: space-between; align-items: center;">
        <div style="font-size: 28px; font-weight: bold; color: #3d3428;">RentFlow</div>
        <div style="font-size: 12px; color: #6b5d4d;">Booking Confirmation</div>
      </div>
      
      <div style="text-align: center; margin-bottom: 20px;">
        <div style="font-size: 18px; font-weight: bold; color: #3d3428; margin-bottom: 5px;">ТАЛОН БРОНИРОВАНИЯ</div>
        <div style="font-size: 11px; color: #6b5d4d;">Номер бронирования: ${booking.id}</div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Информация о недвижимости</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px; font-weight: 600;">${property.title || "Квартира"}</div>
        <div style="font-size: 10px; color: #6b5d4d; margin-bottom: 8px;">${property.address || ""}</div>
        <div style="font-size: 10px; color: #3d3428;">
          Максимум гостей: ${property.maxGuests || 1}
          ${property.checkInTime ? `&nbsp;&nbsp;|&nbsp;&nbsp;Заезд: ${property.checkInTime}` : ""}
          ${property.checkOutTime ? `&nbsp;&nbsp;|&nbsp;&nbsp;Выезд: ${property.checkOutTime}` : ""}
        </div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Даты проживания</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Заезд:</b> ${arrivalDate}</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Выезд:</b> ${departureDate}</div>
        <div style="font-size: 11px; color: #3d3428;"><b>Количество гостей:</b> ${booking.guestsCount || 1}</div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Информация о бронировании</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Статус:</b> ${getStatusText(booking.statusId)}</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Дата бронирования:</b> ${createdDate}</div>
        ${booking.needsContactlessCheckin ? '<div style="font-size: 11px; color: #4aaa5a; margin-top: 5px;"><b>Бесконтактное заселение: ДА</b></div>' : ''}
      </div>
      
      <div style="background: linear-gradient(135deg, #c9a86c, #e0c598); border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 14px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Оплата</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;">Стоимость за ${nightsCount} ночей: ${totalAmount} ₽</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;">Предоплата (${booking.prepaymentPercent || 0}%): ${prepaymentAmount} ₽</div>
        <div style="font-size: 14px; font-weight: bold; color: #3d3428; margin-top: 8px;">ИТОГО: ${totalAmount} ₽</div>
      </div>
      
      ${owner ? `
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Контакты владельца</div>
        ${owner.companyName ? `<div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>${owner.companyName}</b></div>` : ""}
        ${owner.phone ? `<div style="font-size: 10px; color: #3d3428; margin-bottom: 3px;">Тел: ${owner.phone}</div>` : ""}
        ${owner.email ? `<div style="font-size: 10px; color: #3d3428; margin-bottom: 3px;">Email: ${owner.email}</div>` : ""}
        ${owner.telegram ? `<div style="font-size: 10px; color: #3d3428;">Telegram: ${owner.telegram}</div>` : ""}
      </div>
      ` : ""}
      
      <div style="text-align: center; margin-top: 20px; padding-top: 15px; border-top: 1px solid #d0c9bc;">
        <div style="font-size: 9px; color: #968e7d; font-style: italic;">Сохраните данный талон. Он является подтверждением вашего бронирования.</div>
        <div style="font-size: 9px; color: #968e7d; margin-top: 5px;">Сгенерировано: ${new Date().toLocaleString("ru-RU")}</div>
      </div>
    </div>
  `;

  const opt = {
    margin: 0,
    filename: `booking_${booking.id}_${Date.now()}.pdf`,
    image: { type: 'jpeg', quality: 0.98 },
    html2canvas: { scale: 2, useCORS: true },
    jsPDF: { unit: 'px', format: [595, 842], hotfixes: ['px_scaling'] }
  };

  html2pdf().set(opt).from(htmlContent).save();
}

function formatDate(dateValue) {
  if (!dateValue) return "-";
  const date = new Date(dateValue);
  return date.toLocaleDateString("ru-RU", { day: "2-digit", month: "long", year: "numeric" });
}

function getStatusText(statusId) {
  const statuses = {
    1: "Новое",
    2: "Ожидает подтверждения",
    3: "Подтверждено",
    4: "Оплачено",
    5: "Отменено",
    6: "Завершено"
  };
  return statuses[statusId] || "Неизвестно";
}

function getNightsCount(arrivalDate, departureDate) {
  if (!arrivalDate || !departureDate) return 0;
  const start = new Date(arrivalDate);
  const end = new Date(departureDate);
  const diff = end.getTime() - start.getTime();
  return Math.max(1, Math.ceil(diff / (1000 * 60 * 60 * 24)));
}
