import html2pdf from "html2pdf.js";

export function generateBookingPDFTemplate() {
  const htmlContent = `
    <div style="width: 595px; padding: 30px; font-family: 'Segoe UI', Arial, sans-serif; background: #f3efe7; color: #3d3428;">
      <div style="background: #f3efe7; padding: 15px 0; border-bottom: 2px solid #c9a86c; margin-bottom: 20px; display: flex; justify-content: space-between; align-items: center;">
        <div style="font-size: 28px; font-weight: bold; color: #3d3428;">RentFlow</div>
        <div style="font-size: 12px; color: #6b5d4d;">Booking Confirmation</div>
      </div>
      
      <div style="text-align: center; margin-bottom: 20px;">
        <div style="font-size: 18px; font-weight: bold; color: #3d3428; margin-bottom: 5px;">ТАЛОН БРОНИРОВАНИЯ</div>
        <div style="font-size: 11px; color: #6b5d4d;">Номер бронирования: [НОМЕР БРОНИРОВАНИЯ]</div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Информация о недвижимости</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px; font-weight: 600;">[НАЗВАНИЕ НЕДВИЖИМОСТИ]</div>
        <div style="font-size: 10px; color: #6b5d4d; margin-bottom: 8px;">[АДРЕС НЕДВИЖИМОСТИ]</div>
        <div style="font-size: 10px; color: #3d3428;">
          Максимум гостей: [МАКС. ГОСТЕЙ]
          &nbsp;&nbsp;|&nbsp;&nbsp;Заезд: [ВРЕМЯ ЗАЕЗДА]
          &nbsp;&nbsp;|&nbsp;&nbsp;Выезд: [ВРЕМЯ ВЫЕЗДА]
        </div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Даты проживания</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Заезд:</b> [ДАТА ЗАЕЗДА]</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Выезд:</b> [ДАТА ВЫЕЗДА]</div>
        <div style="font-size: 11px; color: #3d3428;"><b>Количество гостей:</b> [КОЛИЧЕСТВО ГОСТЕЙ]</div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Информация о бронировании</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Статус:</b> [СТАТУС БРОНИРОВАНИЯ]</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>Дата бронирования:</b> [ДАТА СОЗДАНИЯ]</div>
        <div style="font-size: 11px; color: #4aaa5a; margin-top: 5px;"><b>Бесконтактное заселение:</b> [ДА/НЕТ]</div>
      </div>
      
      <div style="background: linear-gradient(135deg, #c9a86c, #e0c598); border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 14px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Оплата</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;">Стоимость за [КОЛИЧЕСТВО НОЧЕЙ] ночей: [СУММА] ₽</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;">Предоплата ([ПРОЦЕНТ ПРЕДОПЛАТЫ]%): [СУММА ПРЕДОПЛАТЫ] ₽</div>
        <div style="font-size: 14px; font-weight: bold; color: #3d3428; margin-top: 8px;">ИТОГО: [ОБЩАЯ СУММА] ₽</div>
      </div>
      
      <div style="background: #f0ece4; border-radius: 8px; padding: 15px; margin-bottom: 15px;">
        <div style="font-size: 13px; font-weight: bold; color: #3d3428; margin-bottom: 10px;">Контакты владельца</div>
        <div style="font-size: 11px; color: #3d3428; margin-bottom: 5px;"><b>[НАЗВАНИЕ КОМПАНИИ / ИМЯ ВЛАДЕЛЬЦА]</b></div>
        <div style="font-size: 10px; color: #3d3428; margin-bottom: 3px;">Тел: [ТЕЛЕФОН ВЛАДЕЛЬЦА]</div>
        <div style="font-size: 10px; color: #3d3428; margin-bottom: 3px;">Email: [EMAIL ВЛАДЕЛЬЦА]</div>
        <div style="font-size: 10px; color: #3d3428;">Telegram: [TELEGRAM ВЛАДЕЛЬЦА]</div>
      </div>
      
      <div style="text-align: center; margin-top: 20px; padding-top: 15px; border-top: 1px solid #d0c9bc;">
        <div style="font-size: 9px; color: #968e7d; font-style: italic;">Сохраните данный талон. Он является подтверждением вашего бронирования.</div>
        <div style="font-size: 9px; color: #968e7d; margin-top: 5px;">Сгенерировано: [ДАТА ГЕНЕРАЦИИ]</div>
      </div>
    </div>
  `;

  const opt = {
    margin: 0,
    filename: `booking_template.pdf`,
    image: { type: 'jpeg', quality: 0.98 },
    html2canvas: { scale: 2, useCORS: true },
    jsPDF: { unit: 'px', format: [595, 842], hotfixes: ['px_scaling'] }
  };

  html2pdf().set(opt).from(htmlContent).save();
}
